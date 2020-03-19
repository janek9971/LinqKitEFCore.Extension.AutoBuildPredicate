using System;
using System.ComponentModel.DataAnnotations;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;
using JetBrains.Annotations;

namespace AutoBuildPredicate.PredicateSearchProvider.Models
{
    public class DateTimeFromToFilterComplex
    {
        [Required] [NotNull] public DateTimeValue DateFrom { get; set; } 
        public DateTimeValue DateTo { get; set; }
        public bool TruncateTime { get; set; } = true;
    }

    public class DateTimeValue
    {
        public DateTime? DateTime { get; set; }
        public CompareExpressionType? ExpressionType { get; set; } 
    }
}