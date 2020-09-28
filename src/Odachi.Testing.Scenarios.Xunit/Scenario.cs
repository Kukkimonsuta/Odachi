using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Reflection;

namespace Odachi.Testing.Scenarios.Xunit
{
	public abstract class Scenario : IAsyncLifetime
	{
		async Task IAsyncLifetime.InitializeAsync()
		{
			var members = GetType().GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (var member in members)
			{
				if (member is FieldInfo fieldInfo)
				{
					if (typeof(IAsyncInitializable).IsAssignableFrom(fieldInfo.FieldType))
					{
						await ((IAsyncInitializable)fieldInfo.GetValue(this)).InitializeAsync();
					}
				}
				else if (member is PropertyInfo propertyInfo)
				{
					if (typeof(IAsyncInitializable).IsAssignableFrom(propertyInfo.PropertyType))
					{
						await ((IAsyncInitializable)propertyInfo.GetValue(this)).InitializeAsync();
					}
				}
			}
		}

		Task IAsyncLifetime.DisposeAsync()
		{
			return Task.CompletedTask;
		}
	}
}
