/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public class CommandResponse
{
    public Guid Id { get; set; }

    public bool Success { get; set; }

    public string? Message { get; set; }
}
