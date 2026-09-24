using Microsoft.CodeAnalysis;

namespace Odachi.Testing.Scenarios.XunitV3.Generators;

/// <summary>
/// Diagnostics reported about fixture declarations.
/// </summary>
/// <remarks>
/// The generator only reports the cases where it has to decline to generate an accessor. Rules
/// about fixture declarations themselves, including the ones generated initialization cannot
/// handle, are reported by the analyzer.
/// </remarks>
internal static class ScenarioDiagnostics
{
    private const string Category = "Odachi.Testing.Scenarios";

    /// <summary>
    /// Nothing initializes a fixture declared outside of a scenario, so accessing its value throws.
    /// </summary>
    public static readonly DiagnosticDescriptor FixtureOutsideScenario = new DiagnosticDescriptor(
        id: "ODA001",
        title: "Fixture is declared outside of a scenario",
        messageFormat: "Fixture declared by '{0}' is never initialized because '{1}' does not derive from '" + ScenarioConstants.ScenarioTypeName + "'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>
    /// A fixture declared by a static member resolves once and then keeps the value of the very
    /// first test, so generated code does not initialize it.
    /// </summary>
    public static readonly DiagnosticDescriptor StaticFixtureNotSupported = new DiagnosticDescriptor(
        id: "ODA002",
        title: "Fixture cannot be declared by a static member",
        messageFormat: "Fixture declared by static member '{0}' is never initialized, a fixture keeps the value it resolved to and cannot be shared by more than one test - declare it as an instance member",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>
    /// A scope holds at most one fixture per type, so the second declaration silently resolves to
    /// the instance the first one created and its factory never runs.
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateFixtureType = new DiagnosticDescriptor(
        id: "ODA003",
        title: "Fixture type is declared more than once in the same scope",
        messageFormat: "Fixtures '{0}' and '{1}' both declare '{2}' in the '{3}' scope, only the factory of the first one to resolve is used",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    /// <summary>
    /// Fixtures are initialized by code generated into a partial declaration of the declaring class,
    /// a class that cannot be generated into leaves its fixtures uninitialized.
    /// </summary>
    public static readonly DiagnosticDescriptor TypeMustBePartial = new DiagnosticDescriptor(
        id: "ODA004",
        title: "Type declaring a fixture must be partial",
        messageFormat: "Fixture declared by '{0}' is never initialized because '{1}' is not partial - fixtures are initialized by generated code",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    /// <summary>
    /// Reported when the name derived from the declaring member cannot be used, most often because
    /// the accessor is already written by hand.
    /// </summary>
    public static readonly DiagnosticDescriptor AccessorNameUnavailable = new DiagnosticDescriptor(
        id: "ODA005",
        title: "Fixture accessor name is not available",
        messageFormat: "Accessor for fixture declared by '{0}' was not generated because the name '{1}' is already taken",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );

    /// <summary>
    /// Reported when no accessor name can be derived from the declaring member name at all.
    /// </summary>
    public static readonly DiagnosticDescriptor AccessorNameUndeterminable = new DiagnosticDescriptor(
        id: "ODA006",
        title: "Fixture accessor name cannot be derived",
        messageFormat: "Accessor for fixture declared by '{0}' was not generated because no accessor name can be derived from that member name",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );
}
