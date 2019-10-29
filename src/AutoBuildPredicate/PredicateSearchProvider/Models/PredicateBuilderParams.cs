using System.Linq.Expressions;

namespace AutoBuildPredicate.PredicateSearchProvider.Models
{
    public class PredicateBuilderParams
    {
        public MemberExpression PropertyOrField { get; set; }
    }
}