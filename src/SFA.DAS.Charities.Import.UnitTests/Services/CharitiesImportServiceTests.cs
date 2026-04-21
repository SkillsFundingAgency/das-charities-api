using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.UnitTests.Services;

[TestFixture]
public class CharitiesImportServiceTests
{
    //[Test]
    //public async Task ImportData_CallsRepositoryMethods()
    //{
    //    var httpFactory = new Mock<IHttpClientFactory>();
    //    var handler = new StubHttpMessageHandler();
    //    var client = new HttpClient(handler) { BaseAddress = new System.Uri("https://chartiy-commissions.co.uk") };
    //    httpFactory.Setup(f => f.CreateClient("CharityCommissions")).Returns(client);

    //    var repo = new Mock<ICharitiesImportRepository>();
    //    repo.Setup(r => r.DeleteStagingData(It.IsAny<string>())).Returns(Task.CompletedTask);
    //    repo.Setup(r => r.BulkInsert(It.IsAny<IEnumerable<CharityStaging>>())).Returns(Task.CompletedTask);
    //    repo.Setup(r => r.BulkInsert(It.IsAny<IEnumerable<CharityTrusteeStaging>>())).Returns(Task.CompletedTask);
    //    repo.Setup(r => r.LoadDataFromStagingInToLive()).Returns(Task.CompletedTask);

    //    var dataHelper = new Mock<ICharityCommissionDataHelper>();
    //    dataHelper.Setup(d => d.ExtractDataStream<CharityModel>(It.IsAny<Stream>())).Returns(ToAsyncEnumerable([new CharityModel { CharityId = 1, Name = "A" }]));
    //    dataHelper.Setup(d => d.ExtractDataStream<CharityTrusteeModel>(It.IsAny<Stream>())).Returns(ToAsyncEnumerable([new CharityTrusteeModel { CharityId = 1, TrusteeName = "Joe" }]));

    //    var logger = new Mock<ILogger<CharitiesImportService>>();

    //    var sut = new CharitiesImportService(httpFactory.Object, repo.Object, dataHelper.Object, logger.Object);

    //    await sut.LoadDataFromStagingToLive(CancellationToken.None);

    //    repo.Verify(r => r.DeleteStagingData("CharityStaging"), Times.Once);
    //    repo.Verify(r => r.DeleteStagingData("CharityTrusteeStaging"), Times.Once);
    //    repo.Verify(r => r.BulkInsert(It.IsAny<IEnumerable<CharityStaging>>()), Times.Once);
    //    repo.Verify(r => r.BulkInsert(It.IsAny<IEnumerable<CharityTrusteeStaging>>()), Times.Once);
    //    repo.Verify(r => r.LoadDataFromStagingInToLive(), Times.Once);
    //}

    private Mock<ICharitiesImportRepository> _repoMock;
    private Mock<ICharityCommissionDataHelper> _commissionDataHelperMock;
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private CharitiesImportService _sut;

    [SetUp]
    public void Before_Each_test()
    {
        _repoMock = new();
        _commissionDataHelperMock = new();
        _httpClientFactoryMock = new();

        _sut = new(_httpClientFactoryMock.Object, _repoMock.Object, _commissionDataHelperMock.Object, Mock.Of<ILogger<CharitiesImportService>>());
    }

    [Test]
    public async Task DownloadFile_ReturnsReadableStream()
    {
        const string FileName = "data.json";
        MemoryStream ms = GetSampleStream();

        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = HttpStatusCode.OK,
               // Wrap your stream in StreamContent
               Content = new StreamContent(ms)
           });

        var client = new HttpClient(handlerMock.Object) { BaseAddress = new System.Uri("https://charity-commissions.co.uk") };

        // arrange factory to return the client that will provide the zip stream
        _httpClientFactoryMock.Setup(f => f.CreateClient("CharityCommissions")).Returns(client);

        CancellationToken cancellationToken = new();

        var actual = await _sut.DownloadFile(FileName, cancellationToken);

        _httpClientFactoryMock.Verify(f => f.CreateClient("CharityCommissions"), Times.Once);
        actual.Should().BeReadable();
    }

    [Test]
    public async Task ClearStagingData_InvokesDeleteOnType_ReturnsReadableStream()
    {
        MemoryStream ms = GetSampleStream();
        CancellationToken cancellationToken = new();

        var actual = await _sut.ClearStagingData<CharityStaging>(ms, cancellationToken);

        _repoMock.Verify(f => f.DeleteStagingData("CharityStaging", cancellationToken), Times.Once);
        actual.Should().BeReadable();
    }

    [Test]
    public async Task ExtractDataFromStream_ReturnsListOfModels()
    {
        Stream ms = GetSampleStream();
        CancellationToken cancellationToken = new();
        _commissionDataHelperMock.Setup(d => d.ExtractDataStream<CharityStaging>(ms, cancellationToken)).Returns(ToAsyncEnumerable(new List<CharityStaging>()));

        var actual = await _sut.ExtractDataFromStream<CharityStaging>(ms, cancellationToken);

        actual.Should().NotBeNull();
        actual.Should().BeOfType<List<CharityStaging>>();
    }

    [Test]
    public async Task MapToCharity_ReturnsEntities()
    {
        Fixture fixture = new();
        var charityModels = fixture.Build<CharityModel>().With(m => m.RegistrationStatus, RegistrationStatus.Registered.ToString()).CreateMany().ToList();
        var actual = await _sut.MapToCharity(charityModels, CancellationToken.None);
        actual.Should().BeOfType<List<CharityStaging>>();
        actual.Should().HaveCount(charityModels.Count);
    }

    [Test]
    public async Task MapToTrustee_ReturnsEntities()
    {
        Fixture fixture = new();
        var trusteeModels = fixture.CreateMany<CharityTrusteeModel>().ToList();
        var actual = await _sut.MapToTrustee(trusteeModels, CancellationToken.None);
        actual.Should().BeOfType<List<CharityTrusteeStaging>>();
        actual.Should().HaveCount(trusteeModels.Count);
    }

    [Test]
    public async Task BulkInsert_InvokesRepo()
    {
        Fixture fixture = new();
        var trusteeModels = fixture.CreateMany<CharityTrusteeStaging>().ToList();
        CancellationToken cancellationToken = new();

        await _sut.BulkInsert(trusteeModels, cancellationToken);

        _repoMock.Verify(r => r.BulkInsert(trusteeModels, cancellationToken), Times.Once());
    }

    [Test]
    public async Task LoadDataFromStagingToLive_InvokesRepo()
    {
        CancellationToken cancellationToken = new();

        await _sut.LoadDataFromStagingToLive(cancellationToken);

        _repoMock.Verify(r => r.LoadDataFromStagingInToLive(cancellationToken), Times.Once());
    }

    private static MemoryStream GetSampleStream()
    {
        var content = "Your mock stream data";
        var byteArray = Encoding.UTF8.GetBytes(content);
        var ms = new MemoryStream(byteArray);
        return ms;
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            await Task.Yield();
            yield return item;
        }
    }
}
