using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Testing.Scenarios.XunitV3;

/// <summary>
/// Base class for tests that declare their dependencies using <see cref="Fixture"/>. All
/// <see cref="Fixture{T}"/> fields and auto-properties are resolved before the test is executed.
/// </summary>
/// <remarks>
/// Fixtures are initialized by generated code, so every class declaring them has to be partial.
/// Requires the test assembly to be annotated with '[assembly: AssemblyFixture(typeof(ScenarioFixtureManager))]'.
/// </remarks>
public abstract class Scenario : IAsyncLifetime
{
    /// <summary>
    /// Initializes the fixtures declared by this scenario. Implemented by generated code, each class
    /// of the hierarchy initializing the fixtures it declares after the ones of its base class.
    /// </summary>
    protected virtual ValueTask InitializeFixturesAsync() => default;

    /// <summary>
    /// Resolves a single fixture declared by this scenario, doing nothing for a member that holds no
    /// fixture. Called by generated code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected static ValueTask InitializeFixtureAsync<TFixture>(Fixture<TFixture>? fixture)
    {
        return fixture is null ? default : fixture.InitializeAsync();
    }

    ValueTask IAsyncLifetime.InitializeAsync()
    {
        return InitializeFixturesAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        return default;
    }
}
