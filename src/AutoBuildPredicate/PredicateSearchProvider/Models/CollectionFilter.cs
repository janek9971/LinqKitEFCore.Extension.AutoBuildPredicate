using System.Collections.Generic;
using JetBrains.Annotations;

namespace AutoBuildPredicate.PredicateSearchProvider.Models
{
   public class CollectionFilter<T>
    {
        [UsedImplicitly]
        public List<T> Collection { get; set; }
    }

   internal enum Delegates
   {
       ContainsEqualityComparer,
   }
}
