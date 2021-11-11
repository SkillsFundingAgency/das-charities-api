using System;
using System.Collections.Generic;

namespace SFA.DAS.Charities.Domain.Entities
{
    public class Charity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompanyNumber { get; set; }
        public int RegisteredCharityNumber { get; set; }
        public int LinkedCharityId { get; set; }
        public int RegistrationDate { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; }
        public CharityType? CharityType { get; set; }
        public DateTime? FinancialPeriodStartDate { get; set; }
        public DateTime? FinancialPeriodEndDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public string Postcode { get; set; }
        public bool IsInsolvent { get; set; }
        public bool IsInAdministration { get; set; }
        public bool WasPreviouslyExcepted { get; set; }
        public DateTime? RemovalDate { get; set; }
        public List<CharityTrustee> Trustees { get; set; }
    }
}
