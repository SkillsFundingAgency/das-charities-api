using Newtonsoft.Json;
using SFA.DAS.Charities.Domain.Entities;
using System;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels
{
    public class CharityModel
    {
        [JsonProperty("date_of_extract")]
        public DateTime ExtractedDate { get; set; }
        [JsonProperty("organisation_number")]
        public int CharityId { get; set; }
        [JsonProperty("registered_charity_number")]
        public int RegisteredCharityNumber { get; set; }
        [JsonProperty("charity_name")]
        public string Name { get; set; }
        [JsonProperty("linked_charity_number")]
        public int LinkedCharityNumber { get; set; }
        [JsonProperty("charity_type")]
        public string Type { get; set; }
        [JsonProperty("charity_registration_status")]
        public string RegistrationStatus { get; set; }
        [JsonProperty("date_of_registration")]
        public DateTime RegistrationDate { get; set; }
        [JsonProperty("date_of_removal")]
        public DateTime? RemovalDate { get; set; }
        [JsonProperty("charity_reporting_status")]
        public string ReportingStatus { get; set; }
        [JsonProperty("latest_acc_fin_period_start_date")]
        public DateTime? LatestAccountingFinancePeriodStartDate { get; set; }
        [JsonProperty("latest_acc_fin_period_end_date")]
        public DateTime? LatestAccountingFinancePeriodEndDate { get; set; }

        [JsonProperty("latest_income")]
        public decimal? LatestIncome { get; set; }
        [JsonProperty("latest_expenditure")]
        public decimal? LatestExpenditure { get; set; }
        [JsonProperty("charity_contact_address1")]
        public string AddressLine1 { get; set; }
        [JsonProperty("charity_contact_address2")]
        public string AddressLine2 { get; set; }
        [JsonProperty("charity_contact_address3")]
        public string AddressLine3 { get; set; }
        [JsonProperty("charity_contact_address4")]
        public string AddressLine4 { get; set; }
        [JsonProperty("charity_contact_address5")]
        public string AddressLine5 { get; set; }
        [JsonProperty("charity_contact_postcode")]
        public string PostCode { get; set; }
        [JsonProperty("charity_contact_phone")]
        public string Phone { get; set; }
        [JsonProperty("charity_contact_email")]
        public string Email { get; set; }
        [JsonProperty("charity_contact_web")]
        public string Website { get; set; }
        [JsonProperty("charity_company_registration_number")]
        public string CompaniesHouseNumber { get; set; }
        [JsonProperty("charity_insolvent")]
        public bool IsInsolvent { get; set; }
        [JsonProperty("charity_in_administration")]
        public bool IsInAdministration { get; set; }
        [JsonProperty("charity_previously_excepted")]
        public bool? WasPreviouslyExcepted { get; set; }
        [JsonProperty("charity_is_cdf_or_cif")]
        public string IsCharityCdfOrCif { get; set; }
        [JsonProperty("charity_is_cio")]
        public bool? CharityIsCio { get; set; }
        [JsonProperty("cio_is_dissolved")]
        public bool? IsCioDissolved { get; set; }
        [JsonProperty("date_cio_dissolution_notice")]
        public DateTime? CioDissolvedNoticeDate { get; set; }
        [JsonProperty("charity_activities")]
        public string CharityActivities { get; set; }
        [JsonProperty("charity_gift_aid")]
        public bool? IsRegisteredForGiftAid { get; set; }
        [JsonProperty("charity_has_land")]
        public bool? HasLand { get; set; }

        public static implicit operator CharityStaging(CharityModel model)
            => new CharityStaging
            {
                Id = model.CharityId,
                Name = model.Name,
                CompaniesHouseNumber = model.CompaniesHouseNumber,
                RegisteredCharityNumber = model.RegisteredCharityNumber,
                LinkedCharityId = model.LinkedCharityNumber,
                RegistrationDate = model.RegistrationDate,
                RegistrationStatus = Enum.Parse<RegistrationStatus>(model.RegistrationStatus, true),
                Type = Enum.TryParse<CharityType>(model.Type, true, out var charityType) ? charityType : (CharityType?)null,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                AddressLine3 = model.AddressLine3,
                AddressLine4 = model.AddressLine4,
                AddressLine5 = model.AddressLine5,
                Postcode = model.PostCode,
                IsInsolvent = model.IsInsolvent,
                IsInAdministration = model.IsInAdministration,
                WasPreviouslyExcepted = model.WasPreviouslyExcepted,
                RemovalDate = model.RemovalDate
            };
    }
}
