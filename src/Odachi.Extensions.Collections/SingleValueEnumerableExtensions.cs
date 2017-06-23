using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Extensions.Collections
{
    public static class SingleValueEnumerableExtensions
    {
		/// <summary>
		/// Wraps <paramref name="value"/> in <see cref="SingleValueEnumerable{T}"/>.
		/// </summary>
		/// <typeparam name="T">Value type</typeparam>
		/// <param name="value">Value</param>
		/// <returns>Value as <see cref="SingleValueEnumerable{T}"/></returns>
		public static SingleValueEnumerable<T> AsSingleValueEnumerable<T>(this T value)
		{
			return new SingleValueEnumerable<T>(value);
		}
	}
}
