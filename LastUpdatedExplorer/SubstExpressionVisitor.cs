using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LastUpdatedExplorer
{
    internal class SubstExpressionVisitor : ExpressionVisitor
    {
        private readonly Dictionary<Expression, Expression> _subst = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_subst.TryGetValue(node, out Expression newValue))
            {
                return newValue;
            }

            return node;
        }

        public Expression this[Expression original]
        {
            get => _subst[original];
            set => _subst[original] = value;
        }
    }
}
