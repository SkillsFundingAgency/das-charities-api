using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

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

        public static IEnumerable<T> ExtractDataStream<T>(Stream zipFile)
        {
            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
            var zipEntry = archive.Entries.FirstOrDefault();
            var reader = new StreamReader(zipEntry.Open());

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                yield return JsonConvert.DeserializeObject<T>(line);
            }
        }

        public static int GetZipFileEntriesCount(Stream contentStream)
        {
            // Ensure the stream position is at the beginning before processing
            if (contentStream.CanSeek)
            {
                contentStream.Position = 0;
            }

            using (var archive = new ZipArchive(contentStream, ZipArchiveMode.Read, leaveOpen: true))
            {
                return archive.Entries.Count;
            }
        }

    }
}
