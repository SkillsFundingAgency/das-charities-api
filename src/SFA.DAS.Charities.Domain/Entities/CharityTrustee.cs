using System;

namespace SFA.DAS.Charities.Domain.Entities
{
    public class CharityTrustee
    {
        public int Id { get; set; }
        public int CharityId { get; set; }
        public int RegistrationNumber { get; set; }
        public int TrusteeId { get; set; }
        public string Name { get; set; }
        public bool IsChair { get; set; }
        public TrusteeType TrusteeType { get; set; }
        public DateTime? AppointmentDate { get; set; }
    }
}
