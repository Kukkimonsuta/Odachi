; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 10.0.1

### New Rules

Rule ID | Category | Severity | Notes
--------|--------------------------|----------|-------------------------------------------------------------------
ODA001  | Odachi.Testing.Scenarios | Error    | Fixture is declared outside of a scenario
ODA002  | Odachi.Testing.Scenarios | Error    | Fixture cannot be declared by a static member
ODA003  | Odachi.Testing.Scenarios | Warning  | Fixture type is declared more than once in the same scope
ODA004  | Odachi.Testing.Scenarios | Error    | Type declaring a fixture must be partial
ODA005  | Odachi.Testing.Scenarios | Info     | Fixture accessor name is not available
ODA006  | Odachi.Testing.Scenarios | Info     | Fixture accessor name cannot be derived
