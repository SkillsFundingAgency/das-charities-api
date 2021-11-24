using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    public static class CharityCommissionDataHelper 
    {
        public static List<T> ExtractData<T>(Stream zipFile)
        {
            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
            var zipEntry = archive.Entries.FirstOrDefault();

            var reader = new StreamReader(zipEntry.Open());
            var json = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<List<T>>(json);

            return result;
        }

        public static int GetZipFileEntriesCount(byte[] content)
        {
            using (var memoryStream = new MemoryStream(content))
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
            return archive.Entries.Count;
        }
    }
}
