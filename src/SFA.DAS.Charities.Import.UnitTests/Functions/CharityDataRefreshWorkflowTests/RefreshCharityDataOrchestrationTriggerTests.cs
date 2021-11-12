using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Sequences;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;
using SFA.DAS.Charities.Import.Functions.LoadChairtyCommissionsDataInToStaging;
using System.Threading.Tasks;
using Xunit;
using SFA.DAS.Charities.Import.Functions;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.CharityDataRefreshWorkflowTests
{
    public class RefreshCharityDataOrchestrationTriggerTests
    {
        private readonly Mock<IDurableOrchestrationContext> _contextMock = new Mock<IDurableOrchestrationContext>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        public RefreshCharityDataOrchestrationTriggerTests()
        {
            Sequence.ContextMode = SequenceContextMode.Async;
        }

        [Fact]
        public async Task RefreshCharityDataOrchestrationTrigger_InvokesWorkflows_InOrder()
        {
            var subject = new CharityDataRefreshWorkflow(Mock.Of<IDateTimeProvider>());
            
            var sequence = new MockSequence();
            
            using (Sequence.Create())
            {
                _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null)).InSequence();
                _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(LoadChairtyCommissionsDataInToStagingWorkflow), null)).InSequence();
                _contextMock.Setup(c => c.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null)).InSequence();
                await subject.RefreshCharityDataOrchestrationTrigger(_contextMock.Object, _loggerMock.Object);

            }

        }
    }
}
