using System.Linq.Expressions;

namespace Odachi.EntityFrameworkCore.Internal;

public class ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    : ExpressionVisitor
{
    public override Expression? Visit(Expression? node)
    {
        return node == oldValue ? newValue : base.Visit(node);
    }
}
