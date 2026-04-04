using System.Threading.Tasks;
using Microsoft.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities;


namespace SFA.DAS.Charities.Import.UnitTests.Functions;

[TestFixture]
public class ImportCharityCommissionDataWorkflowTests
{
    [Test]
    public async Task ImportCharityCommissionData_ForEachFileInConfig_CallsDownloadActivity()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.SetupGet(x => x["CharitiesDownloadFileNames"]).Returns("file1, file2");

        var subject = new ImportCharityCommissionDataWorkflow(configMock.Object);
        var contextMock = new Mock<TaskOrchestrationContext>();
        contextMock.Setup(c => c.CreateReplaySafeLogger(nameof(ImportCharityCommissionDataWorkflow))).Returns(Mock.Of<ILogger>());

        await subject.ImportCharityCommissionData(contextMock.Object);

        contextMock.Verify(c => c.CallActivityAsync(nameof(ImportCharityCommissionDataActivity), "file1"));
        contextMock.Verify(c => c.CallActivityAsync(nameof(ImportCharityCommissionDataActivity), "file2"));
    }
}
