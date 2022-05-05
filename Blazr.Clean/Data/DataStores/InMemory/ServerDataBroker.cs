﻿using Blazr.Clean.Core;
using Microsoft.EntityFrameworkCore;

namespace Blazr.Clean.Data;

public class ServerDataBroker : IDataBroker
{
    private readonly IDbContextFactory<InMemoryDbContext> database;
    private bool _initialized = false;

    public ServerDataBroker(IDbContextFactory<InMemoryDbContext> db)
    {
        this.database = db;
        // We need to polulate the database so we get a teat data set from WeatherForecastData
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

    public async ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord : class, new ()
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
