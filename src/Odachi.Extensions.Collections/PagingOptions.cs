using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.Extensions.Collections;

namespace Odachi.Extensions.Collections
{
	/// <summary>
	/// Holds information about requested paging.
	/// </summary>
	public class PagingOptions
	{
		public const int DefaultPageSize = 10;
		public const int DefaultMaxPageCount = 20;

		public PagingOptions()
			: this(0, DefaultPageSize)
		{
		}
		public PagingOptions(int number, int size = DefaultPageSize, bool acquireTotal = true, int offset = 0, int? maxPageCount = DefaultMaxPageCount)
		{
			Number = number;
			Size = size;
			AcquireTotal = acquireTotal;
			Offset = offset;
			MaximumCount = maxPageCount;
		}

		private int _page;
		private int _pageSize;
		private int _offset;
		private int? _maximumCount;

		/// <summary>
		/// Zero indexed number of requested page.
		/// </summary>
		public int Number
		{
			get { return _page; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), "Page must be more or equal to zero");

				_page = value;
			}
		}

		/// <summary>
		/// Maximum size of page.
		/// </summary>
		public int Size
		{
			get { return _pageSize; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value), "PageSize must be more than zero");

				_pageSize = value;
			}
		}

		/// <summary>
		/// Acquire total number of items.
		/// </summary>
		public bool AcquireTotal { get; set; }

		/// <summary>
		/// Pretend there is number of items at the start of source collection.
		/// </summary>
		public int Offset
		{
			get { return _offset; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), "Offset must be greater than or equal to zero");

				_offset = value;
			}
		}

		/// <summary>
		/// Maximum expected number of pages.
		/// </summary>
		public int? MaximumCount
		{
			get { return _maximumCount; }
			set
			{
				if (value != null && value <= 0)
					throw new ArgumentOutOfRangeException("value", "MaxPageCount must be greater than zero or null");

				_maximumCount = value;
			}
		}
	}
}
