using System.Threading.Tasks;
using Microsoft.DurableTask;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadCharityCommissionsDataInToStaging;

[TestFixture]
public class LoadCharityCommissionsDataInToStagingWorkflowTests
{
    [Test]
    public async Task LoadCharityCommissionsDataInToStaging_CallsActivities()
    {
        var charityTrusteeFileName = "CharityTrusteeFileName.zip";
        var charityFileName = "CharityFileName.zip";
        var configMock = new Mock<IConfiguration>();
        configMock.SetupGet(x => x["CharityTrusteeFileName"]).Returns(charityTrusteeFileName);
        configMock.SetupGet(x => x["CharityFileName"]).Returns(charityFileName);

        var contextMock = new Mock<TaskOrchestrationContext>();

        var subject = new LoadCharityCommissionsDataInToStagingWorkflow(configMock.Object);

        await subject.LoadCharityCommissionsDataInToStaging(contextMock.Object);

        contextMock.Verify(c => c.CallActivityAsync(nameof(ClearStagingDataActivity), null));
        contextMock.Verify(c => c.CallActivityAsync(nameof(LoadCharityDataInToStagingActivity), charityFileName));
        contextMock.Verify(c => c.CallActivityAsync(nameof(LoadCharityTrusteeDataInToStagingActivity), charityTrusteeFileName));
    }
}
