using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

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
