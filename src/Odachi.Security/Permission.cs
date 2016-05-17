using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Odachi.Security
{
	public sealed class PermissionArgument
	{
		private PermissionArgument()
		{ }

		public static readonly PermissionArgument Any = new PermissionArgument();
	}

	/// <summary>
	/// Class representing a permission.
	/// </summary>
	public sealed class Permission
	{
		public const string PermissionClaim = "http://schemas.algorim.com/2015/09/odachi/claims/permission";

		private static readonly Regex VariableRegex = new Regex(@"\\\{([0-9]+)\}");

		public Permission(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			Name = name;

			var pattern = Regex.Escape(name);
			pattern = VariableRegex.Replace(pattern, m => $"(?<arg{m.Groups[1].Value}>.*?)");
			_regex = new Regex($"^{pattern}$");

			var match = _regex.Match(name);
			if (!match.Success)
				throw new InvalidOperationException("Failed to construct permission regex");

			VariableCount = match.Groups.Count - 1;
		}

		private Regex _regex;

		public string Name { get; private set; }
		public int VariableCount { get; private set; }

		public bool Matches(string value)
		{
			return VariableCount == 0 && Name == value;
		}

		public bool Matches(string value, object arg0)
		{
			if (VariableCount != 1)
				return false;

			var match = _regex.Match(value);
			if (!match.Success)
				return false;

			return arg0 == PermissionArgument.Any || match.Groups["arg0"]?.Value == (arg0?.ToString() ?? "");
		}

		public bool Matches(string value, object arg0, object arg1)
		{
			if (VariableCount != 2)
				return false;

			var match = _regex.Match(value);
			if (!match.Success)
				return false;

			return
				(arg0 == PermissionArgument.Any || match.Groups["arg0"]?.Value == (arg0?.ToString() ?? "")) &&
				(arg1 == PermissionArgument.Any || match.Groups["arg1"]?.Value == (arg1?.ToString() ?? ""));
		}

		public bool Matches(string value, params object[] args)
		{
			if (VariableCount != args.Length)
				return false;

			var match = _regex.Match(value);
			if (!match.Success)
				return false;

			for (var index = 0; index < args.Length; index++)
			{
				var arg = args[index];

				if (arg == PermissionArgument.Any)
					continue;

				if (match.Groups[$"arg{index}"]?.Value != (arg?.ToString() ?? ""))
					return false;
			}

			return true;
		}
	}
}
