using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using Sras.PublicCoreflow.Extension;
using static Sras.PublicCoreflow.ConferenceManagement.TrackAppService;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TrackAppService : PublicCoreflowAppService, ITrackAppService
    {
        private readonly IConferenceRepository _conferenceRepository;
        private readonly IIncumbentRepository _incumbentRepository;
        private readonly IRepository<Track, Guid> _trackRepository;
        private readonly IRepository<ActivityDeadline, Guid> _activityDeadlineRepository;
        private readonly IRepository<Guideline, Guid> _guidelineRepository;
        private readonly IRepository<Question, Guid> _questionRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ITrackRepository _trackRepository2;

        public TrackAppService(IConferenceRepository conferenceRepository, 
            IIncumbentRepository incumbentRepository, IRepository<Track, Guid> trackRepository, 
            ICurrentUser currentUser, IGuidGenerator guidGenerator, ITrackRepository trackRepository1,
            IRepository<ActivityDeadline, Guid> activityDeadlineRepository,
            IRepository<Guideline, Guid> guidelineRepository,
            IRepository<Question, Guid> questionRepository)
        {
            _conferenceRepository = conferenceRepository;
            _incumbentRepository = incumbentRepository;
            _trackRepository = trackRepository;
            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
            _trackRepository2 = trackRepository1;
            _activityDeadlineRepository = activityDeadlineRepository;
            _guidelineRepository = guidelineRepository;
            _questionRepository = questionRepository;
        }

        public async Task<List<TrackBriefInfo>?> GetAllAsync(Guid conferenceId)
        {
            var conference = await _conferenceRepository.FindAsync(conferenceId);

            if (conference == null)
            {
                return null;
            }
            else
            {
                return ObjectMapper.Map<List<Track>, List<TrackBriefInfo>>(conference.Tracks.ToList());
            }
        }

        public async Task<TrackBriefInfo?> CreateAsync(Guid conferenceId, string trackName)
        {
            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, conferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToAddConferenceTrack);
                }
            }

            var conference = await _conferenceRepository.FindAsync(conferenceId);
            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }
            else
            {
                if (conference.Tracks.Any(x => x.IsDefault))
                {
                    conference.DeleteTrack(conference.Tracks.Single(y => y.IsDefault).Id);
                }

                Guid trackId = _guidGenerator.Create();
                conference.IsSingleTrack = false;
                conference.AddTrack(new Track(trackId, false, trackName, conferenceId, null, null, null, null, null, null, null, null, JsonSerializer.Serialize(TrackConsts.DefaultSubjectAreaRelevanceCoefficients)));

                await _conferenceRepository.UpdateAsync(conference);
                return ObjectMapper.Map<Track, TrackBriefInfo>(await _trackRepository.FindAsync(trackId));
            }
        }

        public async Task<TrackBriefInfo?> UpdateTrackNameAsync(Guid conferenceId, Guid trackId, string trackName)
        {
            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, conferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var conference = await _conferenceRepository.FindAsync(conferenceId);
            if (conference == null)
                return null;
            else
            {
                var track = conference.Tracks.FirstOrDefault(x => x.Id == trackId);
                if (track == null)
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

                track.SetName(trackName);

                await _conferenceRepository.UpdateAsync(conference);
                return ObjectMapper.Map<Track, TrackBriefInfo>(await _trackRepository.FindAsync(trackId));
            }
        }

        public async Task<string?> UpdateSubmissionSettingsAsync(Guid id, string? submissionSettings, string? submissionInstruction)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.SubmissionSettings = submissionSettings;
            track.SubmissionInstruction = submissionInstruction;

            await _trackRepository.UpdateAsync(track);

            return submissionSettings;
        }

        public async Task<string?> GetSubmissionSettingsAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.SubmissionSettings;
        }

        public async Task<string?> GetSubmissionInstructionAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.SubmissionInstruction;
        }

        public async Task<string?> UpdateConflictSettingsAsync(Guid id, string? conflictSettings)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.ConflictSettings = conflictSettings;

            await _trackRepository.UpdateAsync(track);

            return conflictSettings;
        }

        public async Task<string?> GetConflictSettingsAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.ConflictSettings;
        }

        public async Task<string?> UpdateReviewSettingsAsync(Guid id, string? reviewSettings)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.ReviewSettings = reviewSettings;

            await _trackRepository.UpdateAsync(track);

            return reviewSettings;
        }

        public async Task<string?> GetReviewSettingsAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.ReviewSettings;
        }

        public async Task<string?> UpdateRevisionSettingsAsync(Guid id, string? revisionSettings)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.RevisionSettings = revisionSettings;

            await _trackRepository.UpdateAsync(track);

            return revisionSettings;
        }

        public async Task DeleteRevisionSettingsAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.RevisionSettings = null;

            await _trackRepository.UpdateAsync(track);
        }

        public async Task<string?> GetRevisionSettingsAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.RevisionSettings;
        }

        public async Task<string?> UpdateDecisionChecklistAsync(Guid id, string? decisionChecklist)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.DecisionChecklist = decisionChecklist;

            await _trackRepository.UpdateAsync(track);

            return decisionChecklist;
        }

        public async Task<string?> GetDecisionChecklistAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.DecisionChecklist;
        }

        public async Task<string?> UpdateCameraReadySubmissionSettingsAsync(Guid id, string? cameraReadySubmissionSettings)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.CameraReadySubmissionSettings = cameraReadySubmissionSettings;

            await _trackRepository.UpdateAsync(track);

            return cameraReadySubmissionSettings;
        }

        public async Task<string?> GetCameraReadySubmissionSettingsAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.CameraReadySubmissionSettings;
        }

        public async Task<string?> UpdatePresentationSettingsAsync(Guid id, string? presentationSettings)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            track.PresentationSettings = presentationSettings;

            await _trackRepository.UpdateAsync(track);

            return presentationSettings;
        }

        public async Task<string?> GetPresentationSettingsAsync(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            return track.PresentationSettings;
        }

        public async Task<object> GetAllTrackByConferenceId(Guid guid)
        {
            return await _trackRepository2.GetAllTrackByConferenceId(guid);
        }

        public async Task<object> CreateTrackAsync(Guid conferenceId, string trackName)
        {
            return await _trackRepository2.CreateTrackAsync(conferenceId, trackName);
        }


        public async Task<object> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId)
        {
            return await _trackRepository2.GetTracksAndRoleOfUser(userId, conferenceId);
        }

        public async Task<object?> UpdateTrackSubjectAreaRelevanceCoefficientsAsync(Guid trackId, SubjectAreaRelevanceCoefficients input)
        {
            try
            {
                var track = await _trackRepository.FindAsync(x => x.Id == trackId);
                if (track == null)
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

                if (input.IsDefault)
                {
                    track.SubjectAreaRelevanceCoefficients = JsonSerializer.Serialize(TrackConsts.DefaultSubjectAreaRelevanceCoefficients);
                }
                else
                {
                    track.SubjectAreaRelevanceCoefficients = JsonSerializer.Serialize(input);
                }

                await _trackRepository.UpdateAsync(track);
                return new
                {
                    Id = track.Id,
                    Name = track.Name,
                    SubjectAreaRelevanceCoefficients = JsonSerializer.Deserialize<SubjectAreaRelevanceCoefficients>(track.SubjectAreaRelevanceCoefficients)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<object?> GetTrackSubjectAreaRelevanceCoefficientsAsync(Guid trackId)
        {
            var track = await _trackRepository.FindAsync(x => x.Id == trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            return new
            {
                Id = track.Id,
                Name = track.Name,
                SubjectAreaRelevanceCoefficients = track.SubjectAreaRelevanceCoefficients == null ? null : JsonSerializer.Deserialize<SubjectAreaRelevanceCoefficients>(track.SubjectAreaRelevanceCoefficients)
            };
        }

        private List<TrackPlanRecordInput> GetInitialListTrackProgressRecord(Guid trackId, int numberOfRevisions,
            DateTime startDate, DateTime endDate)
        {
            int id = 0;

            List<TrackPlanRecordInput> list = new List<TrackPlanRecordInput>()
            {
                new TrackPlanRecordInput
                {
                    Id = _guidGenerator.Create(),
                    TrackId = trackId,
                    Phase = ActivityDeadlineConsts.StartDatePhase,
                    Name = ActivityDeadlineConsts.StartDate,
                    PlanDeadline = startDate,
                    Deadline = startDate,
                    IsCurrent = true,
                    IsNext = false,
                    Status = ActivityDeadlineConsts.EnabledStatus,
                    CompletionTime = null,
                    GuidelineGroup = ActivityDeadlineConsts.PreSubmissionGuidelineGroup,
                    IsGuidelineShowed = false,
                    Factor = 1,
                    IsBeginPhaseMark = true,
                    CanSkip = false,
                },
                new TrackPlanRecordInput
                {
                    Id = _guidGenerator.Create(),
                    TrackId = trackId,
                    Phase = ActivityDeadlineConsts.CallForPapersDeadlinePhase,
                    Name = ActivityDeadlineConsts.CallForPapersDeadline,
                    PlanDeadline = null,
                    Deadline = null,
                    IsCurrent = false,
                    IsNext = true,
                    Status = ActivityDeadlineConsts.EnabledStatus,
                    CompletionTime = null,
                    GuidelineGroup = null,
                    IsGuidelineShowed = false,
                    Factor = 2,
                    IsBeginPhaseMark = true,
                    CanSkip = false,
                },
                new TrackPlanRecordInput
                {
                    Id = _guidGenerator.Create(),
                    TrackId = trackId,
                    Phase = ActivityDeadlineConsts.SubmissionDeadlinePhase,
                    Name = ActivityDeadlineConsts.SubmissionDeadline,
                    PlanDeadline = null,
                    Deadline = null,
                    IsCurrent = false,
                    IsNext = false,
                    Status = ActivityDeadlineConsts.EnabledStatus,
                    CompletionTime = null,
                    GuidelineGroup = ActivityDeadlineConsts.PreReviewSubmissionGuidelineGroup,
                    IsGuidelineShowed = false,
                    Factor = 3,
                    IsBeginPhaseMark = false,
                    CanSkip = false,
                },
                new TrackPlanRecordInput
                {
                    Id = _guidGenerator.Create(),
                    TrackId = trackId,
                    Phase = ActivityDeadlineConsts.SubmissionEditsDeadlinePhase,
                    Name = ActivityDeadlineConsts.SubmissionEditsDeadline,
                    PlanDeadline = null,
                    Deadline = null,
                    IsCurrent = false,
                    IsNext = false,
                    Status = ActivityDeadlineConsts.EnabledStatus,
                    CompletionTime = null,
                    GuidelineGroup = null,
                    IsGuidelineShowed = false,
                    Factor = 4,
                    IsBeginPhaseMark = false,
                    CanSkip = true,
                },
                new TrackPlanRecordInput
                {
                    Id = _guidGenerator.Create(),
                    TrackId = trackId,
                    Phase = ActivityDeadlineConsts.SupplementaryMaterialDeadlinePhase,
                    Name = ActivityDeadlineConsts.SupplementaryMaterialDeadline,
                    PlanDeadline = null,
                    Deadline = null,
                    IsCurrent = false,
                    IsNext = false,
                    Status = ActivityDeadlineConsts.EnabledStatus,
                    CompletionTime = null,
                    GuidelineGroup = null,
                    IsGuidelineShowed = false,
                    Factor = 5,
                    IsBeginPhaseMark = false,
                    CanSkip = true,
                },
                new TrackPlanRecordInput
                {
                    Id = _guidGenerator.Create(),
                    TrackId = trackId,
                    Phase = ActivityDeadlineConsts.ReviewSubmissionDeadlinePhase,
                    Name = ActivityDeadlineConsts.ReviewSubmissionDeadline,
                    PlanDeadline = null,
                    Deadline = null,
                    IsCurrent = false,
                    IsNext = false,
                    Status = ActivityDeadlineConsts.EnabledStatus,
                    CompletionTime = null,
                    GuidelineGroup = ActivityDeadlineConsts.PreResultNotificationGuidelineGroup,
                    IsGuidelineShowed = false,
                    Factor = 6,
                    IsBeginPhaseMark = false,
                    CanSkip = false,
                }
            };

            for (int i = 1; i <= numberOfRevisions; i++)
            {
                list.Add(
                    new TrackPlanRecordInput
                    {
                        Id = _guidGenerator.Create(),
                        TrackId = trackId,
                        Phase = ActivityDeadlineConsts.RevisionNSubmissionDeadlinePhase.Replace("{n}", i.ToString()),
                        Name = ActivityDeadlineConsts.RevisionNSubmissionDeadline.Replace("{n}", i.ToString()),
                        PlanDeadline = null,
                        Deadline = null,
                        IsCurrent = false,
                        IsNext = false,
                        Status = ActivityDeadlineConsts.EnabledStatus,
                        CompletionTime = null,
                        GuidelineGroup = null,
                        IsGuidelineShowed = false,
                        Factor = 5+2*i,
                        IsBeginPhaseMark = false,
                        CanSkip = true,
                    });
                list.Add(
                    new TrackPlanRecordInput
                    {
                        Id = _guidGenerator.Create(),
                        TrackId = trackId,
                        Phase = ActivityDeadlineConsts.RevisionNReviewSubmissionDeadlinePhase.Replace("{n}", i.ToString()),
                        Name = ActivityDeadlineConsts.RevisionNReviewSubmissionDeadline.Replace("{n}", i.ToString()),
                        PlanDeadline = null,
                        Deadline = null,
                        IsCurrent = false,
                        IsNext = false,
                        Status = ActivityDeadlineConsts.EnabledStatus,
                        CompletionTime = null,
                        GuidelineGroup = null,
                        IsGuidelineShowed = false,
                        Factor = 6+2*i,
                        IsBeginPhaseMark = false,
                        CanSkip = true,
                    });
            }

            list.Add(
                    new TrackPlanRecordInput
                    {
                        Id = _guidGenerator.Create(),
                        TrackId = trackId,
                        Phase = ActivityDeadlineConsts.ResultNotificationDeadlinePhase,
                        Name = ActivityDeadlineConsts.ResultNotificationDeadline,
                        PlanDeadline = null,
                        Deadline = null,
                        IsCurrent = false,
                        IsNext = false,
                        Status = ActivityDeadlineConsts.EnabledStatus,
                        CompletionTime = null,
                        GuidelineGroup = ActivityDeadlineConsts.PreCameraReadySubmissionGuidelineGroup,
                        IsGuidelineShowed = false,
                        Factor = 6 + 2 * numberOfRevisions + 1,
                        IsBeginPhaseMark = false,
                        CanSkip = false,
                    });
            list.Add(
                    new TrackPlanRecordInput
                    {
                        Id = _guidGenerator.Create(),
                        TrackId = trackId,
                        Phase = ActivityDeadlineConsts.CameraReadySubmissionDeadlinePhase,
                        Name = ActivityDeadlineConsts.CameraReadySubmissionDeadline,
                        PlanDeadline = null,
                        Deadline = null,
                        IsCurrent = false,
                        IsNext = false,
                        Status = ActivityDeadlineConsts.EnabledStatus,
                        CompletionTime = null,
                        GuidelineGroup = ActivityDeadlineConsts.PrePresentationSubmissionGuidelineGroup,
                        IsGuidelineShowed = false,
                        Factor = 6 + 2 * numberOfRevisions + 2,
                        IsBeginPhaseMark = false,
                        CanSkip = false,
                    });
            list.Add(
                    new TrackPlanRecordInput
                    {
                        Id = _guidGenerator.Create(),
                        TrackId = trackId,
                        Phase = ActivityDeadlineConsts.PresentationSubmissionDeadlinePhase,
                        Name = ActivityDeadlineConsts.PresentationSubmissionDeadline,
                        PlanDeadline = null,
                        Deadline = null,
                        IsCurrent = false,
                        IsNext = false,
                        Status = ActivityDeadlineConsts.EnabledStatus,
                        CompletionTime = null,
                        GuidelineGroup = null,
                        IsGuidelineShowed = false,
                        Factor = 6 + 2 * numberOfRevisions + 3,
                        IsBeginPhaseMark = false,
                        CanSkip = false,
                    });
            list.Add(
                    new TrackPlanRecordInput
                    {
                        Id = _guidGenerator.Create(),
                        TrackId = trackId,
                        Phase = ActivityDeadlineConsts.EndDatePhase,
                        Name = ActivityDeadlineConsts.EndDate,
                        PlanDeadline = endDate,
                        Deadline = endDate,
                        IsCurrent = false,
                        IsNext = false,
                        Status = ActivityDeadlineConsts.EnabledStatus,
                        CompletionTime = null,
                        GuidelineGroup = null,
                        IsGuidelineShowed = false,
                        Factor = 6 + 2 * numberOfRevisions + 4,
                        IsBeginPhaseMark = false,
                        CanSkip = false,
                    });

            return list;
        }

        public async Task<List<TrackPlanRecordInput>> InitializeTrackPlan(Guid id, int numberOfRevisions)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            if (numberOfRevisions < 0)
            {
                throw new Exception("NumberOfRevisions cannot be a negative number");
            }

            RevisionSettings? revisionSettings;
            if (string.IsNullOrEmpty(track.RevisionSettings))
            {
                revisionSettings = new RevisionSettings()
                {
                    NumberOfRevisions = numberOfRevisions
                };

                var initialListTrackProgressRecord = GetInitialListTrackProgressRecord(track.Id, revisionSettings.NumberOfRevisions.Value, conference.StartDate, conference.EndDate);

                track.RevisionSettings = JsonSerializer.Serialize<RevisionSettings>(revisionSettings);
                await _trackRepository.UpdateAsync(track);

                // return Initialized Track Plan
                return initialListTrackProgressRecord;

            }
            else
            {
                revisionSettings = JsonSerializer.Deserialize<RevisionSettings>(track.RevisionSettings);
                if (revisionSettings?.NumberOfRevisions != null)
                {
                    throw new BusinessException("NumberOfRevisions Already Set");
                }
                else
                {
                    if (revisionSettings != null)
                    {
                        revisionSettings.NumberOfRevisions = numberOfRevisions;

                        var initialListTrackProgressRecord = GetInitialListTrackProgressRecord(track.Id, revisionSettings.NumberOfRevisions.Value, conference.StartDate, conference.EndDate);

                        track.RevisionSettings = JsonSerializer.Serialize<RevisionSettings>(revisionSettings);
                        await _trackRepository.UpdateAsync(track);

                        // return Initialized Track Plan
                        return initialListTrackProgressRecord;
                    }

                    throw new Exception("Cannot Parse Revision Settings Json");
                }
            }
        }

        public async Task<List<TrackPlanRecordInput>> GetInitialTrackPlan(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            if (string.IsNullOrEmpty(track.RevisionSettings))
            {
                throw new Exception("Revision Settings Not Set");
            }
            else
            {
                RevisionSettings? revisionSettings = JsonSerializer.Deserialize<RevisionSettings>(track.RevisionSettings);
                if (revisionSettings?.NumberOfRevisions != null)
                {
                    return GetInitialListTrackProgressRecord(track.Id, revisionSettings.NumberOfRevisions.Value, conference.StartDate, conference.EndDate);
                }
                else
                {
                    throw new Exception("Cannot Find the NumberOfRevisions Settings");
                }
            }
        }
    
        public async Task<List<TrackPlanRecordInput>> SaveTrackPlanAsync(Guid trackId, List<TrackPlanRecordInput> trackPlanRecords)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            // Check if exist plan
            if(await _activityDeadlineRepository.AnyAsync(x => x.TrackId == trackId))
            {
                throw new BusinessException("Track Plan already existed");
            }

            trackPlanRecords = trackPlanRecords.Where(x => !x.Status.ToLower().Equals(ActivityDeadlineConsts.DisabledStatus.ToLower())).ToList();

            int twiceNumberOfRevisions = 0;

            // Calculate number of revisions
            for (int i = 0; i < trackPlanRecords.Count; i++)
            {
                trackPlanRecords[i].Factor = i + 1;
                trackPlanRecords[i].Deadline = trackPlanRecords[i].PlanDeadline;

                if (trackPlanRecords[i].Name.ToLower().StartsWith("revision"))
                {
                    twiceNumberOfRevisions++;
                }
            }

            RevisionSettings? revisionSettings = null;

            if (string.IsNullOrEmpty(track.RevisionSettings))
            {
                revisionSettings = new RevisionSettings()
                {
                    NumberOfRevisions = twiceNumberOfRevisions / 2
                };
            }
            else
            {
                revisionSettings = JsonSerializer.Deserialize<RevisionSettings>(track.RevisionSettings);

                if (revisionSettings == null)
                {
                    throw new Exception("Cannot parse Revision Settings");
                }

                revisionSettings.NumberOfRevisions = twiceNumberOfRevisions / 2;
            }

            track.RevisionSettings = JsonSerializer.Serialize<RevisionSettings>(revisionSettings);
            await _trackRepository.UpdateAsync(track);

            List<ActivityDeadline> activityDeadlines = new List<ActivityDeadline>();
            trackPlanRecords.ForEach(record =>
            {
                activityDeadlines.Add(
                    new ActivityDeadline(record.Id, trackId, record.Phase, record.Name, record.PlanDeadline?.Date, record.PlanDeadline?.Date, record.IsCurrent, record.IsNext, ActivityDeadlineConsts.Enabled, 
                    null, record.GuidelineGroup, record.IsGuidelineShowed, record.Factor, record.IsBeginPhaseMark, record.CanSkip));
            });

            await _activityDeadlineRepository.InsertManyAsync(activityDeadlines);

            return trackPlanRecords;
        }

        public async Task<List<TrackPlanRecordInput>> GetTrackActivityTimeline(Guid id)
        {
            var track = await _trackRepository.FindAsync(id);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var deadlines = await _activityDeadlineRepository.GetListAsync(x => x.TrackId == track.Id && !x.Name.ToLower().Equals(ActivityDeadlineConsts.StartDate.ToLower())
            && !x.Name.ToLower().Equals(ActivityDeadlineConsts.EndDate.ToLower()));

            return ObjectMapper.Map<List<ActivityDeadline>, List<TrackPlanRecordInput>>(deadlines);
        }

        public async Task<List<TrackPlanRecordInput>> ExtendActivityDeadline(Guid trackId, TrackPlanRecordInput activityDeadline)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var deadlines = await _activityDeadlineRepository.GetListAsync(x => x.TrackId == track.Id && !x.Name.ToLower().Equals(ActivityDeadlineConsts.StartDate.ToLower())
            && !x.Name.ToLower().Equals(ActivityDeadlineConsts.EndDate.ToLower()));

            var currentPlanDeadline = deadlines.First(x => x.IsCurrent).PlanDeadline;
            if (currentPlanDeadline == null)
            {
                throw new BusinessException("Current Plan Deadline not Existed");
            }

            var nextPlanDeadline = deadlines.First(x => x.IsNext).PlanDeadline;
            if (nextPlanDeadline == null)
            {
                throw new BusinessException("Next Plan Deadline not Existed");
            }

            if ((DateTime.Now.Date.AddDays(1) - currentPlanDeadline.Value.Date).Days < 0)
            {
                throw new BusinessException("Cannot extend Deadline before the day before the Plan Deadline");
            }
            else
            {
                var updatingDeadline = deadlines.Single(x => x.Id == activityDeadline.Id);

                var today = DateTimeExtensions.GetToday();
                var oldDeadline = updatingDeadline.Deadline.Value;
                var newDeadline = activityDeadline.Deadline.Value;

                if (!today.IsLessThan(DateTimeExtensions.MinDate(oldDeadline, newDeadline)))
                {
                    throw new Exception("Both the new Deadline and the old Deadline should be at least a day after today");
                }
                else
                {
                    if (!newDeadline.IsGreaterThan(DateTimeExtensions.MaxDate(today, currentPlanDeadline.Value)))
                    {
                        throw new Exception("The new Deadline should greater both today and plan deadline");
                    }
                    else
                    {
                        if(!newDeadline.IsLessThan(nextPlanDeadline.Value))
                        {
                            throw new Exception("The new deadline should less than the next plan deadline");
                        }
                        else
                        {
                            // updating

                            deadlines.ForEach(x =>
                            {
                                if(x.Factor >= updatingDeadline.Factor && x.Deadline.Value.Date == updatingDeadline.Deadline.Value.Date)
                                {
                                    x.Deadline = activityDeadline.Deadline;
                                }
                            });

                            await _activityDeadlineRepository.UpdateManyAsync(deadlines);

                            return ObjectMapper.Map<List<ActivityDeadline>, List<TrackPlanRecordInput>>(deadlines);
                        }
                    }
                }
            }
        }

        public async Task<TrackPlanRecordInput> CompleteActivityDeadlineAsync(Guid trackId, Guid activityDeadlineId)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var activityDeadline = await _activityDeadlineRepository.FindAsync(activityDeadlineId);

            if(activityDeadline == null)
            {
                throw new BusinessException("Cannot find activity deadline");
            }

            activityDeadline.Status = ActivityDeadlineConsts.Completed;
            activityDeadline.CompletionTime = DateTime.Now.Date;

            await _activityDeadlineRepository.UpdateAsync(activityDeadline);

            return ObjectMapper.Map<ActivityDeadline, TrackPlanRecordInput>(activityDeadline);
        }
    
        public async Task<GuidelineGroupDto> GetGuidelines(Guid trackId, bool isForChair)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var isNotFirstTime = await _activityDeadlineRepository.AnyAsync(x => x.TrackId == trackId);

            if (!isNotFirstTime)
            {
                var guidelines = await _guidelineRepository
                    .GetListAsync(x => x.GuidelineGroup.ToLower().Equals(ActivityDeadlineConsts.PreSubmissionGuidelineGroup.ToLower()));

                guidelines = guidelines.OrderBy(x => x.Factor).ToList();

                return new GuidelineGroupDto
                {
                    GuidelineGroup = ActivityDeadlineConsts.PreSubmissionGuidelineGroup,
                    Guidelines = ObjectMapper.Map<List<Guideline>, List<TrackGuidelineDto>>(guidelines)
                };
            }

            var guideline = await _activityDeadlineRepository.FirstOrDefaultAsync(x => x.IsCurrent && !string.IsNullOrWhiteSpace(x.GuidelineGroup) && !x.IsGuidelineShowed);

            if (guideline == null)
            {
                return new GuidelineGroupDto
                {
                    GuidelineGroup = null,
                    Guidelines = null,
                };
            }
            else
            {
                var guidelines = await _guidelineRepository.GetListAsync(x => (x.IsChairOnly == isForChair && !x.IsChairOnly) && x.GuidelineGroup.ToLower().Equals(guideline.GuidelineGroup.ToLower()));

                guidelines = guidelines.OrderBy(x => x.Factor).ToList();

                return new GuidelineGroupDto
                {
                    GuidelineGroup = guideline.GuidelineGroup,
                    Guidelines = ObjectMapper.Map<List<Guideline>, List<TrackGuidelineDto>>(guidelines)
                };
            }
        }

        public class QuestionOperation
        {
            public Question Question { get; set; }
            public QuestionManipulationOperators Operation { get; set; }
        }

        public async Task<ResponseDto> CreateOrUpdateQuestionListAsync(Guid trackId, QuestionListInput input)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            // Skip clean input

            // OldList
            var existedQuestions = await _questionRepository.GetListAsync(x => x.TrackId == trackId);

            if (input.Questions == null)
                input.Questions = new List<QuestionDto>();

            List<QuestionOperation> questionOperations = new List<QuestionOperation>();

            for (int i = 0; i < input.Questions.Count; i++)
            {
                var q = new Question(
                    input.Questions[i].Id,
                    input.Questions[i].QuestionGroupId,
                    input.Questions[i].TrackId,
                    input.Questions[i].Title,
                    input.Questions[i].Text,
                    input.Questions[i].IsRequired,
                    input.Questions[i].IsVisibleToReviewers,
                    input.Questions[i].Type,
                    input.Questions[i].TypeName,
                    JsonSerializer.Serialize(input.Questions[i].ShowAs),
                    i);

                questionOperations.Add(new QuestionOperation
                {
                    Question = q,
                    Operation = existedQuestions.Exists(x => x.Id == q.Id) ?
                    QuestionManipulationOperators.Update : QuestionManipulationOperators.Add
                });
            }

            existedQuestions.ForEach(x =>
            {
                if (!input.Questions.Exists(y => y.Id == x.Id))
                {
                    questionOperations.Add(new QuestionOperation
                    {
                        Question = x,
                        Operation = QuestionManipulationOperators.Delete
                    });
                }
            });

            questionOperations.ForEach(async x =>
            {
                if(x.Operation == QuestionManipulationOperators.Delete)
                {
                    await _questionRepository.DeleteAsync(x.Question.Id);
                }
                else if(x.Operation == QuestionManipulationOperators.Add)
                {
                    await _questionRepository.InsertAsync(x.Question);
                }
                else if(x.Operation == QuestionManipulationOperators.Update)
                {
                    await _questionRepository.UpdateAsync(x.Question);
                }
            });

            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Update Question List successfully"
            };
        }
    
        public async Task<List<QuestionDto>> GetSubmissionQuestionListAsync(Guid trackId)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var existedQuestions = await _questionRepository.GetListAsync(x => x.TrackId == trackId && x.QuestionGroupId 
            == QuestionGroup.DefaultQuestionGroups.SubmissionQuestionGroup.Id);

            existedQuestions = existedQuestions.OrderBy(x => x.Index).ToList();

            var result = new List<QuestionDto>();

            existedQuestions.ForEach(x =>
            {
                result.Add(new QuestionDto
                {
                    Id = x.Id,
                    QuestionGroupId = x.QuestionGroupId,
                    TrackId = x.TrackId,
                    Title = x.Title,
                    Text = x.Text,
                    IsRequired = x.IsRequired,
                    IsVisibleToReviewers = x.IsVisibleToReviewers,
                    Type = x.Type,
                    TypeName = x.TypeName,
                    ShowAs = JsonSerializer.Deserialize<ShowAsDto>(x.ShowAs)
                });
            });

            return result;
        }

        public async Task<List<QuestionDto>> GetDecisionChecklistQuestionsAsync(Guid trackId)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var existedQuestions = await _questionRepository.GetListAsync(x => x.TrackId == trackId && x.QuestionGroupId
            == QuestionGroup.DefaultQuestionGroups.DecisionChecklistGroup.Id);

            existedQuestions = existedQuestions.OrderBy(x => x.Index).ToList();

            var result = new List<QuestionDto>();

            existedQuestions.ForEach(x =>
            {
                result.Add(new QuestionDto
                {
                    Id = x.Id,
                    QuestionGroupId = x.QuestionGroupId,
                    TrackId = x.TrackId,
                    Title = x.Title,
                    Text = x.Text,
                    IsRequired = x.IsRequired,
                    IsVisibleToReviewers = x.IsVisibleToReviewers,
                    Type = x.Type,
                    TypeName = x.TypeName,
                    ShowAs = JsonSerializer.Deserialize<ShowAsDto>(x.ShowAs)
                });
            });

            return result;
        }

        public async Task<List<QuestionDto>> GetCameraReadyChecklistQuestionsAsync(Guid trackId)
        {
            var track = await _trackRepository.FindAsync(trackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var conference = await _conferenceRepository.FindAsync(track.ConferenceId);
            if (conference == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            if (_currentUser != null && _currentUser.Id != null)
            {
                var isChair = await _incumbentRepository.IsConferenceChair(_currentUser.Id.Value, track.ConferenceId);
                if (!isChair)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConferenceTrack);
                }
            }

            var existedQuestions = await _questionRepository.GetListAsync(x => x.TrackId == trackId && x.QuestionGroupId
            == QuestionGroup.DefaultQuestionGroups.CameraReadyChecklistGroup.Id);

            existedQuestions = existedQuestions.OrderBy(x => x.Index).ToList();

            var result = new List<QuestionDto>();

            existedQuestions.ForEach(x =>
            {
                result.Add(new QuestionDto
                {
                    Id = x.Id,
                    QuestionGroupId = x.QuestionGroupId,
                    TrackId = x.TrackId,
                    Title = x.Title,
                    Text = x.Text,
                    IsRequired = x.IsRequired,
                    IsVisibleToReviewers = x.IsVisibleToReviewers,
                    Type = x.Type,
                    TypeName = x.TypeName,
                    ShowAs = JsonSerializer.Deserialize<ShowAsDto>(x.ShowAs)
                });
            });

            return result;
        }
    }
}
