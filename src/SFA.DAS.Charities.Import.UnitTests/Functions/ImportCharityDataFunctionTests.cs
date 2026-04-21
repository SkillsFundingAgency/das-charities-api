using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.CharityCommissionModels;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.UnitTests.Functions;

public class ImportCharityDataFunctionTests
{
    [Test]
    public async Task Function_InvokesServiceMethods()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["CharityFileName"]).Returns("charities.zip");
        configMock.Setup(c => c["CharityTrusteeFileName"]).Returns("charityTrustees.zip");

        CancellationToken cancellationToken = new();

        var serviceMock = new Mock<ICharitiesImportService>();

        var function = new ImportCharityDataFunction(Mock.Of<ILogger<ImportCharityDataFunction>>(), configMock.Object, serviceMock.Object);

        await function.Run(null, cancellationToken);

        serviceMock.Verify(s => s.DownloadFile("charities.zip", cancellationToken), Times.Once);
        serviceMock.Verify(s => s.ClearStagingData<CharityStaging>(It.IsAny<Stream>(), cancellationToken), Times.Once);
        serviceMock.Verify(s => s.ExtractDataFromStream<CharityModel>(It.IsAny<Stream>(), cancellationToken), Times.Once);
        serviceMock.Verify(s => s.MapToCharity(It.IsAny<List<CharityModel>>(), cancellationToken), Times.Once);
        serviceMock.Verify(s => s.BulkInsert(It.IsAny<List<CharityStaging>>(), cancellationToken), Times.Once);

        serviceMock.Verify(s => s.DownloadFile("charityTrustees.zip", cancellationToken), Times.Once);
        serviceMock.Verify(s => s.ClearStagingData<CharityTrusteeStaging>(It.IsAny<Stream>(), cancellationToken), Times.Once);
        serviceMock.Verify(s => s.ExtractDataFromStream<CharityTrusteeModel>(It.IsAny<Stream>(), cancellationToken), Times.Once);
        serviceMock.Verify(s => s.MapToTrustee(It.IsAny<List<CharityTrusteeModel>>(), cancellationToken), Times.Once);
        serviceMock.Verify(s => s.BulkInsert(It.IsAny<List<CharityTrusteeStaging>>(), cancellationToken), Times.Once);

        serviceMock.Verify(s => s.LoadDataFromStagingToLive(It.IsAny<CancellationToken>()), Times.Once);
    }
}
