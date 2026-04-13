using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.Extensions;
using SFA.DAS.Charities.Import.Functions.LoadCharityCommissionsDataInToStaging.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.Services;

public interface ICharitiesImportService
{
    Task ImportData(CancellationToken cancellationToken);
}

public class CharitiesImportService(
    IConfiguration _configuration,
    IHttpClientFactory _httpClientFactory,
    ICharitiesImportRepository _charitiesImportRepository,
    ICharityCommissionDataHelper _dataHelper,
    ILogger<CharitiesImportService> _logger) : ICharitiesImportService
{
    public async Task ImportData(CancellationToken cancellationToken)
    {
        using var performanceLogger = new PerformanceLogger("Importing data from charity commissions", _logger);

        var charityTrusteeFileName = _configuration["CharityTrusteeFileName"];
        var charityFileName = _configuration["CharityFileName"];

        await DownloadFile(charityFileName)
            .Then(ClearStagingData<CharityStaging>)
            .Then(ExtractDataFromStream<CharityModel>)
            .Then(MapToCharity)
            .Then(BulkInsert);

        await DownloadFile(charityTrusteeFileName)
            .Then(ClearStagingData<CharityTrusteeStaging>)
            .Then(ExtractDataFromStream<CharityTrusteeModel>)
            .Then(MapToTrustee)
            .Then(BulkInsert);

        await _charitiesImportRepository.LoadDataFromStagingInToLive();
    }

    private async Task<Stream> DownloadFile(string fileName)
    {
        using var performanceLogger = new PerformanceLogger($"Downloading file {fileName}", _logger);
        var client = _httpClientFactory.CreateClient("CharityCommissions");

        using var response = await client.GetAsync(fileName, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var ms = new MemoryStream();
        await responseStream.CopyToAsync(ms).ConfigureAwait(false);
        ms.Position = 0;
        return ms;
    }

    private async Task<Stream> ClearStagingData<T>(Stream stream)
    {
        var tableName = typeof(T).Name;
        using var performanceLogger = new PerformanceLogger($"Truncating {tableName}", _logger);
        await _charitiesImportRepository.DeleteStagingData(tableName);
        return stream;
    }

    private async Task<List<TModel>> ExtractDataFromStream<TModel>(Stream stream) where TModel : class
    {
        using var performanceLogger = new PerformanceLogger($"Loading data in to {typeof(TModel)}", _logger);
        var charities = await _dataHelper.ExtractDataStream<TModel>(stream).ToListAsync();
        await stream.DisposeAsync();
        return charities;
    }

    private Task<List<CharityStaging>> MapToCharity(List<CharityModel> models)
    {
        using var performanceLogger = new PerformanceLogger("Mapping CharityModels to CharityStaging", _logger);
        return Task.FromResult(models.Select(m => (CharityStaging)m).ToList());
    }

    private Task<List<CharityTrusteeStaging>> MapToTrustee(List<CharityTrusteeModel> models)
    {
        using var performanceLogger = new PerformanceLogger("Mapping CharityTrusteeModels to CharityTrusteeStaging", _logger);
        return Task.FromResult(models.Select(m => (CharityTrusteeStaging)m).ToList());
    }

    private Task BulkInsert<T>(IEnumerable<T> data) where T : class
    {
        using var performanceLogger = new PerformanceLogger($"Bulk inserting into {data.FirstOrDefault().GetType().Name}", _logger);
        return _charitiesImportRepository.BulkInsert(data);
    }
}
