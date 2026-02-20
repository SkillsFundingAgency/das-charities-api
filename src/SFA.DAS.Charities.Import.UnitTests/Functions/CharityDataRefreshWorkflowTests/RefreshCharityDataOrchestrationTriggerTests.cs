using System.Threading.Tasks;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Sequences;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData;
using SFA.DAS.Charities.Import.Functions.LoadActiveDataFromStaging.Activities;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.CharityDataRefreshWorkflowTests;

[TestFixture]
public class RefreshCharityDataOrchestrationTriggerTests
{
    private Mock<TaskOrchestrationContext> _contextMock;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<TaskOrchestrationContext>();
        _contextMock.Setup(c => c.CreateReplaySafeLogger(nameof(CharityDataRefreshWorkflow))).Returns(Mock.Of<ILogger>());
        Sequence.ContextMode = SequenceContextMode.Async;
    }

    [Test]
    public async Task RefreshCharityDataOrchestrationTrigger_InvokesWorkflows_InOrder()
    {
        var subject = new CharityDataRefreshWorkflow(Mock.Of<IDateTimeProvider>(), Mock.Of<ILogger<CharityDataRefreshWorkflow>>(), Mock.Of<IDurableTaskClientWrapper>());

        // Tests the tasks are invoked in a particular sequence
        using (Sequence.Create())
        {
            _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null)).InSequence();
            _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(LoadCharityCommissionsDataInToStagingWorkflow), null)).InSequence();
            _contextMock.Setup(c => c.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null)).InSequence();
            await subject.RefreshCharityDataOrchestrationTrigger(_contextMock.Object);
        }
    }
}
