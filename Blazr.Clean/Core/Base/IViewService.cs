namespace Blazr.Clean.Core;

public interface IViewService<TRecord> where TRecord : class
{
    public event EventHandler? ListUpdated;
    public IEnumerable<TRecord> Records { get; }
    public ValueTask<bool> AddRecordAsync(TRecord record);
    public ValueTask GetRecordsAsync(ListOptions options);
}
