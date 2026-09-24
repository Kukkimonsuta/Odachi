using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.v3;

namespace Odachi.Testing.Scenarios.XunitV3;

/// <summary>
/// Provides ambient access to the scenario scopes of the currently executing test.
/// </summary>
/// <remarks>
/// Scopes are maintained by <see cref="ScenarioFixtureManager"/>, which has to be registered
/// in the test assembly using '[assembly: AssemblyFixture(typeof(ScenarioFixtureManager))]'.
/// </remarks>
public static class ScenarioContext
{
    /// <summary>
    /// Scope of the test collection that is currently being executed, if any.
    /// </summary>
    public static ScenarioScope<IXunitTestCollection>? CurrentCollection =>
        ScenarioFixtureManager.Current?.FindCollectionScope(TestContext.Current?.TestCollection?.UniqueID);

    /// <summary>
    /// Scope of the test class that is currently being executed, if any.
    /// </summary>
    public static ScenarioScope<IXunitTestClass>? CurrentClass =>
        ScenarioFixtureManager.Current?.FindClassScope(TestContext.Current?.TestClass?.UniqueID);

    /// <summary>
    /// Scope of the test that is currently being executed, if any.
    /// </summary>
    public static ScenarioScope<IXunitTest>? CurrentCase =>
        ScenarioFixtureManager.Current?.FindCaseScope(TestContext.Current?.Test?.UniqueID);
}

/// <summary>
/// Holds fixtures created within a single scope. Fixtures are keyed by their type, so a single
/// scope never contains more than one fixture of any given type.
/// </summary>
public class ScenarioScope<T> : IAsyncDisposable
{
    internal ScenarioScope(T value)
    {
        Value = value;
    }

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Dictionary<Type, object?> _fixtures = new();
    private readonly Dictionary<Type, Exception> _failures = new();
    private readonly List<object?> _disposables = new();
    private bool _disposed = false;

    /// <summary>
    /// The xunit object this scope is attached to.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Returns the fixture of type <typeparamref name="TFixture"/> belonging to this scope, creating
    /// and initializing it using <paramref name="factory"/> when it doesn't exist yet.
    /// </summary>
    /// <remarks>
    /// A fixture that fails to be created is not created again - every later request for the same type
    /// within this scope fails with the very same error.
    /// </remarks>
    public async Task<TFixture> GetFixtureAsync<TFixture>(Func<Task<TFixture>> factory)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        var type = typeof(TFixture);

        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ScenarioScope<T>), "Cannot create fixture in a scope that was already disposed");
            }

            if (_fixtures.TryGetValue(type, out var value))
            {
                return (TFixture)value!;
            }

            if (_failures.TryGetValue(type, out var failure))
            {
                throw new InvalidOperationException($"Fixture of type '{type.Name}' already failed to be created in this scope", failure);
            }

            TFixture fixture;
            try
            {
                fixture = await factory().ConfigureAwait(false);

                // track the fixture before initialization so that it gets disposed even when initialization fails
                _disposables.Add(fixture);

                if (fixture is IAsyncLifetime asyncLifetime)
                {
                    await asyncLifetime.InitializeAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // remember the failure so that an expensive fixture isn't attempted again by every
                // test sharing this scope
                _failures[type] = ex;

                throw;
            }

            _fixtures.Add(type, fixture);

            return fixture;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Disposes all fixtures created within this scope, in reverse order of their creation.
    /// </summary>
    internal async ValueTask DisposeScopeAsync()
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            List<Exception>? exceptions = null;

            for (var i = _disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    switch (_disposables[i])
                    {
                        case IAsyncDisposable asyncDisposable:
                            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                            break;

                        case IDisposable disposable:
                            disposable.Dispose();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    (exceptions ??= new List<Exception>()).Add(ex);
                }
            }

            _fixtures.Clear();
            _failures.Clear();
            _disposables.Clear();

            if (exceptions is not null)
            {
                throw exceptions.Count == 1 ? exceptions[0] : new AggregateException("One or more fixtures failed to dispose", exceptions);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeScopeAsync().ConfigureAwait(false);
    }
}
