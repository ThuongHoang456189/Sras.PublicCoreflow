namespace Sras.PublicCoreflow.ConferenceManagement
{
    public static class AccountConsts
    {
        public const string DefaultSorting = "Email asc";

        public const string FirstNamePropertyName = "Name";
        public const string LastNamePropertyName = "Surname";
        public const string MiddleNamePropertyName = "MiddleName";
        public const string OrganizationPropertyName = "Organization";
        public const string CountryPropertyName = "Country";
        public const string DomainConflictsPropertyName = "DomainConflicts";
        public const string NamePrefixPropertyName = "NamePrefix";

        public const int MaxMiddleNameLength = 64;
        public const int MaxOrganizationLength = 128;
        public const int MaxCountryLength = 64;
        public const int MaxDomainConflictsLength = 1024;
        public const int MaxNamePrefixLength = 32;
    }
}