using System;
using System.ComponentModel;

namespace AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class FieldAttributeBase : Attribute
    {
    }
}