using System;
using System.Collections.Generic;
using System.Text;

namespace Odachi.Extensions.Collections
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Partitions into arrays of <paramref name="size" />
		/// </summary>
		public static IEnumerable<T[]> Partition<T>(this IEnumerable<T> enumerable, int size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException(nameof(size));

			using var enumerator = enumerable.GetEnumerator();

			while (true)
			{
				var partition = new T[size];

				var length = 0;
				while (length < size && enumerator.MoveNext())
				{
					partition[length++] = enumerator.Current;
				}

				if (length <= 0)
				{
					break;
				}
				else if (length >= size)
				{
					yield return partition;
				}
				else
				{
					Array.Resize(ref partition, length);
					yield return partition;
				}
			}
		}
	}
}
