using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Sequences;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging;
using SFA.DAS.Charities.Import.Infrastructure;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.CharityDataRefreshWorkflowTests
{
    [TestFixture]
    public class RefreshCharityDataOrchestrationTriggerTests
    {
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private Mock<IDurableOrchestrationContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IDurableOrchestrationContext>();
            Sequence.ContextMode = SequenceContextMode.Async;
        }

        [Test]
        public async Task RefreshCharityDataOrchestrationTrigger_InvokesWorkflows_InOrder()
        {
            var subject = new CharityDataRefreshWorkflow(Mock.Of<IDateTimeProvider>());
            
            var sequence = new MockSequence();
            
            using (Sequence.Create())
            {
                _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null)).InSequence();
                _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(LoadCharityCommissionsDataInToStagingWorkflow), null)).InSequence();
                _contextMock.Setup(c => c.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null)).InSequence();
                await subject.RefreshCharityDataOrchestrationTrigger(_contextMock.Object, _loggerMock.Object);

            }

        }
    }
}
