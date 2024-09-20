using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FluentAssertions;
using System;

namespace SFA.DAS.Charities.Import.UnitTests.Infrastructure
{
    [TestFixture]
    public class CharityCommissionDataHelperTests
    {
        [Test]
        public void ExtractDataStream_ValidDataFile_ReturnsData()
        {
            var jsonData = @"[{'name':'Pumbaa','type':'Warthog'},{'name':'Timon','type':'Meerkat'}]";
            var charityData = CharityCommissionDataHelper.ExtractDataStream<Character>(GetZipFile(jsonData)).ToList();

            charityData.Should().NotBeNull();
            charityData.Count().Should().Be(2);
        }

        [Test]
        public void ExtractDataStream_InvalidDataFile_ThrowsException()
        {
            // Arrange
            var stream = GetZipFile("bad json data");

            // Act & Assert
            Action action = () => CharityCommissionDataHelper.ExtractDataStream<Character>(stream).ToList();

            action.Should().Throw<JsonReaderException>();
        }


        private Stream GetZipFile(string json)
        {
            var memoryStream = new MemoryStream();
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

            var demoFile = archive.CreateEntry("foo.txt");

            using (var entryStream = demoFile.Open())
            using (var streamWriter = new StreamWriter(entryStream))
            {
                streamWriter.Write(json);
            }
            return memoryStream;
        }

        internal class Character
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

    }
}
