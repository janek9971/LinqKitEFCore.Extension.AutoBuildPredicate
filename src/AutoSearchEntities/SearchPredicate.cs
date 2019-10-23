using System.Linq;
using System.Linq.Expressions;
using AutoSearchEntities.PredicateSearchProvider;
using LinqKit;

namespace AutoSearchEntities
{
    public class SearchPredicate<TEntity> where TEntity : class, new()
    {
        private ParameterExpression Item { get; }
//        private ExpressionStarter<TEntity> Predicate { get; set; }
        public SearchPredicate()
        {
            Item = Expression.Parameter(typeof(TEntity), "entity");

        }
        public ExpressionStarter<TEntity> SearchByFilterPredicateProvidedByCustomExpressions<TU>(TU filter = default) 
            where TU : class, ICustomExpressions<TEntity>
        {
            var prodProgVerPredicate =
                PredicateBuilderMapping<TEntity>.PredicateCore(filter, Item);

            var expressions = PredicateBuilderMapping<TEntity>.GetCustomExpressions(filter, Item);
            if (!expressions.Any()) return prodProgVerPredicate;
            foreach (var filterExpression in expressions)
                prodProgVerPredicate = prodProgVerPredicate.And(filterExpression);

            return prodProgVerPredicate;
        }
        public ExpressionStarter<TEntity> SearchByFilterPredicate<TU>(TU filter = default) where TU : class
        {

            var prodProgVerPredicate =
                PredicateBuilderMapping<TEntity>.PredicateCore(filter, Item);

            return prodProgVerPredicate;
        }
    }
}
