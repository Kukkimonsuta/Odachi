using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeGen.CSharp.Internal
{
	public static class CSharpHelpers
	{
		public static string CombineNamespaces(params string[] fragments)
		{
			var builder = new StringBuilder();

			foreach (var fragment in fragments)
			{
				if (string.IsNullOrEmpty(fragment))
					continue;

				if (builder.Length > 0)
					builder.Append(".");

				builder.Append(fragment);
			}

			if (builder.Length <= 0)
				return null;

			return builder.ToString();
		}

        public static string GetFullName(Type type)
        {
            var name = type.FullName;

            if (type.GetTypeInfo().IsGenericType)
            {
                name = name.Remove(name.IndexOf('`'));
            }

            return name;
        }

        public static string GetName(Type type)
        {
            var name = type.Name;

            if (type.GetTypeInfo().IsGenericType)
            {
                name = name.Remove(name.IndexOf('`'));
            }

            return name;
        }
    }
}
