namespace Blazr.Clean.Core;

public class ViewServiceBase<TRecord> : IViewService<TRecord> where TRecord : class, new()
{
    private IDataBroker _broker;
    private ListOptions _options = new ListOptions { PageSize = 1000 };
    
    public event EventHandler? ListUpdated;
    
    public IEnumerable<TRecord> Records { get; protected set; } = new List<TRecord>();

    public ViewServiceBase(IDataBroker dataBroker)
        => _broker = dataBroker;

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

    public async ValueTask GetRecordsAsync(ListOptions options)
    {
        _options = options;
        this.Records = await _broker.GetRecordsAsync<TRecord>(options);
    }
}
