using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Data;

public static class SortExtensions
{
	/// <summary>
	/// Sorts collection using given sort options
	/// </summary>
	public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, SortOptions<TEntity> options)
	{
		if (query == null)
			throw new ArgumentNullException(nameof(query));
		if (options == null)
			throw new ArgumentNullException(nameof(options));

		return options.Execute(query);
	}

	/// <summary>
	/// Sorts collection using given sort options
	/// </summary>
	public static IEnumerable<TEntity> OrderBy<TEntity>(this IEnumerable<TEntity> query, SortOptions<TEntity> options)
	{
		if (query == null)
			throw new ArgumentNullException(nameof(query));
		if (options == null)
			throw new ArgumentNullException(nameof(options));

		return options.Execute(query);
	}
}