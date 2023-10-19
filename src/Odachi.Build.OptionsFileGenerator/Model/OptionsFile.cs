using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Odachi.Build.OptionsFileGenerator.Model;

public class OptionsFile
{
    public ClassDeclarationSyntax ClassDeclarationSyntax { get; set; } = null!;
    public AttributeSyntax AttributeSyntax { get; set; } = null!;

    public string Name { get; set; } = null!;
    public OptionsFileFormat Format { get; set; } = OptionsFileFormat.Auto;
    public string? Key { get; set; }

    public List<OptionsProperty> Properties { get; set; } = null!;

    #region Static members

    public static OptionsFile Create(ClassDeclarationSyntax classDeclarationSyntax, AttributeSyntax attributeSyntax, string filaName, OptionsFileFormat format, string? key)
    {
        return new OptionsFile()
        {
            ClassDeclarationSyntax = classDeclarationSyntax,
            AttributeSyntax = attributeSyntax,

            Name = filaName,
            Format = format,
            Key = key,
        };
    }

    #endregion
}
