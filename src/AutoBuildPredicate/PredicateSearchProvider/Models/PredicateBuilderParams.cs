using System.Linq.Expressions;

namespace AutoSearchEntities.PredicateSearchProvider.Models
{
    public class PredicateBuilderParams
    {
        public MemberExpression PropertyOrField { get; set; }
    }
}