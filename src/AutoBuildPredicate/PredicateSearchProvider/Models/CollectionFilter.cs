using System.Collections.Generic;

namespace AutoBuildPredicate.PredicateSearchProvider.Models
{
   public class CollectionFilter<T>
    {
        public List<T> Collection { get; set; }
    }

   internal enum Delegates
   {
       ContainsEqualityComparer,
       ContainsIndexOf,
   }
}
