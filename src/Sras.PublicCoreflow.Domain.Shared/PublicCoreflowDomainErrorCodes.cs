namespace Sras.PublicCoreflow;

public static class PublicCoreflowDomainErrorCodes
{
    private const string Prefix = "Sras:";

    // Conference Related Error Codes
    private const string ConferencePrefix = Prefix + "ConferenceManagement:";

    public const string AccountAlreadyAddedToConference = ConferencePrefix + "AccountAlreadyAddedToConference";
    public const string ConferenceAccountNotFound = ConferencePrefix + "ConferenceAccountNotFound";
    public const string PaperStatusAlreadyExistToConference = ConferencePrefix + "PaperStatusAlreadyExistToConference";
    public const string PaperStatusNotFound = ConferencePrefix + "PaperStatusNotFound";
    public const string TrackAlreadyExistToConference = ConferencePrefix + "TrackAlreadyExistToConference";
    public const string TrackNotFound = ConferencePrefix + "TrackNotFound";
    public const string ConferenceAlreadyExist = ConferencePrefix + "ConferenceAlreadyExist";
    public const string ConferenceNotFound = ConferencePrefix + "ConferenceNotFound";
    public const string UserNotAuthorizedToUpdateConference = ConferencePrefix + "UserNotAuthorizedToUpdateConference";
    public const string UserNotAuthorizedToDeleteConference = ConferencePrefix + "UserNotAuthorizedToDeleteConference";
    public const string IncumbentAlreadyAssigned = ConferencePrefix + "IncumbentAlreadyAssigned";
    public const string IncumbentNotFound = ConferencePrefix + "IncumbentNotFound";
    public const string NoAssignmentOfChairsToConference = ConferencePrefix + "NoAssignmentOfChairsToConference";
    public const string InvalidAccountOnChairList = ConferencePrefix + "InvalidAccountOnChairList";
    public const string UserNotAuthorizedToProceedToUpdateConferenceRoles = ConferencePrefix + "UserNotAuthorizedToProceedToUpdateConferenceRoles";
    public const string AccountNotFound = ConferencePrefix + "AccountNotFound";
    public const string UserNotAuthorizedToAddConferenceTrack = ConferencePrefix + "UserNotAuthorizedToAddConferenceTrack";
    public const string UserNotAuthorizedToUpdateConferenceTrack = ConferencePrefix + "UserNotAuthorizedToUpdateConferenceTrack";
    public const string SubjectAreaAlreadyExistToTrack = ConferencePrefix + "SubjectAreaAlreadyExistToTrack";
    public const string SubjectAreaNotFound = ConferencePrefix + "SubjectAreaNotFound";
    public const string SubmissionAlreadyExistToTrack = ConferencePrefix + "SubmissionAlreadyExistToTrack";
    public const string SubjectAreaAlreadyExistToSubmission = ConferencePrefix + "SubjectAreaAlreadyExistToSubmission";
    public const string AuthorAlreadyExistToSubmission = ConferencePrefix + "AuthorAlreadyExistToSubmission";
    public const string AwaitingDecisionPaperStatusNotFound = ConferencePrefix + "AwaitingDecisionPaperStatusNotFound";
    public const string PrimaryContactCannotSetToNonAccountParticipant = ConferencePrefix + "PrimaryContactCannotSetToNonAccountParticipant";
    public const string ConferenceRoleAuthorNotFound = ConferencePrefix + "ConferenceRoleAuthorNotFound";
    public const string ReviewerAlreadyExist = ConferencePrefix + "ReviewerAlreadyExist";
    public const string ReviewerNotFound = ConferencePrefix + "ReviewerNotFound";
}
