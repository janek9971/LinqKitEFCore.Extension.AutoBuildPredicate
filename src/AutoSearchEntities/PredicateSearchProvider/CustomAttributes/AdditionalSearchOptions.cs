using System;

#pragma warning disable 1591

namespace AutoSearchEntities.PredicateSearchProvider.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field )]
    public class AdditionalSearchOptions : FieldAttributeBase
    {
//        private string _searchPath;
        public string EntityName { get; set; }

//        public CustomSearchPath CustomSearchPath { get; set; }
        //        public Type[] NestedTypes { get; set; } 
        //       public string SearchPath
        //        {
        //            get => _searchPath;
        //            set => _searchPath = value + ".";
        //        }
    
        public StringMethods StringSearchType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CustomSearchPath : FieldAttributeBase
    {
        public CustomSearchPath(string searchPath, string assemblyName, string typeName)
        {
            SearchPath = searchPath;
            AssemblyName = assemblyName;
            TypeName = typeName;
        }
        private string _searchPath;

        public string SearchPath
        {
            get => _searchPath;
            private set => _searchPath = value + ".";
        }
        public string AssemblyName { get; }
        public string TypeName { get;}

    }
    public enum StringMethods
    {
        Equals,
        Contains
    }
}