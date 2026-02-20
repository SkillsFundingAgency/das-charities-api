using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;

namespace SFA.DAS.Charities.Import.UnitTests.Functions;

public static class FunctionsMockingHelper
{
    public static Mock<FunctionContext> GetMockedFunctionContextWithLogger()
    {
        Mock<FunctionContext> contextMock = new();
        Mock<ILoggerFactory> loggerFactoryMock = new();
        loggerFactoryMock.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(Mock.Of<ILogger>());
        Mock<IServiceProvider> serviceCollectionMock = new();
        serviceCollectionMock.Setup(service => service.GetService(typeof(ILoggerFactory))).Returns(loggerFactoryMock.Object);
        contextMock.Setup(c => c.InstanceServices).Returns(serviceCollectionMock.Object);

        return contextMock;
    }

}
