using Odachi.AspNetCore.JsonRpc.Internal;
using Odachi.AspNetCore.JsonRpc.Modules;
using Odachi.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Model
{
	public class JsonRpcMethodCollection : ICollection<JsonRpcMethod>
	{
		public JsonRpcMethodCollection()
		{
			_methods = new Dictionary<string, JsonRpcMethod>();
		}

		private IDictionary<string, JsonRpcMethod> _methods;

		public bool TryGetMethod(string name, out JsonRpcMethod method) => _methods.TryGetValue(name, out method);

		/// <summary>
		/// Adds all public methods of `T` marked with `RpcMethodAttribute`.
		/// </summary>
		public void AddReflected<T>()
		{
			var type = typeof(T);
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			foreach (var method in methods.Where(m => m.DeclaringType == type))
			{
				var attribute = method.GetCustomAttribute<RpcMethodAttribute>();
				if (attribute == null)
					continue;

				var moduleName = attribute.ModuleName ?? ModuleNameResolver(type, method);
				var methodName = attribute.MethodName ?? MethodNameResolver(type, method);

				var reflectedMethod = new ReflectedJsonRpcMethod(moduleName, methodName, type, method);

				Add(reflectedMethod);
			}
		}
		/// <summary>
		/// Adds all public methods of `T` with given name.
		/// </summary>
		public void AddReflected<T>(Expression<Func<T, string>> methodExpression)
		{
			var constantExpression = methodExpression.Body as ConstantExpression;
			if (constantExpression == null)
				throw new ArgumentException("Must be a constant expression", nameof(methodExpression));

			AddReflected<T>(constantExpression.Value.ToString());
		}
		/// <summary>
		/// Adds all public methods of `T` with given name.
		/// </summary>
		public void AddReflected<T>(string name)
		{
			var type = typeof(T);
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			foreach (var method in methods)
			{
				if (method.Name != name)
					continue;

				var moduleName = ModuleNameResolver(type, method);
				var methodName = MethodNameResolver(type, method);

				var reflectedMethod = new ReflectedJsonRpcMethod(moduleName, methodName, type, method);

				Add(reflectedMethod);
			}
		}

		#region ICollection

		public int Count => _methods.Count;

		public bool IsReadOnly => false;

		public void Add(JsonRpcMethod item)
		{
			JsonRpcMethod current;
			if (TryGetMethod(item.Name, out current) && current == item)
				return;

			_methods.Add(item.Name, item);
		}

		public void Clear() => _methods.Clear();

		public bool Contains(JsonRpcMethod item) => _methods.Values.Contains(item);

		public void CopyTo(JsonRpcMethod[] array, int arrayIndex) => _methods.Values.CopyTo(array, arrayIndex);

		public IEnumerator<JsonRpcMethod> GetEnumerator() => _methods.Values.GetEnumerator();

		public bool Remove(JsonRpcMethod item) => _methods.Remove(item.Name);

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#endregion

		#region Static members

		public static Func<Type, MethodInfo, string> ModuleNameResolver { get; set; } = (type, method) =>
		{
			var moduleName = type.Name;
			if (moduleName.EndsWith("Module"))
			{
				moduleName = moduleName.Substring(0, moduleName.Length - 6);
			}
			else if (moduleName.EndsWith("Service"))
			{
				moduleName = moduleName.Substring(0, moduleName.Length - 7);
			}

			return moduleName;
		};

		public static Func<Type, MethodInfo, string> MethodNameResolver { get; set; } = (type, method) =>
		{
			var methodName = "";
			for (var i = 0; i < method.Name.Length; i++)
			{
				if (char.IsLower(method.Name, i))
				{
					methodName += method.Name.Substring(i);
					break;
				}

				methodName += char.ToLower(method.Name[i]);
			}
			if (methodName.EndsWith("Async"))
			{
				methodName = methodName.Substring(0, methodName.Length - 5);
			}

			return methodName;
		};

		#endregion
	}
}
