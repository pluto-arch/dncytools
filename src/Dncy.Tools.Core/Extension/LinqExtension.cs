using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotnetGeek.Tools
{
    public static partial class LinqExtension
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return left.CombineLambdas(right, ExpressionType.AndAlso);
        }


        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            return left.CombineLambdas(right, ExpressionType.OrElse);
        }



        private static Expression<Func<T, bool>> CombineLambdas<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, ExpressionType expressionType)
        {
            var visitor = new SubstituteParameterVisitor
            {
                Sub =
                {
                    [right.Parameters[0]] = left.Parameters[0]
                }
            };

            Expression body = Expression.MakeBinary(expressionType, left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, left.Parameters[0]);
        }

    }


    internal class SubstituteParameterVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Sub = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Sub.TryGetValue(node, out var newValue) ? newValue : node;
        }
    }
}
