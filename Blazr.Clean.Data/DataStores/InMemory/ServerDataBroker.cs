/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public class ServerDataBroker : IDataBroker
{
    private readonly IDbContextFactory<InMemoryWeatherDbContext> database;
    private bool _initialized = false;

    public ServerDataBroker(IDbContextFactory<InMemoryWeatherDbContext> db)
    {
        this.database = db;
        // We need to populate the database so we get a test data set from the static class WeatherForecastData
        if (!_initialized)
        {
            using var dbContext = database.CreateDbContext();
            dbContext.AddRange(WeatherForecastData.GetForecasts());
            dbContext.SaveChanges();
            _initialized = true;
        }
    }

    public async ValueTask<bool> AddRecordAsync<TRecord>(TRecord item) where TRecord : class, new()
    {
        using var dbContext = database.CreateDbContext();
        // Use the add method on the DbContect.  It knows what it's doing and will find the correct DbSet to add the rcord to
        dbContext.Add(item);

        // We should have added a single record so the return count should be 1
        return await dbContext.SaveChangesAsync() == 1;
    }

    public async ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord : class, new()
    {
        using var dbContext = database.CreateDbContext();

        // build the query against the DataSet
        // dbContext.Set<T> finds the correct DataSet in the DbContext and returns it as an IQueryable collection
        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        // Add the paging if the PageSize is not zero
        if (options.PageSize != 0)
        {
            query = query
            .Skip(options.StartIndex)
            .Take(options.PageSize);
        }

        // run the query and return the resulting IEnumerable collection
        return await query
            .ToListAsync();
    }
}
