using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Xunit
{
	public enum FixtureScope
	{
		Case,
		Class,
		Collection,
	}

	public static class Fixture
	{
		#region Static members

		public static Fixture<T> Case<T>(Func<Task<T>> factory)
		{
			return new Fixture<T>(factory, FixtureScope.Case);
		}

		public static Fixture<T> Class<T>(Func<Task<T>> factory)
		{
			return new Fixture<T>(factory, FixtureScope.Class);
		}

		public static Fixture<T> Collection<T>(Func<Task<T>> factory)
		{
			return new Fixture<T>(factory, FixtureScope.Collection);
		}

		#endregion
	}

	public interface IAsyncInitializable
	{
		Task InitializeAsync();
	}

	public class Fixture<T> : IAsyncInitializable
	{
		public Fixture(Func<Task<T>> factory, FixtureScope scope)
		{
			_factory = factory;
			_scope = scope;
		}

		private Func<Task<T>> _factory;
		private FixtureScope _scope;
		private bool _initialized = false;
#pragma warning disable CS8601 // Possible null reference assignment.
		private T _value = default;
#pragma warning restore CS8601 // Possible null reference assignment.

		public T Value => _initialized ? _value : throw new InvalidOperationException("Cannot access uninitialized fixture");

		async Task IAsyncInitializable.InitializeAsync()
		{
			if (_initialized)
			{
				throw new InvalidOperationException("Fixture was already initialized");
			}

			_value = _scope switch
			{
				FixtureScope.Collection => await Odachi.Testing.Scenarios.Xunit.ScenarioContext.CurrentCollection!.GetFixtureAsync(_factory),
				FixtureScope.Class => await Odachi.Testing.Scenarios.Xunit.ScenarioContext.CurrentClass!.GetFixtureAsync(_factory),
				FixtureScope.Case => await Odachi.Testing.Scenarios.Xunit.ScenarioContext.CurrentCase!.GetFixtureAsync(_factory),
				_ => throw new InvalidOperationException($"Undefined behavior for '{_scope}'"),
			};
			_initialized = true;
		}
	}
}
