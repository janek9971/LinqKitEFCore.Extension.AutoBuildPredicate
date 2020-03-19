using System;
using System.ComponentModel.DataAnnotations;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;
using JetBrains.Annotations;

namespace AutoBuildPredicate.PredicateSearchProvider.Models
{
    public class DateTimeFromToFilter
    {
        [Required] [NotNull] public DateTime? DateFrom { get; set; } 
        public DateTime? DateTo { get; set; }
        public bool TruncateTime { get; set; } = true;
    }
 
}