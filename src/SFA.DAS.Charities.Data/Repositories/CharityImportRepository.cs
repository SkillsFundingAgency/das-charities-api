﻿using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Data.Repositories
{
    public class CharityImportRepository : ICharityImportRepository
    {
        private readonly CharitiesDataContext _charitiesDataContext;

        public CharityImportRepository(CharitiesDataContext charitiesDataContext)
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

            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"delete from CharityTrusteeStaging");
            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"delete from CharityStaging");

            await tx.CommitAsync();
        }
    }
}
