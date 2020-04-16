using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Odachi.Testing.Scenarios.Xunit
{
	public static class ScenarioContext
	{
		internal static AsyncLocal<ScenarioScope<ScenarioTestAssemblyRunner>?> CurrentRunnerStore = new AsyncLocal<ScenarioScope<ScenarioTestAssemblyRunner>?>();
		public static ScenarioScope<ScenarioTestAssemblyRunner>? CurrentRunner => CurrentRunnerStore.Value;

		internal static AsyncLocal<ScenarioScope<ITestCollection>?> CurrentCollectionStore = new AsyncLocal<ScenarioScope<ITestCollection>?>();
		public static ScenarioScope<ITestCollection>? CurrentCollection => CurrentCollectionStore.Value;

		internal static AsyncLocal<ScenarioScope<ITestClass>?> CurrentClassStore = new AsyncLocal<ScenarioScope<ITestClass>?>();
		public static ScenarioScope<ITestClass>? CurrentClass => CurrentClassStore.Value;

		internal static AsyncLocal<ScenarioScope<ITestMethod>?> CurrentMethodStore = new AsyncLocal<ScenarioScope<ITestMethod>?>();
		public static ScenarioScope<ITestMethod>? CurrentMethod => CurrentMethodStore.Value;

		internal static AsyncLocal<ScenarioScope<ITestCase>?> CurrentCaseStore = new AsyncLocal<ScenarioScope<ITestCase>?>();
		public static ScenarioScope<ITestCase>? CurrentCase => CurrentCaseStore.Value;
	}

	public class ScenarioScope<T> : IAsyncDisposable
	{
		internal ScenarioScope(T value)
		{
			Value = value;
		}

		private Dictionary<Type, object> _fixtures = new Dictionary<Type, object>();

		public T Value { get; }

		public async Task<TFixture> GetFixtureAsync<TFixture>(Func<Task<TFixture>> factory)
		{
			var type = typeof(TFixture);

			if (_fixtures.TryGetValue(type, out var value))
			{
				return (TFixture)value;
			}

			var fixture = await factory();

			if (fixture is IAsyncLifetime asyncLifetime)
			{
				await asyncLifetime.InitializeAsync();
			}

#pragma warning disable CS8604 // Possible null reference argument.
			_fixtures.Add(type, fixture);
#pragma warning restore CS8604 // Possible null reference argument.

			return fixture;
		}

		internal async Task DisposeAsync()
		{
			foreach (var fixture in _fixtures.Values)
			{
				switch (fixture)
				{
					case IAsyncLifetime asyncLifetime:
						await asyncLifetime.DisposeAsync();
						break;

					case IAsyncDisposable asyncDisposable:
						await asyncDisposable.DisposeAsync();
						break;

					case IDisposable disposable:
						disposable.Dispose();
						break;
				}
			}
		}

		async ValueTask IAsyncDisposable.DisposeAsync()
		{
			await DisposeAsync();
		}
	}

	internal static class Tracker
	{
		public static Tracker<T> Create<T>(AsyncLocal<T?> store, T newValue)
			where T : class
		{
			return new Tracker<T>(store, newValue);
		}
	}

	internal sealed class Tracker<T> : IAsyncDisposable
		where T : class
	{
		public Tracker(AsyncLocal<T?> store, T newValue)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));

			if (store.Value != default)
				throw new InvalidOperationException("Context value is already set");

			store.Value = newValue;
		}

		private AsyncLocal<T?> _store;

		public async ValueTask DisposeAsync()
		{
			if (_store.Value is IAsyncDisposable asyncDisposable)
			{
				await asyncDisposable.DisposeAsync();
			}

			_store.Value = default;
		}
	}

	/// <inheritdoc/>
	public class ScenarioTestFramework : XunitTestFramework
	{
		/// <inheritdoc/>
		public ScenarioTestFramework(IMessageSink messageSink)
			: base(messageSink)
		{
		}

		/// <inheritdoc/>
		protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
		{
			return new XunitTestFrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);
		}

		/// <inheritdoc/>
		protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
		{
			return new ScenarioTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
		}
	}
}
