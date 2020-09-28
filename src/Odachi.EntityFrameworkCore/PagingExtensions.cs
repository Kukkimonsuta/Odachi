using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Odachi.Extensions.Collections;
using Odachi.Extensions.Collections.Internal;

namespace Odachi.EntityFrameworkCore
{
	public static class PagingExtensions
	{
		/// <summary>
		/// Selects page from a collection.
		/// </summary>
		public static async Task<Page<TEntity>> ToPageAsync<TEntity>(this IQueryable<TEntity> query, PagingOptions options)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			var result = query;

			var skipCount = options.GetSkipCount();
			if (skipCount > 0)
				result = result.Skip(skipCount);

			var takeCount = options.GetTakeCount();
			var data = takeCount <= 0 ? new TEntity[0] : await result.Take(takeCount).ToArrayAsync();

			var total = -1;
			var overflow = false;
			if (options.AcquireTotal)
			{
				if (options.MaximumCount != null)
				{
					var max = options.MaximumCount.Value * options.Size - options.Offset;

					total = options.Offset + await query.Take(max + 1).CountAsync();

					if (total > max)
					{
						overflow = true;
						total = max;
					}
				}
				else
				{
					total = options.Offset + await query.CountAsync();
				}
			}

			return new Page<TEntity>(
				data,
				options.Number,
				options.Size,
				total,
				overflow
			);
		}
	}
}
