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

    public static bool TryGetLiteralValue_Int8(this ExpressionSyntax expression, out sbyte? value)
    {
	    switch (expression)
	    {
		    case LiteralExpressionSyntax { Token.Value: sbyte tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: byte tokenValue }:
			    if (tokenValue <= sbyte.MaxValue)
			    {
				    value = (sbyte)tokenValue;
				    return true;
			    }
			    break;
	    }

	    value = default;
	    return false;
    }

    public static bool TryGetLiteralValue_UInt8(this ExpressionSyntax expression, out byte? value)
    {
	    switch (expression)
	    {
		    case LiteralExpressionSyntax { Token.Value: sbyte tokenValue }:
			    if (tokenValue >= byte.MinValue)
			    {
				    value = (byte)tokenValue;
				    return true;
			    }
			    break;

		    case LiteralExpressionSyntax { Token.Value: byte tokenValue }:
			    value = tokenValue;
			    return true;
	    }

	    value = default;
	    return false;
    }

    public static bool TryGetLiteralValue_Int16(this ExpressionSyntax expression, out short? value)
    {
	    switch (expression)
	    {
		    case LiteralExpressionSyntax { Token.Value: sbyte tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: byte tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: short tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: ushort tokenValue }:
			    if (tokenValue <= short.MaxValue)
			    {
				    value = (short)tokenValue;
				    return true;
			    }
			    break;
	    }

	    value = default;
	    return false;
    }

    public static bool TryGetLiteralValue_Int32(this ExpressionSyntax expression, out int? value)
    {
	    switch (expression)
	    {
		    case LiteralExpressionSyntax { Token.Value: sbyte tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: byte tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: short tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: ushort tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: int tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: uint tokenValue }:
			    if (tokenValue <= int.MaxValue)
			    {
				    value = (int)tokenValue;
				    return true;
			    }
			    break;
	    }

	    value = default;
	    return false;
    }

    public static bool TryGetLiteralValue_Int64(this ExpressionSyntax expression, out long? value)
    {
	    switch (expression)
	    {
		    case LiteralExpressionSyntax { Token.Value: sbyte tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: byte tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: short tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: ushort tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: int tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: uint tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: long tokenValue }:
			    value = tokenValue;
			    return true;

		    case LiteralExpressionSyntax { Token.Value: ulong tokenValue }:
			    if (tokenValue <= long.MaxValue)
			    {
				    value = (long)tokenValue;
				    return true;
			    }
			    break;
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

    public static bool TryGetEnumValue<T>(this ExpressionSyntax expression, out T? enumValue)
	    where T : Enum
    {
	    switch (expression)
	    {
		    // TODO: implement enum lookup
		    case MemberAccessExpressionSyntax a:
			    enumValue = default;
			    return true;
	    }

	    enumValue = default;
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

            // TODO: this could be alias or completely diff type named the same?
            case MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.Value: "TimeSpan" }, Name.Identifier.Value: "Zero" }:
	            value = TimeSpan.Zero;
	            return true;

            case ObjectCreationExpressionSyntax { Type: IdentifierNameSyntax { Identifier.Value: "TimeSpan" }, ArgumentList.Arguments.Count: 0 }:
	            value = new TimeSpan();
	            return true;

            case ObjectCreationExpressionSyntax { Type: IdentifierNameSyntax { Identifier.Value: "TimeSpan" }, ArgumentList.Arguments: { Count: > 0 } timeSpanArguments }:
	            // ticks
	            {
		            if (
			            timeSpanArguments.Count == 1 &&
			            timeSpanArguments[0].Expression.TryGetLiteralValue_Int64(out var timeSpanTicksValue)
		            )
		            {
			            value = new TimeSpan(timeSpanTicksValue!.Value);
			            return true;
		            }
	            }

		        // hours / minutes / seconds
	            {
		            if (
			            timeSpanArguments.Count == 3 &&
			            timeSpanArguments[0].Expression.TryGetLiteralValue_Int32(out var timeSpanHoursValue) &&
			            timeSpanArguments[1].Expression.TryGetLiteralValue_Int32(out var timeSpanMinutesValue) &&
			            timeSpanArguments[2].Expression.TryGetLiteralValue_Int32(out var timeSpanSecondsValue)
		            )
		            {
			            value = new TimeSpan(timeSpanHoursValue!.Value, timeSpanMinutesValue!.Value, timeSpanSecondsValue!.Value);
			            return true;
		            }
	            }

	            // days / hours / minutes / seconds
	            {
		            if (
			            timeSpanArguments.Count == 4 &&
			            timeSpanArguments[0].Expression.TryGetLiteralValue_Int32(out var timeSpanDaysValue) &&
			            timeSpanArguments[1].Expression.TryGetLiteralValue_Int32(out var timeSpanHoursValue) &&
			            timeSpanArguments[2].Expression.TryGetLiteralValue_Int32(out var timeSpanMinutesValue) &&
			            timeSpanArguments[3].Expression.TryGetLiteralValue_Int32(out var timeSpanSecondsValue)
		            )
		            {
			            value = new TimeSpan(timeSpanDaysValue!.Value, timeSpanHoursValue!.Value, timeSpanMinutesValue!.Value, timeSpanSecondsValue!.Value);
			            return true;
		            }
	            }

	            // days / hours / minutes / seconds / milliseconds
	            {
		            if (
			            timeSpanArguments.Count == 5 &&
			            timeSpanArguments[0].Expression.TryGetLiteralValue_Int32(out var timeSpanDaysValue) &&
			            timeSpanArguments[1].Expression.TryGetLiteralValue_Int32(out var timeSpanHoursValue) &&
			            timeSpanArguments[2].Expression.TryGetLiteralValue_Int32(out var timeSpanMinutesValue) &&
			            timeSpanArguments[3].Expression.TryGetLiteralValue_Int32(out var timeSpanSecondsValue) &&
			            timeSpanArguments[4].Expression.TryGetLiteralValue_Int32(out var timeSpanMillisecondsValue)
		            )
		            {
			            value = new TimeSpan(timeSpanDaysValue!.Value, timeSpanHoursValue!.Value, timeSpanMinutesValue!.Value, timeSpanSecondsValue!.Value, timeSpanMillisecondsValue!.Value);
			            return true;
		            }
	            }


	            // days / hours / minutes / seconds
	            {
		            if (
			            timeSpanArguments.Count == 6 &&
			            timeSpanArguments[0].Expression.TryGetLiteralValue_Int32(out var timeSpanDaysValue) &&
			            timeSpanArguments[1].Expression.TryGetLiteralValue_Int32(out var timeSpanHoursValue) &&
			            timeSpanArguments[2].Expression.TryGetLiteralValue_Int32(out var timeSpanMinutesValue) &&
			            timeSpanArguments[3].Expression.TryGetLiteralValue_Int32(out var timeSpanSecondsValue) &&
			            timeSpanArguments[4].Expression.TryGetLiteralValue_Int32(out var timeSpanMillisecondsValue) &&
			            timeSpanArguments[5].Expression.TryGetLiteralValue_Int32(out var timeSpanMicrosecondsValue)
		            )
		            {
			            value = new TimeSpan(
				            timeSpanDaysValue!.Value * TimeSpan.TicksPerDay +
				            timeSpanHoursValue!.Value * TimeSpan.TicksPerHour +
				            timeSpanMinutesValue!.Value * TimeSpan.TicksPerMinute +
				            timeSpanSecondsValue!.Value * TimeSpan.TicksPerSecond +
				            timeSpanMillisecondsValue!.Value * TimeSpan.TicksPerMillisecond +
				            timeSpanMicrosecondsValue!.Value * (TimeSpan.TicksPerMillisecond / 1000)
			            );
			            return true;
		            }
	            }

	            break;

            case MemberAccessExpressionSyntax { Name.Identifier.Value: string }:
            case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.BitwiseOrExpression }:
                if (expression.TryGetEnumValue(out var enumValue))
                {
                    value = enumValue;
                    return true;
                }
                break;

            case ImplicitArrayCreationExpressionSyntax { Initializer: { Expressions: { } arrayItems } }:
            {
	            var items = new object?[arrayItems.Count];
	            for (var i = 0; i < arrayItems.Count; i++)
	            {
		            if (!arrayItems[i].TryGet_DefaultValue(out var arrayItemValue))
		            {
			            value = null;
			            return false;
		            }

		            items[i] = arrayItemValue;
	            }

	            value = items;
	            return true;
            }

            case InitializerExpressionSyntax { RawKind: (int)SyntaxKind.ArrayInitializerExpression } arrayInitializerExpression:
            {
	            var items = new object?[arrayInitializerExpression.Expressions.Count];
	            for (var i = 0; i < arrayInitializerExpression.Expressions.Count; i++)
	            {
		            if (!arrayInitializerExpression.Expressions[i].TryGet_DefaultValue(out var arrayItemValue))
		            {
			            value = null;
			            return false;
		            }

		            items[i] = arrayItemValue;
	            }
	            value = items;
	            return true;
            }
        }

        value = null;
        return false;
    }
}
