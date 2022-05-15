# Data

The application implements two data pipelines to demonstrate two methodologies:

1. A Data Broker pattern
2. A Command/Query pattern 

## Data Classes

The data classes are core domain objects: they are used throughout the application.

The modified data class `WeatherForecast` looks like this:

```csharp
public class WeatherForecast
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
```

It has a new unique ID field labelled with the [Key] attribute for database compatibility: the database needs a unique field to identify records.  `TemperatureF` has gone.  It's an internal calculated parameter.  We'll add it back in the business logic.

### Collections

The application code:
1. Never retrieves unconstrained collections.  All methods that retrieve lists that can grow require a `ListOptions` object argument that constrains the number of records retrieved.
2. Returns `IEnumerable<T>` collections for normal query methods.

The `ListOptions` class looks like this:

```csharp
public class ListOptions
{
    public int StartIndex { get; set; } = 0;
    public int PageSize { get; set; } = 1000;
}
```

This can be extended to include sorting and filtering if required.  Note the default page size is set to 1000.

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

The application implements database access through an IDbContextFactory defined as a DI service as follows: 

```csharp
builder.Services.AddDbContextFactory<InMemoryDbContext>(options => options.UseInMemoryDatabase("TestDb"));
```


## Data Brokers

Data brokers are the link between the Core and Data domains.  `IDataBroker` is defined in the core domain.

```csharp
public interface IDataBroker
{
    public ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord: class, new();
    public ValueTask<bool> AddRecordAsync<TRecord>(TRecord record) where TRecord : class, new();
}
```

The application only implements two CRUD operations.  Note:

1. The methods are generic.  We'll see how the implementation works in the base class.
2. All the methods are `async` and use `ValueTasks`.
3. The `GetRecordsAsync` has a `ListOptions` argument.

### Server Data Broker

The Server data broker is the `IDataBroker` implementation for Blazor Server and the API web server.

```csharp
public class ServerDataBroker : IDataBroker
{
    private readonly IDbContextFactory<InMemoryDbContext> database;
    private bool _initialized = false;

    public ServerDataBroker(IDbContextFactory<InMemoryDbContext> db)
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

1. As this is a test data broker the test data is added in the new method.  The static class `WeatherForecastData` generates the test data.

2. `DbContexts` are created (and disposed) for each method.  Their lifecycles are controlled by the  `IDbContextFactory`.  We are living in an async world: there may be more than one act at any one time.

3. The CUD (Create/Update/Delete) methods pass the new/updated record to the DbContext and let EF do it's magic finding the right DbSet and record and updating/adding/deleting it to the database.

4. The Read/List methods accept a generic `TRecord` which they use through the `Set` method on the `DbContext` to obtain the correct `IQueryable` collection.

5. The list method builds a query based on `ListOptions` against the `IQueryable` collection and then returns the async result.

This design is a little different to the classic `IRepository` pattern.  The DataBroker is generic.  It simplifies the data access classes you need: you use the same DI instance for all standard CRUD/List operations on all DBSets in the database.

### APIDataBroker

The API data broker implementation uses the `HttpClient` to query the server side API.  The key to making this generic is using structured nomeclature. 

```csharp
public class APIDataBroker : IDataBroker
{
    private readonly HttpClient? _httpClient;
    private HttpClient httpClient => _httpClient!;

    public APIDataBroker(HttpClient httpClient)
        => this._httpClient = httpClient!;

    public async ValueTask<bool> AddRecordAsync<TRecord>(TRecord record) where TRecord : class, new()
    {
        var result = false;
        var response = await this.httpClient.PostAsJsonAsync<TRecord>($"/api/add/{record?.GetType().Name}", record!);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<bool>();
        return result;
    }

    public async ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord : class, new()
    {
        IEnumerable<TRecord>? result = null;
        var rec = new TRecord();
        var response = await this.httpClient.PostAsJsonAsync<ListOptions>($"/api/list/{rec.GetType().Name}", options);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<IEnumerable<TRecord>>();
        return result ?? new List<TRecord>();
    }
}
```
## API Controllers

The controller on the other end of the API interfaces with the Server Data Broker.  We can't make this generic, so either create a single controller with a lot of `ifs` or `case` statements, or create a controller per record set. We can use generics to simplify this and get all the boiulerplate code into an abstract base class.

The controllers are placed in a separate project because some of the libraries required are not compatible with Blazor WASM compilation.

### AppControllerBase

```csharp
[ApiController]
public abstract class AppControllerBase<TRecord> 
    : Mvc.ControllerBase
    where TRecord : class, new()
{
    private IDataBroker _dataBroker;

    public AppControllerBase(IDataBroker dataBroker)
        => _dataBroker = dataBroker;

    [Mvc.Route("/api/list/[controller]")]
    [Mvc.HttpPost]
    public async Task<IEnumerable<TRecord>> GetRecordsAsync([FromBody] ListOptions options)
        => await _dataBroker.GetRecordsAsync<TRecord>(options);

    [Mvc.Route("/api/add/[controller]")]
    [Mvc.HttpPost]
    public async Task<bool> AddRecordAsync([FromBody] TRecord record)
        => await _dataBroker.AddRecordAsync<TRecord>(record);
}
```

### WeatherForecastController

`WeatherForecastController` is the concrete implementation for `WeatherForecast`.
  
```csharp
[ApiController]
public class WeatherForecastController : AppControllerBase<WeatherForecast>
{
    public WeatherForecastController(IDataBroker dataBroker) 
        : base(dataBroker)
    {}
}
```
