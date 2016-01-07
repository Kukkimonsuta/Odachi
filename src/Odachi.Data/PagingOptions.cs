using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Data
{
	/// <summary>
	/// Holds information about requested paging
	/// </summary>
	public sealed class PagingOptions
	{
		private PagingOptions(int page, int pageSize, bool total = true, int offset = 0, int? maxPageCount = null)
		{
			Page = page;
			PageSize = pageSize;
			Total = total;
			Offset = offset;
			MaxPageCount = maxPageCount;
		}

		private int _page;
		private int _pageSize;
		private int _offset;
		private int? _maxPageCount;

		public int Page
		{
			get { return _page; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), "Page must be more or equal to zero");

				_page = value;
			}
		}
		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value), "PageSize must be more than zero");

				_pageSize = value;
			}
		}
		public bool Total { get; set; }
		public int Offset
		{
			get { return _offset; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), "Offset must be more or than zero");

				_offset = value;
			}
		}
		public int? MaxPageCount
		{
			get { return _maxPageCount; }
			set
			{
				if (value != null && value <= 0)
					throw new ArgumentOutOfRangeException("value", "MaxPageCount must be more than zero or null");

				_maxPageCount = value;
			}
		}

		private int GetSkipCount()
		{
			return Math.Max(0, this.Page * this.PageSize - this.Offset);
		}

		private int GetTakeCount()
		{
			// get offset on selected page
			var offset = Math.Max(0, this.Offset - this.Page * this.PageSize);

			return Math.Max(0, this.PageSize - offset);
		}

		internal CollectionPage<TEntity> Execute<TEntity>(IQueryable<TEntity> query)
		{
			var result = query;

			var skipCount = GetSkipCount();
			if (skipCount > 0)
				result = result.Skip(skipCount);

			var takeCount = GetTakeCount();
			var data = takeCount <= 0 ? new TEntity[0] : result.Take(takeCount).ToArray();

			var total = -1;
			var overflow = false;
			if (this.Total)
			{
				if (this.MaxPageCount != null)
				{
					var max = this.MaxPageCount.Value * this.PageSize - this.Offset;

					total = query.Take(max + 1).Count() + this.Offset;

					if (total > max)
					{
						overflow = true;
						total = max;
					}
				}
				else
					total = query.Count() + this.Offset;
			}

			return new CollectionPage<TEntity>(
				data,
				this.Page,
				this.PageSize,
				total,
				overflow
			);
		}

		internal async Task<CollectionPage<TEntity>> ExecuteAsync<TEntity>(IQueryable<TEntity> query)
		{
			var result = query;

			var skipCount = GetSkipCount();
			if (skipCount > 0)
				result = result.Skip(skipCount);

			var takeCount = GetTakeCount();
			var data = takeCount <= 0 ? new TEntity[0] : await result.Take(takeCount).ToArrayAsync();

			var total = -1;
			var overflow = false;
			if (this.Total)
			{
				if (this.MaxPageCount != null)
				{
					var max = this.MaxPageCount.Value * this.PageSize - this.Offset;

					total = await query.Take(max + 1).CountAsync() + this.Offset;

					if (total > max)
					{
						overflow = true;
						total = max;
					}
				}
				else
					total = await query.CountAsync() + this.Offset;
			}

			return new CollectionPage<TEntity>(
				data,
				this.Page,
				this.PageSize,
				total,
				overflow
			);
		}

		internal CollectionPage<TEntity> Execute<TEntity>(IEnumerable<TEntity> resource)
		{
			var result = resource;

			var skipCount = GetSkipCount();
			if (skipCount > 0)
				result = result.Skip(skipCount);

			var takeCount = GetTakeCount();
			var data = takeCount <= 0 ? new TEntity[0] : result.Take(takeCount).ToArray();

			var total = -1;
			var overflow = false;
			if (this.Total)
			{
				if (this.MaxPageCount != null)
				{
					var max = this.MaxPageCount.Value * this.PageSize - this.Offset;

					total = resource.Take(max + 1).Count() + this.Offset;

					if (total > max)
					{
						overflow = true;
						total = max;
					}
				}
				else
					total = resource.Count() + this.Offset;
			}

			return new CollectionPage<TEntity>(
				data,
				this.Page,
				this.PageSize,
				total,
				overflow
			);
		}

		#region Static members

		public static PagingOptions Create(int page, int pageSize, bool total = true, int offset = 0, int? maxPageCount = null)
		{
			return new PagingOptions(page, pageSize, total: total, offset: offset, maxPageCount: maxPageCount);
		}

		#endregion
	}
}
