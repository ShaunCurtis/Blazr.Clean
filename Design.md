# Clean Design

The solution is based on **Clean Design** principles.

![Clean Design](./clean-design.png)

The separation of concerns principles are enforced through projects and project dependancies.  All the application code resides in libraries.  The applications, in this case Blazor Server and Blazor WASM SPAs, are endpoints.

I try and implemnent good code practices throughout the solution: proincipally SOLID and CQRS.  I use coding patterns where appropiate.

Of all the SOLID principles, the most importsnt to understand is Dependancy Injection.  Blazor is built on DI services and components.  A fundimental understanding of DI and service containers is imperitive for good design in Blazor. 



