namespace Blazr.Clean.Core;

public interface IDataBroker
{
    public ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord: class;
    public ValueTask<bool> AddRecordAsync<TRecord>(TRecord record) where TRecord : class;
}
