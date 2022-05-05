/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public interface IViewService<TRecord> where TRecord : class
{
    public event EventHandler? ListUpdated;
    public IEnumerable<TRecord> Records { get; }
    public ValueTask<bool> AddRecordAsync(TRecord record);
    public ValueTask GetRecordsAsync(ListOptions options);
}
