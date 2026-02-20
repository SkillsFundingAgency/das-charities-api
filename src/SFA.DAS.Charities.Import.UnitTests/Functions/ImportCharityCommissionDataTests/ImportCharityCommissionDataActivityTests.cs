using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.UnitTests.Functions.ImportCharityCommissionDataActivityTests;

[TestFixture]
public class ImportCharityCommissionDataActivityTests
{
    private readonly string _fileNameUrl = "https://www.site.com/charity-data.zip";

    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private Mock<ICharityCommissionDataHelper> _charityCommissionDataHelper;
    private FunctionContext _mockedFunctionContext;

    private HttpClient _httpClient;
    private ImportCharityCommissionDataActivity _sut;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        _charityCommissionDataHelper = new Mock<ICharityCommissionDataHelper>();
        _charityCommissionDataHelper.Setup(x => x.GetZipFileEntriesCount(It.IsAny<Stream>())).Returns(1);

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

        _sut = new ImportCharityCommissionDataActivity(_httpClientFactoryMock.Object, _charityCommissionDataHelper.Object);

        _mockedFunctionContext = FunctionsMockingHelper.GetMockedFunctionContextWithLogger().Object;
    }

    [Test]
    public async Task Run_ShouldDownloadAndSaveFileSuccessfully()
    {
        // Arrange
        var zipStream = GetValidZipStream();

        SetupHttpResponse(HttpStatusCode.OK, zipStream);

        // Act
        Func<Task> action = async () => await _sut.Run(_fileNameUrl, _mockedFunctionContext);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Test]
    public async Task Run_ShouldThrowHttpRequestException_WhenDownloadFails()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.BadRequest, Stream.Null);

        // Act
        Func<Task> action = async () => await _sut.Run(_fileNameUrl, _mockedFunctionContext);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Response status code does not indicate success: 400 (Bad Request).");
    }

    [Test]
    public async Task Run_ShouldThrowInvalidOperationException_WhenZipFileContainsNoFiles()
    {
        // Arrange
        var emptyZipStream = GetEmptyZipStream();

        SetupHttpResponse(HttpStatusCode.OK, emptyZipStream);

        _charityCommissionDataHelper.Setup(x => x.GetZipFileEntriesCount(It.IsAny<Stream>())).Returns(0);

        // Act
        Func<Task> action = async () => await _sut.Run(_fileNameUrl, _mockedFunctionContext);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Unsupported charity data zip file for {_fileNameUrl}. File contained no files.");
    }

    [Test]
    public async Task Run_ShouldThrowInvalidOperationException_WhenZipFileContainsMultipleFiles()
    {
        // Arrange
        var multiFileZipStream = GetMultiFileZipStream();

        SetupHttpResponse(HttpStatusCode.OK, multiFileZipStream);

        _charityCommissionDataHelper.Setup(x => x.GetZipFileEntriesCount(It.IsAny<Stream>())).Returns(2);

        // Act
        Func<Task> action = async () => await _sut.Run(_fileNameUrl, _mockedFunctionContext);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Unsupported charity data zip file for {_fileNameUrl}. File contained more than 1 file.");
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, Stream contentStream)
    {
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StreamContent(contentStream)
            });
    }

    private static Stream GetValidZipStream()
    {
        var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
        var fileEntry = archive.CreateEntry("data.json");
        using var entryStream = fileEntry.Open();
        using var writer = new StreamWriter(entryStream);
        writer.Write("{}");
        return memoryStream;
    }

    private static Stream GetEmptyZipStream()
    {
        var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
        return memoryStream;
    }

    private static Stream GetMultiFileZipStream()
    {
        var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
        archive.CreateEntry("data1.json");
        archive.CreateEntry("data2.json");
        return memoryStream;
    }
}
