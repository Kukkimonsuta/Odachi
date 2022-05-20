using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Security
{
	public class PermissionTest
	{
		[Theory]
		[InlineData("testing-scope:without-variables")]
		[InlineData("testing-scope:without-variables-2")]
		public void Matches_no_variable(string permissionName)
		{
			var permission = new Permission(permissionName);
			var value = permissionName;

			// matches only exact permission
			Assert.True(permission.Matches(value));
			Assert.False(permission.Matches($"pre{value}"));
			Assert.False(permission.Matches($"{value}post"));
			Assert.False(permission.Matches($"pre{value}post"));

			// doesn't match anything with extra arg
			Assert.False(permission.Matches(value, ""));
			Assert.False(permission.Matches($"pre{value}", ""));
			Assert.False(permission.Matches($"{value}post", ""));
			Assert.False(permission.Matches($"pre{value}post", ""));

			// can read variables
			Assert.Equal(Array.Empty<string>(), permission.ReadVariables(value));
		}

		[Theory]
		[InlineData("testing-scope:with-variables({0})", "1")]
		[InlineData("testing-scope:with-variables({0})", "cookies")]
		public void Matches_one_variable(string permissionName, object arg0)
		{
			var permission = new Permission(permissionName);
			var value = string.Format(permissionName, arg0);

			// matches only exact permission
			Assert.True(permission.Matches(value, arg0));
			Assert.False(permission.Matches($"pre{value}", arg0));
			Assert.False(permission.Matches($"{value}post", arg0));
			Assert.False(permission.Matches($"pre{value}post", arg0));

			// doesn't match anything without arg0
			Assert.False(permission.Matches(value));
			Assert.False(permission.Matches($"pre{value}"));
			Assert.False(permission.Matches($"{value}post"));
			Assert.False(permission.Matches($"pre{value}post"));

			// doesn't match anything with extra arg
			Assert.False(permission.Matches(value, arg0, ""));
			Assert.False(permission.Matches($"pre{value}", arg0, ""));
			Assert.False(permission.Matches($"{value}post", arg0, ""));
			Assert.False(permission.Matches($"pre{value}post", arg0, ""));

			// doesn't match anything with different arg
			Assert.False(permission.Matches(value, Guid.NewGuid()));
			Assert.False(permission.Matches($"pre{value}", Guid.NewGuid()));
			Assert.False(permission.Matches($"{value}post", Guid.NewGuid()));
			Assert.False(permission.Matches($"pre{value}post", Guid.NewGuid()));

			// can read variables
			Assert.Equal(new[] { arg0.ToString() }, permission.ReadVariables(value));
		}

		[Theory]
		[InlineData("testing-scope:with-variables({0}):{1}", "1", "2")]
		[InlineData("testing-scope:with-variables({0}):{1}", "cookies", "milk")]
		public void Matches_two_variables(string permissionName, object arg0, object arg1)
		{
			var permission = new Permission(permissionName);
			var value = string.Format(permissionName, arg0, arg1);

			// matches only exact permission
			Assert.True(permission.Matches(value, arg0, arg1));
			Assert.False(permission.Matches($"pre{value}", arg0, arg1));
			Assert.False(permission.Matches($"{value}post", arg0, arg1));
			Assert.False(permission.Matches($"pre{value}post", arg0, arg1));

			// doesn't match anything without args
			Assert.False(permission.Matches(value));
			Assert.False(permission.Matches($"pre{value}"));
			Assert.False(permission.Matches($"{value}post"));
			Assert.False(permission.Matches($"pre{value}post"));

			// doesn't match anything without arg1
			Assert.False(permission.Matches(value, arg0));
			Assert.False(permission.Matches($"pre{value}", arg0));
			Assert.False(permission.Matches($"{value}post", arg0));
			Assert.False(permission.Matches($"pre{value}post", arg0));

			// doesn't match anything with extra arg
			Assert.False(permission.Matches(value, arg0, arg1, ""));
			Assert.False(permission.Matches($"pre{value}", arg0, arg1, ""));
			Assert.False(permission.Matches($"{value}post", arg0, arg1, ""));
			Assert.False(permission.Matches($"pre{value}post", arg0, arg1, ""));

			// doesn't match anything with different args
			Assert.False(permission.Matches(value, Guid.NewGuid(), Guid.NewGuid()));
			Assert.False(permission.Matches($"pre{value}", Guid.NewGuid(), Guid.NewGuid()));
			Assert.False(permission.Matches($"{value}post", Guid.NewGuid(), Guid.NewGuid()));
			Assert.False(permission.Matches($"pre{value}post", Guid.NewGuid(), Guid.NewGuid()));

			// can read variables
			Assert.Equal(new[] { arg0.ToString(), arg1.ToString() }, permission.ReadVariables(value));
		}

		public static IEnumerable<object[]> Matches_array_variables_data
		{
			get
			{
				return new[]
				{
					new object[] { "{0}:testing-scope:with-variables", new object[] { "1" } },
					new object[] { "testing-scope:with-variables({0})", new object[] { "1" } },
					new object[] { "testing-scope:with-variables({0})", new object[] { ClaimVariable.Any } },
					new object[] { "testing-scope:with-variables({0})-({1})", new object[] { "1", "2" } },
					new object[] { "testing-scope:with-variables({0})-({1})", new object[] { "1", ClaimVariable.Any } },
					new object[] { "testing-scope:stuff({0}, {1})-{2}:insanity({3})-({4})", new object[] { "1", "2", "3", "4", "5" } },
					new object[] { "testing-scope:stuff({0}, {1})-{2}:insanity({3})-({4})", new object[] { "1", "2", "3", ClaimVariable.Any, "5" } }
				};
			}
		}
		[Theory]
		[MemberData(nameof(Matches_array_variables_data))]
		public void Matches_array_variables(string permissionName, object[] args)
		{
			var permission = new Permission(permissionName);
			var value = string.Format(permissionName, args);

			// matches only exact permission
			Assert.True(permission.Matches(value, args));
			Assert.False(permission.Matches($"pre{value}", args));
			Assert.False(permission.Matches($"{value}post", args));
			Assert.False(permission.Matches($"pre{value}post", args));

			// doesn't match anything with less args
			var lessArgs = args.Take(args.Length - 1).ToArray();
			Assert.False(permission.Matches(value, lessArgs));
			Assert.False(permission.Matches($"pre{value}", lessArgs));
			Assert.False(permission.Matches($"{value}post", lessArgs));
			Assert.False(permission.Matches($"pre{value}post", lessArgs));

			// doesn't match anything with extra arg
			var moreArgs = args.Concat(new[] { "" }).ToArray();
			Assert.False(permission.Matches(value, moreArgs));
			Assert.False(permission.Matches($"pre{value}", moreArgs));
			Assert.False(permission.Matches($"{value}post", moreArgs));
			Assert.False(permission.Matches($"pre{value}post", moreArgs));

			// doesn't match different args
			if (!args.All(a => a == ClaimVariable.Any))
			{
				var differentArgs = args.Select(x => Guid.NewGuid()).ToArray();
				Assert.False(permission.Matches(value, differentArgs));
				Assert.False(permission.Matches($"pre{value}", differentArgs));
				Assert.False(permission.Matches($"{value}post", differentArgs));
				Assert.False(permission.Matches($"pre{value}post", differentArgs));
			}

			// can read variables
			Assert.Equal(args.Select(a => a.ToString()), permission.ReadVariables(value));
		}
	}
}
