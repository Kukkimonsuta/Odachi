using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Gettext.Internal
{
	public struct ReadOnlyListSegment<T> : IReadOnlyList<T>
	{
		public ReadOnlyListSegment(IReadOnlyList<T> list, int offset, int length)
		{
			List = list;
			Offset = offset;
			Length = length;
		}

		public IReadOnlyList<T> List { get; set; }

		public int Offset { get; set; }

		public int Length { get; set; }

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Length)
					throw new ArgumentOutOfRangeException(nameof(index));

				return List[Offset + index];
			}
		}

		public int Count
		{
			get
			{
				return Length;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ReadOnlyListSegmentEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ReadOnlyListSegmentEnumerator(this);
		}

		#region Nested member

		private sealed class ReadOnlyListSegmentEnumerator : IEnumerator<T>
		{
			internal ReadOnlyListSegmentEnumerator(ReadOnlyListSegment<T> segment)
			{
				_list = segment.List;
				_start = segment.Offset;
				_end = _start + segment.Count;
				_current = _start - 1;
			}

			private IReadOnlyList<T> _list;
			private int _start;
			private int _end;
			private int _current;

			public bool MoveNext()
			{
				if (_current < _end)
				{
					_current++;
					return (_current < _end);
				}

				return false;
			}

			public T Current
			{
				get
				{
					if (_current < _start)
						throw new InvalidOperationException("Enumeration has not started");
					if (_current >= _end)
						throw new InvalidOperationException("Enumeration has ended");

					return _list[_current];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			void IEnumerator.Reset()
			{
				_current = _start - 1;
			}

			public void Dispose()
			{
			}
		}

		#endregion
	}
}
