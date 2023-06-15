using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public static class ConferenceConsts
    {
        public const string DefaultSorting = "ShortName asc";
        public const int MaxResultCount = 50;

        public const int MaxFullnameLength = 200;
        public const int MaxShortNameLength = 50;
        public const int MaxCityLength = 100;
        public const int MaxCountryLength = 100;

        public const int MaxConferenceUserEachPage = 50;

        public static PriceTable DefaultPriceTable = new PriceTable
        {
            MaxValidNumberOfPages = 10,
            IsEarlyRegistrationEnabled = true,
            EarlyRegistrationDeadline = new DateTime(day: 19, month: 8, year: 2023),
            MaxNumberOfExtraPapers = 2,
            Rows = new List<PriceTableRow>
            {
                new PriceTableRow
                {
                    Option = "IEEE Member",
                    EarlyRegistration = 780,
                    RegularRegistration = 840
                },
                new PriceTableRow
                {
                    Option = "Non IEEE Member",
                    EarlyRegistration = 900,
                    RegularRegistration = 960
                },
                new PriceTableRow
                {
                    Option = "Student IEEE Member",
                    EarlyRegistration = 300,
                    RegularRegistration = 350
                },
                new PriceTableRow
                {
                    Option = "Student Non IEEE Member",
                    EarlyRegistration = 350,
                    RegularRegistration = 400
                },
                new PriceTableRow
                {
                    Option = "IEEE Life Member",
                    EarlyRegistration = 300,
                    RegularRegistration = 350
                },
                new PriceTableRow
                {
                    Option = "Charge: One Extra Page",
                    EarlyRegistration = 113,
                    RegularRegistration = 113
                },
                new PriceTableRow
                {
                    Option = "Charge: One Extra Paper",
                    EarlyRegistration = 200,
                    RegularRegistration = 200
                },
                new PriceTableRow
                {
                    Option = "Charge: Two Extra Paper",
                    EarlyRegistration = 350,
                    RegularRegistration = 350
                }
            }
        };
    }
}
