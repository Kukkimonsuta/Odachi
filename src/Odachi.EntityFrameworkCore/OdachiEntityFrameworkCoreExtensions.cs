using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Odachi.EntityFrameworkCore.Internal;

namespace Odachi.EntityFrameworkCore;

public static class OdachiEntityFrameworkCoreExtensions
{
    public static DbContextOptionsBuilder UseProjectTranslation(this DbContextOptionsBuilder optionsBuilder)
    {
        return optionsBuilder.AddInterceptors(new ProjectQueryExpressionInterceptor());
    }

    public static DbContextOptionsBuilder<TContext> UseProjectTranslation<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder)
        where TContext : DbContext
    {
        return (DbContextOptionsBuilder<TContext>)UseProjectTranslation((DbContextOptionsBuilder)optionsBuilder);
    }

    public static TResult Project<TSource, TResult>(this DbFunctions functions, TSource? source, Expression<Func<TSource, TResult>> selector)
    {
        throw new NotImplementedException();
    }
    public static TResult Project<TSource1, TSource2, TResult>(this DbFunctions functions, TSource1? source1, TSource2? source2, Expression<Func<TSource1, TSource2, TResult>> selector)
    {
        throw new NotImplementedException();
    }
    public static TResult Project<TSource1, TSource2, TSource3, TResult>(this DbFunctions functions, TSource1? source1, TSource2? source2, TSource3? source3, Expression<Func<TSource1, TSource2, TSource3, TResult>> selector)
    {
        throw new NotImplementedException();
    }
    public static TResult Project<TSource1, TSource2, TSource3, TSource4, TResult>(this DbFunctions functions, TSource1? source1, TSource2? source2, TSource3? source3, TSource4? source4, Expression<Func<TSource1, TSource2, TSource3, TSource4, TResult>> selector)
    {
        throw new NotImplementedException();
    }
    public static TResult Project<TSource1, TSource2, TSource3, TSource4, TSource5, TResult>(this DbFunctions functions, TSource1? source1, TSource2? source2, TSource3? source3, TSource4? source4, TSource5? source5, Expression<Func<TSource1, TSource2, TSource3, TSource4, TSource5, TResult>> selector)
    {
        throw new NotImplementedException();
    }
    public static TResult Project<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TResult>(this DbFunctions functions, TSource1? source1, TSource2? source2, TSource3? source3, TSource4? source4, TSource5? source5, TSource6? source6, Expression<Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TResult>> selector)
    {
        throw new NotImplementedException();
    }
    public static TResult Project<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TResult>(this DbFunctions functions, TSource1? source1, TSource2? source2, TSource3? source3, TSource4? source4, TSource5? source5, TSource6? source6, TSource7? source7, Expression<Func<TSource1, TSource2, TSource3, TSource4, TSource5, TSource6, TSource7, TResult>> selector)
    {
        throw new NotImplementedException();
    }
}
