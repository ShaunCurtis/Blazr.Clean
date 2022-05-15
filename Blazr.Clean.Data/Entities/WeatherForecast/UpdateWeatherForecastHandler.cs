﻿namespace Blazr.Clean.Data;

public class UpdateWeatherForecastHandler
    : IRecordActionHandler<UpdateWeatherForecastCommand, CommandResponse>
{
    private readonly IDbContextFactory<InMemoryWeatherDbContext> _dbContextFactory;
    private readonly UpdateWeatherForecastCommand _command;

    public UpdateWeatherForecastHandler(IDbContextFactory<InMemoryWeatherDbContext> factory, UpdateWeatherForecastCommand command )
    { 
        _dbContextFactory = factory;
        _command = command;
    }

    public async ValueTask<CommandResponse> ExecuteAsync()
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Update(_command.Record);
        var count = await context.SaveChangesAsync();

        return count == 1
            ? new CommandResponse { Id = _command.Record.Id, Message = "Record Saved", Success = true }
            : new CommandResponse { Id = _command.Record.Id, Message = "Error saving Record", Success = false };
   }
}
