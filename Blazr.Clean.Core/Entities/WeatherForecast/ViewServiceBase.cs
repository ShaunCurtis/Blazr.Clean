/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

/// <summary>
/// This is the base class for all standard View Services.
/// It implements the standard functionality required by specific implementations using generics
/// It's normal Scope is Scoped i.e. one per user session.
/// </summary>
/// <typeparam name="TRecord"></typeparam>
public abstract class ViewServiceBase<TRecord> : IViewService<TRecord> where TRecord : class, new()
{
    // The IDataBroker we'll use to access the data store
    private IDataBroker _broker;

    // The current list options being applied to the Records collection
    // set when GetReecordsAsync is called
    private ListOptions _options = new ListOptions { PageSize = 1000 };
    
    // The event raised whenever the list changes
    // UI components can register event handlers with thia to force UI updates of their content
    public event EventHandler? ListUpdated;
    
    // The current record collection
    // Updated whenever GetRecordsAsync is called
    public IEnumerable<TRecord> Records { get; protected set; } = new List<TRecord>();

    // New - gets the DI registered IDataBroker registeted service
    public ViewServiceBase(IDataBroker dataBroker)
        => _broker = dataBroker;

    // Command Method that adds a new record to the data store
    public async ValueTask<bool> AddRecordAsync(TRecord record)
    {
        var result = await _broker.AddRecordAsync<TRecord>(record);

        if (result)
        {
            await this.GetRecordsAsync(_options);
            this.ListUpdated?.Invoke(this, EventArgs.Empty);
        }

        return result;
    }

    // Command method that populates the Records collection based on the List Options
    public async ValueTask GetRecordsAsync(ListOptions options)
    {
        _options = options;
        this.Records = await _broker.GetRecordsAsync<TRecord>(options);
    }
}
