using System.Collections.Generic;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Charities.Data.Repositories
{
    public class CharitiesImportRepository : ICharitiesImportRepository
    {
        private readonly CharitiesDataContext _charitiesDataContext;

        public CharitiesImportRepository(CharitiesDataContext charitiesDataContext)
        {
            _charitiesDataContext = charitiesDataContext;
        }

        public async Task BulkInsert<T>(IList<T> data) where T : class
        {
            using var tx = await _charitiesDataContext.Database.BeginTransactionAsync();

            await _charitiesDataContext.BulkInsertAsync<T>(data);

            await tx.CommitAsync();
        }

        public async Task ClearStagingData()
        {
            using var tx = await _charitiesDataContext.Database.BeginTransactionAsync();

            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"TRUNCATE TABLE CharityTrusteeStaging");
            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"TRUNCATE TABLE CharityStaging");

            await tx.CommitAsync();
        }

        public async Task LoadDataFromStagingInToLive()
        {
            using var tx = await _charitiesDataContext.Database.BeginTransactionAsync();

            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"TRUNCATE TABLE CharityTrustee");
            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"TRUNCATE TABLE Charity");
            await _charitiesDataContext.Database.ExecuteSqlRawAsync("INSERT INTO Charity SELECT * FROM CharityStaging");
            await _charitiesDataContext.Database.ExecuteSqlRawAsync("INSERT INTO CharityTrustee SELECT * FROM CharityTrusteeStaging");

            await tx.CommitAsync();
        }
    }
}
