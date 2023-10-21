using System;
using Microsoft.CodeAnalysis;

namespace Odachi.Build.OptionsFileGenerator.Internal;

public static class Extensions
{
	public static bool IsOptionsBranchAttributeType(this AttributeData attribute)
	{
		return
			attribute.AttributeClass?.ContainingNamespace.MetadataName == "Annotations" &&
			attribute.AttributeClass?.ContainingNamespace.ContainingNamespace.MetadataName == "Odachi" &&
			attribute.AttributeClass?.ContainingNamespace.ContainingNamespace.ContainingNamespace.IsGlobalNamespace == true &&
			attribute.AttributeClass?.MetadataName == OptionsFileConstants.BranchAttributeName;
	}

	public static bool InheritsFromIEnumerable(this ITypeSymbol symbol)
	{
		if (symbol.SpecialType == SpecialType.System_Collections_IEnumerable)
		{
			return true;
		}

		foreach (var @interface in symbol.Interfaces)
		{
			if (InheritsFromIEnumerable(@interface))
			{
				return true;
			}
		}

		return false;
	}
}
