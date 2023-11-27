using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Odachi.Build.OptionsFileGenerator.Internal;

namespace Odachi.Build.OptionsFileGenerator.Model;

public abstract class OptionsProperty
{
    public string Key { get; set; } = null!;
    public string? Description { get; set; }

    public ITypeSymbol Type { get; set; } = null!;

    #region Static members

    private static void OverwriteDefaultValuesWithInitializer(BranchOptionsProperty branchOptionsProperty, BaseObjectCreationExpressionSyntax objectCreationExpressionSyntax)
    {
        if (objectCreationExpressionSyntax.Initializer == null)
        {
            return;
        }

        foreach (var expression in objectCreationExpressionSyntax.Initializer.Expressions)
        {
            switch (expression)
            {
                case AssignmentExpressionSyntax assignmentExpressionSyntax:
                {
                    if (assignmentExpressionSyntax.Left.TryGetIdentifierName(out var propertyName))
                    {
                        if (assignmentExpressionSyntax.Right.TryGet_DefaultValue(out var defaultValue))
                        {
                            if (branchOptionsProperty.Properties.Find(x => x.Key == propertyName) is LeafOptionsProperty overridenProperty)
                            {
                                overridenProperty.DefaultValue = defaultValue;
                            }
                        }
                        else if (assignmentExpressionSyntax.Right is BaseObjectCreationExpressionSyntax childObjectCreationExpressionSyntax)
                        {
                            if (branchOptionsProperty.Properties.Find(x => x.Key == propertyName) is BranchOptionsProperty overridenProperty)
                            {
                                OverwriteDefaultValuesWithInitializer(overridenProperty, childObjectCreationExpressionSyntax);
                            }
                        }
                    }

                    break;
                }
            }
        }
    }

    public static IEnumerable<OptionsProperty> FindForType(ITypeSymbol type)
    {
        var properties = type
            .GetMembers()
            .OfType<IPropertySymbol>();

        if (type.BaseType is not null)
        {
            foreach (var optionsProperty in FindForType(type.BaseType))
            {
                yield return optionsProperty;
            }
        }

        foreach (var property in properties)
        {
            var propertyType = property.Type;
            var attributes = propertyType.GetAttributes();

            // try get documentation
            var description = default(string);

            //  - TODO: try get documentation from attribute

            //  - try get documentation from xml doc
            {
                var xmlDoc = property.GetDocumentationCommentXml();
                if (!string.IsNullOrWhiteSpace(xmlDoc))
                {
                    var document = XDocument.Parse(xmlDoc, LoadOptions.PreserveWhitespace);

                    description = document.Root?.Element("summary")?.Value.Trim();
                }
            }

            // create property
            var propertyDeclarationSyntax = property.DeclaringSyntaxReferences.Select(x => x.GetSyntax()).OfType<PropertyDeclarationSyntax>().SingleOrDefault();

            if (attributes.Any(a => a.IsOptionsBranchAttributeType()))
            {
                var branchOptionsProperty = new BranchOptionsProperty()
                {
                    Key = property.Name,
                    Description = description,

                    Type = propertyType,
                    Properties = FindForType(propertyType).ToList(),
                };

                // Overwrite default values if there is object initializer.
                // This is ok because even if the type is reused it is scanned again and all OptionsProperty instances are recreated from scratch.
                if (propertyDeclarationSyntax is not null)
                {
                    switch (propertyDeclarationSyntax.Initializer?.Value)
                    {
                        case BaseObjectCreationExpressionSyntax objectCreationExpressionSyntax:
                        {
                            OverwriteDefaultValuesWithInitializer(branchOptionsProperty, objectCreationExpressionSyntax);
                            break;
                        }
                    }
                }

                yield return branchOptionsProperty;
            }
            else
            {
                // try get default value
                // TODO: try get default value from attribute

                object? defaultValue;
                if (propertyDeclarationSyntax is { Initializer: not null } && propertyDeclarationSyntax.Initializer.Value.TryGet_DefaultValue(out var initializerDefaultValue))
                {
                    defaultValue = initializerDefaultValue;
                }
                else
                {
	                defaultValue = propertyType.SpecialType switch
	                {
		                SpecialType.System_Boolean => false,
		                SpecialType.System_Char => '\0',
		                SpecialType.System_SByte => (sbyte)0,
		                SpecialType.System_Byte => (byte)0,
		                SpecialType.System_Int16 => (short)0,
		                SpecialType.System_UInt16 => (ushort)0,
		                SpecialType.System_Int32 => 0,
		                SpecialType.System_UInt32 => (uint)0,
		                SpecialType.System_Int64 => (long)0,
		                SpecialType.System_UInt64 => (ulong)0,
		                SpecialType.System_Decimal => (decimal)0,
		                SpecialType.System_Single => (float)0,
		                SpecialType.System_Double => (double)0,
		                SpecialType.System_String => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? "" : null,
		                SpecialType.System_IntPtr => IntPtr.Zero,
		                SpecialType.System_UIntPtr => UIntPtr.Zero,
		                SpecialType.System_Array => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_IEnumerable => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_Generic_IEnumerable_T => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_Generic_IList_T => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_Generic_ICollection_T => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_IEnumerator => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_Generic_IEnumerator_T => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_Generic_IReadOnlyList_T => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Collections_Generic_IReadOnlyCollection_T => propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null,
		                SpecialType.System_Nullable_T => null,
		                _ => default
	                };

	                if (defaultValue == null && propertyType.SpecialType == SpecialType.None && propertyType.InheritsFromIEnumerable())
	                {
		                defaultValue = propertyType.NullableAnnotation == NullableAnnotation.NotAnnotated ? Array.Empty<object>() : null;
	                }
                }

                yield return new LeafOptionsProperty()
                {
                    Key = property.Name,
                    Description = description,

                    Type = propertyType,
                    DefaultValue = defaultValue,
                };
            }
        }
    }

    #endregion
}

public class BranchOptionsProperty : OptionsProperty
{
    public List<OptionsProperty> Properties { get; set; } = null!;
}

public class LeafOptionsProperty : OptionsProperty
{
    public object? DefaultValue { get; set; }
}
