using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Charities.Data;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Api.AcceptanceTests.Infrastructure
{
    public static class DbUtilities
    {
        public static void LoadTestData(CharitiesDataContext context)
        {
            var charities = GetCharities();
            context.Charities.AddRange(charities);
            context.SaveChanges();
        }

        private static Charity[] GetCharities()
        {
            return new[]
            {
                new Charity
                {
                    Id = 1,
                    RegistrationNumber = 1001,
                    Name = "Charity Name",
                    LinkedCharityId = 0,
                    Trustees = new List<CharityTrustee>
                    {
                        new CharityTrustee
                        {
                            RegistrationNumber = 1001,
                            Name = "Trustee name",
                        }
                    }
                }
            };
        }

        public static Charity GetCharity(int registrationNumber) => GetCharities().FirstOrDefault(c => c.RegistrationNumber.Equals(registrationNumber));
    }
}
