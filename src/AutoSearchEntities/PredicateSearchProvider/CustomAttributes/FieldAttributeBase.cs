using System;
using System.ComponentModel;

namespace AutoSearchEntities.PredicateSearchProvider.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class FieldAttributeBase : Attribute
    {
    }
}