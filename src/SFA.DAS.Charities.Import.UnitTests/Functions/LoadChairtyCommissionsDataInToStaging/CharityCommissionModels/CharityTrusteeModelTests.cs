using AutoFixture;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging.CharityCommissionModels;
using Xunit;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadChairtyCommissionsDataInToStaging.CharityCommissionModels
{
    public class CharityTrusteeModelTests
    {
        [Theory]
        [InlineData('I', TrusteeType.Individual)]
        [InlineData('O', TrusteeType.Organisation)]
        public void CharityTrusteeModel_Operator_ReturnsInstanceOfCharityTrusteeStaging(char individualOrOrganisation, TrusteeType expectedTrusteeType)
        {
            var fixture = new Fixture();
            var subject = fixture
                .Build<CharityTrusteeModel>()
                .With(c => c.IndividualOrOrganisation, individualOrOrganisation)
                .Create();

            CharityTrusteeStaging outcome = subject;

            Assert.Equal(subject.CharityId, outcome.CharityId);
            Assert.Equal(subject.RegisteredCharityNumber, outcome.RegisteredCharityNumber);
            Assert.Equal(subject.TrusteeId, outcome.TrusteeId);
            Assert.Equal(subject.TrusteeName, outcome.Name);
            Assert.Equal(subject.IsChair, outcome.IsChair);
            Assert.Equal(subject.AppointmentDate, outcome.AppointmentDate);
            Assert.Equal(expectedTrusteeType, outcome.TrusteeType);
        }
    }
}
