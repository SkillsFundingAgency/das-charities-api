using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Domain;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities
{
    public class ImportCharityCommissionDataActivity
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICharityCommissionDataHelper _dataHelper;

        public ImportCharityCommissionDataActivity(IHttpClientFactory factory , ICharityCommissionDataHelper dataHelper)
        {
            _httpClientFactory = factory;
            _dataHelper = dataHelper;
        }

        [FunctionName(nameof(ImportCharityCommissionDataActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName,
            [Blob("charity-files/{fileName}", FileAccess.Write, Connection = "CharitiesStorageConnectionString")] Stream file,
            ILogger log)
        {
            using var performanceLogger = new PerformanceLogger($"Download and save file {fileName}", log);
            var client = _httpClientFactory.CreateClient("CharityCommissions");

            log.LogDebug("Downloading file: {fileName}", fileName);

            try
            {
                using var response = await client.GetAsync(fileName, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using var httpStream = await response.Content.ReadAsStreamAsync();

                using var memoryStream = new MemoryStream();
                await httpStream.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                ValidateDownloadedFile(memoryStream, log, fileName);

                await memoryStream.CopyToAsync(file);
            }
            catch (HttpRequestException ex)
            {
                log.LogError(ex, "An unexpected error occurred when trying to download {fileName}", fileName);
                throw;
            }

            log.LogInformation("File: {fileName} downloaded and saved successfully.");
        }

        private void ValidateDownloadedFile(MemoryStream stream, ILogger logger, string fileName)
        {
            var entriesCount = _dataHelper.GetZipFileEntriesCount(stream);
            if (entriesCount == 0)
                throw new InvalidOperationException(
                    $"Unsupported charity data zip file for {fileName}. File contained no files.");

            if (entriesCount > 1)
                throw new InvalidOperationException(
                    $"Unsupported charity data zip file for {fileName}. File contained more than 1 file.");

            logger.LogInformation("File: {fileName} is valid. Writing to Blob storage.");

            stream.Position = 0;
        }
    }
}