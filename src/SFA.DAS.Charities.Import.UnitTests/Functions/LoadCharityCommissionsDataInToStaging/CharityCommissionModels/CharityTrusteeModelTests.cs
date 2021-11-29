using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels
{
    [TestFixture]
    public class CharityTrusteeModelTests
    {
        [TestCase('I', TrusteeType.Individual)]
        [TestCase('O', TrusteeType.Organisation)]
        public void CharityTrusteeModel_Operator_ReturnsInstanceOfCharityTrusteeStaging(char individualOrOrganisation, TrusteeType expectedTrusteeType)
        {
            var fixture = new Fixture();
            var subject = fixture
                .Build<CharityTrusteeModel>()
                .With(c => c.IndividualOrOrganisation, individualOrOrganisation)
                .Create();

            CharityTrusteeStaging outcome = subject;

            Assert.AreEqual(subject.CharityId, outcome.CharityId);
            Assert.AreEqual(subject.RegisteredCharityNumber, outcome.RegistrationNumber);
            Assert.AreEqual(subject.TrusteeId, outcome.TrusteeId);
            Assert.AreEqual(subject.TrusteeName, outcome.Name);
            Assert.AreEqual(subject.IsChair, outcome.IsChair);
            Assert.AreEqual(subject.AppointmentDate, outcome.AppointmentDate);
            Assert.AreEqual(expectedTrusteeType, outcome.TrusteeType);
        }
    }
}
