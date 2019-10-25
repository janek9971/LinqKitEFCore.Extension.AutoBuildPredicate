using System;

namespace AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotEntityPropertyAttribute : FieldAttributeBase
    {
    }
}