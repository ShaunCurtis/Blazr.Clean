# Data

The data classes reside in the Core domain.  They are used by all three domains.

The modified data class `WeatherForecast` looks like this:

```csharp
public class WeatherForecast
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}
```

It has a new unique ID field labelled with the [Key] attribute to make it database compatible.

### Collections

Collections present a few issues that are not often considered when designing an application.  They only come to light at a later stage.  In this application we:
1. Never retrieve unconstrained collections.  All methods that retrieve lists that can grow require a `ListOptions` object argument that constrains the number of records retrieved.
2. 

 