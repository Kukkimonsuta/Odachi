using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.Extensions.Collections;
using Odachi.Extensions.Collections.Internal;

namespace Odachi.Extensions.Collections;

public static class PagingExtensions
{
	/// <summary>
	/// Selects page from a collection.
	/// </summary>
	public static Page<TEntity> ToPage<TEntity>(this IQueryable<TEntity> query, PagingOptions options)
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
		var data = takeCount <= 0 ? Array.Empty<TEntity>() : result.Take(takeCount).ToArray();

		var total = -1;
		var overflow = false;
		if (options.AcquireTotal)
		{
			if (options.MaximumCount != null)
			{
				var max = options.MaximumCount.Value * options.Size - options.Offset;

				total = options.Offset + query.Take(max + 1).Count();

				if (total > max)
				{
					overflow = true;
					total = max;
				}
			}
			else
			{
				total = options.Offset + query.Count();
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

	/// <summary>
	/// Selects page from a collection.
	/// </summary>
	public static Page<TEntity> ToPage<TEntity>(this IEnumerable<TEntity> resource, PagingOptions options)
	{
		if (resource == null)
			throw new ArgumentNullException(nameof(resource));
		if (options == null)
			throw new ArgumentNullException(nameof(options));

		var result = resource;

		var skipCount = options.GetSkipCount();
		if (skipCount > 0)
			result = result.Skip(skipCount);

		var takeCount = options.GetTakeCount();
		var data = takeCount <= 0 ? Array.Empty<TEntity>() : result.Take(takeCount).ToArray();

		var total = -1;
		var overflow = false;
		if (options.AcquireTotal)
		{
			if (options.MaximumCount != null)
			{
				var max = options.MaximumCount.Value * options.Size - options.Offset;

				total = resource.Take(max + 1).Count() + options.Offset;

				if (total > max)
				{
					overflow = true;
					total = max;
				}
			}
			else
			{
				total = options.Offset + resource.Count();
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
