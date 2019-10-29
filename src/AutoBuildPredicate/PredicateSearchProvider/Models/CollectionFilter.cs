using System.Collections.Generic;

namespace AutoSearchEntities.PredicateSearchProvider.Models
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
