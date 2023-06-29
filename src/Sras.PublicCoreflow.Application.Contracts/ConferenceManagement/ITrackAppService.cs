using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ITrackAppService : IApplicationService
    {
        Task<TrackBriefInfo?> CreateAsync(Guid conferenceId, string trackName);
        Task<TrackBriefInfo?> UpdateTrackNameAsync(Guid conferenceId, Guid trackId, string trackName);
        Task<List<TrackBriefInfo>?> GetAllAsync(Guid conferenceId);
        Task<object> GetAllTrackByConferenceId(Guid guid);
        Task<object> CreateTrackAsync(Guid conferenceId, string trackName);
        Task<object> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId);
        Task<object?> UpdateTrackSubjectAreaRelevanceCoefficientsAsync(Guid trackId, SubjectAreaRelevanceCoefficients input);
        Task<object?> GetTrackSubjectAreaRelevanceCoefficientsAsync(Guid trackId);
        Task<string?> UpdateSubmissionSettingsAsync(Guid id, string? submissionSettings, string? submissionInstruction);
        Task<string?> GetSubmissionSettingsAsync(Guid id);
        Task<string?> GetSubmissionInstructionAsync(Guid id);
        Task<string?> UpdateConflictSettingsAsync(Guid id, string? conflictSettings);
        Task<string?> GetConflictSettingsAsync(Guid id);
        Task<string?> UpdateReviewSettingsAsync(Guid id, string? reviewSettings);
        Task<string?> GetReviewSettingsAsync(Guid id);
        Task<string?> UpdateRevisionSettingsAsync(Guid id, string? revisionSettings);
        Task<string?> GetRevisionSettingsAsync(Guid id);
        Task<string?> UpdateDecisionChecklistAsync(Guid id, string? decisionChecklist);
        Task<string?> GetDecisionChecklistAsync(Guid id);
        Task<string?> UpdateCameraReadySubmissionSettingsAsync(Guid id, string? cameraReadySubmissionSettings);
        Task<string?> GetCameraReadySubmissionSettingsAsync(Guid id);
        Task<string?> UpdatePresentationSettingsAsync(Guid id, string? presentationSettings);
        Task<string?> GetPresentationSettingsAsync(Guid id);
    }
}
