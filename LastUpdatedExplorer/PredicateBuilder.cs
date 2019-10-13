using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LastUpdatedExplorer
{
    public static class PredicateBuilder
    {
        public static Expression<Predicate<T>> And<T>(this Expression<Predicate<T>> a, Expression<Predicate<T>> b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            ParameterExpression parameter = a.Parameters[0];

            SubstParameterExpressionVisitor visitor = new SubstParameterExpressionVisitor();
            visitor[b.Parameters[0]] = parameter;

            Expression combinedBody = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Predicate<T>>(combinedBody, parameter);
        }

        public static Expression<Predicate<T>> Or<T>(this Expression<Predicate<T>> a, Expression<Predicate<T>> b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            ParameterExpression parameter = a.Parameters[0];

            SubstParameterExpressionVisitor visitor = new SubstParameterExpressionVisitor();
            visitor[b.Parameters[0]] = parameter;

            Expression combinedBody = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Predicate<T>>(combinedBody, parameter);
        }

        public static Expression<Predicate<T>> OrIfNotNull<T>(this Expression<Predicate<T>> a, Expression<Predicate<T>> b) => 
            a == null ? b : a.Or(b);
    }
}
