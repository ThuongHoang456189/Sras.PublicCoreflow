using Sras.PublicCoreflow.BlobContainer;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAppService : PublicCoreflowAppService, ISubmissionAppService
    {
        private readonly IRepository<Track, Guid> _trackRepository;
        private readonly IRepository<PaperStatus, Guid> _paperStatusRepository;
        private readonly IRepository<SubjectArea, Guid> _subjectAreaRepository;
        private readonly IIncumbentRepository _incumbentRepository;
        private readonly IConferenceRepository _conferenceRepository;
        private readonly IRepository<ConferenceAccount, Guid> _conferenceAccountRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IConflictRepository _conflictRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IBlobContainer<SubmissionContainer> _submissionBlobContainer;

        private const string AwaitingDecision = "Awaiting Decision";

        public SubmissionAppService(
            IRepository<Track, Guid> trackRepository, 
            IRepository<PaperStatus, Guid> paperStatusRepository, 
            IRepository<SubjectArea, Guid> subjectAreaRepository,
            IIncumbentRepository incumbentRepository,
            IConferenceRepository conferenceRepository,
            IRepository<ConferenceAccount, Guid> conferenceAccountRepository,
            IRepository<IdentityUser, Guid> userRepository,
            ISubmissionRepository submissionRepository,
            IConflictRepository conflictRepository,
            ICurrentUser currentUser, 
            IGuidGenerator guidGenerator, 
            IBlobContainer<SubmissionContainer> submissionBlobContainer)
        {
            _trackRepository = trackRepository;
            _paperStatusRepository = paperStatusRepository;
            _subjectAreaRepository = subjectAreaRepository;
            _incumbentRepository = incumbentRepository;
            _conferenceRepository = conferenceRepository;
            _conferenceAccountRepository = conferenceAccountRepository;
            _userRepository = userRepository;
            _submissionRepository = submissionRepository;
            _conflictRepository = conflictRepository;

            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
            _submissionBlobContainer = submissionBlobContainer;  
        }

        //public async Task SaveBytesAsync(byte[] bytes)
        //{
        //    await _submissionBlobContainer.SaveAsync("my-blob-1", bytes);
        //}

        //public async Task<byte[]> GetBytesAsync()
        //{
        //    return await _submissionBlobContainer.GetAllBytesOrNullAsync("my-blob-1");
        //}

        private async Task CreateSubmissionFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
            await _submissionBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }

        private async Task DeleteSubmissionFilesAsync(string blobName)
        {
            await _submissionBlobContainer.DeleteAsync(blobName);
        }

        public async Task<Guid> CreateAsync(SubmissionInput input)
        {
            // Check authority

            //Check if track is valid
            var track = await _trackRepository.FindAsync(x => x.Id == input.TrackId);
            if (track == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);
            }

            // Clean Authors Input List
            // Throw exception if two isprimary
            // Clean duplicate

            // Clean SubmissionSubjectArea Input List

            // Get Awaiting Decision PaperStatusId
            var awaitingDecisionPaperStatus = await _paperStatusRepository.FindAsync(x => x.IsDefault && x.Name.Equals(AwaitingDecision));

            if (awaitingDecisionPaperStatus == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.AwaitingDecisionPaperStatusNotFound);

            var submissionId = _guidGenerator.Create();
            Submission submission = new Submission(submissionId, input.Title, input.Abstract, submissionId.ToString(), track.Id,
                input.DomainConflicts, null, null, input.Answers, awaitingDecisionPaperStatus.Id, null, null, null, false);

            // Proceed author list
            var conferenceId = track.ConferenceId;
            var conference = await _conferenceRepository.FindAsync(x => x.Id == conferenceId);
            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }

            var authorOperationTable = await _incumbentRepository.GetAuthorOperationTableAsync(conferenceId, track.Id, input.Authors);

            // Apply operation on author list
            authorOperationTable.ForEach(x =>
            {
                submission.AddAuthor(_guidGenerator.Create(), x.ParticipantId, x.IsPrimaryContact);

                if (x.Operation == AuthorManipulationOperators.Add2)
                {
                    var conferenceAccount = new ConferenceAccount(_guidGenerator.Create(), conferenceId, x.AccountId.GetValueOrDefault(), false);
                    conferenceAccount.AddIncumbent(_guidGenerator.Create(), x.AuthorRoleId.GetValueOrDefault(), track.Id, x.IsPrimaryContact);
                    conference.AddConferenceAccount(conferenceAccount);
                }
                else if (x.Operation == AuthorManipulationOperators.UpAdd)
                {
                    var conferenceAccount = conference.ConferenceAccounts.Single(y => y.Id == x.ConferenceAccountId.GetValueOrDefault());
                    conferenceAccount.AddIncumbent(_guidGenerator.Create(), x.AuthorRoleId.GetValueOrDefault(), track.Id, x.IsPrimaryContact);
                }
            });

            // Proceed subject area list
            input.SubjectAreas.ForEach(x =>
            {
                submission.AddSubmissionSubjectArea(_guidGenerator.Create(), x.SubjectAreaId, x.IsPrimary);
            });

            await _conferenceRepository.UpdateAsync(conference);
            track.AddSubmission(submission);
            await _trackRepository.UpdateAsync(track);

            return submission.Id;
        }

        public ResponseDto CreateSubmissionFiles(Guid submissionId, List<RemoteStreamContent> files)
        {
            ResponseDto response = new();

            try
            {
                // Assume that the file extension is exactly matched its file name extension
                files.ForEach(async file =>
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        await CreateSubmissionFilesAsync(submissionId.ToString() + "/" + file.FileName, file, true);
                    }
                });

                response.IsSuccess = true;
                response.Message = "Create submission files successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }
            
            return response;
        }

        public async Task<object> GetNumberOfSubmission(Guid trackId)
        {
            return await _submissionRepository.GetNumberOfSubmission(trackId);
        }

        public async Task<object> GetNumberOfSubmissionAndEmail(SubmissionWithEmailRequest request)
        {
            if (request.AllAuthors)
            {
                return await _submissionRepository.GetNumOfSubmissionAndEmailWithAllAuthor(request);
            } else
            {
                return await _submissionRepository.GetNumOfSubmissionAndEmailWithPrimaryContactAuthor(request);
            }
        }

        public async Task<IEnumerable<object>> GetSubmissionsAsync()
        {
            return await _submissionRepository.GetSubmissionAsync();
        }

        public async Task<ResponseDto> UpdateSubmissionConflict(Guid submissionId, List<ConflictInput> conflicts)
        {
            ResponseDto response = new ResponseDto();

            // Get submission
            var submission = await _submissionRepository.FindAsync(submissionId);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            var submissionConflicts = await _conflictRepository.GetListAsync(x => x.SubmissionId == submissionId);
            if (!submission.Conflicts.Any())
            {
                submissionConflicts.ForEach(x =>
                {
                    submission.Conflicts.Add(x);
                });
            }

            // Clean and Validate input

            try
            {
                var submissionConflictOperationTable = await _conflictRepository.GetSubmissionConflictOperationTableAsync(submissionId);

                // Allocation operation
                submissionConflictOperationTable.ForEach(x =>
                {
                    if(!conflicts.Any(y => y.ConflictCaseId == x.ConflictCaseId && y.ReviewerId == x.IncumbentId))
                    {
                        x.Operation = ConflictManipulationOperators.Del;
                    }
                });

                conflicts.ForEach(x =>
                {
                    if(!submissionConflictOperationTable.Any(y => y.IncumbentId == x.ReviewerId && y.ConflictCaseId == x.ConflictCaseId))
                    {
                        ConflictOperation newOperation = new ConflictOperation
                        {
                            SubmissionId = submissionId,
                            IncumbentId = x.ReviewerId,
                            ConflictCaseId = x.ConflictCaseId,
                            IsDefinedByReviewer = false,
                            Operation = ConflictManipulationOperators.Add
                        };

                        submissionConflictOperationTable.Add(newOperation);
                    }
                });

                // Perform operations
                submissionConflictOperationTable.ForEach(x =>
                {
                    if(x.Operation == ConflictManipulationOperators.Add)
                    {
                        Conflict newConflict = new Conflict(_guidGenerator.Create(), x.SubmissionId, x.IncumbentId, x.ConflictCaseId, x.IsDefinedByReviewer);
                        submission.Conflicts.Add(newConflict);
                    }else if(x.Operation == ConflictManipulationOperators.Del)
                    {
                        var foundConflict = submission.Conflicts.FirstOrDefault(y => y.ConflictCaseId == x.ConflictCaseId && y.IncumbentId == x.IncumbentId && !y.IsDefinedByReviewer);
                        if (foundConflict != null)
                        {
                            submission.Conflicts.Remove(foundConflict);
                        }
                    }
                });

                await _submissionRepository.UpdateAsync(submission);

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

        public async Task<PagedResultDto<ReviewerWithConflictDetails>> GetListReviewerWithConflictDetails(Guid submissionId)
        {
            var reviewers = await _submissionRepository.GetListReviewerWithConflictDetails(submissionId);

            var totalCount = await _submissionRepository.GetCountConflictedReviewer(submissionId);

            return new PagedResultDto<ReviewerWithConflictDetails>(totalCount, reviewers);
        }
    }
}
