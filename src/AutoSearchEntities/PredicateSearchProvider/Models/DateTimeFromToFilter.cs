using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace AutoSearchEntities.PredicateSearchProvider.Models
{
    public class DateTimeFromToFilter
    {
        [Required] [NotNull] public DateTimeValue DateFrom { get; set; }
        public DateTimeValue DateTo { get; set; }
    }

    public class DateTimeValue
    {
        public DateTime? DateTime { get; set; }
        public CompareExpressionType ExpressionType { get; set; }
    }
}