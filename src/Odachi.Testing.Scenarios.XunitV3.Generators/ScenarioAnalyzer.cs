using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Odachi.Testing.Scenarios.XunitV3.Generators.Internal;

namespace Odachi.Testing.Scenarios.XunitV3.Generators;

/// <summary>
/// Reports fixture declarations that don't do what they look like they do, including the ones
/// generated initialization cannot handle.
/// </summary>
/// <remarks>
/// Rules about generated accessors are reported by <see cref="FixtureAccessorGenerator"/>.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ScenarioAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        ScenarioDiagnostics.FixtureOutsideScenario,
        ScenarioDiagnostics.StaticFixtureNotSupported,
        ScenarioDiagnostics.DuplicateFixtureType,
        ScenarioDiagnostics.TypeMustBePartial
    );

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(static compilationContext =>
        {
            if (ScenarioSymbols.Known.From(compilationContext.Compilation) is not { } known)
            {
                return;
            }

            compilationContext.RegisterSymbolStartAction(
                symbolContext => Analyze(symbolContext, known),
                SymbolKind.NamedType
            );
        });
    }

    private static void Analyze(SymbolStartAnalysisContext context, ScenarioSymbols.Known known)
    {
        if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class } type)
        {
            return;
        }

        var members = ScenarioSymbols.GetFixtureMembers(type, known);

        if (members.Count <= 0)
        {
            return;
        }

        if (!ScenarioSymbols.DerivesFrom(type, known.Scenario))
        {
            context.RegisterSymbolEndAction(endContext => ReportFixturesOutsideScenario(endContext, type, members));

            return;
        }

        // the scope a fixture is declared with is only known when the declaration calls one of the
        // 'Fixture' factories directly, which the initializers collect as they are visited
        var scopes = new ConcurrentDictionary<ISymbol, string>(SymbolEqualityComparer.Default);

        context.RegisterOperationAction(
            operationContext => CollectDeclaredScope(operationContext, scopes),
            OperationKind.FieldInitializer,
            OperationKind.PropertyInitializer
        );

        context.RegisterSymbolEndAction(endContext =>
        {
            ReportDuplicateFixtureTypes(endContext, members, scopes);
            ReportUngeneratableMembers(endContext, type, members);
        });
    }

    private static void CollectDeclaredScope(OperationAnalysisContext context, ConcurrentDictionary<ISymbol, string> scopes)
    {
        var (initialized, value) = context.Operation switch
        {
            IFieldInitializerOperation field => ((IEnumerable<ISymbol>)field.InitializedFields, field.Value),
            IPropertyInitializerOperation property => (property.InitializedProperties, property.Value),
            _ => (null, null),
        };

        if (initialized is null || value is not IInvocationOperation invocation)
        {
            return;
        }

        if (invocation.TargetMethod.ContainingType?.ToDisplayString() != ScenarioConstants.FixtureFactoryTypeName)
        {
            return;
        }

        foreach (var symbol in initialized)
        {
            // the name of the factory is also the name of the scope
            scopes[symbol] = invocation.TargetMethod.Name;
        }
    }

    private static void ReportFixturesOutsideScenario(SymbolAnalysisContext context, INamedTypeSymbol type, List<FixtureMember> members)
    {
        foreach (var member in members)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                ScenarioDiagnostics.FixtureOutsideScenario,
                member.Symbol.Locations.FirstOrDefault(),
                member.Symbol.Name,
                type.Name
            ));
        }
    }

    /// <summary>
    /// Two fixtures of the same type in the same scope resolve to a single instance, so one of the
    /// two factories never runs.
    /// </summary>
    private static void ReportDuplicateFixtureTypes(SymbolAnalysisContext context, List<FixtureMember> members, ConcurrentDictionary<ISymbol, string> scopes)
    {
        var declarations = members
            .Select(member => (Member: member, Scope: scopes.TryGetValue(member.Symbol, out var scope) ? scope : null))
            .Where(declaration => declaration.Scope is not null)
            .ToList();

        foreach (var group in declarations.GroupBy(declaration => declaration, ScopedFixtureComparer.Instance))
        {
            var duplicates = group.ToList();

            if (duplicates.Count <= 1)
            {
                continue;
            }

            // the first declaration is the one that keeps working, report the rest
            for (var i = 1; i < duplicates.Count; i++)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    ScenarioDiagnostics.DuplicateFixtureType,
                    duplicates[i].Member.Symbol.Locations.FirstOrDefault(),
                    duplicates[0].Member.Symbol.Name,
                    duplicates[i].Member.Symbol.Name,
                    duplicates[i].Member.ValueType.Name,
                    duplicates[i].Scope
                ));
            }
        }
    }

    /// <summary>
    /// Fixtures are initialized by generated code only, so a member the generator cannot initialize
    /// is never initialized at all.
    /// </summary>
    private static void ReportUngeneratableMembers(SymbolAnalysisContext context, INamedTypeSymbol type, List<FixtureMember> members)
    {
        var canGenerate = ScenarioSymbols.CanGenerateInto(type, out var blockedBy);

        foreach (var member in members)
        {
            if (member.Symbol.IsStatic)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    ScenarioDiagnostics.StaticFixtureNotSupported,
                    member.Symbol.Locations.FirstOrDefault(),
                    member.Symbol.Name
                ));
            }
            else if (!canGenerate)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    ScenarioDiagnostics.TypeMustBePartial,
                    member.Symbol.Locations.FirstOrDefault(),
                    member.Symbol.Name,
                    blockedBy.Name
                ));
            }
        }
    }

    private sealed class ScopedFixtureComparer : IEqualityComparer<(FixtureMember Member, string? Scope)>
    {
        public static readonly ScopedFixtureComparer Instance = new ScopedFixtureComparer();

        public bool Equals((FixtureMember Member, string? Scope) left, (FixtureMember Member, string? Scope) right)
        {
            return SymbolEqualityComparer.Default.Equals(left.Member.ValueType, right.Member.ValueType)
                && left.Scope == right.Scope;
        }

        public int GetHashCode((FixtureMember Member, string? Scope) value)
        {
            return unchecked(
                SymbolEqualityComparer.Default.GetHashCode(value.Member.ValueType) * 31 + (value.Scope?.GetHashCode() ?? 0)
            );
        }
    }
}
