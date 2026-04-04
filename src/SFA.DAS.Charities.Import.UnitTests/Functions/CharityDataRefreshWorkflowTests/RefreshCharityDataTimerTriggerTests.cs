using System;
using System.Threading.Tasks;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.CharityDataRefreshWorkflowTests;

[TestFixture]
public class RefreshCharityDataTimerTriggerTests
{
    private readonly DateTime _today = new DateTime(2021, 11, 11, 0, 0, 0, DateTimeKind.Unspecified);
    private Mock<IDateTimeProvider> _timeProviderMock;
    private Mock<IDurableTaskClientWrapper> _durableTaskClientMock;
    private string _instanceId;

    private CharityDataRefreshWorkflow _subject;

    [SetUp]
    public void SetUp()
    {
        _timeProviderMock = new Mock<IDateTimeProvider>();
        _durableTaskClientMock = new Mock<IDurableTaskClientWrapper>();
        _timeProviderMock.Setup(t => t.Today).Returns(_today);
        _subject = new CharityDataRefreshWorkflow(_timeProviderMock.Object, Mock.Of<ILogger<CharityDataRefreshWorkflow>>(), _durableTaskClientMock.Object);
        _instanceId = $"charity-data-refresh-instance-{_today:yyyy-MM-dd}";
    }

    [Test]
    public async Task TimerTrigger_FirstInstance_InvokesWorkflow()
    {
        await _subject.RefreshCharityDataTimerTrigger(null, null);

        _durableTaskClientMock.Verify(s => s.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), _instanceId));
    }

    [Test]
    public async Task TimerTrigger_SimultaneousInstance_DoesNotInvokeWorkflow()
    {
        _durableTaskClientMock.Setup(d => d.GetInstanceAsync(_instanceId)).ReturnsAsync(new OrchestrationMetadata(It.IsAny<string>(), _instanceId) { RuntimeStatus = OrchestrationRuntimeStatus.Running });

        await _subject.RefreshCharityDataTimerTrigger(null, null);

        _durableTaskClientMock.Verify(s => s.ScheduleNewOrchestrationInstanceAsync(nameof(CharityDataRefreshWorkflow), It.IsAny<string>()), Times.Never);
    }
}
