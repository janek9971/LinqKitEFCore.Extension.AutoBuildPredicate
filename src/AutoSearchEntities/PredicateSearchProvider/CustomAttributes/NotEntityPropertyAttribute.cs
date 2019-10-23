using System;

namespace AutoSearchEntities.PredicateSearchProvider.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotEntityPropertyAttribute : FieldAttributeBase
    {
    }
}