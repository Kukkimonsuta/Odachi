using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Odachi.Data;

/// <summary>
/// Holds information about sorting
/// </summary>
public abstract class SortOptions<TEntity>
{
	public SortOptions()
	{ }

	public abstract IOrderedQueryable<TEntity> Execute(IQueryable<TEntity> query);
	public abstract IOrderedEnumerable<TEntity> Execute(IEnumerable<TEntity> query);

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
internal class SortOptions<TEntity, TKey> : SortOptions<TEntity>
{
	public SortOptions(Expression<Func<TEntity, TKey>> expression, bool descending = false)
	{
		if (expression == null)
			throw new ArgumentNullException(nameof(expression));

		_expression = expression;
		Descending = descending;
	}
	public SortOptions(SortOptions<TEntity> parent, Expression<Func<TEntity, TKey>> expression, bool descending = false)
	{
		if (expression == null)
			throw new ArgumentNullException(nameof(expression));

		_parent = parent;
		_expression = expression;
		Descending = descending;
	}

	private SortOptions<TEntity>? _parent;
	private Expression<Func<TEntity, TKey>> _expression;
	private bool Descending { get; set; }

	public override IOrderedQueryable<TEntity> Execute(IQueryable<TEntity> query)
	{
		if (_parent != null)
		{
			var result = _parent.Execute(query);

			return Descending ? result.ThenByDescending(_expression) : result.ThenBy(_expression);
		}
		else
			return Descending ? query.OrderByDescending(_expression) : query.OrderBy(_expression);
	}

	public override IOrderedEnumerable<TEntity> Execute(IEnumerable<TEntity> query)
	{
		var compiled = _expression.Compile();

		if (_parent != null)
		{
			var result = _parent.Execute(query);

			return Descending ? result.ThenByDescending(compiled) : result.ThenBy(compiled);
		}
		else
			return Descending ? query.OrderByDescending(compiled) : query.OrderBy(compiled);
	}
}