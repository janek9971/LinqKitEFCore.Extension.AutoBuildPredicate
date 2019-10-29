using System;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;

namespace AutoBuildPredicate.PredicateSearchProvider.Models
{
   public class StringFilter
   {
       public string Str { get; set; }
       public StringSearchOption StringSearchOption { get; set; } = StringSearchOption.Equals;
       public StringComparison? StringComparison { get; set; } = null;
   }


}
