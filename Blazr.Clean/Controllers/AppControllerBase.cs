/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Controllers;

[ApiController]
public abstract class AppControllerBase<TRecord> 
    : Mvc.ControllerBase
    where TRecord : class, new()
{
    private IDataBroker _dataBroker;

    public AppControllerBase(IDataBroker dataBroker)
        => _dataBroker = dataBroker;

    [Mvc.Route("/api/list/[controller]")]
    [Mvc.HttpPost]
    public async Task<IEnumerable<TRecord>> GetRecordsAsync([FromBody] ListOptions options)
        => await _dataBroker.GetRecordsAsync<TRecord>(options);

    [Mvc.Route("/api/add/[controller]")]
    [Mvc.HttpPost]
    public async Task<bool> AddRecordAsync([FromBody] TRecord record)
        => await _dataBroker.AddRecordAsync<TRecord>(record);
}
