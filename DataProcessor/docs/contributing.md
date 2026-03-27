# Contributing

## Coding Standards
- Follow C# naming conventions and existing patterns in the codebase.
- Prefer explicit null handling and argument validation.
- Keep public API in `source/API`, core logic in `source/Core`, algorithms in `source/EngineWrapper`.

## Testing
- Add tests for new behavior.
- xUnit is preferred for new tests in `tests/`.

## Pull Request Checklist
- [ ] Code compiles locally (`dotnet build`).
- [ ] Tests pass or documented known failures.
- [ ] New APIs are documented in `docs/`.
