/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public interface IRecordQueryHandler<in TQuery, TResponse>
    where TQuery : IRecordQuery<TResponse>
{
    ValueTask<TResponse> ExecuteAsync();
}
