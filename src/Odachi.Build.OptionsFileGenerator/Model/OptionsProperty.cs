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
                var defaultValue = default(object?);
                if (propertyDeclarationSyntax is { Initializer: not null })
                {
                    propertyDeclarationSyntax.Initializer.Value.TryGet_DefaultValue(out defaultValue);
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
