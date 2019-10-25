using System;
using System.Linq.Expressions;
using AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Enums;

#pragma warning disable 1591

namespace AutoSearchEntities.PredicateSearchProvider.Models
{
    public class SearchPredicatePropertyInfo
    {
        public Type InstanceTypeOfProperty { get; set; }
        public object PropertyValue { get; set; }
        public string EntityName { get; set; }
        public PredicateBuilderParams PredicateBuilderParams { get; set; }
    }
    public class PredicateBuilderParams
    {
        public MemberExpression PropertyOrField { get; set; }
        public StringSearchOption StringSearchMethod { get; set; }
        public Expression Expression { get; set; }
    }
}
