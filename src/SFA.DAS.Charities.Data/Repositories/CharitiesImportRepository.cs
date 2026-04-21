using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Charities.Data.Repositories;

[ExcludeFromCodeCoverage]
public class CharitiesImportRepository : ICharitiesImportRepository
{
    private readonly CharitiesDataContext _charitiesDataContext;

    public CharitiesImportRepository(CharitiesDataContext charitiesDataContext)
    {
        _charitiesDataContext = charitiesDataContext;
        _charitiesDataContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
    }

    public async Task BulkInsert<T>(IEnumerable<T> data, CancellationToken cancellationToken) where T : class
    {
        var strategy = _charitiesDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var tx = await _charitiesDataContext.Database.BeginTransactionAsync(cancellationToken);

            _charitiesDataContext.Database.CreateExecutionStrategy();

            await _charitiesDataContext.BulkInsertAsync(data, cancellationToken: cancellationToken);
            await tx.CommitAsync(cancellationToken);
        });
    }

    public async Task DeleteStagingData(string tableName, CancellationToken cancellationToken)
    {
        string command = tableName switch
        {
            "CharityStaging" => "TRUNCATE TABLE [dbo].[CharityStaging]",
            "CharityTrusteeStaging" => "TRUNCATE TABLE [dbo].[CharityTrusteeStaging]",
            _ => throw new ArgumentException("Unknown table", nameof(tableName))
        };

        await _charitiesDataContext.Database.ExecuteSqlRawAsync(command, cancellationToken);
    }

    public async Task LoadDataFromStagingInToLive(CancellationToken cancellationToken)
    {
        var strategy = _charitiesDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var tx = await _charitiesDataContext.Database.BeginTransactionAsync(cancellationToken);

            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"TRUNCATE TABLE CharityTrustee", cancellationToken);
            await _charitiesDataContext.Database.ExecuteSqlInterpolatedAsync($"TRUNCATE TABLE Charity", cancellationToken);
            await _charitiesDataContext.Database.ExecuteSqlRawAsync("INSERT INTO Charity SELECT * FROM CharityStaging", cancellationToken);
            await _charitiesDataContext.Database.ExecuteSqlRawAsync("INSERT INTO CharityTrustee SELECT * FROM CharityTrusteeStaging", cancellationToken);

            await tx.CommitAsync(cancellationToken);
        });
    }
}
