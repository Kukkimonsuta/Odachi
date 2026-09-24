using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.XunitV3;

/// <summary>
/// Lifetime of a scenario fixture.
/// </summary>
public enum FixtureScope
{
    /// <summary>
    /// Fixture is created for the current test and disposed when the test finishes.
    /// </summary>
    Case,

    /// <summary>
    /// Fixture is created for the current test class and disposed when the test class finishes.
    /// </summary>
    Class,

    /// <summary>
    /// Fixture is created for the current test collection and disposed when the test collection finishes.
    /// </summary>
    Collection,
}

/// <summary>
/// Declares fixtures that are resolved when the declaring <see cref="Scenario"/> is initialized.
/// </summary>
public static class Fixture
{
    #region Static members

    /// <summary>
    /// Declares a fixture that is shared by all fixtures of the same type within a single test.
    /// </summary>
    public static Fixture<T> Case<T>(Func<T> factory, [CallerMemberName] string? declaringMember = null)
    {
        return new Fixture<T>(Wrap(factory), FixtureScope.Case, declaringMember);
    }

    /// <summary>
    /// Declares a fixture that is shared by all fixtures of the same type within a single test.
    /// </summary>
    public static Fixture<T> Case<T>(Func<Task<T>> factory, [CallerMemberName] string? declaringMember = null)
    {
        return new Fixture<T>(factory, FixtureScope.Case, declaringMember);
    }

    /// <summary>
    /// Declares a fixture that is shared by all fixtures of the same type within a single test class.
    /// </summary>
    public static Fixture<T> Class<T>(Func<T> factory, [CallerMemberName] string? declaringMember = null)
    {
        return new Fixture<T>(Wrap(factory), FixtureScope.Class, declaringMember);
    }

    /// <summary>
    /// Declares a fixture that is shared by all fixtures of the same type within a single test class.
    /// </summary>
    public static Fixture<T> Class<T>(Func<Task<T>> factory, [CallerMemberName] string? declaringMember = null)
    {
        return new Fixture<T>(factory, FixtureScope.Class, declaringMember);
    }

    /// <summary>
    /// Declares a fixture that is shared by all fixtures of the same type within a single test collection.
    /// </summary>
    public static Fixture<T> Collection<T>(Func<T> factory, [CallerMemberName] string? declaringMember = null)
    {
        return new Fixture<T>(Wrap(factory), FixtureScope.Collection, declaringMember);
    }

    /// <summary>
    /// Declares a fixture that is shared by all fixtures of the same type within a single test collection.
    /// </summary>
    public static Fixture<T> Collection<T>(Func<Task<T>> factory, [CallerMemberName] string? declaringMember = null)
    {
        return new Fixture<T>(factory, FixtureScope.Collection, declaringMember);
    }

    private static Func<Task<T>> Wrap<T>(Func<T> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        return () => Task.FromResult(factory());
    }

    #endregion
}

/// <summary>
/// Fixture of type <typeparamref name="T"/> resolved from the scope given by <see cref="FixtureScope"/>.
/// </summary>
public sealed class Fixture<T>(Func<Task<T>> factory, FixtureScope scope, string? declaringMember = null)
{
    private readonly Func<Task<T>> _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    private bool _initialized = false;
    private T _value = default!;

    /// <summary>
    /// Scope this fixture is resolved from.
    /// </summary>
    public FixtureScope Scope => scope;

    /// <summary>
    /// Name of the member this fixture was declared by, when known.
    /// </summary>
    public string? DeclaringMember => declaringMember;

    public T Value => _initialized ? _value : throw new InvalidOperationException($"Cannot access {Describe()} before it is initialized. Fixtures are initialized before the test starts, make sure the declaring class is partial and derives from '{nameof(Scenario)}'");

    /// <summary>
    /// Resolves the value of this fixture from its scope. Generated code reaches it through
    /// <see cref="Scenario.InitializeFixtureAsync{TFixture}"/>.
    /// </summary>
    internal async ValueTask InitializeAsync()
    {
        if (_initialized)
        {
            throw new InvalidOperationException($"{Describe()} was already initialized. Fixtures keep the value they resolved to, so a single instance cannot be reused by more than one test - make sure the fixture is not declared in a 'static' member");
        }

        // each scope level is a distinct type, so the null check cannot be shared by the arms
        var resolving = scope switch
        {
            FixtureScope.Collection => ScenarioContext.CurrentCollection?.GetFixtureAsync(_factory),
            FixtureScope.Class => ScenarioContext.CurrentClass?.GetFixtureAsync(_factory),
            FixtureScope.Case => ScenarioContext.CurrentCase?.GetFixtureAsync(_factory),
            _ => throw new InvalidOperationException($"Undefined behavior for '{scope}'"),
        };

        if (resolving is null)
        {
            throw new InvalidOperationException($"Cannot resolve {Describe()} because there is no such scope in the current test context. Make sure the fixture is resolved while a test is running and that the test assembly is annotated with '[assembly: AssemblyFixture(typeof({nameof(ScenarioFixtureManager)}))]'");
        }

        _value = await resolving;
        _initialized = true;
    }

    public override string ToString() => Describe();

    private string Describe()
    {
        return declaringMember is null
            ? $"'{scope}' scoped fixture of type '{typeof(T).Name}'"
            : $"'{scope}' scoped fixture of type '{typeof(T).Name}' declared by '{declaringMember}'";
    }
}
