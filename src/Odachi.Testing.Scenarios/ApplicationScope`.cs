using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Odachi.Testing.Scenarios
{
	public sealed class ApplicationScope<T1> : ApplicationScope
		where T1 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
		}

		public T1 Service1 { get; }

		public void Deconstruct(out T1 service1)
		{
			service1 = Service1;
		}
	}

	public sealed class ApplicationScope<T1, T2> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }

		public void Deconstruct(out T1 service1, out T2 service2)
		{
			service1 = Service1;
			service2 = Service2;
		}
	}

	public sealed class ApplicationScope<T1, T2, T3> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
		where T3 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
			Service3 = scope.ServiceProvider.GetRequiredService<T3>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }
		public T3 Service3 { get; }

		public void Deconstruct(out T1 service1, out T2 service2, out T3 service3)
		{
			service1 = Service1;
			service2 = Service2;
			service3 = Service3;
		}
	}

	public sealed class ApplicationScope<T1, T2, T3, T4> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
		where T3 : notnull
		where T4 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
			Service3 = scope.ServiceProvider.GetRequiredService<T3>();
			Service4 = scope.ServiceProvider.GetRequiredService<T4>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }
		public T3 Service3 { get; }
		public T4 Service4 { get; }

		public void Deconstruct(out T1 service1, out T2 service2, out T3 service3, out T4 service4)
		{
			service1 = Service1;
			service2 = Service2;
			service3 = Service3;
			service4 = Service4;
		}
	}

	public sealed class ApplicationScope<T1, T2, T3, T4, T5> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
		where T3 : notnull
		where T4 : notnull
		where T5 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
			Service3 = scope.ServiceProvider.GetRequiredService<T3>();
			Service4 = scope.ServiceProvider.GetRequiredService<T4>();
			Service5 = scope.ServiceProvider.GetRequiredService<T5>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }
		public T3 Service3 { get; }
		public T4 Service4 { get; }
		public T5 Service5 { get; }

		public void Deconstruct(out T1 service1, out T2 service2, out T3 service3, out T4 service4, out T5 service5)
		{
			service1 = Service1;
			service2 = Service2;
			service3 = Service3;
			service4 = Service4;
			service5 = Service5;
		}
	}

	public sealed class ApplicationScope<T1, T2, T3, T4, T5, T6> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
		where T3 : notnull
		where T4 : notnull
		where T5 : notnull
		where T6 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
			Service3 = scope.ServiceProvider.GetRequiredService<T3>();
			Service4 = scope.ServiceProvider.GetRequiredService<T4>();
			Service5 = scope.ServiceProvider.GetRequiredService<T5>();
			Service6 = scope.ServiceProvider.GetRequiredService<T6>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }
		public T3 Service3 { get; }
		public T4 Service4 { get; }
		public T5 Service5 { get; }
		public T6 Service6 { get; }

		public void Deconstruct(out T1 service1, out T2 service2, out T3 service3, out T4 service4, out T5 service5, out T6 service6)
		{
			service1 = Service1;
			service2 = Service2;
			service3 = Service3;
			service4 = Service4;
			service5 = Service5;
			service6 = Service6;
		}
	}

	public sealed class ApplicationScope<T1, T2, T3, T4, T5, T6, T7> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
		where T3 : notnull
		where T4 : notnull
		where T5 : notnull
		where T6 : notnull
		where T7 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
			Service3 = scope.ServiceProvider.GetRequiredService<T3>();
			Service4 = scope.ServiceProvider.GetRequiredService<T4>();
			Service5 = scope.ServiceProvider.GetRequiredService<T5>();
			Service6 = scope.ServiceProvider.GetRequiredService<T6>();
			Service7 = scope.ServiceProvider.GetRequiredService<T7>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }
		public T3 Service3 { get; }
		public T4 Service4 { get; }
		public T5 Service5 { get; }
		public T6 Service6 { get; }
		public T7 Service7 { get; }

		public void Deconstruct(out T1 service1, out T2 service2, out T3 service3, out T4 service4, out T5 service5, out T6 service6, out T7 service7)
		{
			service1 = Service1;
			service2 = Service2;
			service3 = Service3;
			service4 = Service4;
			service5 = Service5;
			service6 = Service6;
			service7 = Service7;
		}
	}

	public sealed class ApplicationScope<T1, T2, T3, T4, T5, T6, T7, T8> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
		where T3 : notnull
		where T4 : notnull
		where T5 : notnull
		where T6 : notnull
		where T7 : notnull
		where T8 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
			Service3 = scope.ServiceProvider.GetRequiredService<T3>();
			Service4 = scope.ServiceProvider.GetRequiredService<T4>();
			Service5 = scope.ServiceProvider.GetRequiredService<T5>();
			Service6 = scope.ServiceProvider.GetRequiredService<T6>();
			Service7 = scope.ServiceProvider.GetRequiredService<T7>();
			Service8 = scope.ServiceProvider.GetRequiredService<T8>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }
		public T3 Service3 { get; }
		public T4 Service4 { get; }
		public T5 Service5 { get; }
		public T6 Service6 { get; }
		public T7 Service7 { get; }
		public T8 Service8 { get; }

		public void Deconstruct(out T1 service1, out T2 service2, out T3 service3, out T4 service4, out T5 service5, out T6 service6, out T7 service7, out T8 service8)
		{
			service1 = Service1;
			service2 = Service2;
			service3 = Service3;
			service4 = Service4;
			service5 = Service5;
			service6 = Service6;
			service7 = Service7;
			service8 = Service8;
		}
	}

	public sealed class ApplicationScope<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ApplicationScope
		where T1 : notnull
		where T2 : notnull
		where T3 : notnull
		where T4 : notnull
		where T5 : notnull
		where T6 : notnull
		where T7 : notnull
		where T8 : notnull
		where T9 : notnull
	{
		internal ApplicationScope(ApplicationInstance application, IServiceScope scope, IPrincipal? principal = null)
			: base(application, scope, principal: principal)
		{
			Service1 = scope.ServiceProvider.GetRequiredService<T1>();
			Service2 = scope.ServiceProvider.GetRequiredService<T2>();
			Service3 = scope.ServiceProvider.GetRequiredService<T3>();
			Service4 = scope.ServiceProvider.GetRequiredService<T4>();
			Service5 = scope.ServiceProvider.GetRequiredService<T5>();
			Service6 = scope.ServiceProvider.GetRequiredService<T6>();
			Service7 = scope.ServiceProvider.GetRequiredService<T7>();
			Service8 = scope.ServiceProvider.GetRequiredService<T8>();
			Service9 = scope.ServiceProvider.GetRequiredService<T9>();
		}

		public T1 Service1 { get; }
		public T2 Service2 { get; }
		public T3 Service3 { get; }
		public T4 Service4 { get; }
		public T5 Service5 { get; }
		public T6 Service6 { get; }
		public T7 Service7 { get; }
		public T8 Service8 { get; }
		public T9 Service9 { get; }

		public void Deconstruct(out T1 service1, out T2 service2, out T3 service3, out T4 service4, out T5 service5, out T6 service6, out T7 service7, out T8 service8, out T9 service9)
		{
			service1 = Service1;
			service2 = Service2;
			service3 = Service3;
			service4 = Service4;
			service5 = Service5;
			service6 = Service6;
			service7 = Service7;
			service8 = Service8;
			service9 = Service9;
		}
	}

	public static class ApplicationScopeExtensions
	{
		public static async Task<ApplicationScope<T1>> CreateScopeAsync<T1>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2>> CreateScopeAsync<T1, T2>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2, T3>> CreateScopeAsync<T1, T2, T3>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
			where T3 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2, T3>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2, T3, T4>> CreateScopeAsync<T1, T2, T3, T4>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
			where T3 : notnull
			where T4 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2, T3, T4>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2, T3, T4, T5>> CreateScopeAsync<T1, T2, T3, T4, T5>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
			where T3 : notnull
			where T4 : notnull
			where T5 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2, T3, T4, T5>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2, T3, T4, T5, T6>> CreateScopeAsync<T1, T2, T3, T4, T5, T6>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
			where T3 : notnull
			where T4 : notnull
			where T5 : notnull
			where T6 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2, T3, T4, T5, T6>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2, T3, T4, T5, T6, T7>> CreateScopeAsync<T1, T2, T3, T4, T5, T6, T7>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
			where T3 : notnull
			where T4 : notnull
			where T5 : notnull
			where T6 : notnull
			where T7 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2, T3, T4, T5, T6, T7>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2, T3, T4, T5, T6, T7, T8>> CreateScopeAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
			where T3 : notnull
			where T4 : notnull
			where T5 : notnull
			where T6 : notnull
			where T7 : notnull
			where T8 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2, T3, T4, T5, T6, T7, T8>(application, scope, principal: principal);
		}
		public static async Task<ApplicationScope<T1, T2, T3, T4, T5, T6, T7, T8, T9>> CreateScopeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ApplicationInstance application, IPrincipal? principal = null)
			where T1 : notnull
			where T2 : notnull
			where T3 : notnull
			where T4 : notnull
			where T5 : notnull
			where T6 : notnull
			where T7 : notnull
			where T8 : notnull
			where T9 : notnull
		{
			var scope = await application.CreateScopeInternalAsync();

			return new ApplicationScope<T1, T2, T3, T4, T5, T6, T7, T8, T9>(application, scope, principal: principal);
		}
	}
}
