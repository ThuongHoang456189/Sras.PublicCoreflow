using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TrackAppService : PublicCoreflowAppService, ITrackAppService
    {
        private readonly IConferenceRepository _conferenceRepository;
        private readonly IIncumbentRepository _incumbentRepository;
        private readonly IRepository<Track, Guid> _trackRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ITrackRepository _trackRepository2;

        public TrackAppService(IConferenceRepository conferenceRepository, IIncumbentRepository incumbentRepository, IRepository<Track, Guid> trackRepository, ICurrentUser currentUser, IGuidGenerator guidGenerator, ITrackRepository trackRepository1)
        {
            _conferenceRepository = conferenceRepository;
            _incumbentRepository = incumbentRepository;
            _trackRepository = trackRepository;
            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
            _trackRepository2 = trackRepository1;
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

            return track.SubmissionSettings;
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
    }
}
