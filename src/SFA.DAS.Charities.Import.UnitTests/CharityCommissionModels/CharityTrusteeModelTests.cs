using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.CharityCommissionModels;

namespace SFA.DAS.Charities.Import.UnitTests.CharityCommissionModels;

[TestFixture]
public class CharityTrusteeModelTests
{
    [TestCase('P', TrusteeType.Individual)]
    [TestCase('O', TrusteeType.Organisation)]
    public void CharityTrusteeModel_Operator_ReturnsInstanceOfCharityTrusteeStaging(char individualOrOrganisation, TrusteeType expectedTrusteeType)
    {
        var fixture = new Fixture();
        var subject = fixture
            .Build<CharityTrusteeModel>()
            .With(c => c.IndividualOrOrganisation, individualOrOrganisation)
            .Create();

        CharityTrusteeStaging outcome = subject;

        subject.CharityId.Should().Be(outcome.CharityId);
        subject.RegisteredCharityNumber.Should().Be(outcome.RegistrationNumber);
        subject.TrusteeId.Should().Be(outcome.TrusteeId);
        subject.TrusteeName.Should().Be(outcome.Name);
        subject.IsChair.Should().Be(outcome.IsChair);
        subject.AppointmentDate.Should().Be(outcome.AppointmentDate);
        expectedTrusteeType.Should().Be(outcome.TrusteeType);
    }
}
