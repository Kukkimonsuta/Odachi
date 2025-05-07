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
                switch (member)
                {
                    case FieldInfo fieldInfo:
                    {
                        if (typeof(IAsyncInitializable).IsAssignableFrom(fieldInfo.FieldType))
                        {
                            if (fieldInfo.GetValue(this) is IAsyncInitializable value)
                            {
                                await value.InitializeAsync();
                            }
                        }

                        break;
                    }
                    case PropertyInfo propertyInfo:
                    {
                        if (typeof(IAsyncInitializable).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            if (propertyInfo.GetValue(this) is IAsyncInitializable value)
                            {
                                await value.InitializeAsync();
                            }
                        }

                        break;
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
