// using Microsoft.Azure.WebJobs.Extensions.DurableTask;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NUnit.Framework;
// using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging;
// using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.Activities;
// using System.Threading.Tasks;
//
// namespace SFA.DAS.Charities.Import.UnitTests.Functions.LoadCharityCommissionsDataInToStaging
// {
//MFCMFC commented out while resolving config issues
//     [TestFixture]
//     public class LoadCharityCommissionsDataInToStagingWorkflowTests
//     {
//         [Test]
//         public async Task LoadCharityCommissionsDataInToStaging_CallsActivities()
//         {
//             var charityTrusteeFileName = "CharityTrusteeFileName.zip";
//             var charityFileName = "CharityFileName.zip";
//             var configMock = new Mock<IConfiguration>();
//             configMock.SetupGet(x => x["CharityTrusteeFileName"]).Returns(charityTrusteeFileName);
//             configMock.SetupGet(x => x["CharityFileName"]).Returns(charityFileName);
//
//             var contextMock = new Mock<IDurableOrchestrationContext>();
//
//             var subject = new LoadCharityCommissionsDataInToStagingWorkflow(configMock.Object);
//
//             await subject.LoadCharityCommissionsDataInToStaging(contextMock.Object, Mock.Of<ILogger>());
//
//             contextMock.Verify(c => c.CallActivityAsync(nameof(ClearStagingDataActivity), null));
//             contextMock.Verify(c => c.CallActivityAsync(nameof(LoadCharityDataInToStagingActivity), charityFileName));
//             contextMock.Verify(c => c.CallActivityAsync(nameof(LoadCharityTrusteeDataInToStagingActivity), charityTrusteeFileName));
//
//         }
//     }
// }