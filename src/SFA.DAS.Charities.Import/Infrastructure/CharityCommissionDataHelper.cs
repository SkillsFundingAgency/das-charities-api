using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    public static class CharityCommissionDataHelper
    {      
        public static IEnumerable<T> ExtractDataStream<T>(Stream zipFile)
        {
            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
            var zipEntry = archive.Entries.FirstOrDefault();

            using var reader = new StreamReader(zipEntry.Open());
            var json = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<List<T>>(json);

            foreach (var item in result)
            {
                yield return item;
            }
        }

        public static int GetZipFileEntriesCount(Stream contentStream)
        {
            // Ensure the stream position is at the beginning before processing
            if (contentStream.CanSeek)
            {
                contentStream.Position = 0;
            }

            using var archive = new ZipArchive(contentStream, ZipArchiveMode.Read, leaveOpen: true);
            return archive.Entries.Count;
        }

    }
}
