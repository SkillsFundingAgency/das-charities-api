using EFCore.BulkExtensions;
using SFA.DAS.Charities.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public class CharityTrusteeStagingRepository : ICharityTrusteeStagingRepository
    {
        private readonly CharitiesDataContext _charitiesDataContext;

        public CharityTrusteeStagingRepository(CharitiesDataContext charitiesDataContext)
        {
            _charitiesDataContext = charitiesDataContext;
        }

        public async Task BulkInsert(List<CharityTrusteeStaging> data)
        {
            using var tx = await _charitiesDataContext.Database.BeginTransactionAsync();

            await _charitiesDataContext.BulkInsertAsync(data);

            await tx.CommitAsync();
        }
    }
}
