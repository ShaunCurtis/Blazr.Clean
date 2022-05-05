# Data

The data classes reside in the Core domain.  They are used by all three domains.

The modified data class `WeatherForecast` looks like this:

```csharp
public class WeatherForecast
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}
```

It has a new unique ID field labelled with the [Key] attribute to make it database compatible.

### Collections

Collections present a few issues that are not often considered when designing an application.  They only come to light at a later stage.  In this application we:
1. Never retrieve unconstrained collections.  All methods that retrieve lists that can grow require a `ListOptions` object argument that constrains the number of records retrieved.
2. Return `IEnumerable<T>` collections for normal query methods.

`ListOptions` looks like this:

```csharp
public class ListOptions
{
    public int StartIndex { get; set; } = 0;
    public int PageSize { get; set; } = 25;
}
```

This can get extended to include sorting and filtering if required.  Note the default page size is set small.

## DBContext

The DbContext is defined in the data domain.

```csharp
public class InMemoryDbContext : DbContext
{
    public DbSet<WeatherForecast>? WeatherForecast { get; set; }

    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : base(options) { }
}
```

It defines a single `DbSet` for the the `WeatherForecast` data table.

The application implements database access through an IDbContextFactory defined DI service as follows: 

```csharp
builder.Services.AddDbContextFactory<InMemoryDbContext>(options => options.UseInMemoryDatabase("TestDb"));
```


## Data Brokers

Data brokers provide the link between the Core and Data domains.  `IDataBroker` is defined in the core domain.

```csharp
public interface IDataBroker
{
    public ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord: class, new();
    public ValueTask<bool> AddRecordAsync<TRecord>(TRecord record) where TRecord : class, new();
}
```

In this simple application we only implement two CRUD operations.  Note:

1. The methods use generics.  We'll see how the implementation works in the base class.
2. All the methods are `Async` and use `ValueTasks`.
3. The `GetRecordsAsync` has a `ListOptions` argument.

The Server data broker looks like:

```csharp
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
```

Thee are comments in the code.

1. As this is a test data broker the test data is added in the new method.  `WeatherForecastData` generates the teat data.

2. Each method creates a `DbContext` from the `IDbContextFactory` for the duration of the method.

3. The CUD methods pass the new/updated record to the DbContext and let EF do the magic of finding the right DbSet and record and updating iit/adding it to the database.

4. The Read/List methods accept a generic `TRecord` which they use through the `Set` method on the `DbContext` to obtain the correct `IQueryable` collection.

5. The list method builds a query based on `ListOptions` against the `IQueryable` collection and then returns the async result.

This design is a little different to the classic `IRepository` pattern.  The DataBroker is generic.  It simplifies the data access classes you need: you use the same DI instance for all standard CRUD/List operations on all DBSets in the database.
  

## Views

In Blazor views, which provide the links between the Core and UI domain code are implemented as services.  Their scope depends on their usage.

This application uses an `IViewService` interface to define and then a generics based `ViewServiceBase` abstract class to implement core functionality.  In more complex applications you may need to implement a more complex model, but this simplistic impelementation demonstrates the key principles.

**IViewService**

```csharp
public interface IViewService<TRecord> where TRecord : class
{
    public event EventHandler? ListUpdated;
    public IEnumerable<TRecord> Records { get; }
    public ValueTask<bool> AddRecordAsync(TRecord record);
    public ValueTask GetRecordsAsync(ListOptions options);
}
```

****
 