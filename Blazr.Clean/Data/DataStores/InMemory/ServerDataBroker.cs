using Blazr.Clean.Core;
using Microsoft.EntityFrameworkCore;

namespace Blazr.Clean.Data;

public class ServerDataBroker : IDataBroker
{
    private readonly IDbContextFactory<InMemoryDbContext> database;
    private bool _initialized = false;

    public ServerDataBroker(IDbContextFactory<InMemoryDbContext> db)
    {
        this.database = db;
        if (!_initialized)
        {
            using var dbContext = database.CreateDbContext();
            dbContext.AddRange(WeatherForecastData.GetForecasts());
            dbContext.SaveChanges();
            _initialized = true;
        }
    }

    public async ValueTask<bool> AddRecordAsync<TRecord>(TRecord item) where TRecord : class
    {
        using var dbContext = database.CreateDbContext();
        dbContext.Add(item);

        return await dbContext.SaveChangesAsync() == 1;
    }

    public async ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord : class
    {
        using var dbContext = database.CreateDbContext();

        return await dbContext.Set<TRecord>()
            .Skip(options.StartIndex)
            .Take(options.PageSize)
            .ToListAsync();
    }
}
