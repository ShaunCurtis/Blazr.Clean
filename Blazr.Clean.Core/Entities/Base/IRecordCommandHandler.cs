/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public interface IRecordCommandHandler<in TCommand, TResult> where TCommand : IRecordCommand<TResult>
{
    ValueTask<TResult> ExecuteAsync();
}
