using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Domain.Entities;
using SFA.DAS.Charities.Import.CharityCommissionModels;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.Services;

public class CharitiesImportService(
    IHttpClientFactory _httpClientFactory,
    ICharitiesImportRepository _charitiesImportRepository,
    ICharityCommissionDataHelper _dataHelper,
    ILogger<CharitiesImportService> _logger) : ICharitiesImportService
{
    public async Task<Stream> DownloadFile(string fileName, CancellationToken cancellationToken)
    {
        using var performanceLogger = new PerformanceLogger($"Downloading file {fileName}", _logger);
        var client = _httpClientFactory.CreateClient("CharityCommissions");

        using var response = await client.GetAsync(fileName, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var ms = new MemoryStream();
        await responseStream.CopyToAsync(ms, cancellationToken);
        ms.Position = 0;
        return ms;
    }

    public async Task<Stream> ClearStagingData<T>(Stream stream, CancellationToken cancellationToken)
    {
        var tableName = typeof(T).Name;
        using var performanceLogger = new PerformanceLogger($"Truncating {tableName}", _logger);
        await _charitiesImportRepository.DeleteStagingData(tableName, cancellationToken);
        return stream;
    }

    public async Task<List<TModel>> ExtractDataFromStream<TModel>(Stream stream, CancellationToken cancellationToken) where TModel : class
    {
        using var performanceLogger = new PerformanceLogger($"Loading data in to {typeof(TModel)}", _logger);
        List<TModel> charities = await _dataHelper.ExtractDataStream<TModel>(stream, cancellationToken).ToListAsync();
        await stream.DisposeAsync();
        return charities;
    }

    public Task<List<CharityStaging>> MapToCharity(List<CharityModel> models, CancellationToken cancellationToken)
    {
        using var performanceLogger = new PerformanceLogger("Mapping CharityModels to CharityStaging", _logger);
        return Task.FromResult(models.Select(m => (CharityStaging)m).ToList());
    }

    public Task<List<CharityTrusteeStaging>> MapToTrustee(List<CharityTrusteeModel> models, CancellationToken cancellationToken)
    {
        using var performanceLogger = new PerformanceLogger("Mapping CharityTrusteeModels to CharityTrusteeStaging", _logger);
        return Task.FromResult(models.Select(m => (CharityTrusteeStaging)m).ToList());
    }

    public Task BulkInsert<T>(IEnumerable<T> data, CancellationToken cancellationToken) where T : class
    {
        using var performanceLogger = new PerformanceLogger($"Bulk inserting into {data.FirstOrDefault().GetType().Name}", _logger);
        return _charitiesImportRepository.BulkInsert(data, cancellationToken);
    }

    public async Task LoadDataFromStagingToLive(CancellationToken cancellationToken)
    {
        using var performanceLogger = new PerformanceLogger("Loading data from staging to live", _logger);
        await _charitiesImportRepository.LoadDataFromStagingInToLive(cancellationToken);
    }
}
