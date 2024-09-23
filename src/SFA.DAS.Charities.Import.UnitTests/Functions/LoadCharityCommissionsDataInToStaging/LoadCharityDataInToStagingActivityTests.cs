using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadCharityCommissionsDataInToStaging
{
    [TestFixture]
    public class LoadCharityDataInToStagingActivityTests
    {
        private Mock<ICharitiesImportRepository> _charityStagingRepositoryMock;
        private Mock<ICharityCommissionDataHelper> _dataHelperMock;
        private LoadCharityDataInToStagingActivity _activity;

        [SetUp]
        public void SetUp()
        {
            _charityStagingRepositoryMock = new Mock<ICharitiesImportRepository>();
            _dataHelperMock = new Mock<ICharityCommissionDataHelper>();
            _activity = new LoadCharityDataInToStagingActivity(_charityStagingRepositoryMock.Object, _dataHelperMock.Object);
        }

        [Test]
        public async Task Run_ShouldCallBulkInsert_WhenTrusteeDataIsAvailable()
        {
            // Arrange
            var fileName = "test-file.zip";
            var trusteeData = new List<CharityModel>
            {
                new CharityModel {Type = CharityType.Trust.ToString(), RegistrationStatus = "Registered"},
                new CharityModel {Type = CharityType.Trust.ToString(), RegistrationStatus = "Registered"}
            };
            var loggerMock = Mock.Of<ILogger>();
            var stream = new MemoryStream();

            _dataHelperMock.Setup(helper => helper.ExtractDataStream<CharityModel>(stream))
                .Returns(trusteeData);

            // Act
            await _activity.Run(fileName, stream, loggerMock);

            // Assert
            _charityStagingRepositoryMock.Verify(repo => repo.BulkInsert(It.IsAny<List<CharityStaging>>()), Times.Once);
        }

        [Test]
        public async Task Run_ShouldProcessInBatches_WhenTrusteeDataExceedsBatchSize()
        {
            // Arrange
            var fileName = "test-file.zip";
            var trusteeData = new List<CharityModel>();
            for (int i = 0; i < 1500; i++)
            {
                trusteeData.Add(new CharityModel {Type = CharityType.Trust.ToString(), RegistrationStatus = "Registered" });
            }
            var loggerMock = Mock.Of<ILogger>();
            var stream = new MemoryStream();

            _dataHelperMock.Setup(helper => helper.ExtractDataStream<CharityModel>(stream))
                .Returns(trusteeData);

            // Act
            await _activity.Run(fileName, stream, loggerMock);

            // Assert
            _charityStagingRepositoryMock.Verify(repo => repo.BulkInsert(It.IsAny<List<CharityStaging>>()), Times.Exactly(2));
        }

        [Test]
        public async Task Run_ShouldNotCallBulkInsert_WhenNoTrusteeDataIsAvailable()
        {
            // Arrange
            var fileName = "test-file.zip";
            var trusteeData = new List<CharityModel>();
            var loggerMock = Mock.Of<ILogger>();
            var stream = new MemoryStream();

            _dataHelperMock.Setup(helper => helper.ExtractDataStream<CharityModel>(stream))
                .Returns(trusteeData);

            // Act
            await _activity.Run(fileName, stream, loggerMock);

            // Assert
            _charityStagingRepositoryMock.Verify(repo => repo.BulkInsert(It.IsAny<List<CharityStaging>>()), Times.Never);
        }

        [Test]
        public void Run_ShouldThrowException_WhenExtractDataStreamFails()
        {
            // Arrange
            var fileName = "test-file.zip";
            var loggerMock = Mock.Of<ILogger>();
            var stream = new MemoryStream();

            _dataHelperMock.Setup(helper => helper.ExtractDataStream<CharityModel>(stream))
                .Throws(new InvalidOperationException("Test exception"));

            // Act
            Func<Task> action = async () => await _activity.Run(fileName, stream, loggerMock);

            // Assert
            action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");
        }
    }
}
