using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.v3;

namespace Odachi.Testing.Scenarios.XunitV3;

/// <summary>
/// Assembly fixture that creates and disposes scenario scopes as xunit walks the test pipeline.
/// </summary>
/// <remarks>
/// Has to be registered in the test assembly:
/// <code>
/// [assembly: AssemblyFixture(typeof(ScenarioFixtureManager))]
/// </code>
/// Collection scopes live for the duration of a test collection, class scopes for the duration of a
/// test class and case scopes for the duration of a single test (which means each data row of a theory
/// gets its own case scope).
/// </remarks>
public sealed class ScenarioFixtureManager :
    INotifyTestCollectionLifecycleAsync,
    INotifyTestClassLifecycleAsync,
    INotifyTestLifecycleAsync,
    IAsyncDisposable
{
    public ScenarioFixtureManager()
    {
        if (Interlocked.CompareExchange(ref _current, this, null) is not null)
        {
            throw new InvalidOperationException($"Another '{nameof(ScenarioFixtureManager)}' is already running. Make sure the test assembly registers it exactly once using '[assembly: AssemblyFixture(typeof({nameof(ScenarioFixtureManager)}))]'");
        }
    }

    private readonly ConcurrentDictionary<string, ScenarioScope<IXunitTestCollection>> _collections = new();
    private readonly ConcurrentDictionary<string, ScenarioScope<IXunitTestClass>> _classes = new();
    private readonly ConcurrentDictionary<string, ScenarioScope<IXunitTest>> _cases = new();

    internal ScenarioScope<IXunitTestCollection>? FindCollectionScope(string? uniqueId) => Find(_collections, uniqueId);
    internal ScenarioScope<IXunitTestClass>? FindClassScope(string? uniqueId) => Find(_classes, uniqueId);
    internal ScenarioScope<IXunitTest>? FindCaseScope(string? uniqueId) => Find(_cases, uniqueId);

    private static ScenarioScope<T>? Find<T>(ConcurrentDictionary<string, ScenarioScope<T>> scopes, string? uniqueId)
    {
        if (uniqueId is null)
        {
            return null;
        }

        return scopes.TryGetValue(uniqueId, out var scope) ? scope : null;
    }

    private static ValueTask StartScope<T>(ConcurrentDictionary<string, ScenarioScope<T>> scopes, string uniqueId, T value)
    {
        if (!scopes.TryAdd(uniqueId, new ScenarioScope<T>(value)))
        {
            throw new InvalidOperationException($"Scope '{uniqueId}' was already started");
        }

        return default;
    }

    private static async ValueTask FinishScope<T>(ConcurrentDictionary<string, ScenarioScope<T>> scopes, string uniqueId)
    {
        if (scopes.TryRemove(uniqueId, out var scope))
        {
            await scope.DisposeScopeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public ValueTask OnTestCollectionStartingAsync(IXunitTestCollection testCollection) =>
        StartScope(_collections, testCollection.UniqueID, testCollection);

    /// <inheritdoc/>
    public ValueTask OnTestCollectionFinishedAsync(IXunitTestCollection testCollection) =>
        FinishScope(_collections, testCollection.UniqueID);

    /// <inheritdoc/>
    public ValueTask OnTestClassStartingAsync(IXunitTestClass testClass) =>
        StartScope(_classes, testClass.UniqueID, testClass);

    /// <inheritdoc/>
    public ValueTask OnTestClassFinishedAsync(IXunitTestClass testClass) =>
        FinishScope(_classes, testClass.UniqueID);

    /// <inheritdoc/>
    public ValueTask OnTestStartingAsync(IXunitTest test) =>
        StartScope(_cases, test.UniqueID, test);

    /// <inheritdoc/>
    public ValueTask OnTestFinishedAsync(IXunitTest test) =>
        FinishScope(_cases, test.UniqueID);

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        Interlocked.CompareExchange(ref _current, null, this);

        List<Exception>? exceptions = null;

        foreach (var uniqueId in new List<string>(_cases.Keys))
        {
            await Collect(() => FinishScope(_cases, uniqueId)).ConfigureAwait(false);
        }

        foreach (var uniqueId in new List<string>(_classes.Keys))
        {
            await Collect(() => FinishScope(_classes, uniqueId)).ConfigureAwait(false);
        }

        foreach (var uniqueId in new List<string>(_collections.Keys))
        {
            await Collect(() => FinishScope(_collections, uniqueId)).ConfigureAwait(false);
        }

        if (exceptions is not null)
        {
            throw exceptions.Count == 1 ? exceptions[0] : new AggregateException("One or more scenario scopes failed to dispose", exceptions);
        }

        async ValueTask Collect(Func<ValueTask> action)
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                (exceptions ??= new List<Exception>()).Add(ex);
            }
        }
    }

    #region Static members

    private static ScenarioFixtureManager? _current;

    /// <summary>
    /// The manager of the test assembly that is currently being executed, if any.
    /// </summary>
    public static ScenarioFixtureManager? Current => Volatile.Read(ref _current);

    #endregion
}
