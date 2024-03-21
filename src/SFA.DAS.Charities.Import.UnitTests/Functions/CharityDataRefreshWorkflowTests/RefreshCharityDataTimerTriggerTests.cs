namespace SFA.DAS.Charities.Import.UnitTests.Functions.CharityDataRefreshWorkflowTests;

//MFCMFC commented out while resolving config issues
// [TestFixture]
// public class RefreshCharityDataTimerTriggerTests
// {
//     private readonly DateTime _today = new DateTime(2021, 11, 11);
//     private Mock<IDateTimeProvider> _timeProviderMock;
//     private Mock<DurableTaskClient> _durableTaskClientMock;
//     private readonly Mock<FunctionContext> _functionContext = new();
//
//     private string _instanceId;
//
//     private CharityDataRefreshWorkflow _subject;
//     private Mock<ILogger> _loggerMock = new();
//
//     [SetUp]
//     public void SetUp()
//     {
//         _instanceId = $"charity-data-refresh-instance-{_today:yyyy-MM-dd}";
//         _timeProviderMock = new Mock<IDateTimeProvider>();
//         _durableTaskClientMock = new Mock<DurableTaskClient>(_instanceId);
//         _timeProviderMock.Setup(t => t.Today).Returns(_today);
//         _subject = new CharityDataRefreshWorkflow(_timeProviderMock.Object);
//         _durableTaskClientMock.
//             Setup(x => x.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), _instanceId, default)).
//             ReturnsAsync(_instanceId);
//
//         // Extension methods (here: FunctionContextLoggerExtensions.GetLogger) may not be used in setup / verification expressions.
//         _functionContext.Setup(x => x.GetLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
//
//     }
//
//     [Test]
//     public async Task TimerTrigger_FirstInstance_InvokesWorkflow()
//     {
//         await _subject.RefreshCharityDataTimerTrigger(Mock.Of<TimerInfo>(), _durableTaskClientMock.Object, _functionContext.Object);
//
//         // _durableTaskClientMock.Verify(s => s.StartNewAsync(nameof(CharityDataRefreshWorkflow), _instanceId));
//
//         //        _durableTaskClientMock.Verify(s => s.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), It.IsAny<string>(), It.IsAny<CancellationToken>()));
//
//         _durableTaskClientMock.Verify(s => s.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), _instanceId, null, It.IsAny<CancellationToken>()));
//
//     }
//
//     // [Test]
//     // public async Task TimerTrigger_SimultaneousInstance_DoesNotInvokeWorkflow()
//     // {
//     //     _durableTaskClientMock.Setup(d => d.GetStatusAsync(_instanceId, false, false, true)).ReturnsAsync(new DurableOrchestrationStatus { RuntimeStatus = OrchestrationRuntimeStatus.Running });
//     //
//     //     await _subject.RefreshCharityDataTimerTrigger(null, _durableTaskClientMock.Object, _loggerMock.Object);
//     //
//     //     _durableTaskClientMock.Verify(s => s.StartNewAsync(nameof(CharityDataRefreshWorkflow), It.IsAny<string>()), Times.Never);
//     // }
// }
