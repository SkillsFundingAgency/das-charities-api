using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.Charities.Import.Infrastructure;

public interface ICharityCommissionDataHelper
{
    int GetZipFileEntriesCount(Stream contentStream);
    IEnumerable<T> ExtractDataStream<T>(Stream zipFile);
}

[ExcludeFromCodeCoverage]
public class CharityCommissionDataHelper : ICharityCommissionDataHelper
{
    public IEnumerable<T> ExtractDataStream<T>(Stream zipFile)
    {
        using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
        var zipEntry = archive.Entries.FirstOrDefault();

        using Stream stream = zipEntry.Open();
        using var sr = new StreamReader(stream);
        using var reader = new JsonTextReader(sr);

        var serializer = new JsonSerializer();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                yield return serializer.Deserialize<T>(reader);
            }
            else if (reader.TokenType == JsonToken.EndArray)
            {
                yield break;
            }
        }
    }

    public int GetZipFileEntriesCount(Stream contentStream)
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
