using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Import.Infrastructure
{
    internal interface ICharityCommissionDataExtractService
    {
        List<T> ExtractData<T>(Stream zipFile);
    }
}