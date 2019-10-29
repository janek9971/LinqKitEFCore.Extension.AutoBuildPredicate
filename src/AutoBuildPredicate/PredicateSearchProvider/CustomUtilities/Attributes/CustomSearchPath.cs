using System;
using System.ComponentModel;

#pragma warning disable 1591

namespace AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CustomSearchPath : Attribute
    {
        public CustomSearchPath(string[] searchPath)
        {
            SearchPath = string.Join("@", searchPath);
//            AssemblyName = assemblyName;
//            TypeName = typeName;
        }
        public CustomSearchPath(string[] searchPath,string assemblyName, string typeName)
        {
            SearchPath = string.Join("@", searchPath);
            AssemblyName = assemblyName;
            TypeName = typeName;
        }

        public string SearchPath { get; }

        public string AssemblyName { get;  }
        public string TypeName { get;  }

    }
}