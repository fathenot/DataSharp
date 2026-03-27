# Architecture

## Design Principles
- **Separation of concerns**: API layer in `source/API`, core data structures in `source/Core`, algorithms in `source/EngineWrapper`.
- **Typed storage**: Concrete storage classes for each primitive type reduce boxing and improve performance.
- **Index-aware operations**: Index implementations are first-class and used across Series/DataFrame.

## Core Components

### API Layer
- **NonGenericsSeries** (`source/API/NonGenericsSeries`)
  - `Series`: Dynamic series with `object?` values.
  - Provides indexing, grouping, sorting, and query helpers.
- **GenericsSeries** (`source/API/GenericsSeries`)
  - `Series<T>`: Strongly typed series.

### Core Layer
- **IndexTypes** (`source/Core/IndexTypes`)
  - RangeIndex, StringIndex, DateTimeIndex, MultiIndex, etc.
  - Index operations: lookups, distinct, position mapping.
- **ValueStorage** (`source/Core/ValueStorage`)
  - Storage for primitives and objects.
  - Null tracking via `NullBitMap`.
  - `ValuesSpan` accessors for low-overhead reads.

### Engine Layer
- **ComputationEngine**
  - SIMD-based sum/mean operations for numeric types.
- **SortingEngine**
  - Index/value sorting utilities.
- **QueryEngine**
  - Query plan nodes and executors.
  - Eager query approach returning positions for reconstruction in Series.

## Data Flow (Example)
1. `Series.Query(...)` executes in API layer.
2. `QueryEngine` returns matching positions.
3. API layer reconstructs `Series` using values/index at those positions.

## Error Handling
- Argument validation at API and core layers.
- Exceptions for unsupported operations or invalid indices.

## Performance Considerations
- Typed storage avoids repeated boxing.
- `ReadOnlySpan<T>` exposes data for fast operations.
- SIMD in computation engine for numeric aggregation.
