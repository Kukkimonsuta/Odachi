using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Odachi.Testing.Scenarios.XunitV3.Generators.Internal;

/// <summary>
/// Symbol level queries shared by the generator and the analyzer.
/// </summary>
internal static class ScenarioSymbols
{
    /// <summary>
    /// Types the generator and the analyzer need to recognize. Absent when the scenario library is
    /// not referenced, in which case there is nothing to do.
    /// </summary>
    public sealed class Known
    {
        private Known(INamedTypeSymbol scenario, INamedTypeSymbol fixture)
        {
            Scenario = scenario;
            Fixture = fixture;
        }

        public INamedTypeSymbol Scenario { get; }
        public INamedTypeSymbol Fixture { get; }

        public static Known? From(Compilation compilation)
        {
            var scenario = compilation.GetTypeByMetadataName(ScenarioConstants.ScenarioTypeName);
            var fixture = compilation.GetTypeByMetadataName(ScenarioConstants.FixtureTypeMetadataName);

            if (scenario is null || fixture is null)
            {
                return null;
            }

            return new Known(scenario, fixture);
        }
    }

    public static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether the type is declared in source as partial. All declarations of a partial type carry
    /// the modifier, otherwise the compilation itself is broken, so one of them is enough.
    /// </summary>
    public static bool IsPartial(INamedTypeSymbol type)
    {
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax() is TypeDeclarationSyntax declaration && declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether members can be generated into the type, which needs the type and everything
    /// containing it to be partial and declared in source.
    /// </summary>
    public static bool CanGenerateInto(INamedTypeSymbol type, out INamedTypeSymbol blockedBy)
    {
        for (var current = type; current is not null; current = current.ContainingType)
        {
            if (!IsPartial(current))
            {
                blockedBy = current;

                return false;
            }
        }

        blockedBy = type;

        return true;
    }

    /// <summary>
    /// Members that hold a fixture, in declaration order.
    /// </summary>
    /// <remarks>
    /// Only members that store a value are returned. A computed property is skipped, because it can
    /// hand out a fixture that another member already owns, which would be initialized twice.
    /// </remarks>
    public static List<FixtureMember> GetFixtureMembers(INamedTypeSymbol type, Known known)
    {
        var backed = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        foreach (var member in type.GetMembers())
        {
            if (member is IFieldSymbol { AssociatedSymbol: { } associated })
            {
                backed.Add(associated);
            }
        }

        var result = new List<FixtureMember>();

        foreach (var member in type.GetMembers())
        {
            if (member.IsImplicitlyDeclared)
            {
                continue;
            }

            var memberType = member switch
            {
                IPropertySymbol property when property.Parameters.IsEmpty && property.GetMethod is not null && backed.Contains(property) => property.Type,
                IFieldSymbol field when field.AssociatedSymbol is null => field.Type,
                _ => null,
            };

            // the fixture type is sealed, so a member holds a fixture exactly when it is declared as one
            if (memberType is not INamedTypeSymbol { TypeArguments.Length: 1 } named
                || !SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, known.Fixture))
            {
                continue;
            }

            result.Add(new FixtureMember(member, named.TypeArguments[0], memberType.NullableAnnotation == NullableAnnotation.Annotated));
        }

        // GetMembers is documented as returning members in no particular order, the order fixtures
        // are initialized in is part of the contract
        result.Sort(static (left, right) => Compare(left.Symbol, right.Symbol));

        return result;
    }

    private static int Compare(ISymbol left, ISymbol right)
    {
        var leftLocation = left.Locations.FirstOrDefault();
        var rightLocation = right.Locations.FirstOrDefault();

        var byPath = string.CompareOrdinal(
            leftLocation?.SourceTree?.FilePath ?? "",
            rightLocation?.SourceTree?.FilePath ?? ""
        );

        if (byPath != 0)
        {
            return byPath;
        }

        return (leftLocation?.SourceSpan.Start ?? 0).CompareTo(rightLocation?.SourceSpan.Start ?? 0);
    }
}

/// <summary>
/// Member holding a fixture that gets initialized before the test runs.
/// </summary>
/// <param name="Symbol">The declaring field or property.</param>
/// <param name="ValueType">Value type of the <c>Fixture&lt;T&gt;</c> declaration.</param>
/// <param name="IsNullable">
/// Whether the member may not hold anything. Such a member is still initialized when it does, but it
/// cannot be unwrapped into a non nullable accessor.
/// </param>
internal sealed record FixtureMember(ISymbol Symbol, ITypeSymbol ValueType, bool IsNullable);
