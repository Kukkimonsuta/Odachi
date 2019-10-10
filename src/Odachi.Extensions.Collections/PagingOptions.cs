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
		public static int DefaultNumber = 0;
		public static int DefaultSize = 10;
		public static bool DefaultAcquireTotal = true;
		public static int DefaultOffset = 0;
		public static int? DefaultMaximumCount = 20;

		public PagingOptions()
			: this(DefaultNumber)
		{
		}
		public PagingOptions(int number)
			: this(number, DefaultSize, DefaultAcquireTotal, DefaultOffset, DefaultMaximumCount)
		{
		}
		public PagingOptions(int number, int size)
			: this(number, size, acquireTotal: DefaultAcquireTotal, offset: DefaultOffset, DefaultMaximumCount)
		{
		}
		public PagingOptions(int number, int size, bool acquireTotal)
			: this(number, size, acquireTotal: acquireTotal, offset: DefaultOffset, DefaultMaximumCount)
		{
		}
		public PagingOptions(int number, int size, bool acquireTotal, int offset)
			: this(number, size, acquireTotal: acquireTotal, offset: offset, DefaultMaximumCount)
		{
		}
		public PagingOptions(int number, int size, bool acquireTotal, int offset, int? maximumCount)
		{
			Number = number;
			Size = size;
			AcquireTotal = acquireTotal;
			Offset = offset;
			MaximumCount = maximumCount;
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
					throw new ArgumentOutOfRangeException(nameof(value), "Number must be more or equal to zero");

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
					throw new ArgumentOutOfRangeException(nameof(value), "Size must be more than zero");

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
					throw new ArgumentOutOfRangeException(nameof(value), "MaximumCount must be greater than zero or null");

				_maximumCount = value;
			}
		}
	}
}
