using System;
using System.Collections.Generic;

namespace SFA.DAS.Charities.Domain.Entities
{
    public class Charity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public int RegistrationNumber { get; set; }
        public int LinkedCharityId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; }
        public CharityType? CharityType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string Postcode { get; set; }
        public bool IsInsolvent { get; set; }
        public bool IsInAdministration { get; set; }
        public bool? WasPreviouslyExcepted { get; set; }
        public DateTime? RemovalDate { get; set; }
        public List<CharityTrustee> Trustees { get; set; }
    }
}
