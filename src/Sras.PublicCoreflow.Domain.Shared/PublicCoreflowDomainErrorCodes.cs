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
}
