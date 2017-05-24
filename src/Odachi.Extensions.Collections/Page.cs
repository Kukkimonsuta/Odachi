using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Odachi.Abstractions;

namespace Odachi.Extensions.Collections
{
	/// <summary>
	/// Holds information about page of a collection.
	/// </summary>
	public abstract class Page : IPage
	{
		public Page(IEnumerable data, int page, int pageSize, int? total = null, bool overflow = false)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			if (page < 0)
				throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than or equal to zero");
			if (pageSize <= 0)
				throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be greater than zero");
			if (total != null && total < 0)
				throw new ArgumentOutOfRangeException(nameof(total), "Total must be greater than or equal to zero or null");

			_data = data;
			Number = page;
			Size = pageSize;
			Total = total;
			Overflow = overflow;
		}

		private IEnumerable _data;

		/// <summary>
		/// Number of page that is represented by this instance.
		/// </summary>
		public int Number { get; }

		/// <summary>
		/// Maximum size of a page.
		/// </summary>
		public int Size { get; }

		/// <summary>
		/// Total number of pages that can be formed from discovered items.
		/// </summary>
		public int Count
		{
			get { return Total == null ? -1 : (int)Math.Ceiling(Total.Value / (double)Size); }
		}

		/// <summary>
		/// Total number of discovered items.
		/// </summary>
		public int? Total { get; }

		/// <summary>
		/// Notes whether there are more items after `Total` was reached.
		/// </summary>
		public bool Overflow { get; }

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

		#endregion

		#region Static members

		public static Page<T> Empty<T>(int pageSize = 10)
		{
			return new Page<T>(Enumerable.Empty<T>(), 0, pageSize, 0, false);
		}

		#endregion
	}

	/// <summary>
	/// Holds information about page of a collection.
	/// </summary>
	public class Page<T> : Page, IEnumerable<T>
	{
		public Page(IEnumerable<T> data, int page, int pageSize, int? total = null, bool overflow = false)
			: base(data, page, pageSize, total: total, overflow: overflow)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));

			_data = data;
		}

		private IEnumerable<T> _data;

		#region IEnumerable

		public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();

		#endregion
	}
}
