using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace AutoSearchEntities.PredicateSearchProvider.Models
{
   public class DateTimeFromToFilter
    {
        [Required][NotNull] public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
