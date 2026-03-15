# DataSharp

DataSharp is a high-performance data processing library for .NET designed for efficient in-memory data analysis.

The project focuses on building column-based data structures and scalable data processing components inspired by modern data engineering systems.

## Motivation

High-performance data processing libraries are commonly implemented in C++ or Java. 
C++ offers excellent performance but often comes with significant complexity, 
while Java-based solutions may introduce higher memory overhead.

This project explores how the .NET ecosystem, particularly C#, can provide a 
balanced approach — combining strong performance characteristics with a modern, 
productive development environment.

This project is also a learning exploration into data system architecture and column-based data structures.

## Core Features

- Column-based data structures
- Immutable Series design
- Flexible indexing system
- Multiple index types (RangeIndex, MultiIndex, DateTimeIndex)
- Support for heterogeneous data storage
- Designed for integration with high-performance computation engines
  
It is designed to support:
- Immutable and dynamic Series/Frames with typed storage
- Efficient columnar access, vectorized operations, and shared memory interop
- Seamless integration with native backends via C++

## Architecture

DataSharp is organized around several core components:

- **Series** – Immutable column-like data structure
- **Index** – Flexible indexing system for data alignment
- **Value Storage** – Efficient storage for different data types
- **Query Layer** – Planned component for data querying and transformations

## Example

```csharp
var series = new Series(new int[] {1,2,3,4});
Console.WriteLine(series);
```
## Project Status

This project is currently under active development.

## License
MIT
