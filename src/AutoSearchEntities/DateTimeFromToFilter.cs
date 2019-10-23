using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace AutoSearchEntities
{
   public class DateTimeFromToFilter
    {
        [Required][NotNull] public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
