# DataProcessor

## Overview
DataProcessor is a .NET library that provides DataFrame- and Series-like data structures with typed storage, indexing, and query/aggregation utilities. It is designed for high-performance data manipulation with extensible storage backends and engine wrappers for computation, sorting, and querying.

## Goals
- Provide efficient, typed storage for columnar data.
- Offer Series/DataFrame APIs with indexing, slicing, grouping, and querying.
- Keep core algorithms in engine wrappers for reuse and optimization.

## Non-Goals
- Provide a full pandas-compatible API.
- Serve as a database or persistent storage engine.

## Key Capabilities
- Non-generic and generic series APIs.
- Index types (Range, String, DateTime, MultiIndex, etc.).
- Value storage implementations (int, long, double, decimal, char, string, object, DateTime, bool).
- CSV load/export utilities.
- Engine wrappers for computation, sorting, and query execution.
- In-progress query builder API for Series filtering, projection, and sorting.

## Project Structure (High Level)
- `source/API`: Public-facing Series and DataFrame APIs.
- `source/Core`: Index and ValueStorage implementations.
- `source/EngineWrapper`: Computation, sorting, and query engines.
- `source/LoaderAndExporter`: CSV loader/exporter and data IO utilities.
- `source/UserSettings`: Configuration and default values.

## Current Status
- Core Series, index, value storage, CSV IO, and aggregation features are available.
- Query execution supports basic Series query operations and is still being completed.
- Query sorting nodes/executors are in progress and should be treated as active development work.

## Target Framework
- `net9.0`

## Dependencies
- `CsvHelper` (CSV IO)
- `xUnit` and `NUnit` (testing)
