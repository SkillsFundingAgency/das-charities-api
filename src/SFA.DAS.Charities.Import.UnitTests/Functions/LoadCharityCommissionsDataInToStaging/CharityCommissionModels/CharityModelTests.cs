using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.CharityCommissionModels;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;

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

        model.CharityId.Should().Be(outcome.Id);
        model.Name.Should().Be(outcome.Name);
        model.CompaniesHouseNumber.Should().Be(outcome.CompaniesHouseNumber);
        model.RegisteredCharityNumber.Should().Be(outcome.RegistrationNumber);
        model.LinkedCharityNumber.Should().Be(outcome.LinkedCharityId);
        model.RegistrationDate.Should().Be(outcome.RegistrationDate);
        model.AddressLine1.Should().Be(outcome.AddressLine1);
        model.AddressLine2.Should().Be(outcome.AddressLine2);
        model.AddressLine3.Should().Be(outcome.AddressLine3);
        model.AddressLine4.Should().Be(outcome.AddressLine4);
        model.AddressLine5.Should().Be(outcome.AddressLine5);
        model.PostCode.Should().Be(outcome.Postcode);
        model.IsInsolvent.Should().Be(outcome.IsInsolvent);
        model.IsInAdministration.Should().Be(outcome.IsInAdministration);
        model.WasPreviouslyExcepted.Should().Be(outcome.WasPreviouslyExcepted);
        model.RemovalDate.Should().Be(outcome.RemovalDate);
        model.RegistrationStatus.Should().Be(outcome.RegistrationStatus.ToString());
        model.Type.Should().Be(outcome.Type.ToString());
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
        model.RegistrationStatus.Should().Be(outcome.RegistrationStatus.ToString());
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
        model.Type.Should().Be(outcome.Type.ToString());
    }
}
