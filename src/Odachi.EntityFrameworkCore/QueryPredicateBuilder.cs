using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Odachi.EntityFrameworkCore;

public class QueryPredicateBuilder<T>
{
	private readonly ParameterExpression _parameter = Expression.Parameter(typeof(T), "x");
	private Expression? _body = null;

    [MemberNotNullWhen(false, nameof(_body))]
    public bool IsEmpty => _body == null;

    public QueryPredicateBuilder(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate != null)
        {
            And(predicate);
        }
    }

    private unsafe void Add(Expression<Func<T, bool>> predicate, delegate*<Expression, Expression, BinaryExpression> combiner)
    {
        var visitor = new ReplaceExpressionVisitor(predicate.Parameters[0], _parameter);

        if (IsEmpty)
        {
            _body = visitor.Visit(predicate.Body) ?? throw new InvalidOperationException("Failed to replace parameter");
        }
        else
        {
            _body = combiner(_body, visitor.Visit(predicate.Body) ?? throw new InvalidOperationException("Failed to replace parameter"));
        }
    }

    public QueryPredicateBuilder<T> Or(Expression<Func<T, bool>> predicate)
	{
        ArgumentNullException.ThrowIfNull(predicate);

        unsafe
        {
            Add(predicate, &Expression.OrElse);
        }

        return this;
	}
    public QueryPredicateBuilder<T> Or(QueryPredicateBuilder<T> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (!builder.IsEmpty)
        {
            Or(builder.ToExpression());
        }

        return this;
    }
    public QueryPredicateBuilder<T> Or(Action<QueryPredicateBuilder<T>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var filter = new QueryPredicateBuilder<T>();

        builder(filter);

        Or(filter);

        return this;
    }

    public QueryPredicateBuilder<T> And(Expression<Func<T, bool>> predicate)
	{
        ArgumentNullException.ThrowIfNull(predicate);

        unsafe
        {
            Add(predicate, &Expression.AndAlso);
        }

        return this;
	}
    public QueryPredicateBuilder<T> And(QueryPredicateBuilder<T> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (!builder.IsEmpty)
        {
            And(builder.ToExpression());
        }

        return this;
    }
    public QueryPredicateBuilder<T> And(Action<QueryPredicateBuilder<T>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var filter = new QueryPredicateBuilder<T>();

        builder(filter);

        And(filter);

        return this;
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Cannot build expression from empty predicate builder");
        }

        return Expression.Lambda<Func<T, bool>>(_body, _parameter);
    }

	#region Nested type: ReplaceExpressionVisitor

	private class ReplaceExpressionVisitor(Expression oldValue, Expression newValue) : ExpressionVisitor
    {
        public override Expression? Visit(Expression? node)
		{
			return node == oldValue ? newValue : base.Visit(node);
		}
	}

	#endregion
}
