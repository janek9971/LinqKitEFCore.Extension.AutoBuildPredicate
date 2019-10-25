using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoSearchEntities.PredicateSearchProvider.Helpers;
using JetBrains.Annotations;

#pragma warning disable 1591

namespace AutoSearchEntities.PredicateSearchProvider.CustomExpressionProviders
{
    public class ModelExpressions<TEntity> where TEntity : class
    {
        private readonly List<Expression<Func<TEntity, bool>>> _expressions;

        private ModelExpressions(List<Expression<Func<TEntity, bool>>> expressions)
        {
            _expressions = expressions;
        }

        public List<Expression<Func<TEntity, bool>>> GetExpressions()
        {
            return _expressions;
        }
        public class ExpressionsBuilder
        {
            private readonly List<Expression<Func<TEntity, bool>>> _expressions = new List<Expression<Func<TEntity, bool>>>();
            private readonly ParameterExpression _item;

            public ExpressionsBuilder(ParameterExpression item)
            {
                _item = item;
            }

            public ExpressionsBuilder AddExpression(Expression<Func<TEntity, bool>> expression)
            {

                if (expression == null) 
                {
                    throw new ArgumentNullException(nameof(expression), "Expression must have value");
                }
                _expressions.Add(expression);
                return this;

            }

            public void AddExpressionWhenPropNullOrEmpty(Expression<Func<TEntity, object>> leftExpr, [CanBeNull] object value,
                ExpressionType type, object objectWhenNull)
            {
                switch (objectWhenNull)
                {
                    case string strNull:
                    {
                        if (string.IsNullOrEmpty(strNull))
                        {
                            AddExpression(leftExpr, value, type);
                        }

                        break;
                    }

                    case null:
                        AddExpression(leftExpr, value, type);
                        break;
                }
            }
            public void AddExpressionWhenPropNullOrEmpty(Expression<Func<TEntity, bool>> leftExpr, [CanBeNull] object propWhenNull)
            {
                switch (propWhenNull)
                {
                    case string strNull:
                    {
                        if (string.IsNullOrEmpty(strNull))
                        {
                            AddExpression(leftExpr);
                        }

                        break;
                    }

                    case null:
                        AddExpression(leftExpr);
                        break;
                }
            }

            public ExpressionsBuilder AddExpression(Expression<Func<TEntity, object>> leftExpr, object value, ExpressionType type)
            {
                var constant = Expression.Constant(value);

                var leftVisitor = new ReplaceExpressionVisitor(leftExpr.Parameters[0], _item);
                var leftExprBody = leftVisitor.Visit(leftExpr.Body);
                var binaryExpression = Expression.MakeBinary(type, leftExprBody, constant);
                var lambda = binaryExpression.LambdaExpressionBuilder<TEntity>(_item);

                _expressions.Add(lambda);

                return this;
            }
            public ModelExpressions<TEntity> Build()
            {
                return new ModelExpressions<TEntity>(_expressions);
            }
        }
    }
  
}