using Sras.PublicCoreflow.BlobContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
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

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IBlobContainer<SubmissionContainer> _submissionBlobContainer;

        private const string AwaitingDecision = "Awaiting Decision";

        public SubmissionAppService(IRepository<Track, Guid> trackRepository, IRepository<PaperStatus, Guid> paperStatusRepository, 
            IRepository<SubjectArea, Guid> subjectAreaRepository,
            IIncumbentRepository incumbentRepository,
            IConferenceRepository conferenceRepository,
            IRepository<ConferenceAccount, Guid> conferenceAccountRepository,
            IRepository<IdentityUser, Guid> userRepository,
            ICurrentUser currentUser, IGuidGenerator guidGenerator, 
            IBlobContainer<SubmissionContainer> submissionBlobContainer)
        {
            _trackRepository = trackRepository;
            _paperStatusRepository = paperStatusRepository;
            _subjectAreaRepository = subjectAreaRepository;
            _incumbentRepository = incumbentRepository;
            _conferenceRepository = conferenceRepository;
            _conferenceAccountRepository = conferenceAccountRepository;
            _userRepository = userRepository;
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
                input.DomainConflicts, null, null, input.Answers, awaitingDecisionPaperStatus.Id, null, null, false);

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
                submission.AddAuthor(x.ParticipantId, x.IsPrimaryContact);

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
                submission.AddSubmissionSubjectArea(x.SubjectAreaId, x.IsPrimary);
            });

            await _conferenceRepository.UpdateAsync(conference);
            track.AddSubmission(submission);
            await _trackRepository.UpdateAsync(track);

            return submission.Id;
        }

        public void CreateSubmissionFiles(Guid submissionId, List<RemoteStreamContent> files)
        {
            // Assume that the file extension is exactly matched its file name extension
            files.ForEach(async file =>
            {
                if (file != null && file.ContentLength > 0)
                {
                    await CreateSubmissionFilesAsync(submissionId.ToString() + "/" + file.FileName, file, true);
                }
            });
        }
    }
}
