using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Api.Controllers;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Api.UnitTests
{
    public class GetCharityDetailsTests
    {
        private const int ValidRegistrationNumber = 1;
        private const int InvalidRegistrationNumber = 9;
        private CharitiesController _subject;
        private Mock<ICharitiesReadRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ICharitiesReadRepository>();
            _repositoryMock.Setup(r => r.GetCharityById(ValidRegistrationNumber)).ReturnsAsync(new Charity());
            _repositoryMock.Setup(r => r.GetCharityById(InvalidRegistrationNumber)).ReturnsAsync((Charity)null);
            _subject = new CharitiesController(_repositoryMock.Object, Mock.Of<ILogger<CharitiesController>>());
        }

        [TestCase(0, StatusCodes.Status400BadRequest)]
        [TestCase(-1, StatusCodes.Status400BadRequest)]
        [TestCase(ValidRegistrationNumber, StatusCodes.Status200OK)]
        [TestCase(InvalidRegistrationNumber, StatusCodes.Status404NotFound)]
        public async Task GetCharityDetails_OnRequest_ReturnsAppropriateResults(int registrationNumber, int expectedStatusCode)
        {
            var response = await _subject.GetCharityDetails(registrationNumber) as ObjectResult;
            Assert.AreEqual(expectedStatusCode, response.StatusCode);
        }
    }
}