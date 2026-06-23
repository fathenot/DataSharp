# Usage

## Non-Generic Series
```csharp
using DataProcessor.source.API.NonGenericsSeries;

var s = new Series(new object?[] { 10, 20, 30 }, name: "example");
var head = s.Head(2);
var tail = s.Tail(1);
```

## Generic Series
```csharp
using DataProcessor.source.API.GenericsSeries;

var s = new Series<int>(new List<int> { 1, 2, 3 }, name: "ints");
var filtered = s.Filter(x => x > 1);
```

## Indexing and Slicing
```csharp
// Access by index.
var values = s[0];

// Create views.
var view = s.GetView((start: 0, end: 2, step: 1));
```

## Aggregations
```csharp
var sum = s.Sum();
```

## Sorting
```csharp
var sorted = s.SortValues(ascending: true);
```

## Query (Eager)
```csharp
using DataProcessor.source.API.NonGenericsSeries;

var s = new Series(new object?[] { 10, 20, 30 }, name: "query-example");
var result = s.Query(q => q.Where<int>(value => value > 10));
```

Query operations are executed eagerly and return a new `Series`. Filtering and projection are available; query-level sorting is in progress.
