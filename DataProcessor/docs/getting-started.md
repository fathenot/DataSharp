# Getting Started

## Prerequisites
- .NET SDK `9.0`

## Build
```powershell
# From repository root
 dotnet build
```

## Test
```powershell
# From repository root
 dotnet test
```

## Known Build/ Test Status
- Current build has **known failing QueryEngine errors** while that feature is under development.
- If you see failures in:
  - `source/EngineWrapper/QueryEngine/WhereExecutor.cs`
  - `source/EngineWrapper/QueryEngine/SeriesQueryExecutor.cs`
  those are expected until the query feature is completed.

## Running a Quick Example
```csharp
using DataProcessor.source.API.NonGenericsSeries;

var series = new Series(new object?[] { 1, 2, 3, 4, 5 });
var result = series.Take(new[] { 0, 2 });
// result.Values => [1, 3]
```
