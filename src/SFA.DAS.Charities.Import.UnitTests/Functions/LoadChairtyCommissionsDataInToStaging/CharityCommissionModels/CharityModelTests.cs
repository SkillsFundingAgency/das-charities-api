using AutoFixture;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging.CharityCommissionModels;
using System;
using System.Collections.Generic;
using Xunit;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadChairtyCommissionsDataInToStaging.CharityCommissionModels
{
    public class CharityModelTests
    {
        [Fact]
        public void CharityModel_Operator_ReturnsInstanceOfCharityStaging()
        {
            var fixture = new Fixture();

            var model = fixture
                .Build<CharityModel>()
                .With(c => c.RegistrationStatus, "Removed")
                .With(c => c.Type, "Other")
                .Create();

            CharityStaging outcome = model;

            Assert.Equal(model.CharityId, outcome.Id);
            Assert.Equal(model.Name, outcome.Name);
            Assert.Equal(model.CompaniesHouseNumber, outcome.CompaniesHouseNumber);
            Assert.Equal(model.RegisteredCharityNumber, outcome.RegisteredCharityNumber);
            Assert.Equal(model.LinkedCharityNumber, outcome.LinkedCharityId);
            Assert.Equal(model.RegistrationDate, outcome.RegistrationDate);
            Assert.Equal(model.AddressLine1, outcome.AddressLine1);
            Assert.Equal(model.AddressLine2, outcome.AddressLine2);
            Assert.Equal(model.AddressLine3, outcome.AddressLine3);
            Assert.Equal(model.AddressLine4, outcome.AddressLine4);
            Assert.Equal(model.AddressLine5, outcome.AddressLine5);
            Assert.Equal(model.PostCode, outcome.Postcode);
            Assert.Equal(model.IsInsolvent, outcome.IsInsolvent);
            Assert.Equal(model.IsInAdministration, outcome.IsInAdministration);
            Assert.Equal(model.WasPreviouslyExcepted, outcome.WasPreviouslyExcepted);
            Assert.Equal(model.RemovalDate, outcome.RemovalDate);
            Assert.Equal(model.RegistrationStatus, outcome.RegistrationStatus.ToString());
            Assert.Equal(model.Type, outcome.CharityType.ToString());
        }

        [Theory]
        [MemberData(nameof(statusEnumValues))]
        public void CharityModel_Operator_ReturnsCharityStagingWithCorrectStatus(RegistrationStatus status)
        {
            var fixture = new Fixture();

            var model = fixture
                .Build<CharityModel>()
                .With(c => c.RegistrationStatus, status.ToString())
                .With(c => c.Type, "Other")
                .Create();

            CharityStaging outcome = model;
            Assert.Equal(model.RegistrationStatus, outcome.RegistrationStatus.ToString());
        }

        public static IEnumerable<object[]> statusEnumValues()
        {
            foreach (var thing in Enum.GetValues(typeof(RegistrationStatus)))
            {
                yield return new object[] { thing };
            }
        }

        [Theory]
        [MemberData(nameof(typeEnumValues))]
        public void CharityModel_Operator_ReturnsCharityStagingWithCorrectType(CharityType charityType)
        {
            var fixture = new Fixture();

            var model = fixture
                .Build<CharityModel>()
                .With(c => c.RegistrationStatus, "Removed")
                .With(c => c.Type, charityType.ToString())
                .Create();

            CharityStaging outcome = model;
            Assert.Equal(model.Type, outcome.CharityType.ToString());
        }

        public static IEnumerable<object[]> typeEnumValues()
        {
            foreach (var thing in Enum.GetValues(typeof(CharityType)))
            {
                yield return new object[] { thing };
            }
        }
    }
}
