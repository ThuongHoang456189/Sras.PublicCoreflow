using Sras.PublicCoreflow.BlobContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerAppService : PublicCoreflowAppService, IReviewerAppService
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IReviewerSubjectAreaRepository _reviewerSubjectAreaRepository;
        private readonly IIncumbentRepository _incumbentRepository;
        private readonly IConflictRepository _conflictRepository;
        private readonly IRepository<Submission, Guid> _submissionRepository;
        private readonly IReviewAssignmentRepository _reviewAssignmentRepository;
        private readonly IRepository<Track, Guid> _trackRepository;
        
        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IBlobContainer<ReviewContainer> _reviewBlobContainer;

        public ReviewerAppService(
            IReviewerRepository reviewerRepository,
            IReviewerSubjectAreaRepository reviewerSubjectAreaRepository,
            IIncumbentRepository incumbentRepository,
            IConflictRepository conflictRepository,
            IRepository<Submission, Guid> submissionRepository,
            IReviewAssignmentRepository reviewAssignmentRepository,
            IRepository<Track, Guid> trackRepository,
            ICurrentUser currentUser,
            IGuidGenerator guidGenerator,
            IBlobContainer<ReviewContainer> reviewBlobContainer)
        {
            _reviewerRepository = reviewerRepository;
            _reviewerSubjectAreaRepository = reviewerSubjectAreaRepository;
            _incumbentRepository = incumbentRepository;
            _conflictRepository = conflictRepository;
            _submissionRepository = submissionRepository;
            _reviewAssignmentRepository = reviewAssignmentRepository;
            _trackRepository = trackRepository;

            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
            _reviewBlobContainer = reviewBlobContainer;
        }

        public async Task<ResponseDto> UpdateReviewerQuota(ReviewerQuotaInput input)
        {
            ResponseDto response = new ResponseDto();

            try
            {
                var reviewer = await _reviewerRepository.UpdateReviewerQuota(input.AccountId, input.ConferenceId, input.TrackId, input.Quota);

                if(reviewer == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Update failed";
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "Update successfully";
                }
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseDto> UpdateReviewerSubjectArea(ReviewerSubjectAreaInput input)
        {
            ResponseDto response = new ResponseDto();

            // Get reviewer
            var reviewer = await _reviewerRepository.FindAsync(input.AccountId, input.ConferenceId, input.TrackId);
            if (reviewer == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ReviewerNotFound);

            var reviewerSubjectAreas = await _reviewerSubjectAreaRepository.GetListAsync(x => x.ReviewerId == reviewer.Id);

            if (!reviewer.SubjectAreas.Any())
            {
                reviewerSubjectAreas.ForEach(x =>
                {
                    reviewer.SubjectAreas.Add(x);
                });
            }

            // Clean input

            try
            {
                // Allocate operation
                var reviewerSubjectAreaOperationTable = await _reviewerSubjectAreaRepository.GetReviewerSubjectAreaOperationTableAsync(reviewer.Id);
                reviewerSubjectAreaOperationTable.ForEach(x =>
                {
                    if (!input.SubjectAreas.Any(y => y.SubjectAreaId == x.SubjectAreaId))
                    {
                        x.Operation = ReviewerSubjectAreaManipulationOperators.Del;
                    }
                    else
                    {
                        var update = input.SubjectAreas.Find(y => y.SubjectAreaId == x.SubjectAreaId);
                        if (update != null)
                        {
                            if (update.IsPrimary != x.IsPrimary)
                            {
                                x.IsPrimary = update.IsPrimary;
                                x.Operation = ReviewerSubjectAreaManipulationOperators.Up;
                            }
                        }
                    }
                });

                input.SubjectAreas.ForEach(x =>
                {
                    if (!reviewerSubjectAreaOperationTable.Any(y => y.SubjectAreaId == x.SubjectAreaId))
                    {
                        ReviewerSubjectAreaOperation newOperation = new ReviewerSubjectAreaOperation
                        {
                            ReviewerId = reviewer.Id,
                            SubjectAreaId = x.SubjectAreaId,
                            IsPrimary = x.IsPrimary,
                            Operation = ReviewerSubjectAreaManipulationOperators.Add
                        };

                        reviewerSubjectAreaOperationTable.Add(newOperation);
                    }
                });

                // Perform operation
                reviewerSubjectAreaOperationTable.ForEach(x =>
                {
                    if (x.Operation == ReviewerSubjectAreaManipulationOperators.Add)
                    {
                        ReviewerSubjectArea newSubjectArea = new ReviewerSubjectArea(_guidGenerator.Create(), x.ReviewerId, x.SubjectAreaId, x.IsPrimary);
                        reviewer.SubjectAreas.Add(newSubjectArea);
                    }
                    else if (x.Operation == ReviewerSubjectAreaManipulationOperators.Del)
                    {
                        var foundSubjectArea = reviewer.SubjectAreas.FirstOrDefault(y => y.SubjectAreaId == x.SubjectAreaId);
                        if (foundSubjectArea != null)
                        {
                            reviewer.SubjectAreas.Remove(foundSubjectArea);
                        }
                    }
                    else if (x.Operation == ReviewerSubjectAreaManipulationOperators.Up)
                    {
                        var foundSubjectArea = reviewer.SubjectAreas.FirstOrDefault(y => y.SubjectAreaId == x.SubjectAreaId);
                        if (foundSubjectArea != null)
                        {
                            foundSubjectArea.IsPrimary = x.IsPrimary;
                        }
                    }
                });

                await _reviewerRepository.UpdateAsync(reviewer);

                response.IsSuccess = true;
                response.Message = "Update successfully";
            }
            catch(Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseDto> UpdateReviewerConflict (ReviewerConflictInput input)
        {
            ResponseDto response = new ResponseDto();

            var submission = await _submissionRepository.FindAsync(x => x.Id == input.SubmissionId);
            if (submission == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);

            // Get reviewer
            var reviewer = await _reviewerRepository.FindAsync(input.AccountId, input.ConferenceId, submission.TrackId);
            if (reviewer == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ReviewerNotFound);

            var incumbent = await _incumbentRepository.FindAsync(reviewer.Id);
            var incumbentConflicts = await _conflictRepository.GetListAsync(x => x.IncumbentId == reviewer.Id);

            if (!incumbent.Conflicts.Any())
            {
                incumbentConflicts.ForEach(x =>
                {
                    incumbent.Conflicts.Add(x);
                });
            }

            // Clean input

            try
            {
                // Allocate operation
                var reviewerConflictOperationTable = await _conflictRepository.GetReviewerConflictOperationTableAsync(incumbent.Id, input.SubmissionId);
                reviewerConflictOperationTable.ForEach(x =>
                {
                    if (!input.ConflictCases.Any(y => y == x.ConflictCaseId))
                    {
                        x.Operation = ConflictManipulationOperators.Del;
                    }
                });

                input.ConflictCases.ForEach(x =>
                {
                    if (!reviewerConflictOperationTable.Any(y => y.ConflictCaseId == x))
                    {
                        ConflictOperation newOperation = new ConflictOperation
                        {
                            SubmissionId = input.SubmissionId,
                            IncumbentId = incumbent.Id,
                            ConflictCaseId = x,
                            IsDefinedByReviewer = true,
                            Operation = ConflictManipulationOperators.Add
                        };

                        reviewerConflictOperationTable.Add(newOperation);
                    }
                });

                // Perform operation
                reviewerConflictOperationTable.ForEach(x =>
                {
                    if (x.Operation == ConflictManipulationOperators.Add)
                    {
                        Conflict newConflict = new Conflict(_guidGenerator.Create(), x.SubmissionId, x.IncumbentId, x.ConflictCaseId, x.IsDefinedByReviewer);
                        incumbent.Conflicts.Add(newConflict);
                    }
                    else if (x.Operation == ConflictManipulationOperators.Del)
                    {
                        var foundConflict = incumbent.Conflicts.FirstOrDefault(y => y.ConflictCaseId == x.ConflictCaseId && y.SubmissionId == x.SubmissionId && y.IsDefinedByReviewer);
                        if (foundConflict != null)
                        {
                            incumbent.Conflicts.Remove(foundConflict);
                        }
                    }
                });

                await _incumbentRepository.UpdateAsync(incumbent);

                response.IsSuccess = true;
                response.Message = "Update successfully";
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return await Task.FromResult(response);
        }

        public async Task<ReviewerSubmissionConflictDto> GetListReviewerConflictAsync(ReviewerConflictLookUpInput input)
        {
            var submission = await _submissionRepository.FindAsync(x => x.Id == input.SubmissionId);
            if (submission == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);

            var reviewer = await _reviewerRepository.FindAsync(input.AccountId, input.ConferenceId, submission.TrackId);
            if (reviewer == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ReviewerNotFound);

            var track = await _trackRepository.FindAsync(submission.TrackId);
            if (track == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);
            }

            var conflicts =  await _conflictRepository.GetListReviewerConflictAsync(reviewer.Id, input.SubmissionId);

            var result = new ReviewerSubmissionConflictDto
            {
                SubmissionId = submission.Id,
                SubmissionTitle = submission.Title,
                TrackId = track.Id,
                TrackName = track.Name,
            };

            return result;
        }

        private async Task CreateReviewFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
            await _reviewBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }

        private async Task DeleteReviewFilesAsync(string blobName)
        {
            await _reviewBlobContainer.DeleteAsync(blobName);
        }

        public async Task<ResponseDto> UploadReview(Guid reviewAssignmentId, List<RemoteStreamContent> files, int? totalScore)
        {
            ResponseDto response = new ResponseDto();

            try
            {
                var reviewerAssignment = await _reviewAssignmentRepository.FindAsync(reviewAssignmentId);
                if(reviewerAssignment == null)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.ReviewAssignmentNotFound);
                }

                if(files.Count() <= 0)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.NoFilesIncluded);
                }

                await DeleteReviewFilesAsync(reviewAssignmentId.ToString());
                files.ForEach(async file =>
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        await CreateReviewFilesAsync(reviewAssignmentId.ToString() + "/" + file.FileName, file, true);
                    }
                });

                reviewerAssignment.Review = reviewAssignmentId.ToString();
                reviewerAssignment.TotalScore = totalScore;

                await _reviewAssignmentRepository.UpdateAsync(reviewerAssignment);

                response.IsSuccess = true;
                response.Message = "Update successfully";
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return await Task.FromResult(response);
        }
    }
}