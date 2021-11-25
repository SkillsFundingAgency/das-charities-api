using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public class CharityImportRepository : ICharityImportRepository
    {
        private readonly CharitiesDataContext _charitiesDataContext;
        private readonly ILogger<CharityImportRepository> _logger;

        public CharityImportRepository(CharitiesDataContext charitiesDataContext, ILogger<CharityImportRepository> logger)
        {
            _charitiesDataContext = charitiesDataContext;
            _logger = logger;
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
