using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Odachi.Build.OptionsFileGenerator.Internal;
using Odachi.Build.OptionsFileGenerator.Model;

namespace Odachi.Build.OptionsFileGenerator;

public class OptionsFileSyntaxReceiver : ISyntaxReceiver
{
    public List<OptionsFile> OptionsFiles { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax || classDeclarationSyntax.AttributeLists.Count <= 0)
        {
            return;
        }

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                // TODO: find better way to compare without `ToString`
                if (attributeSyntax.Name.ToString() != OptionsFileConstants.BranchAttributeName)
                {
                    continue;
                }

                if (attributeSyntax.ArgumentList is null)
                {
                    continue;
                }

                string? fileName = null;
                var format = OptionsFileFormat.Auto;
                string? key = null;

                foreach (var attributeArgumentSyntax in attributeSyntax.ArgumentList.Arguments)
                {
                    // only equals syntax is supported
                    if (attributeArgumentSyntax.NameEquals is null)
                    {
                        continue;
                    }

                    var attributeName = attributeArgumentSyntax.NameEquals.Name.ToString();
                    switch (attributeName)
                    {
                        case OptionsFileConstants.BranchAttributeFileNamePropertyName when attributeArgumentSyntax.Expression.TryGetLiteralValue<string>(out var fileNameValue):
                        {
                            fileName = fileNameValue;
                            break;
                        }

                        case OptionsFileConstants.BranchAttributeFormatPropertyName when attributeArgumentSyntax.Expression.TryGetEnumValue<OptionsFileFormat>(out var formatValue):
                        {
                            format = formatValue;
                            break;
                        }

                        case OptionsFileConstants.BranchAttributeKeyPropertyName when attributeArgumentSyntax.Expression.TryGetLiteralValue<string>(out var keyValue):
                        {
                            key = keyValue;
                            break;
                        }
                    }
                }

                if (fileName is { Length: > 0 })
                {
                    OptionsFiles.Add(OptionsFile.Create(classDeclarationSyntax, attributeSyntax, fileName, format, key));
                }
            }
        }
    }
}
