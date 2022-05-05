/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public interface IDataBroker
{
    public ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord: class, new();
    public ValueTask<bool> AddRecordAsync<TRecord>(TRecord record) where TRecord : class, new();
}
