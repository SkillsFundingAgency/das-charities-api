using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Functions.ImportCharityCommissionData.Activities
{
    public class ImportCharityCommissionDataActivity
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ImportCharityCommissionDataActivity(IHttpClientFactory factory)
        {
            _httpClientFactory = factory;
        }

        [FunctionName(nameof(ImportCharityCommissionDataActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName,
            [Blob("charity-files/{fileName}", access: FileAccess.Write, Connection = "CharitiesStorageConnectionString")] Stream file,
            ILogger log)
        {
            var client = _httpClientFactory.CreateClient("CharityCommissions");

            log.LogDebug("Downloading file: {fileName}", fileName);

            byte[] content;

            try
            {
                var response = await client.GetAsync(fileName);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException ex)
            {
                log.LogError(ex, "An unexpected error occurred when trying to download {fileName}", fileName);
                throw;
            }

            await using (var memoryStream = new MemoryStream(content))
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    if (archive.Entries.Count == 0)
                        throw new InvalidOperationException(
                            $"Unsupported charity data zip file for {fileName}. Zip file contained no files.");

                    if (archive.Entries.Count > 1)
                        throw new InvalidOperationException(
                            $"Unsupported charity data zip file for {fileName}. File contained more than 1 file.  Files: {archive.Entries.Aggregate(string.Empty, (currText, zippedFile) => $"{currText}{zippedFile.Name}, ")}");
                    log.LogDebug("File: {fileName} appears to be ok.", fileName);
                }
            }

            await file.WriteAsync(content, 0, content.Length);
            log.LogInformation("Finished writing {fileName} to blob storage.", fileName);
        }
    }
}