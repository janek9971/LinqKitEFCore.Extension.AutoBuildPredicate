using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AutoSearchEntities.PredicateSearchProvider.Models;

namespace AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Attributes
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
