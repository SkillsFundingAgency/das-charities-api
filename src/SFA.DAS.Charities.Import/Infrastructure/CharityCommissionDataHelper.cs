using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;

namespace SFA.DAS.Charities.Import.Infrastructure;

public interface ICharityCommissionDataHelper
{
    int GetZipFileEntriesCount(Stream contentStream);
    IAsyncEnumerable<T> ExtractDataStream<T>(Stream zipFile, CancellationToken cancellationToken = default);
}

[ExcludeFromCodeCoverage]
public class CharityCommissionDataHelper : ICharityCommissionDataHelper
{
    public async IAsyncEnumerable<T> ExtractDataStream<T>(Stream zipFile, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
        var zipEntry = archive.Entries.FirstOrDefault();

        using Stream stream = await zipEntry.OpenAsync();
        using var sr = new StreamReader(stream);
        using var reader = new JsonTextReader(sr);

        var serializer = new JsonSerializer();

        while (await reader.ReadAsync(cancellationToken))
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                // JsonSerializer does not provide an async Deserialize for JsonTextReader
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
