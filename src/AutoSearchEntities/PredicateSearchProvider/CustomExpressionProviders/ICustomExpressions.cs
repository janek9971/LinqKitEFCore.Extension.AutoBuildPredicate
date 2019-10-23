#pragma warning disable 1591

namespace AutoSearchEntities.PredicateSearchProvider.CustomExpressionProviders
{
    public interface ICustomExpressions<TEntity> where TEntity : class
    {
         void Expressions(ModelExpressions<TEntity>.ExpressionsBuilder builder);
    }
}
