using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Extensions.Collections;

/// <summary>
/// Wraps <typeparamref name="T"/> in and <see cref="IEnumerable{T}"/>.
/// </summary>
/// <typeparam name="T">Value type</typeparam>
public struct SingleValueEnumerable<T> : IEnumerable<T>
{
	public SingleValueEnumerable(T value)
	{
		_value = value;
	}

	private readonly T _value;

	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(ref this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(ref this);
	}

	#region Nested type: Enumerator

	public struct Enumerator : IEnumerator<T>
	{
		public Enumerator(ref SingleValueEnumerable<T> enumerable)
		{
			_enumerable = enumerable;
			_index = -1;
		}

		private readonly SingleValueEnumerable<T> _enumerable;
		private int _index;

		public T Current
		{
			get
			{
				if (_index != 0)
					throw new IndexOutOfRangeException();

				return _enumerable._value;
			}
		}

		object? IEnumerator.Current => Current;

		public SingleValueEnumerable<T> Enumerable
		{
			get
			{
				return _enumerable;
			}
		}

		public void Dispose() { }

		public bool MoveNext()
		{
			if (_index > 0)
				return false;

			return ++_index <= 0;
		}

		public void Reset()
		{
			_index = -1;
		}
	}

	#endregion
}