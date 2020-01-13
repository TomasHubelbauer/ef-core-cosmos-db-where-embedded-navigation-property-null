# EF Core CosmosDB `Where` Embedded Navigation Property `null`

In this repository I aim to analyze an apparent issue where CosmosDB EF Core provider's `Where` clause implementation
provides an argument to the predicate whose embedded navigation property resolves to `null` but after materialization of
the query, the entities have their embedded navigation properties materialized as well.

An example code where this could be triggered follows:

```csharp
var items = appDbContext.Items.Where(i => i.Metadata != null).ToArray();
```

In the above code, the query will return zero items even though an item exists whose `Metadata` property resolves to an
embedded entity which is not `null`. Executing the query without the `Where` clause, the materialized entity will not
have a `null` for the navigation property value.

This is hard to debug, because the EF Core provider expects an `Expression` not an actual `Func` (because it is analyzed
and turned into a SQL query string, not ever actually executed) so it is impossible to make the predicate multi-line and
place a breakpoint within to analyze the arguments one-by-one.

The reproduction in this repository is a simple .NET Core console application bootstrapped using `dotnet new console`.
The CosmosDB EF Core provider used is the `Microsoft.EntityFrameworkCore.Cosmos` NuGet package.

I've tried this with both version 2 and version 3 (`3.0.0-preview4.19216.3`) and I am unable to reproduce, so I must be
missing something which causes this to happen in the real application but is missing in my repro.

## To-Do
