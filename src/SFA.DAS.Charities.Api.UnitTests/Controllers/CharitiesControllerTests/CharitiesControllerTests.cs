using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Api.Controllers;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Api.UnitTests
{
    public class CharitiesControllerTests
    {
        private const int ValidRegistrationNumber = 1;
        private const int InvalidRegistrationNumber = 9;
        private const int MaximumResults = 200;
        private const string FoundSearchTerm = "FoundSearchTerm";
        private const string MissingSearchTerm = "MissingSearchTerm";
        private Charity _charityByRegistrationNumberResponse;
        private List<Charity> _charityByTextResponse;
        private CharitiesController _subject;
        private Mock<ICharitiesReadRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {

            var fixture = new Fixture();
            _charityByRegistrationNumberResponse = fixture.Create<Charity>();
            _charityByTextResponse = fixture.CreateMany<Charity>().ToList();

            _repositoryMock = new Mock<ICharitiesReadRepository>();
            _repositoryMock.Setup(r => r.GetCharityById(ValidRegistrationNumber)).ReturnsAsync(_charityByRegistrationNumberResponse);
            _repositoryMock.Setup(r => r.GetCharityById(InvalidRegistrationNumber)).ReturnsAsync((Charity)null);

            _repositoryMock.Setup(r => r.SearchCharities(FoundSearchTerm, MaximumResults)).ReturnsAsync(_charityByTextResponse);
            _repositoryMock.Setup(r => r.SearchCharities(MissingSearchTerm, MaximumResults)).ReturnsAsync(new List<Charity>());

            _subject = new CharitiesController(_repositoryMock.Object, Mock.Of<ILogger<CharitiesController>>());
        }

        [TestCase(0, StatusCodes.Status400BadRequest)]
        [TestCase(-1, StatusCodes.Status400BadRequest)]
        [TestCase(ValidRegistrationNumber, StatusCodes.Status200OK)]
        [TestCase(InvalidRegistrationNumber, StatusCodes.Status404NotFound)]
        public async Task GetCharityDetails_OnRequest_ReturnsAppropriateResults(int registrationNumber, int expectedStatusCode)
        {
            var response = await _subject.GetCharityDetails(registrationNumber) as ObjectResult;
            response.StatusCode.Should().Be(expectedStatusCode);
        }

        [TestCase(FoundSearchTerm, StatusCodes.Status200OK)]
        public async Task SearchCharities_OnRequest_ReturnsAppropriateResults(string searchTerm, int expectedStatusCode)
        {
            var response = await _subject.SearchCharities(searchTerm, MaximumResults) as ObjectResult;
            response.StatusCode.Should().Be(expectedStatusCode);

            var model = response.Value as List<Charity>;
            model.Should().NotBeNull();

            model.Should().BeEquivalentTo(_charityByTextResponse);
        }

        [TestCase(MissingSearchTerm, StatusCodes.Status404NotFound)]
        public async Task SearchCharities_WhenNotFound_Returns_NotFound(string searchTerm, int expectedStatusCode)
        {
            var response = await _subject.SearchCharities(searchTerm, MaximumResults) as ObjectResult;
            response.StatusCode.Should().Be(expectedStatusCode);
        }

        [TestCase("", StatusCodes.Status400BadRequest)]
        [TestCase(null, StatusCodes.Status400BadRequest)]
        public async Task SearchCharities_OnRequest_ReturnsBadRequest(string searchTerm, int expectedStatusCode)
        {
            var response = await _subject.SearchCharities(searchTerm, MaximumResults) as ObjectResult;
            response.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}