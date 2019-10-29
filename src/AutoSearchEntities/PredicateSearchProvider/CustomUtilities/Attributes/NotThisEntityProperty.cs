using System;
using System.ComponentModel;

namespace AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class NotThisEntityProperty : Attribute
    {
    }
}