using System;
using System.Linq.Expressions;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;

namespace AutoBuildPredicate.PredicateSearchProvider.Helpers
{
    public class ExpressionDateTimeInfo
    {
        public ConstantExpression Constant { get; set; }
        public CompareExpressionType? ExpressionType { get; set; }
        public DateTime? DateTime { get; set; }
    }
}