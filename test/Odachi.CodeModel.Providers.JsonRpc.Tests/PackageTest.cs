using System.Linq;
using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Providers.JsonRpc.Tests.Model;
using Odachi.JsonRpc.Server.Internal;
using Odachi.JsonRpc.Server.Model;
using Xunit;

namespace Odachi.CodeModel.Providers.JsonRpc.Tests
{
	public class DescriptionTests
	{
		[Fact]
		public void Can_reflected_jsonrpc_method_nrton()
		{
			var package = new PackageBuilder("Test")
				.UseJsonRpc()
				.Module_Service_JsonRpc("NrtOnServiceRpc", new JsonRpcMethod[]
				{
					new ReflectedJsonRpcMethod("NrtOnService", "TestAsync", typeof(NrtOnService), typeof(NrtOnService).GetMethod(nameof(NrtOnService.TestAsync))),
					new ReflectedJsonRpcMethod("NrtOnService", "Test", typeof(NrtOnService), typeof(NrtOnService).GetMethod(nameof(NrtOnService.Test))),
					new ReflectedJsonRpcMethod("NrtOnService", "TestNullableAsync", typeof(NrtOnService), typeof(NrtOnService).GetMethod(nameof(NrtOnService.TestNullableAsync))),
					new ReflectedJsonRpcMethod("NrtOnService", "TestNullable", typeof(NrtOnService), typeof(NrtOnService).GetMethod(nameof(NrtOnService.TestNullable))),
				})
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Services);

			var service = package.Services[0];

			Assert.Collection(service.Methods,
				method =>
				{
					Assert.Equal(nameof(NrtOnService.TestAsync), method.Name);
					Assert.False(method.ReturnType.IsNullable);

					Assert.Collection(method.ReturnType.GenericArguments,
						arg =>
						{
							Assert.Equal("string", arg.Name);
							Assert.False(arg.IsNullable);
						},
						arg =>
						{
							Assert.Equal("integer", arg.Name);
							Assert.False(arg.IsNullable);
						}
					);
				},
				method =>
				{
					Assert.Equal(nameof(NrtOnService.Test), method.Name);
					Assert.False(method.ReturnType.IsNullable);

					Assert.Collection(method.ReturnType.GenericArguments,
						arg =>
						{
							Assert.Equal("string", arg.Name);
							Assert.False(arg.IsNullable);
						},
						arg =>
						{
							Assert.Equal("integer", arg.Name);
							Assert.False(arg.IsNullable);
						}
					);
				},
				method =>
				{
					Assert.Equal(nameof(NrtOnService.TestNullableAsync), method.Name);
					Assert.False(method.ReturnType.IsNullable);

					Assert.Collection(method.ReturnType.GenericArguments,
						arg =>
						{
							Assert.Equal("string", arg.Name);
							Assert.True(arg.IsNullable);
						},
						arg =>
						{
							Assert.Equal("integer", arg.Name);
							Assert.False(arg.IsNullable);
						}
					);
				},
				method =>
				{
					Assert.Equal(nameof(NrtOnService.TestNullable), method.Name);
					Assert.False(method.ReturnType.IsNullable);

					Assert.Collection(method.ReturnType.GenericArguments,
						arg =>
						{
							Assert.Equal("string", arg.Name);
							Assert.True(arg.IsNullable);
						},
						arg =>
						{
							Assert.Equal("integer", arg.Name);
							Assert.False(arg.IsNullable);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_reflected_jsonrpc_method_nrtoff()
		{
			var package = new PackageBuilder("Test")
				.UseJsonRpc()
				.Module_Service_JsonRpc("NrtOffServiceRpc", new JsonRpcMethod[]
				{
					new ReflectedJsonRpcMethod("NrtOffService", "TestAsync", typeof(NrtOffService), typeof(NrtOffService).GetMethod(nameof(NrtOffService.TestAsync))),
					new ReflectedJsonRpcMethod("NrtOffService", "Test", typeof(NrtOffService), typeof(NrtOffService).GetMethod(nameof(NrtOffService.Test))),
				})
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Services);

			var service = package.Services[0];

			Assert.Collection(service.Methods,
				method =>
				{
					Assert.Equal(nameof(NrtOffService.TestAsync), method.Name);
					Assert.False(method.ReturnType.IsNullable);

					Assert.Collection(method.ReturnType.GenericArguments,
						arg =>
						{
							Assert.Equal("string", arg.Name);
							Assert.True(arg.IsNullable);
						},
						arg =>
						{
							Assert.Equal("integer", arg.Name);
							Assert.False(arg.IsNullable);
						}
					);
				},
				method =>
				{
					Assert.Equal(nameof(NrtOffService.Test), method.Name);
					Assert.False(method.ReturnType.IsNullable);

					Assert.Collection(method.ReturnType.GenericArguments,
						arg =>
						{
							Assert.Equal("string", arg.Name);
							Assert.True(arg.IsNullable);
						},
						arg =>
						{
							Assert.Equal("integer", arg.Name);
							Assert.False(arg.IsNullable);
						}
					);
				}
			);
		}
	}
}
