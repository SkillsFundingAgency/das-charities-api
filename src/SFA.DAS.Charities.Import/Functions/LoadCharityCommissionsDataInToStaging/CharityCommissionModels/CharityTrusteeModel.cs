using Newtonsoft.Json;
using SFA.DAS.Charities.Domain.Entities;
using System;

namespace SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels
{
    public class CharityTrusteeModel
    {
        [JsonProperty("date_of_extract")]
        public DateTime ExtractedDate { get; set; }
        [JsonProperty("organisation_number")]
        public int CharityId { get; set; }
        [JsonProperty("registered_charity_number")]
        public int RegisteredCharityNumber { get; set; }
        [JsonProperty("linked_charity_number")]
        public int LinkedCharityNumber { get; set; }
        [JsonProperty("trustee_id")]
        public int TrusteeId { get; set; }
        [JsonProperty("trustee_name")]
        public string TrusteeName { get; set; }
        [JsonProperty("trustee_is_chair")]
        public bool IsChair { get; set; }
        [JsonProperty("individual_or_organisation")]
        public char IndividualOrOrganisation { get; set; }
        [JsonProperty("trustee_date_of_appointment")]
        public DateTime? AppointmentDate { get; set; }

        public static implicit operator CharityTrusteeStaging(CharityTrusteeModel model)
            => new CharityTrusteeStaging
            {
                CharityId = model.CharityId,
                RegistrationNumber = model.RegisteredCharityNumber,
                TrusteeId = model.TrusteeId,
                Name = model.TrusteeName,
                IsChair = model.IsChair,
                AppointmentDate = model.AppointmentDate,
                TrusteeType = model.IndividualOrOrganisation == 'I' ? TrusteeType.Individual : TrusteeType.Organisation
            };
    }
}
