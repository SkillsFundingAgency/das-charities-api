using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging.CharityCommissionModels;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadChairtyCommissionsDataInToStaging.CharityCommissionModels
{
    [TestFixture]
    public class CharityModelTests
    {
        [Test]
        public void CharityModel_Operator_ReturnsInstanceOfCharityStaging()
        {
            var fixture = new Fixture();

            var model = fixture
                .Build<CharityModel>()
                .With(c => c.RegistrationStatus, "Removed")
                .With(c => c.Type, "Other")
                .Create();

            CharityStaging outcome = model;

            Assert.AreEqual(model.CharityId, outcome.Id);
            Assert.AreEqual(model.Name, outcome.Name);
            Assert.AreEqual(model.CompaniesHouseNumber, outcome.CompaniesHouseNumber);
            Assert.AreEqual(model.RegisteredCharityNumber, outcome.RegisteredCharityNumber);
            Assert.AreEqual(model.LinkedCharityNumber, outcome.LinkedCharityId);
            Assert.AreEqual(model.RegistrationDate, outcome.RegistrationDate);
            Assert.AreEqual(model.AddressLine1, outcome.AddressLine1);
            Assert.AreEqual(model.AddressLine2, outcome.AddressLine2);
            Assert.AreEqual(model.AddressLine3, outcome.AddressLine3);
            Assert.AreEqual(model.AddressLine4, outcome.AddressLine4);
            Assert.AreEqual(model.AddressLine5, outcome.AddressLine5);
            Assert.AreEqual(model.PostCode, outcome.Postcode);
            Assert.AreEqual(model.IsInsolvent, outcome.IsInsolvent);
            Assert.AreEqual(model.IsInAdministration, outcome.IsInAdministration);
            Assert.AreEqual(model.WasPreviouslyExcepted, outcome.WasPreviouslyExcepted);
            Assert.AreEqual(model.RemovalDate, outcome.RemovalDate);
            Assert.AreEqual(model.RegistrationStatus, outcome.RegistrationStatus.ToString());
            Assert.AreEqual(model.Type, outcome.Type.ToString());
        }

        [Test]
        public void CharityModel_Operator_ReturnsCharityStagingWithCorrectStatus([Values] RegistrationStatus status)
        {
            var fixture = new Fixture();

            var model = fixture
                .Build<CharityModel>()
                .With(c => c.RegistrationStatus, status.ToString())
                .With(c => c.Type, "Other")
                .Create();

            CharityStaging outcome = model;
            Assert.AreEqual(model.RegistrationStatus, outcome.RegistrationStatus.ToString());
        }

        [Test]
        public void CharityModel_Operator_ReturnsCharityStagingWithCorrectType([Values] CharityType charityType)
        {
            var fixture = new Fixture();

            var model = fixture
                .Build<CharityModel>()
                .With(c => c.RegistrationStatus, "Removed")
                .With(c => c.Type, charityType.ToString())
                .Create();

            CharityStaging outcome = model;
            Assert.AreEqual(model.Type, outcome.Type.ToString());
        }
    }
}
