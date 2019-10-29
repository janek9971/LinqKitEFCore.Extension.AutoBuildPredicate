using System;
using System.ComponentModel;

namespace AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class PredicateBitwiseOperation : Attribute
    {
        public BitwiseOperation Operation { get; set; } = BitwiseOperation.And;
    }
    public enum BitwiseOperation
    {
        And,
        Or
    }

}
