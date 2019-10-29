﻿using System;
using System.ComponentModel;

namespace AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field )]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AdditionalSearchOptions : Attribute
    {
        public string EntityPropertyName { get; set; }
    }
}