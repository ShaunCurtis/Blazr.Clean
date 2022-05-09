# Project Architecture and Design

## Clean Design

The solution is based on **Clean Design** principles.

![Clean Design](./clean-design.png)

The separation of concerns principles are enforced through projects and project dependancies.  All the application code resides in libraries.  The applications, in this case Blazor Server and Blazor WASM SPAs, are endpoints.

## Coding Standards

I (try to) implement good code practices throughout the solution: principally SOLID and CQRS.  I use coding patterns where appropiate.

Dependancy Injection is probably the most important SOLID principle to understand and implement in Blazor.  You'll see it implemented throughout the solution.

## Generics and Boilerplating

Generics let us boilerplate code.  A common design pattern is:

1. An `interface` to define the common functionality and abstraction.
2. An `abstract` base class that implements the interface functionality using generics.
3. Concrete implementation class that simply fix the generics and inherit their functionality from the base class.

One interface, one abstract. many concrete.

We can see this in the View Services.  The concrete `WeatherForecastViewService` looks like this:

```csharp
public class WeatherForecastViewService : ViewServiceBase<WeatherForecast>
{
    public WeatherForecastViewService(IDataBroker dataBroker)
        : base(dataBroker)
    { }
}
```

## Database Access

I always use an ORM to simplify database access.  In this project I've stuck with mainstream Entity Framework, but could have used Dapper or Linq2DB.

There's no classic Repository Pattern implementation.  Instead there's a generics based Data Broker pattern that removes most of the repository pattern complexity.  Generics are applied at the method level: the same Data Broker is used for CRUD and List operations on all data sets.

EF is kept simple, mapping to database tables and views.  I'm a firm believer in clean design.  The relationships between data objects is part of the application/business logic and belong on the core domain, definitley not the Data layer! 
