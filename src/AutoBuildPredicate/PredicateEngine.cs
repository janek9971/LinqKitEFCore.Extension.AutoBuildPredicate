using AutoBuildPredicate.PredicateSearchProvider;
using JetBrains.Annotations;
using LinqKit;
using System.Reflection;

namespace AutoBuildPredicate
{
    [PublicAPI]
    public class PredicateEngine<TEntity> where TEntity : class, new()
    {
        //private readonly string _assemblyName;
        //private readonly string _typeName;
        //private readonly bool _assemblyDefined;
        //public PredicateEngine()
        //{
        //    //_assemblyDefined = false;
        //}
        //public PredicateEngine(string assemblyName, string typeName)
        //{
        //    //_assemblyName = assemblyName;
        //    //_typeName = typeName;
        //    //_assemblyDefined = true;
        //}
        public ExpressionStarter<TEntity> PredicateByFilter<TU>(TU filter = default) where TU : class
        {
            var typeFullName = typeof(TEntity).FullName;
            var assemblyName = typeFullName.Substring(0, typeFullName.LastIndexOf('.'));

            var core = new AutoPredicateBuilder<TEntity>(assemblyName);

            core.PredicateCore(filter);

            return core.AutoPredicate;
        }

   
    }
}
