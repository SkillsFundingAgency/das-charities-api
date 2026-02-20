using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities;

public class ImportCharityCommissionDataActivity
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICharityCommissionDataHelper _dataHelper;

    public ImportCharityCommissionDataActivity(IHttpClientFactory factory, ICharityCommissionDataHelper dataHelper)
    {
        _httpClientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        _dataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
    }

    [Function(nameof(ImportCharityCommissionDataActivity))]
    [BlobOutput("charity-files/{fileName}")]
    public async Task<byte[]> Run([ActivityTrigger] string fileName, FunctionContext context)
    {
        ILogger logger = context.GetLogger(nameof(ImportCharityCommissionDataActivity));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("fileName is required", nameof(fileName));

        using var performanceLogger = new PerformanceLogger($"Download and save file {fileName}", logger);
        var client = _httpClientFactory.CreateClient("CharityCommissions");

        logger.LogDebug("Starting download for file: {FileName}", fileName);

        try
        {
            using var response = await client.GetAsync(fileName, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            // Copy the response into memory so we can validate and return the bytes for the BlobOutput
            using var memoryStream = new MemoryStream();
            await responseStream.CopyToAsync(memoryStream).ConfigureAwait(false);

            ValidateDownloadedFile(memoryStream, logger, fileName);

            // Ensure position is at 0 before converting to bytes
            memoryStream.Position = 0;
            var resultBytes = memoryStream.ToArray();

            logger.LogInformation("File: {FileName} downloaded and saved successfully.", fileName);

            return resultBytes;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "An unexpected HTTP error occurred when trying to download {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while downloading or validating {FileName}", fileName);
            throw;
        }
    }

    private void ValidateDownloadedFile(MemoryStream stream, ILogger logger, string fileName)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));

        // Reset position for validation
        stream.Position = 0;

        var entriesCount = _dataHelper.GetZipFileEntriesCount(stream);
        if (entriesCount == 0)
            throw new InvalidOperationException(
                $"Unsupported charity data zip file for {fileName}. File contained no files.");

        if (entriesCount > 1)
            throw new InvalidOperationException(
                $"Unsupported charity data zip file for {fileName}. File contained more than 1 file.");

        logger.LogInformation("File: {FileName} is valid. Writing to Blob storage.", fileName);
    }
}