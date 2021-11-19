using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    internal interface ICharityCommissionDataExtractService
    {
        Task<List<T>> ExtractData<T>(Stream zipFile);
    }
}