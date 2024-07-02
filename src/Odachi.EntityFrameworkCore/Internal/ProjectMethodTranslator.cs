using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Odachi.EntityFrameworkCore.Internal;

public class ProjectQueryExpressionInterceptor : IQueryExpressionInterceptor
{
    public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
    {
        return ReplaceProjectCallExpressionVisitor.Instance.Visit(queryExpression);
    }

    #region Nested type: ReplaceProjectCallExpressionVisitor

    private class ReplaceProjectCallExpressionVisitor : ExpressionVisitor
    {
        private static readonly HashSet<MethodInfo> s_mapMethodInfos = typeof(OdachiEntityFrameworkCoreExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.Name == nameof(OdachiEntityFrameworkCoreExtensions.Project))
            .ToHashSet();

        public static readonly ReplaceProjectCallExpressionVisitor Instance = new();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod)
            {
                var methodDefinition = node.Method.GetGenericMethodDefinition();

                if (s_mapMethodInfos.Contains(methodDefinition))
                {
                    var expectedArgumentCount = node.Arguments.Count - 2;
                    var projectExpression = node.Arguments[^1];

                    switch (projectExpression)
                    {
                        case MemberExpression memberExpression:
                            switch (memberExpression.Member)
                            {
                                case FieldInfo { IsStatic: true } fieldInfo:
                                    var innerExpression = (LambdaExpression)fieldInfo.GetValue(null)!;
                                    projectExpression = innerExpression.Body;
                                    for (var i = 0; i < expectedArgumentCount; i++)
                                    {
                                        projectExpression = new ReplaceExpressionVisitor(innerExpression.Parameters[i], node.Arguments[1 + i]).Visit(projectExpression)!;
                                    }
                                    break;

                                default:
                                    throw new InvalidOperationException($"Cannot translate project member expression of type '{projectExpression.Type}'");
                            }

                            break;

                        case UnaryExpression { NodeType: ExpressionType.Quote } quoteExpression:
                            switch (quoteExpression.Operand)
                            {
                                case LambdaExpression lambdaExpression:
                                    projectExpression = lambdaExpression.Body;
                                    for (var i = 0; i < expectedArgumentCount; i++)
                                    {
                                        projectExpression = new ReplaceExpressionVisitor(lambdaExpression.Parameters[i], node.Arguments[i + 1]).Visit(projectExpression)!;
                                    }
                                    break;

                                default:
                                    throw new InvalidOperationException($"Cannot translate project member expression of type '{projectExpression.Type}'");
                            }

                            break;

                        default:
                            throw new InvalidOperationException($"Cannot translate project expression of type '{projectExpression.Type}'");
                    }

                    return Visit(projectExpression);
                }
            }

            return base.VisitMethodCall(node);
        }
    }

    #endregion
}
