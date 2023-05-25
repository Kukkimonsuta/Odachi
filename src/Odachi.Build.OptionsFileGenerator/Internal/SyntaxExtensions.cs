using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Odachi.Build.OptionsFileGenerator.Model;

namespace Odachi.Build.OptionsFileGenerator.Internal;

public static class SyntaxExtensions
{
    public static bool TryGetIdentifierName(this ExpressionSyntax expression, out string? name)
    {
        switch (expression)
        {
            case IdentifierNameSyntax { Identifier.Value: string identifierName }:
                name = identifierName;
                return true;
        }

        name = default;
        return false;
    }

    public static bool TryGetLiteralValue(this ExpressionSyntax expression, out object? value)
    {
        switch (expression)
        {
            case LiteralExpressionSyntax literalExpressionSyntax:
                value = literalExpressionSyntax.Token.Value;
                return true;
        }

        value = default;
        return false;
    }

    public static bool TryGetLiteralValue<T>(this ExpressionSyntax expression, out T? value)
    {
        switch (expression)
        {
            case LiteralExpressionSyntax { Token.Value: T tokenValue }:
                value = tokenValue;
                return true;
        }

        value = default;
        return false;
    }

    public static bool TryGetEnumValue<T>(this ExpressionSyntax expression, out T? enumValue)
        where T : Enum
    {
        switch (expression)
        {
            case MemberAccessExpressionSyntax a:
                enumValue = default;
                return true;
        }

        enumValue = default;
        return false;
    }

    public static bool TryGetEnumValue(this ExpressionSyntax expression, out EnumValue? value)
    {
        switch (expression)
        {
            case MemberAccessExpressionSyntax { Name.Identifier.Value: string s } memberAccessExpressionSyntax:
                value = new EnumValue()
                {
                    Name = "",
                    Values = new[] { s },
                };
                return true;

            case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.BitwiseOrExpression } binaryExpressionSyntax:
                var leftSuccess = binaryExpressionSyntax.Left.TryGetEnumValue(out var leftValue);
                var rightSuccess = binaryExpressionSyntax.Right.TryGetEnumValue(out var rightValue);

                if (leftSuccess && rightSuccess)
                {
                    if (leftValue!.Name != rightValue!.Name)
                    {
                        value = default;
                        return false;
                    }

                    value = new EnumValue()
                    {
                        Name = leftValue.Name,
                        Values = leftValue.Values.Concat(rightValue.Values).ToArray(),
                    };
                    return true;
                }
                break;
        }

        value = default;
        return false;
    }

    public static bool TryGet_DefaultValue(this ExpressionSyntax expression, out object? value)
    {
        switch (expression)
        {
            case LiteralExpressionSyntax { Token.Value: null }:
                value = null;
                return true;

            case LiteralExpressionSyntax { Token.Value: string s }:
                value = s;
                return true;

            case LiteralExpressionSyntax {} literalExpressionSyntax:
                value = literalExpressionSyntax.Token.Value;
                return true;

            case MemberAccessExpressionSyntax { Name.Identifier.Value: string }:
            case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.BitwiseOrExpression }:
                if (expression.TryGetEnumValue(out var enumValue))
                {
                    value = enumValue;
                    return true;
                }
                break;
        }

        value = null;
        return false;
    }
}
