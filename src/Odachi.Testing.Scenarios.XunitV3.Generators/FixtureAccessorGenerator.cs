using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Odachi.Testing.Scenarios.XunitV3.Generators.Internal;

namespace Odachi.Testing.Scenarios.XunitV3.Generators;

/// <summary>
/// Generates the accessor that unwraps a fixture declaration, so that a test reads the fixture
/// value directly instead of going through <c>Value</c>, together with the code that initializes
/// the fixtures of the scenario.
/// </summary>
/// <remarks>
/// <code>
/// public partial class Sample : Scenario
/// {
///     private Fixture&lt;Foo&gt; FooFixture { get; } = Fixture.Case(() =&gt; new Foo());
///
///     // generated
///     private Foo Foo => FooFixture.Value;
/// }
/// </code>
/// An accessor is not generated when the derived name is already taken, which keeps hand written
/// accessors working and makes adopting the generator a matter of deleting them.
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class FixtureAccessorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sources = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsCandidate(node),
                static (syntaxContext, cancellationToken) => Transform(syntaxContext, cancellationToken)
            )
            .Where(static source => source is not null);

        context.RegisterSourceOutput(sources, static (productionContext, source) =>
        {
            foreach (var diagnostic in source!.Diagnostics)
            {
                productionContext.ReportDiagnostic(diagnostic.ToDiagnostic());
            }

            if (source.HintName is not null && source.Text is not null)
            {
                productionContext.AddSource(source.HintName, source.Text);
            }
        });
    }

    /// <summary>
    /// A class deriving from <c>Scenario</c> either states so in its own base list or is partial and
    /// states so in another declaration.
    /// </summary>
    private static bool IsCandidate(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax declaration
            && (declaration.BaseList is not null || declaration.Modifiers.Any(SyntaxKind.PartialKeyword));
    }

    private static ScenarioSource? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (ScenarioSymbols.Known.From(context.SemanticModel.Compilation) is not { } known)
        {
            return null;
        }

        if (context.SemanticModel.GetDeclaredSymbol(declaration, cancellationToken) is not INamedTypeSymbol type)
        {
            return null;
        }

        if (!ScenarioSymbols.DerivesFrom(type, known.Scenario))
        {
            return null;
        }

        // a partial class is visited once per declaration, generate from a single one of them
        if (!IsPrimaryDeclaration(type, declaration))
        {
            return null;
        }

        var members = ScenarioSymbols.GetFixtureMembers(type, known);

        if (members.Count <= 0)
        {
            return null;
        }

        // fixtures that cannot be generated are never initialized, which the analyzer reports as an
        // error at their declaration
        if (!ScenarioSymbols.CanGenerateInto(type, out _))
        {
            return null;
        }

        var diagnostics = ImmutableArray.CreateBuilder<DiagnosticInfo>();

        var accessors = ResolveAccessors(type, members, diagnostics);

        // each class initializes the fixtures it declares itself, after its base class did the same
        var initializers = members
            .Where(member => !member.Symbol.IsStatic)
            .Select(member => member.Symbol.Name)
            .ToList();

        if (accessors.Count <= 0 && initializers.Count <= 0)
        {
            return new ScenarioSource(null, null, diagnostics.ToImmutable().ToEquatableArray());
        }

        return new ScenarioSource(
            GetHintName(type),
            Emit(type, accessors, initializers),
            diagnostics.ToImmutable().ToEquatableArray()
        );
    }

    private static bool IsPrimaryDeclaration(INamedTypeSymbol type, ClassDeclarationSyntax declaration)
    {
        var references = type.DeclaringSyntaxReferences;

        if (references.Length <= 1)
        {
            return true;
        }

        var primary = references
            .OrderBy(reference => reference.SyntaxTree.FilePath, StringComparer.Ordinal)
            .ThenBy(reference => reference.Span.Start)
            .First();

        return primary.SyntaxTree == declaration.SyntaxTree && primary.Span == declaration.Span;
    }

    /// <summary>
    /// Turns fixture declarations into accessors, dropping the ones that cannot be generated.
    /// </summary>
    private static List<FixtureAccessor> ResolveAccessors(INamedTypeSymbol type, List<FixtureMember> members, ImmutableArray<DiagnosticInfo>.Builder diagnostics)
    {
        var format = SymbolDisplayFormat.FullyQualifiedFormat
            .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        var candidates = new List<(FixtureMember Member, string Name)>();

        foreach (var member in members)
        {
            // a static member is reported by the analyzer, a member that may not hold anything
            // cannot be unwrapped into a non nullable accessor
            if (member.Symbol.IsStatic || member.IsNullable)
            {
                continue;
            }

            var name = AccessorNaming.Derive(member.Symbol.Name);

            if (name is null)
            {
                diagnostics.Add(new DiagnosticInfo(
                    ScenarioDiagnostics.AccessorNameUndeterminable,
                    LocationInfo.From(member.Symbol.Locations.FirstOrDefault()),
                    new[] { member.Symbol.Name }.ToEquatableArray()
                ));

                continue;
            }

            candidates.Add((member, name));
        }

        var result = new List<FixtureAccessor>();

        foreach (var candidate in candidates)
        {
            // the name may be taken by a hand written accessor, by the declaring member itself or by
            // another fixture whose name happens to reduce to the same thing
            var taken = IsNameTaken(type, candidate.Name)
                || candidates.Count(other => other.Name == candidate.Name) > 1;

            if (taken)
            {
                diagnostics.Add(new DiagnosticInfo(
                    ScenarioDiagnostics.AccessorNameUnavailable,
                    LocationInfo.From(candidate.Member.Symbol.Locations.FirstOrDefault()),
                    new[] { candidate.Member.Symbol.Name, candidate.Name }.ToEquatableArray()
                ));

                continue;
            }

            result.Add(new FixtureAccessor(
                candidate.Name,
                candidate.Member.ValueType.ToDisplayString(format),
                candidate.Member.Symbol.Name,
                ToKeyword(candidate.Member.Symbol.DeclaredAccessibility)
            ));
        }

        return result;
    }

    private static bool IsNameTaken(INamedTypeSymbol type, string name)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (!current.GetMembers(name).IsEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private static string GetHintName(INamedTypeSymbol type)
    {
        var builder = new StringBuilder();

        if (type.ContainingNamespace is { IsGlobalNamespace: false } containingNamespace)
        {
            builder.Append(containingNamespace.ToDisplayString()).Append('.');
        }

        foreach (var container in GetDeclarationChain(type))
        {
            // 'Sample`1' is not a valid file name
            builder.Append(container.MetadataName.Replace('`', '_')).Append('.');
        }

        return builder.Append("Fixtures.g.cs").ToString();
    }

    private static string Emit(INamedTypeSymbol type, List<FixtureAccessor> accessors, List<string> initializers)
    {
        var builder = new StringBuilder();
        var indent = 0;

        void AppendLine(string text = "")
        {
            if (text.Length > 0)
            {
                builder.Append(new string(' ', indent * 4));
            }

            builder.Append(text).Append('\n');
        }

        AppendLine("// <auto-generated />");
        AppendLine();
        AppendLine("#nullable enable");
        AppendLine();

        var hasNamespace = type.ContainingNamespace is { IsGlobalNamespace: false };

        if (hasNamespace)
        {
            AppendLine($"namespace {type.ContainingNamespace.ToDisplayString()}");
            AppendLine("{");
            indent++;
        }

        var chain = GetDeclarationChain(type);

        foreach (var container in chain)
        {
            AppendLine($"partial {ToKeyword(container)} {ToDeclarationName(container)}");
            AppendLine("{");
            indent++;
        }

        var first = true;

        foreach (var accessor in accessors)
        {
            if (!first)
            {
                AppendLine();
            }

            first = false;

            AppendLine("/// <summary>");
            AppendLine($"/// Value of the fixture declared by <c>{accessor.MemberName}</c>.");
            AppendLine("/// </summary>");
            AppendLine($"{accessor.Accessibility} {accessor.ValueTypeName} {accessor.Name} => {accessor.MemberName}.Value;");
        }

        if (initializers.Count > 0)
        {
            if (!first)
            {
                AppendLine();
            }

            AppendLine("/// <inheritdoc />");
            AppendLine($"protected override async global::System.Threading.Tasks.ValueTask {ScenarioConstants.InitializeFixturesMethodName}()");
            AppendLine("{");
            indent++;
            AppendLine($"await base.{ScenarioConstants.InitializeFixturesMethodName}();");

            foreach (var initializer in initializers)
            {
                // qualified, so that a member of the same name declared by the scenario cannot hide it
                AppendLine($"await global::{ScenarioConstants.ScenarioTypeName}.{ScenarioConstants.InitializeFixtureMethodName}({initializer});");
            }

            indent--;
            AppendLine("}");
        }

        foreach (var _ in chain)
        {
            indent--;
            AppendLine("}");
        }

        if (hasNamespace)
        {
            indent--;
            AppendLine("}");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Containing types from the outermost one down to <paramref name="type"/> itself.
    /// </summary>
    private static List<INamedTypeSymbol> GetDeclarationChain(INamedTypeSymbol type)
    {
        var result = new List<INamedTypeSymbol>();

        for (var current = type; current is not null; current = current.ContainingType)
        {
            result.Add(current);
        }

        result.Reverse();

        return result;
    }

    private static string ToDeclarationName(INamedTypeSymbol type)
    {
        if (type.TypeParameters.Length <= 0)
        {
            return type.Name;
        }

        return $"{type.Name}<{string.Join(", ", type.TypeParameters.Select(parameter => parameter.Name))}>";
    }

    private static string ToKeyword(INamedTypeSymbol type)
    {
        return type switch
        {
            { IsRecord: true, TypeKind: TypeKind.Struct } => "record struct",
            { IsRecord: true } => "record",
            { TypeKind: TypeKind.Struct } => "struct",
            { TypeKind: TypeKind.Interface } => "interface",
            _ => "class",
        };
    }

    private static string ToKeyword(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => "private",
        };
    }

    private sealed record FixtureAccessor(
        string Name,
        string ValueTypeName,
        string MemberName,
        string Accessibility
    );

    /// <summary>
    /// Value equatable result of the transform, so that unchanged classes don't trigger a re-emit.
    /// </summary>
    private sealed record ScenarioSource(
        string? HintName,
        string? Text,
        EquatableArray<DiagnosticInfo> Diagnostics
    );
}
