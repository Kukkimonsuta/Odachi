using System;

namespace Odachi.Testing.Scenarios.XunitV3.Generators.Internal;

/// <summary>
/// Derives the name of the generated accessor from the name of the member declaring the fixture.
/// </summary>
/// <remarks>
/// A leading underscore and a trailing 'Fixture'/'Factory' are removed and the result is pascal
/// cased, so that '_foo', 'FooFixture' and 'FooFactory' all yield 'Foo'. The suffix only exists to
/// keep the declaring member from colliding with the accessor, fields don't need one.
/// </remarks>
internal static class AccessorNaming
{
    public static string? Derive(string memberName)
    {
        if (string.IsNullOrEmpty(memberName))
        {
            return null;
        }

        var name = memberName.TrimStart('_');

        if (name.Length <= 0)
        {
            return null;
        }

        foreach (var suffix in ScenarioConstants.AccessorNameSuffixes)
        {
            // the suffix has to leave something behind, a member called just 'Fixture' keeps its name
            if (name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.Ordinal))
            {
                name = name.Substring(0, name.Length - suffix.Length);

                break;
            }
        }

        if (!char.IsLetter(name[0]) && name[0] != '_')
        {
            return null;
        }

        return char.ToUpperInvariant(name[0]) + name.Substring(1);
    }
}
