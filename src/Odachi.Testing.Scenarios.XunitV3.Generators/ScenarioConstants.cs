namespace Odachi.Testing.Scenarios.XunitV3.Generators;

/// <summary>
/// Names of the runtime types the generator recognizes.
/// </summary>
internal static class ScenarioConstants
{
    public const string NamespaceName = "Odachi.Testing.Scenarios.XunitV3";

    /// <summary>
    /// Base class a test class has to derive from for its fixtures to be initialized.
    /// </summary>
    public const string ScenarioTypeName = NamespaceName + ".Scenario";

    /// <summary>
    /// Metadata name of the fixture declaration type.
    /// </summary>
    public const string FixtureTypeMetadataName = NamespaceName + ".Fixture`1";

    /// <summary>
    /// Static class declaring fixtures.
    /// </summary>
    public const string FixtureFactoryTypeName = NamespaceName + ".Fixture";

    /// <summary>
    /// Method generated initialization overrides.
    /// </summary>
    public const string InitializeFixturesMethodName = "InitializeFixturesAsync";

    /// <summary>
    /// Method generated initialization overrides call for each fixture.
    /// </summary>
    public const string InitializeFixtureMethodName = "InitializeFixtureAsync";

    /// <summary>
    /// Suffixes stripped from a declaring member name when deriving the accessor name.
    /// </summary>
    public static readonly string[] AccessorNameSuffixes = { "Fixture", "Factory" };
}
