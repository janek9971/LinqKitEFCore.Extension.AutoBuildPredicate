using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Attributes;
using AutoBuildPredicate.PredicateSearchProvider.Helpers;
using AutoBuildPredicate.PredicateSearchProvider.Models;
using PredicateBitwiseOperation =
    AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Attributes.PredicateBitwiseOperation;

namespace AutoBuildPredicate.PredicateSearchProvider
{
    internal partial class AutoPredicateBuilder<TEntity>
    {
        private IDictionary<PropertyInfo, SearchPredicatePropertyInfo> ComputeProperties<TU>(TU filter) where TU : class
        {
            PropertyInfo[] filterProperties;

            if (filter == null || filter.GetHashCode() == 0)
            {
                filterProperties = new PropertyInfo[0];
            }
            else
            {
                filterProperties = filter.GetType().GetProperties();
            }

            var propertyInfoByPropName =
                new Dictionary<PropertyInfo, SearchPredicatePropertyInfo>();

            foreach (var property in filterProperties)
            {
                if (property?.GetValue(filter) == null) continue;
                if (Attribute.IsDefined(property, typeof(NotThisEntityProperty))) continue;

                var propertyName = property.Name.Replace("_", string.Empty);

                var value = property.GetValue(filter, null);

                var propertyInfoForPredicate =
                    new SearchPredicatePropertyInfo();
                var entityName = propertyName;

                var isCustomSearchPath = Attribute.IsDefined(property, typeof(CustomSearchPath));
                var isAdditionalSearchOptions = Attribute.IsDefined(property, typeof(EntityPropertyName));
                var isBitwiseOperationDefined = Attribute.IsDefined(property, typeof(PredicateBitwiseOperation));
                if (property.PropertyType.IsGenericList() || property.PropertyType.IsArray)
                {
                    if (isCustomSearchPath)
                    {
                        throw new Exception($"Collections cannot have an attribute {nameof(CustomSearchPath)}");
                    }

                    if (!isAdditionalSearchOptions)
                    {
                        throw new Exception(
                            $"Collections name must be passed by an attribute {nameof(EntityPropertyName)}.{nameof(EntityPropertyName.Name)}");
                    }

                    propertyInfoForPredicate.IsEntityTypeProperty = false;
                }


                object inst = new TEntity();

                string path = string.Empty;

                if (isCustomSearchPath)
                {
                    var fieldPathSearchAttribute = property.GetCustomAttribute<CustomSearchPath>();
                    path = fieldPathSearchAttribute.SearchPath;

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!string.IsNullOrEmpty(fieldPathSearchAttribute.AssemblyName) &&
                            !string.IsNullOrEmpty(fieldPathSearchAttribute.TypeName))
                        {
                            inst = Inst(path, fieldPathSearchAttribute.AssemblyName, fieldPathSearchAttribute.TypeName);
                        }
                        else if (!string.IsNullOrEmpty(AssemblyName) && !string.IsNullOrEmpty(TypeName))
                        {
                            inst = Inst(path, AssemblyName, TypeName);
                        }
                        else
                        {
                            throw new ArgumentException(
                                FormattableString.Invariant(
                                    $"Params must be passed via constructor or directly to property by an attribute"),
                                FormattableString.Invariant($"{nameof(AssemblyName)} or {nameof(TypeName)}"));
                        }
                    }
                }

                if (isBitwiseOperationDefined)
                {
                    var operationAttribute = property.GetCustomAttribute<PredicateBitwiseOperation>();
                    propertyInfoForPredicate.PredicateBitwiseOperation = operationAttribute.Operation;
                }


                if (isAdditionalSearchOptions)
                {
                    var fieldPathSearchAttribute = property.GetCustomAttribute<EntityPropertyName>();

                    entityName = fieldPathSearchAttribute.Name ?? propertyName;

//                    var name = TryAddPath(entityName, path);
//                    propertyInfoForPredicate.PropertyOrField = Item.GetPropertyOrField(name);
                    //                    searchPredicatePropertyInfo.PredicateBuilderParams.StringSearchMethod =
                    //                        fieldPathSearchAttribute.StringSearchType;
                }
//                else
//                {
////                    var name = TryAddPath(entityName, path);
////                    propertyInfoForPredicate.PropertyOrField =Item.GetPropertyOrField(name);
//                }
                var name = TryAddPath(entityName, path);
                propertyInfoForPredicate.PropertyOrField = Item.GetPropertyOrField(name);

                propertyInfoForPredicate.EntityName = entityName;
                propertyInfoForPredicate.InstanceTypeOfProperty = inst.GetType();


                //                var value = property.GetValue(filter, null);

                switch (value)
                {
                    case DateTime _ :
                        throw new ArgumentException($"Pass date property by type: {nameof(DateTimeFromToFilter)}", entityName);
            
                    default:
                        propertyInfoForPredicate.PropertyValue = value;
                        propertyInfoByPropName.Add(property, propertyInfoForPredicate);
                        break;
                }
            }

            return propertyInfoByPropName;
        }

        private static string TryAddPath(string entityName, string path)
        {
            var name = entityName;
            if (!string.IsNullOrEmpty(path))
            {
                name = path + "@" + entityName;
            }

            return name;
        }

        private static object Inst(string path, string assemblyName, string typeName)
        {
            var entityModelName = assemblyName;

            var lastInPath = path.Split('@').Last();

            var qualifiedName =
                Assembly.CreateQualifiedName(entityModelName,
                    $"{entityModelName}.{typeName}.{lastInPath.Trim('@')}") ?? throw new InvalidOperationException(
                    $"QualifiedName does not find for given parameters: {nameof(entityModelName)} and {nameof(typeName)}");


            var type = Type.GetType(qualifiedName);
            var inst = Activator.CreateInstance(
                type ?? throw new InvalidOperationException($"Could not create instance of {qualifiedName}"));

            return inst;
        }
    }
}