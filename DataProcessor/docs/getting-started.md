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

## Known Build/Test Status
- The solution is expected to compile while the QueryEngine is under active development.
- Query sorting support is still being completed, so behavior around query-level sorting may change.

## Running a Quick Example
```csharp
using DataProcessor.source.API.NonGenericsSeries;

var series = new Series(new object?[] { 1, 2, 3, 4, 5 });
var result = series.Take(new[] { 0, 2 });
// result.Values => [1, 3]
```

## Query Preview
```csharp
using DataProcessor.source.API.NonGenericsSeries;

var series = new Series(new object?[] { 1, 2, 3, 4, 5 });
var filtered = series.Query(q => q.Where<int>(value => value > 2));
```
