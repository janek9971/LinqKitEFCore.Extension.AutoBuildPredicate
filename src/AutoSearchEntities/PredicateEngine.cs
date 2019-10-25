using System.Linq;
using System.Linq.Expressions;
using AutoSearchEntities.PredicateSearchProvider;
using AutoSearchEntities.PredicateSearchProvider.CustomExpressionProviders;
using LinqKit;

namespace AutoSearchEntities
{
    public class PredicateEngine<TEntity> where TEntity : class, new()
    {
        private ParameterExpression Item { get; }
//        private ExpressionStarter<TEntity> Predicate { get; set; }
        public PredicateEngine()
        {
            Item = Expression.Parameter(typeof(TEntity), "entity");

        }
        public ExpressionStarter<TEntity> PredicateProvidedByCustomExpressions<TU>(TU filter = default) 
            where TU : class, ICustomExpressions<TEntity>
        {
            var predicateCore =
                PredicateBuilderMapping<TEntity>.PredicateCore(filter, Item);

            var expressions = PredicateBuilderMapping<TEntity>.GetCustomExpressions(filter, Item);

            if (!expressions.Any()) return predicateCore;
            foreach (var filterExpression in expressions)
                predicateCore = predicateCore.And(filterExpression);

            return predicateCore;
        }
        public ExpressionStarter<TEntity> SimplePredicate<TU>(TU filter = default) where TU : class
        {

            var predicateCore =
                PredicateBuilderMapping<TEntity>.PredicateCore(filter, Item);

            return predicateCore;
        }
    }
}
