namespace SFA.DAS.Charities.Import.UnitTests.Functions.CharityDataRefreshWorkflowTests;

// MFCMFC
// [TestFixture]
// public class RefreshCharityDataOrchestrationTriggerTests
// {
//     private readonly Mock<FunctionContext> _functionContext = new();
//     private Mock<TaskOrchestrationContext> _contextMock;
//
//     [SetUp]
//     public void SetUp()
//     {
//         _contextMock = new Mock<TaskOrchestrationContext>();
//         Sequence.ContextMode = SequenceContextMode.Async;
//     }
//
//     [Test]
//     public async Task RefreshCharityDataOrchestrationTrigger_InvokesWorkflows_InOrder()
//     {
//         var subject = new CharityDataRefreshWorkflow(Mock.Of<IDateTimeProvider>());
//
//         var sequence = new MockSequence();
//
//         using (Sequence.Create())
//         {
//             _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(ImportCharityCommissionDataWorkflow), null)).InSequence();
//             _contextMock.Setup(c => c.CallSubOrchestratorAsync<Task>(nameof(LoadCharityCommissionsDataInToStagingWorkflow), null)).InSequence();
//             _contextMock.Setup(c => c.CallActivityAsync<Task>(nameof(LoadActiveDataFromStagingActivity), null)).InSequence();
//             await subject.RefreshCharityDataOrchestrationTrigger(_contextMock.Object, _functionContext.Object);
//
//         }
//
//     }
// }
