using System;
using AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Enums;

namespace AutoSearchEntities.PredicateSearchProvider.Models
{
   public class StringFilter
   {
       public string Str { get; set; }
       public StringSearchOption StringSearchOption { get; set; } = StringSearchOption.Equals;
       public StringComparison StringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;
   }


}
