using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Odachi.Data
{
	/// <summary>
	/// Holds information about sorting
	/// </summary>
	public abstract class SortOptions<TEntity>
	{
		internal SortOptions()
		{ }

		internal abstract IOrderedQueryable<TEntity> Execute(IQueryable<TEntity> query);
		internal abstract IOrderedEnumerable<TEntity> Execute(IEnumerable<TEntity> query);

		public SortOptions<TEntity> ThenBy<TKey>(Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			return new SortOptions<TEntity, TKey>(this, expression, descending);
		}

		#region Static members

		public static SortOptions<TEntity> Create<TKey>(Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			return new SortOptions<TEntity, TKey>(expression, descending);
		}

		#endregion
	}

	/// <summary>
	/// Holds information about sorting
	/// </summary>
	internal sealed class SortOptions<TEntity, TKey> : SortOptions<TEntity>
	{
		internal SortOptions(Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));

			_expression = expression;
			_descending = descending;
		}
		internal SortOptions(SortOptions<TEntity> parent, Expression<Func<TEntity, TKey>> expression, bool descending = false)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));

			_parent = parent;
			_expression = expression;
			_descending = descending;
		}

		private SortOptions<TEntity> _parent;
		private Expression<Func<TEntity, TKey>> _expression;
		private bool _descending;

		internal override IOrderedQueryable<TEntity> Execute(IQueryable<TEntity> query)
		{
			if (_parent != null)
			{
				var result = _parent.Execute(query);

				return _descending ? result.ThenByDescending(_expression) : result.ThenBy(_expression);
			}
			else
				return _descending ? query.OrderByDescending(_expression) : query.OrderBy(_expression);
		}

		internal override IOrderedEnumerable<TEntity> Execute(IEnumerable<TEntity> query)
		{
			var compiled = _expression.Compile();

			if (_parent != null)
			{
				var result = _parent.Execute(query);

				return _descending ? result.ThenByDescending(compiled) : result.ThenBy(compiled);
			}
			else
				return _descending ? query.OrderByDescending(compiled) : query.OrderBy(compiled);
		}
	}
}
