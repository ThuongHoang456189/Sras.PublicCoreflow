using Ionic.Zip;
using Sras.PublicCoreflow.BlobContainer;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using static OpenIddict.Abstractions.OpenIddictConstants;

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
        private readonly IRepository<SubmissionClone, Guid> _submissionCloneRepository;
        private readonly IReviewAssignmentRepository _reviewAssignmentRepository;
        private readonly IRepository<Revision, Guid> _revisionRepository;
        private readonly IReviewerRepository _reviewerRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IBlobContainer<SubmissionContainer> _submissionBlobContainer;
        private readonly IBlobContainer<SupplementaryMaterialContainer> _supplementaryMaterialContainer;
        private readonly IBlobContainer<RevisionContainer> _revisionBlobContainer;
        private readonly IBlobContainer<CameraReadyContainer> _cameraReadyContainer;
        private readonly IBlobContainer<CopyRightContainer> _copyRightContainer;
        private readonly IBlobContainer<PresentationContainer> _presentationContainer;

        private const string AwaitingDecision = "Awaiting Decision";
        private const string Accept = "Accept";
        private const string BlobRoot = "host";
        private const string SubmissionBlobRoot = "sras-submissions";
        private const string SupplementaryMaterialBlobRoot = "sras-supplementary-materials";
        private const string RevisionBlobRoot = "sras-revisions";
        private const string CameraReadyBlobRoot = "sras-camera-readies";
        private const string CopyRightBlobRoot = "sras-copyrights";
        private const string PresentationBlobRoot = "sras-presentations";

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
            IRepository<SubmissionClone, Guid> submissionCloneRepository,
            IReviewAssignmentRepository reviewAssignmentRepository,
            IRepository<Revision, Guid> revisionRepository,
            IReviewerRepository reviewerRepository,
            IRepository<CameraReady, Guid> cameraReadyRepository,
            ICurrentUser currentUser,
            IGuidGenerator guidGenerator,
            IBlobContainer<SubmissionContainer> submissionBlobContainer,
            IBlobContainer<SupplementaryMaterialContainer> supplementaryMaterialContainer,
            IBlobContainer<RevisionContainer> revisionBlobContainer,
            IBlobContainer<CameraReadyContainer> cameraReadyContainer,
            IBlobContainer<CopyRightContainer> copyRightContainer,
            IBlobContainer<PresentationContainer> presentationContainer)
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
            _submissionCloneRepository = submissionCloneRepository;
            _reviewAssignmentRepository = reviewAssignmentRepository;
            _revisionRepository = revisionRepository;
            _reviewerRepository = reviewerRepository;

            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
            _submissionBlobContainer = submissionBlobContainer;
            _supplementaryMaterialContainer = supplementaryMaterialContainer;
            _revisionBlobContainer = revisionBlobContainer;
            _cameraReadyContainer = cameraReadyContainer;
            _copyRightContainer = copyRightContainer;
            _presentationContainer = presentationContainer;
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

        private async Task CreateRevisionFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
            await _revisionBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }

        private async Task CreateCameraReadyFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
            await _cameraReadyContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
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
                input.DomainConflicts, null, null, input.Answers,
                awaitingDecisionPaperStatus.Id, null, awaitingDecisionPaperStatus.Id, null, false, false, null, false, null, null);

            // Add first clone
            submission.Clones.Add(new SubmissionClone(_guidGenerator.Create(), submissionId, true, 0));

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

            // Check valid submission

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

        public async Task<object> GetNumberOfSubmissionAndEmail(SubmissionWithEmailRequest request)
        {
            if (request.allAuthors)
            {
                return await _submissionRepository.GetNumOfSubmissionAndEmailWithAllAuthor(request);
            }
            else
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
                    if (!conflicts.Any(y => y.ConflictCaseId == x.ConflictCaseId && y.ReviewerId == x.IncumbentId))
                    {
                        x.Operation = ConflictManipulationOperators.Del;
                    }
                });

                conflicts.ForEach(x =>
                {
                    if (!submissionConflictOperationTable.Any(y => y.IncumbentId == x.ReviewerId && y.ConflictCaseId == x.ConflictCaseId))
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
                    if (x.Operation == ConflictManipulationOperators.Add)
                    {
                        Conflict newConflict = new Conflict(_guidGenerator.Create(), x.SubmissionId, x.IncumbentId, x.ConflictCaseId, x.IsDefinedByReviewer);
                        submission.Conflicts.Add(newConflict);
                    }
                    else if (x.Operation == ConflictManipulationOperators.Del)
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

        public async Task<SubmissionReviewerConflictDto> GetListReviewerWithConflictDetails(Guid submissionId)
        {
            var submission = await _submissionRepository.FindAsync(submissionId);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            var track = await _trackRepository.FindAsync(submission.TrackId);
            if (track == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);
            }

            var reviewerList = await _submissionRepository.GetListReviewerWithConflictDetails(submissionId);

            var totalCount = await _submissionRepository.GetCountConflictedReviewer(submissionId);

            var reviewers = new PagedResultDto<ReviewerWithConflictDetails>(totalCount, reviewerList);

            return new SubmissionReviewerConflictDto
            {
                SubmissionId = submission.Id,
                SubmissionTitle = submission.Title,
                TrackId = track.Id,
                TrackName = track.Name,
                Reviewers = reviewers
            };
        }

        //public async Task<SubmissionReviewerAssignmentSuggestionDto> GetSubmissionReviewerAssignmentSuggestionAsync(Guid submissionId)
        //{
        //    var result = new SubmissionReviewerAssignmentSuggestionDto();

        //    var data = await _submissionRepository.GetSubmissionReviewerAssignmentSuggestionAsync(submissionId);

        //    result.TrackId = data.TrackId;
        //    result.TrackName = data.TrackName;
        //    result.SubmissionId = data.SubmissionId;
        //    result.SubmissionTitle = data.SubmissionTitle;
        //    result.SubmissionSubjectAreas = data.SubmissionSubjectAreas;
        //    result.Reviewers = new PagedResultDto<ReviewerWithFacts>(data.Reviewers.Count, data.Reviewers);

        //    return result;
        //}

        public async Task<CreationResponseDto> CreateRevisionAsync(Guid submissionId, List<RemoteStreamContent> files)
        {
            CreationResponseDto response = new();

            // Get submission
            var submission = await _submissionRepository.FindAsync(submissionId);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            // Lay !IsLast submission clone, update IsLast thanh true
            var previousLastSubmissionClone = await _submissionCloneRepository.GetAsync(x => x.IsLast && x.SubmissionId == submission.Id);
            if (previousLastSubmissionClone == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.LastSubmissionCloneNotFound);

            try
            {
                // Get last active review assignment
                var lastActiveReviewAssignmentList = await _reviewAssignmentRepository.GetActiveReviewAssignment(previousLastSubmissionClone.Id);

                // Create new clone submission, CloneNo ++
                var newCloneSubmission = new SubmissionClone(_guidGenerator.Create(), submissionId, true, previousLastSubmissionClone.CloneNo + 1);
                previousLastSubmissionClone.IsLast = false;

                // Change submission thanh Status Waiting Decision, notify status la Waiting Decision, IsNotified = null
                var awaitingDecisionPaperStatus = await _paperStatusRepository.FindAsync(x => x.IsDefault && x.Name.Equals(AwaitingDecision));
                submission.StatusId = awaitingDecisionPaperStatus.Id;
                submission.NotifiedStatusId = awaitingDecisionPaperStatus.Id;
                submission.IsNotified = null;

                // Add new revision
                var previousRevision = await _revisionRepository.FindAsync(x => x.Id == previousLastSubmissionClone.Id);

                var newRevisionId = _guidGenerator.Create();
                var newRevision = new Revision(newRevisionId, newRevisionId.ToString(), previousRevision?.Id);
                newCloneSubmission.Revisions.Add(newRevision);

                // Assume that the file extension is exactly matched its file name extension
                files.ForEach(async file =>
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        await CreateRevisionFilesAsync(newRevisionId.ToString() + "/" + file.FileName, file, true);
                    }
                });

                // Copy review assignment + Is Notify = false
                lastActiveReviewAssignmentList.ForEach(x =>
                {
                    newCloneSubmission.Reviews.Add(new ReviewAssignment(_guidGenerator.Create(), newCloneSubmission.Id, x.ReviewerId, null, null, true, false));
                });
                submission.Clones.Add(newCloneSubmission);

                await _submissionRepository.UpdateAsync(submission);

                response.IsSuccess = true;
                response.Message = "Create revision successfully";
                response.Id = newRevisionId;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
                response.Id = null;
            }

            return response;
        }

        public async Task<ResponseDto> AssignReviewerAsync(Guid submissionId, Guid reviewerId, bool isAssigned)
        {
            ResponseDto response = new();

            // Get submission
            var submission = await _submissionRepository.FindAsync(submissionId);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            // Lay last submission clone
            await _submissionCloneRepository.GetListAsync(x => x.SubmissionId == submissionId);
            var lastSubmissionClone = submission.Clones.FirstOrDefault(x => x.IsLast && x.SubmissionId == submission.Id);
            if (lastSubmissionClone == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.LastSubmissionCloneNotFound);

            try
            {
                // Get last review assignment
                var lastAssignmentList = await _reviewAssignmentRepository.GetListAsync(x => x.SubmissionCloneId == lastSubmissionClone.Id);

                // Reviewer Id 
                var reviewer = await _reviewerRepository.FindAsync(reviewerId, submission.TrackId);
                var reviewAssignment = lastSubmissionClone.Reviews.FirstOrDefault(x => x.ReviewerId == reviewerId && x.SubmissionCloneId == lastSubmissionClone.Id);
                if (reviewer == null)
                {
                    if (reviewAssignment != null)
                        lastSubmissionClone.Reviews.Remove(reviewAssignment);

                    throw new BusinessException(PublicCoreflowDomainErrorCodes.ReviewerNotFound);
                }

                // Neu chua co va trang thai can update la true => tao record reviewAssignment moi, IsNotify la false
                // Co roi thi chi update IsActive
                if (reviewAssignment != null)
                {
                    if (reviewAssignment.IsActive != isAssigned)
                    {
                        reviewAssignment.IsActive = isAssigned;
                        reviewAssignment.IsNotified = false;
                    }
                }
                else
                {
                    lastSubmissionClone.Reviews.Add(new ReviewAssignment(_guidGenerator.Create(), lastSubmissionClone.Id, reviewerId, null, null, true, false));
                }

                await _submissionRepository.UpdateAsync(submission);
                var action = isAssigned ? "Assign" : "Unassign";

                response.IsSuccess = true;
                response.Message = action + " successfully";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return response;
        }

        public async Task<ResponseDto> DecideOnPaper(Guid submissionId, Guid paperStatusId)
        {
            // Lay trang status voi trang thai moi, neu trung thi khong update

            // Neu khong trung thi trang thai moi la Awaiting Decision thi IsNotified = null

            // Nguoc lai false

            ResponseDto response = new();

            // Get submission
            var submission = await _submissionRepository.FindAsync(submissionId);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            var paperStatus = await _paperStatusRepository.FindAsync(x => x.Id == paperStatusId);
            if (paperStatus == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.PaperStatusNotFound);
            }

            try
            {
                if (submission.StatusId != paperStatusId)
                {
                    submission.StatusId = paperStatusId;

                    var awaitingDecisionPaperStatus = await _paperStatusRepository.FindAsync(x => x.IsDefault && x.Name.Equals(AwaitingDecision));
                    if (paperStatusId != awaitingDecisionPaperStatus.Id)
                    {
                        submission.IsNotified = false;
                    }
                    else
                    {
                        submission.IsNotified = null;
                    }
                }

                await _submissionRepository.UpdateAsync(submission);

                response.IsSuccess = true;
                response.Message = "Update successfully";
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return response;
        }

        public async Task<PagedResultDto<SubmissionAggregation>> GetListSubmissionAggregation(SubmissionAggregationListFilterDto filter)
        {
            var items = await _submissionRepository.GetListSubmissionAggregation(filter.ConferenceId,
                filter.TrackId, filter.Sorting.IsNullOrEmpty() ? SubmissionConsts.DefaultSorting : filter.Sorting, filter.SkipCount, filter.MaxResultCount);

            return new PagedResultDto<SubmissionAggregation>(items.Count, items);
        }

        public async Task<CreationResponseDto> CreateCameraReadyAsync(Guid submissionId, List<RemoteStreamContent> files)
        {
            CreationResponseDto response = new();

            // Get submission
            var submission = await _submissionRepository.FindAsync(submissionId);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            if (submission.CameraReadies.Any(x => x.Id == submission.Id))
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.CameraReadyAlreadyExist);
            }

            bool isDemo = true;
            if (!isDemo)
            {
                // Check author relationship

                if (!(submission.IsRequestedForCameraReady &&
                    submission.NotifiedStatus != null && submission.NotifiedStatus.Name.ToLower().Equals(Accept.ToLower())))
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionIsNotAllowedToSubmitCameraReady);
                }
            }

            try
            {
                // Assume that the file extension is exactly matched its file name extension
                files.ForEach(async file =>
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        await CreateCameraReadyFilesAsync(submission.Id.ToString() + "/" + file.FileName, file, true);
                    }
                });

                submission.CameraReadies.Add(new CameraReady(submission.Id, submission.Id.ToString(), null, null));
                await _submissionRepository.UpdateAsync(submission);

                response.IsSuccess = true;
                response.Message = "Create camera ready successfully";
                response.Id = submission.Id;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
                response.Id = null;
            }

            return response;
        }

        public async Task<ResponseDto> RequestCameraReady(Guid submissionId, bool isRequested)
        {
            ResponseDto response = new();

            // Get submission
            var submission = await _submissionRepository.FindAsync(submissionId);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            try
            {
                submission.IsRequestedForCameraReady = isRequested;
                if (isRequested)
                {
                    submission.CameraReadyRequestTime = DateTime.Now;
                }
                else
                {
                    submission.CameraReadyRequestTime = null;
                }

                await _submissionRepository.UpdateAsync(submission);

                response.IsSuccess = true;
                response.Message = "Update successfully";
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return response;
        }

        public async Task<PagedResultDto<SubmissionAggregationV1Dto>> GetListSubmissionAggregationSP(string? inclusionText, Guid conferenceId, Guid? trackId, Guid? statusId, int skipCount, int maxResultCount)
        {
            var result = await _submissionRepository.GetListSubmissionAggregationSP(inclusionText, conferenceId, trackId, statusId, skipCount, maxResultCount);

            var items = ObjectMapper.Map<List<SubmissionAggregationSP>, List<SubmissionAggregationV1Dto>>(result);

            return new PagedResultDto<SubmissionAggregationV1Dto>(result != null && result.Count > 0 && result[0] != null && result[0].TotalCount != null ? (long)result[0].TotalCount.Value : 0, items);
        }

        //public async Task<byte[]> GetWebsiteFiles(Guid id)
        //{
        //    return await _submissionBlobContainer.GetAllBytesOrNullAsync(id.ToString()+"/green-bird-pink.png");
        //}

        public async Task<ZipFileDto> DownloadSubmissionFiles(Guid id)
        {
            var submission = await _submissionRepository.FindAsync(id);
            if (submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            var zipFileName = submission.Title.ToLower().Replace(" ", "-").Truncate(SubmissionConsts.DefaultMaxZipFileNameLength) + ".zip";
            var storePath = string.Join("/", BlobRoot, SubmissionBlobRoot, zipFileName);

            using (ZipFile zip = new())
            {
                // Get all filepath from folder
                var submissionPath = string.Join("/", BlobRoot, SubmissionBlobRoot, id.ToString());

                string[] files = Directory.GetFiles(submissionPath);
                int i = 0;
                foreach (string file in files)
                {
                    i++;
                    zip.AddFile(file, "");
                }

                zip.CompressionMethod = CompressionMethod.BZip2;
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                zip.Save(storePath);
            }

            var result = await _submissionBlobContainer.GetAsync(zipFileName);

            File.Delete(storePath);

            return new ZipFileDto
            {
                FileName = zipFileName,
                FileStream = result,
            };
        }

        public async Task<SelectedSubmissionBriefInfo> GetSelectedSubmissionBriefInfoAsync(Guid id)
        {
            var submission = await _submissionRepository.FindAsync(id);

            if(submission == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);
            }

            var track = await _trackRepository.FindAsync(submission.TrackId);

            if (track == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);
            }

            return new SelectedSubmissionBriefInfo
            {
                SubmissionId = submission.Id,
                Title = submission.Title,
                TrackId = submission.TrackId,
                TrackName = track.Name
            };
        }

        private List<SubmissionAuthorDto>? GetListAggregationSubmissionAuthor(string? authorsStr)
        {
            if (string.IsNullOrWhiteSpace(authorsStr))
                return null;

            List<string> authors = authorsStr.Split(';').ToList();
            List<SubmissionAuthorDto> authorList = new List<SubmissionAuthorDto>();

            authors.ForEach(x =>
            {
                List<string> authorFacts = x.Split('|').ToList();

                authorList.Add(new SubmissionAuthorDto()
                {
                    AuthorEmail = authorFacts[0].IsNullOrWhiteSpace() ? null : authorFacts[0],
                    AuthorNamePrefix = authorFacts[1].IsNullOrWhiteSpace() ? null : authorFacts[1],
                    AuthorFullName = authorFacts[2].IsNullOrWhiteSpace() ? null : authorFacts[2],
                    AuthorOrganization = authorFacts[3].IsNullOrWhiteSpace() ? null : authorFacts[3],
                    HasAccount = authorFacts[4].Equals("1"),
                    IsPrimaryContact = authorFacts[5].Equals("1")
                });
            });

            return authorList;
        }

        private List<AggregationSubjectAreaDto>? GetListAggregationSubjectArea(string? aggregationSubjectAreasStr)
        {
            if (string.IsNullOrWhiteSpace(aggregationSubjectAreasStr))
                return null;

            List<string> subjectAreas = aggregationSubjectAreasStr.Split(';').ToList();
            List<AggregationSubjectAreaDto> aggregationSubjectAreasStrList = new List<AggregationSubjectAreaDto>();

            subjectAreas.ForEach(x =>
            {
                List<string> subjectAreaFacts = x.Split('|').ToList();

                aggregationSubjectAreasStrList.Add(new AggregationSubjectAreaDto()
                {
                    SubjectAreaName = subjectAreaFacts[0].IsNullOrWhiteSpace() ? null : subjectAreaFacts[0],
                    IsPrimary = subjectAreaFacts[1].Equals("1")
                });
            });

            return aggregationSubjectAreasStrList;
        }

        private List<SubmissionSummarySubmissionConflictedIncumbentDto>? GetListSubmissionSummarySubmissionConflictedIncumbent(string? submissionConflictedIncumbentStr)
        {
            if (string.IsNullOrWhiteSpace(submissionConflictedIncumbentStr))
                return null;

            List<string> submissionConflictedIncumbents = submissionConflictedIncumbentStr.Split(';').ToList();
            List<SubmissionSummarySubmissionConflictedIncumbentDto> submissionConflictedIncumbentList = new List<SubmissionSummarySubmissionConflictedIncumbentDto>();

            submissionConflictedIncumbents.ForEach(x =>
            {
                List<string> submissionConflictedIncumbentFacts = x.Split('|').ToList();

                submissionConflictedIncumbentList.Add(new SubmissionSummarySubmissionConflictedIncumbentDto()
                {
                    IncumbentNamePrefix = submissionConflictedIncumbentFacts[0].IsNullOrWhiteSpace() ? null : submissionConflictedIncumbentFacts[0],
                    IncumbentFullName = submissionConflictedIncumbentFacts[1].IsNullOrWhiteSpace() ? null : submissionConflictedIncumbentFacts[1],
                    IncumbentOrganization = submissionConflictedIncumbentFacts[2].IsNullOrWhiteSpace() ? null : submissionConflictedIncumbentFacts[2],
                    IncumbentEmail = submissionConflictedIncumbentFacts[3].IsNullOrWhiteSpace() ? null : submissionConflictedIncumbentFacts[3],
                    Conflicts = submissionConflictedIncumbentFacts[4].IsNullOrWhiteSpace() ? null : submissionConflictedIncumbentFacts[4].Substring(1, submissionConflictedIncumbentFacts[4].Length - 2).Split(@"\").ToList()
                });
            });

            return submissionConflictedIncumbentList;
        }

        private List<string>? GetSubmissionSummarySubmissionFiles(string? submissionRootFilePath)
        {
            if(string.IsNullOrWhiteSpace(submissionRootFilePath)) 
                return null;

            var submissionPath = string.Join("/", BlobRoot, SubmissionBlobRoot, submissionRootFilePath);

            try
            {
                return Directory.GetFiles(submissionPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch(Exception)
            {
                return null;
            }
        }

        private List<string>? GetSubmissionSummarySupplementaryMaterialFiles(string? supplementaryMaterialRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(supplementaryMaterialRootFilePath))
                return null;

            var supplementaryMaterialPath = string.Join("/", BlobRoot, SupplementaryMaterialBlobRoot, supplementaryMaterialRootFilePath);

            try
            {
                return Directory.GetFiles(supplementaryMaterialPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetSubmissionSummaryRevisionFiles(string? revisionRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(revisionRootFilePath))
                return null;

            var revisionPath = string.Join("/", BlobRoot, RevisionBlobRoot, revisionRootFilePath);

            try
            {
                return Directory.GetFiles(revisionPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetSubmissionSummaryCameraReadyFiles(string? cameraReadyRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(cameraReadyRootFilePath))
                return null;

            var cameraReadyPath = string.Join("/", BlobRoot, CameraReadyBlobRoot, cameraReadyRootFilePath);

            try
            {
                return Directory.GetFiles(cameraReadyPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetSubmissionSummaryCopyRightFiles(string? copyRightFilePath)
        {
            if (string.IsNullOrWhiteSpace(copyRightFilePath))
                return null;

            var copyRightPath = string.Join("/", BlobRoot, CopyRightBlobRoot, copyRightFilePath);

            try
            {
                return Directory.GetFiles(copyRightPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetSubmissionSummaryPresentationFiles(string? presentationRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(presentationRootFilePath))
                return null;

            var presentationPath = string.Join("/", BlobRoot, PresentationBlobRoot, presentationRootFilePath);

            try
            {
                return Directory.GetFiles(presentationPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<SubmissionSummaryDto?> GetSubmissionSummaryAsync(Guid submissionId)
        {
            var result = await _submissionRepository.GetSubmissionSummaryAsync(submissionId);

            if (result == null)
                return null;

            var summary = new SubmissionSummaryDto()
            {
                ConferenceFullName = result.ConferenceFullName,
                ConferenceShortName = result.ConferenceShortName,
                TrackName = result.TrackName,
                PaperId = result.PaperId,
                Title = result.Title,
                Abstract = result.Abstract,
                CreationTime = result.CreationTime,
                LastModificationTime = result.LastModificationTime,
                Authors = GetListAggregationSubmissionAuthor(result.SelectedAuthors),
                SubjectAreas = GetListAggregationSubjectArea(result.SelectedSubmissionSubjectAreas),
                DomainConflicts = result.DomainConflicts,
                ConflictsOfInterest = GetListSubmissionSummarySubmissionConflictedIncumbent(result.SelectedSubmissionConflictedIncumbents),
                SubmissionFiles = GetSubmissionSummarySubmissionFiles(result.SubmissionRootFilePath),
                SupplementaryMaterialFiles = GetSubmissionSummarySupplementaryMaterialFiles(result.SupplementaryMaterialRootFilePath),
                SubmittedRevisionNo = result.SubmittedRevisionNo,
                RevisionFiles = GetSubmissionSummaryRevisionFiles(result.RevisionRootFilePath),
                CameraReadyFiles = GetSubmissionSummaryCameraReadyFiles(result.CameraReadyRootFilePath),
                CopyRightFiles = GetSubmissionSummaryCopyRightFiles(result.CopyRightFilePath),
                PresentationFiles = GetSubmissionSummaryPresentationFiles(result.PresentationRootFilePath),
                SubmissionQuestionsResponse = result.SubmissionQuestionsResponse
            };

            return summary;
        }

        public class SubjectAreaRow : AggregationSubjectAreaDto, IEquatable<SubjectAreaRow>
        {
            public bool Equals(SubjectAreaRow? other)
            {
                if (other is null || this.SubjectAreaName == null || other.SubjectAreaName == null)
                    return false;

                return
                    this.SubjectAreaName.ToLower().Equals(other.SubjectAreaName.ToLower())
                    && this.IsPrimary == other.IsPrimary;
            }

            public override bool Equals(object? obj) => Equals(obj as SubjectAreaRow);
            public override int GetHashCode() => (SubjectAreaName, IsPrimary).GetHashCode();
        }

        private double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1.0 + k);
        }

        private List<SubjectAreaRow> TransformListAggregationSubjectAreaDto(List<AggregationSubjectAreaDto>? aggregationSubjectAreaDtos)
        {
            List<SubjectAreaRow> subjectAreaRows = new List<SubjectAreaRow>();

            if(aggregationSubjectAreaDtos != null && aggregationSubjectAreaDtos.Count != 0)
            {
                aggregationSubjectAreaDtos.ForEach(x =>
                {
                    subjectAreaRows.Add(new SubjectAreaRow
                    {
                        SubjectAreaName = x.SubjectAreaName,
                        IsPrimary = x.IsPrimary
                    });
                });
            }

            return subjectAreaRows;
        }

        private double CalculateRelevance(
            List<AggregationSubjectAreaDto> submissionSubjectAreas,
            List<AggregationSubjectAreaDto> reviewerSubjectAreas,
            SubjectAreaRelevanceCoefficients formula)
        {
            var subSaList = TransformListAggregationSubjectAreaDto(submissionSubjectAreas);
            var revSaList = TransformListAggregationSubjectAreaDto(reviewerSubjectAreas);

            var commonSaList = subSaList.Intersect(revSaList);

            var ppcoef = commonSaList.Any(x => x.IsPrimary == true) ? 1 : 0;

            var primarySubSa = submissionSubjectAreas?.FirstOrDefault(x => x.IsPrimary == true);
            var spcoef = reviewerSubjectAreas.Any(x => !x.IsPrimary && x.SubjectAreaName.ToLower().Equals(primarySubSa?.SubjectAreaName.ToLower())) ? 1 : 0;

            var primaryRevSa = reviewerSubjectAreas?.FirstOrDefault(x => x.IsPrimary == true);
            var pscoef = submissionSubjectAreas.Any(x => !x.IsPrimary && x.SubjectAreaName.ToLower().Equals(primaryRevSa?.SubjectAreaName.ToLower())) ? 1 : 0;

            var commonSecondarySaList = commonSaList.ToList();
            commonSecondarySaList.RemoveAll(x => x.IsPrimary == true);

            var sscoef = commonSecondarySaList.Count;

            var temp = Sigmoid(sscoef);

            if (formula.pp != null && formula.sp != null && formula.ps != null && formula.ss != null)
            {
                return Math.Round((double)(formula.pp * ppcoef + formula.sp * spcoef + formula.ps * pscoef
                    + formula.ss * 2 * (Sigmoid(sscoef) - 0.5)), SubmissionConsts.NumberOfRelevanceScoreDigits);
            }

            return 0;
        }

        private List<string>? GetListAggregationConflict(string? aggregationConflictStr)
        {
            if (string.IsNullOrWhiteSpace(aggregationConflictStr))
                return null;

            List<string> conflicts = aggregationConflictStr.Split(';').ToList();

            return conflicts;
        }

        public async Task<SubmissionReviewerAssignmentSuggestionDto?> GetSubmissionReviewerAssignmentSuggestionAsync(SubmissionReviewerAssignmentSuggestionInput input)
        {
            SubmissionReviewerAssignmentSuggestionDto? result = null;

            var foundSubmission = await _submissionRepository.GetReviewerAssignmentSuggestionSubmissionPart(input.SubmissionId);
            var foundReviewers = await _submissionRepository.GetSubmissionReviewerAssignmentSuggestionAsync(input.InclusionText, input.SubmissionId, input.IsAssigned);

            if (foundSubmission == null)
                return null;

            result = new SubmissionReviewerAssignmentSuggestionDto()
            {
                TrackId = foundSubmission.TrackId,
                TrackName = foundSubmission.TrackName,
                PaperId = foundSubmission.PaperId,
                Title = foundSubmission.Title,
                SubmissionSubjectAreas = GetListAggregationSubjectArea(foundSubmission.SelectedSubmissionSubjectAreas),
                Reviewers = null
            };

            if (foundReviewers == null)
                return result;

            var relevanceFormula = foundSubmission.SubjectAreaRelevanceCoefficients == null ?
                TrackConsts.DefaultSubjectAreaRelevanceCoefficients :
                JsonSerializer.Deserialize<SubjectAreaRelevanceCoefficients>(foundSubmission.SubjectAreaRelevanceCoefficients);

            List<ReviewerAssignmentSuggestionDto> reviewers = new List<ReviewerAssignmentSuggestionDto>();

            foundReviewers.ForEach(x =>
            {
                var reviewerSubjectAreas = GetListAggregationSubjectArea(x.SelectedReviewerSubjectAreas);
                var submissionConflicts = GetListAggregationConflict(x.SelectedSubmissionConflicts);
                var reviewerConflicts = GetListAggregationConflict(x.SelectedReviewerConflicts);

                var sortingFactor = 2;
                if (submissionConflicts != null && reviewerConflicts != null)
                {
                    if (x.Quota != null && x.Quota - x.NumberOfAssignments <= 0)
                        sortingFactor = 1;
                    else
                        sortingFactor = 0;
                }

                reviewers.Add(new ReviewerAssignmentSuggestionDto()
                {
                    ReviewerId = x.ReviewerId,
                    FullName = x.FullName,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Organization = x.Organization,
                    SubmissionConflicts = submissionConflicts,
                    ReviewerConflicts = reviewerConflicts,
                    ReviewerSubjectAreas = reviewerSubjectAreas,
                    Quota = x.Quota,
                    IsAssigned = x.IsAssigned,
                    NumberOfAssignments = x.NumberOfAssignments,
                    Relevance = CalculateRelevance(
                        result.SubmissionSubjectAreas ?? new List<AggregationSubjectAreaDto>(),
                        reviewerSubjectAreas ?? new List<AggregationSubjectAreaDto>(),
                        relevanceFormula ?? TrackConsts.DefaultSubjectAreaRelevanceCoefficients),
                    SortingFactor = sortingFactor
                });
            });

            var orderedReviewers = reviewers.OrderBy(x => x.SortingFactor).ThenBy(x => -x.Relevance).ToList();

            PagedResultDto<ReviewerAssignmentSuggestionDto> resultReviewers = new PagedResultDto<ReviewerAssignmentSuggestionDto>(orderedReviewers.Count, 
                orderedReviewers.Skip(input.SkipCount ?? 0).Take(input.MaxResultCount ?? PublicCoreflowConsts.DefaultMaxResultCount).ToList());

            result.Reviewers = resultReviewers;

            return result;
        }

        public async Task<PagedResultDto<SubmissionAggregationDto>?> GetTopAverageScoreSubmissionAggregationAsync(SubmissionAggregationInput input)
        {
            var foundItems = await _submissionRepository.GetTopAverageScoreSubmissionAggregationAsync
                (
                    input.InclusionText,
                    input.ConferenceId,
                    input.TrackId,
                    input.StatusId,
                    input.SkipCount == null ? 0 : input.SkipCount.Value,
                    input.MaxResultCount == null ? PublicCoreflowConsts.DefaultMaxResultCount : input.MaxResultCount.Value
                );

            // process output
            if (foundItems == null || foundItems.Count == 0)
                return null;

            var count = (long)foundItems.First().TotalCount.Value;

            List<SubmissionAggregationDto> items = new List<SubmissionAggregationDto>();

            foundItems.ForEach(x =>
            {
                items.Add(new SubmissionAggregationDto()
                {
                    PaperId = x.Id,
                    Title = x.Title,
                    Abstract = x.Abstract,
                    Authors = GetListAggregationSubmissionAuthor(x.SelectedAuthors),
                    SubjectAreas = GetListAggregationSubjectArea(x.SelectedSubmissionSubjectAreas),
                    TrackId = x.TrackId,
                    TrackName = x.TrackName,
                    SubmissionConflicts = x.SubmissionConflicts,
                    ReviewerConflicts = x.ReviewerConflicts,
                    Assigned = x.Assigned,
                    Reviewed = x.Reviewed,
                    AverageScore = x.AverageScore,
                    StatusId = x.StatusId,
                    Status = x.Status,
                    RevisionSubmitted = x.CloneNo == null ? null : x.LatestSubmissionCloneId != null,
                    RevisionNo = x.CloneNo,
                    IsRequestedForCameraReady = x.IsRequestedForCameraReady,
                    CameraReadySubmitted = x.CameraReadyId == null && x.IsRequestedForCameraReady == false ? null : x.CameraReadyId != null,
                    IsRequestedForPresentation = x.IsRequestedForPresentation
                });
            });

            return new PagedResultDto<SubmissionAggregationDto>(count, items);
        }

        public async Task<PagedResultDto<SubmissionAggregationDto>?> GetTopTimeSubmissionAggregationAsync(SubmissionAggregationInput input)
        {
            var foundItems = await _submissionRepository.GetTopTimeSubmissionAggregationAsync
                (
                    input.InclusionText,
                    input.ConferenceId,
                    input.TrackId,
                    input.StatusId,
                    input.SkipCount == null ? 0 : input.SkipCount.Value,
                    input.MaxResultCount == null ? PublicCoreflowConsts.DefaultMaxResultCount : input.MaxResultCount.Value
                );

            // process output
            if (foundItems == null || foundItems.Count == 0)
                return null;

            var count = (long)foundItems.First().TotalCount.Value;

            List<SubmissionAggregationDto> items = new List<SubmissionAggregationDto>();

            foundItems.ForEach(x =>
            {
                items.Add(new SubmissionAggregationDto()
                {
                    PaperId = x.Id,
                    Title = x.Title,
                    Abstract = x.Abstract,
                    Authors = GetListAggregationSubmissionAuthor(x.SelectedAuthors),
                    SubjectAreas = GetListAggregationSubjectArea(x.SelectedSubmissionSubjectAreas),
                    TrackId = x.TrackId,
                    TrackName = x.TrackName,
                    SubmissionConflicts = x.SubmissionConflicts,
                    ReviewerConflicts = x.ReviewerConflicts,
                    Assigned = x.Assigned,
                    Reviewed = x.Reviewed,
                    AverageScore = x.AverageScore,
                    StatusId = x.StatusId,
                    Status = x.Status,
                    RevisionSubmitted = x.CloneNo == null ? null : x.LatestSubmissionCloneId != null,
                    RevisionNo = x.CloneNo,
                    IsRequestedForCameraReady = x.IsRequestedForCameraReady,
                    CameraReadySubmitted = x.CameraReadyId == null && x.IsRequestedForCameraReady == false ? null : x.CameraReadyId != null,
                    IsRequestedForPresentation = x.IsRequestedForPresentation
                });
            });

            return new PagedResultDto<SubmissionAggregationDto>(count, items);
        }
    }
}