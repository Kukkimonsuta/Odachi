using System;
using System.Linq;
using System.Linq.Expressions;

namespace Odachi.EntityFrameworkCore;

public static class QueryPredicateBuilderExtensions
{
    /// <summary>
    /// Filters a sequence of values based on a predicate constructed by predicate builder.
    /// </summary>
    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, QueryPredicateBuilder<T> predicateBuilder)
    {
        ArgumentNullException.ThrowIfNull(queryable);
        ArgumentNullException.ThrowIfNull(predicateBuilder);

        return predicateBuilder.IsEmpty ? queryable : queryable.Where(predicateBuilder.ToExpression());
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate constructed by predicate builder. This overload constructs
    /// predicate builder using provided builder.
    /// </summary>
    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, Action<QueryPredicateBuilder<T>> builder)
    {
        ArgumentNullException.ThrowIfNull(queryable);
        ArgumentNullException.ThrowIfNull(builder);

        var filter = new QueryPredicateBuilder<T>();

        builder(filter);

        return filter.IsEmpty ? queryable : queryable.Where(filter.ToExpression());
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate constructed by predicate builder. This overload constructs
    /// predicate builder using provided builder using provided predicate as a base.
    /// </summary>
    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate, Action<QueryPredicateBuilder<T>> builder)
    {
        ArgumentNullException.ThrowIfNull(queryable);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(builder);

        var filter = new QueryPredicateBuilder<T>(predicate);

        builder(filter);

        return filter.IsEmpty ? queryable : queryable.Where(filter.ToExpression());
    }
}
