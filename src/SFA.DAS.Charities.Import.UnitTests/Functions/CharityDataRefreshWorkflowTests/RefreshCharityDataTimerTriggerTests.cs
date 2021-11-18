using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.CharityDataRefreshWorkflowTests
{
    [TestFixture]
    public class RefreshCharityDataTimerTriggerTests
    {
        private readonly DateTime _today = new DateTime(2021, 11, 11);
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private Mock<IDateTimeProvider> _timeProviderMock;
        private Mock<IDurableOrchestrationClient> _durableOrchestrationClientMock;
        private string _instanceId;

        private CharityDataRefreshWorkflow _subject;

        [SetUp]
        public void SetUp()
        {
            _timeProviderMock = new Mock<IDateTimeProvider>();
            _durableOrchestrationClientMock = new Mock<IDurableOrchestrationClient>();
            _timeProviderMock.Setup(t => t.Today).Returns(_today);
            _subject = new CharityDataRefreshWorkflow(_timeProviderMock.Object);
            _instanceId = $"charity-data-refresh-instance-{_today:yyyy-MM-dd}";
        }

        [Test]
        public async Task TimerTrigger_FirstInstance_InvokesWorkflow()
        {
            await _subject.RefreshCharityDataTimerTrigger(null, _durableOrchestrationClientMock.Object, _loggerMock.Object);

            _durableOrchestrationClientMock.Verify(s => s.StartNewAsync(nameof(CharityDataRefreshWorkflow), _instanceId));
        }

        [Test]
        public async Task TimerTrigger_SimultaneousInstance_DoesNotInvokeWorkflow()
        {
            _durableOrchestrationClientMock.Setup(d => d.GetStatusAsync(_instanceId, false, false, true)).ReturnsAsync(new DurableOrchestrationStatus { RuntimeStatus = OrchestrationRuntimeStatus.Running });

            await _subject.RefreshCharityDataTimerTrigger(null, _durableOrchestrationClientMock.Object, _loggerMock.Object);

            _durableOrchestrationClientMock.Verify(s => s.StartNewAsync(nameof(CharityDataRefreshWorkflow), It.IsAny<string>()), Times.Never);
        }
    }
}
