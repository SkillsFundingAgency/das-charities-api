using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    public class CharityCommissionDataExtractService : ICharityCommissionDataExtractService
    {
        private readonly ILogger<CharityCommissionDataExtractService> _logger;

        public CharityCommissionDataExtractService(ILogger<CharityCommissionDataExtractService> logger)
        {
            _logger = logger;
        }
        public List<T> ExtractData<T>(Stream zipFile)
        {
            var stopWatch = Stopwatch.StartNew();

            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
            var zipEntry = archive.Entries.FirstOrDefault();

            try
            {
                var reader = new StreamReader(zipEntry.Open());
                var json = reader.ReadToEnd();
                var result = JsonConvert.DeserializeObject<List<T>>(json);
                _logger.LogInformation($"It took {stopWatch.ElapsedMilliseconds}ms to extract {zipEntry.Name}");

                return result;
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, $"An error occurred trying to extract data {zipEntry.Name}");
                throw;
            }
        }
    }
}
