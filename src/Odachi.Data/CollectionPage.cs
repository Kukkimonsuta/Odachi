using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Data
{
	/// <summary>
	/// Holds information about a part of collection.
	/// </summary>
	public abstract class CollectionPage
	{
		public CollectionPage(int page, int pageSize, int total, bool overflow)
		{
			if (page < 0)
				throw new ArgumentOutOfRangeException(nameof(page), "Page must be more or equal to zero");
			if (pageSize <= 0)
				throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be more than zero");
			if (total < 0)
				throw new ArgumentOutOfRangeException(nameof(total), "Total must be more or equal to zero");

			Page = page;
			PageSize = pageSize;
			Total = total;
			Overflow = overflow;
		}

		public int Page { get; protected set; }
		public int PageSize { get; protected set; }
		public int Total { get; protected set; }
		public bool Overflow { get; protected set; }

		public int PageCount
		{
			get { return Total != -1 ? (int)Math.Ceiling(Total / (double)PageSize) : -1; }
		}

		public bool HasPrevious
		{
			get { return Page > 0; }
		}

		public bool HasNext
		{
			get { return Page + 1 < PageCount; }
		}

		#region Static members

		public static CollectionPage<T> Empty<T>(int pageSize = 1)
		{
			return new CollectionPage<T>(Enumerable.Empty<T>(), 0, pageSize, 0, false);
		}

		#endregion
	}

	/// <summary>
	/// Holds information about a part of collection.
	/// </summary>
	public class CollectionPage<T> : CollectionPage, IEnumerable<T>
	{
		public CollectionPage(IEnumerable<T> data, int page, int pageSize, int total, bool totalOverflow)
			: base(page, pageSize, total, totalOverflow)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			this._data = data;
		}

		private IEnumerable<T> _data;

		#region IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		#endregion
	}
}
