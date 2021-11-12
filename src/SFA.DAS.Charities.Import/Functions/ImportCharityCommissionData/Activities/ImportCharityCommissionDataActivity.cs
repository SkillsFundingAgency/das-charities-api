using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
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
        private readonly string downloadUrl;
        private readonly HttpClient _httpClient;

        public ImportCharityCommissionDataActivity(IConfiguration configuration, HttpClient httpClient)
        {
            downloadUrl = configuration["CharityFilesDownloadUrl"];
            _httpClient = httpClient;
        }

        [FunctionName(nameof(ImportCharityCommissionDataActivity))]
        public async Task Run(
            [ActivityTrigger] string fileName, 
            [Blob("charity-files/{fileName}", FileAccess.Write)] Stream file,
            ILogger log)
        {
            var charityFileDownloadUri = new Uri(Path.Combine(downloadUrl, fileName));
            log.LogDebug("Downloading file: {charityFileDownloadUrl}", charityFileDownloadUri);

            byte[] response;

            try
            {
                response = await _httpClient.GetByteArrayAsync(charityFileDownloadUri);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An unexpected error occurred when trying to download {charityFileDownloadUrl}", charityFileDownloadUri);
                throw;
            }

            await using (var memoryStream = new MemoryStream(response))
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    if (archive.Entries.Count == 0)
                        throw new InvalidOperationException(
                            $"Unsupported charity data zip file for {fileName}. Zip file contained no files.");

                    if (archive.Entries.Count > 1)
                        throw new InvalidOperationException(
                            $"Unsupported charity data zip file for {fileName}. File contained more than 1 file.  Files: {archive.Entries.Aggregate(string.Empty, (currText, zippedFile) => $"{currText}{zippedFile.Name}, ")}");
                    log.LogDebug($"File: {fileName} appears to be ok.");
                }
            }

            await file.WriteAsync(response, 0, response.Length);
            log.LogInformation($"Finished writing {fileName} to blob storage.");
        }
    }
}