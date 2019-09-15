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

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Predicate<T>>(body, p);
        }

        public static Expression<Predicate<T>> Or<T>(this Expression<Predicate<T>> a, Expression<Predicate<T>> b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            if (b == null)
                throw new ArgumentNullException(nameof(b));

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Predicate<T>>(body, p);
        }

        public static Expression<Predicate<T>> OrIfNotNull<T>(this Expression<Predicate<T>> a, Expression<Predicate<T>> b) => 
            a == null ? b : a.Or(b);
    }
}
