# Contributing

## Coding Standards
- Follow C# naming conventions and existing patterns in the codebase.
- Prefer explicit null handling and argument validation.
- Keep public API in `source/API`, core logic in `source/Core`, algorithms in `source/EngineWrapper`.
- Document public APIs and update `docs/` when behavior changes.

## Testing
- Add tests for new behavior.
- xUnit is preferred for new tests in `test/`.
- Run `dotnet build` before handing off changes.

## Pull Request Checklist
- [ ] Code compiles locally (`dotnet build`).
- [ ] Tests pass, or any known failures are documented with the feature that owns them.
- [ ] New APIs are documented in `docs/`.
