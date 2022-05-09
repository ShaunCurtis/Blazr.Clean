# Views

Views provide the link between the Core and UI domain code.  They are implemented as services: their scope dependant on their usage.

Views hold and manage the data displayed in the UI.  Data should be in the views, not the UI components.  The record set collection is held in `Records`, and the current record in `Record`.

This application uses an `IViewService` interface to define core view functionality, and a generics based `ViewServiceBase` abstract class to implement the functionality.  In more complex applications you may need to implement a more complex model, but this simplistic implementation demonstrates the key principles.

We implement only add and list functionality, so define two async methods to match that functionality.  Note: 

1. `GetRecordsAsync` doesn't return a collection: the collection is loaded into the `Records` collection property.  The UI uses this list.
2. `AddRecordAsync` add the current `Record` which will normally have been edited in the UI. 
3. `ListUpdated` is an event that the UI can use to react to changes to `Records`. 

**IViewService**

```csharp
public interface IViewService<TRecord> where TRecord : class
{
    public event EventHandler? ListUpdated;
    public IEnumerable<TRecord> Records { get; }
    public TRecord Record { get; }
    public ValueTask<bool> AddRecordAsync();
    public ValueTask GetRecordsAsync(ListOptions options);
}
```

### ViewServiceBase

`ViewServiceBase` is abstract and generic: you can't use it directly, and need to define `TRecord` in a concrete implementation.  The code is commented to explain functionality. 
```
public abstract class ViewServiceBase<TRecord> : IViewService<TRecord> where TRecord : class, new()
{
    // The IDataBroker we'll use to access the data store
    private IDataBroker _broker;

    // The current list options being applied to the Records collection
    // set when GetReecordsAsync is called
    private ListOptions _options = new ListOptions { PageSize = 1000 };
    
    // The event raised whenever the list changes
    // UI components can register event handlers with this to force UI updates of their content
    public event EventHandler? ListUpdated;
    
    // The current record collection
    // Updated whenever GetRecordsAsync is called
    public IEnumerable<TRecord> Records { get; protected set; } = new List<TRecord>();

    // The current record being displayed or edited
    public TRecord Record { get; protected set; } = new TRecord();

    // New - gets the DI registered IDataBroker registered service
    public ViewServiceBase(IDataBroker dataBroker)
        => _broker = dataBroker;

    // Command Method that adds a new record to the data store
    public async ValueTask<bool> AddRecordAsync()
    {
        // Add the record using the Data Broker
        var result = await _broker.AddRecordAsync<TRecord>(this.Record);

        if (result)
        {
            //Read the record back from the data store
            await this.GetRecordsAsync(_options);
            // Raise the ListChanged event as one record within it has changed
            this.ListUpdated?.Invoke(this, EventArgs.Empty);
        }

        return result;
    }

    // Command method that populates the Records collection based on the List Options
    public async ValueTask GetRecordsAsync(ListOptions options)
    {
        // Save the latest options locally
        _options = options;
        // Gets the record collection
        this.Records = await _broker.GetRecordsAsync<TRecord>(options);
    }
}
```

We can then implement our concrete `WeatherForecastViewService` like this:

```
public class WeatherForecastViewService : ViewServiceBase<WeatherForecast>
{
    public WeatherForecastViewService(IDataBroker dataBroker)
        : base(dataBroker)
    { }
}
```

### Important Points

1. All the data is retained in the View.
2. Generics and base abstract classes make implementing View Services for a set of records simple and straight forward.
3. All inter domain communication is through interfaces defined in the Core domain.  