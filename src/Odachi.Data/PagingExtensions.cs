using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Data
{
	public static class PagingExtensions
	{
		/// <summary>
		/// Selects only part of a collection.
		/// </summary>
		public static CollectionPage<TEntity> ToCollectionPage<TEntity>(this IQueryable<TEntity> query, PagingOptions options)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			return options.Execute(query);
		}

		/// <summary>
		/// Selects only part of a collection.
		/// </summary>
		public static Task<CollectionPage<TEntity>> ToCollectionPageAsync<TEntity>(this IQueryable<TEntity> query, PagingOptions options)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			return options.ExecuteAsync(query);
		}


		/// <summary>
		/// Selects only part of a collection.
		/// </summary>
		public static CollectionPage<TEntity> ToCollectionPage<TEntity>(this IEnumerable<TEntity> resource, PagingOptions options)
		{
			if (resource == null)
				throw new ArgumentNullException(nameof(resource));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			return options.Execute(resource);
		}
	}
}
