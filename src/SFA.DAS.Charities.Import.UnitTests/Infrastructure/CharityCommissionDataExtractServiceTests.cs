using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Charities.Import.Infrastructure;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.UnitTests.Infrastructure
{
    [TestFixture]
    public class CharityCommissionDataExtractServiceTests
    {        
        [Test]
        public async Task ExtractData_ValidDataFile_ReturnsData()
        {
            var jsonData = @"[{'name':'Pumbaa','type':'Warthog'},{'name':'Timon','type':'Meerkat'}]";
            var subject = new CharityCommissionDataExtractService(Mock.Of<ILogger<CharityCommissionDataExtractService>>());            
            var data = await subject.ExtractData<Character>(GetZipFile(jsonData));
            Assert.IsNotNull(data);
            Assert.AreEqual(2, data.Count);

        }

        [Test]
        public void ExtractData_InvalidDataFile_ThrowsException()
        {
            var subject = new CharityCommissionDataExtractService(Mock.Of<ILogger<CharityCommissionDataExtractService>>());
            Assert.ThrowsAsync(typeof(JsonReaderException), () => subject.ExtractData<Character>(GetZipFile("bad json data")));

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
