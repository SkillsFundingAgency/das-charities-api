using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities;
using System.Threading.Tasks;


namespace SFA.DAS.Charities.Import.UnitTests.Functions
{
    [TestFixture]
    public class ImportCharityCommissionDataWorkflowTests
    {
        [Test]
        public async Task ImportCharityCommissionData_ForEachFileInConfig_CallsDownloadActivity()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.SetupGet(x => x["CharitiesDownloadFileNames"]).Returns("file1, file2");

            var subject = new ImportCharityCommissionDataWorkflow(configMock.Object);
            var contextMock = new Mock<IDurableOrchestrationContext>();

            await subject.ImportCharityCommissionData(contextMock.Object, Mock.Of<ILogger>());

            contextMock.Verify(c => c.CallActivityAsync(nameof(ImportCharityCommissionDataActivity), "file1"));
            contextMock.Verify(c => c.CallActivityAsync(nameof(ImportCharityCommissionDataActivity), "file2"));
        }
    }
}
