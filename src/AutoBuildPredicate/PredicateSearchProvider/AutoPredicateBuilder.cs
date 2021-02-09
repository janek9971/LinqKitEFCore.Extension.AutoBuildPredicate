using System;
using System.Linq;
using System.Linq.Expressions;
using AutoBuildPredicate.PredicateSearchProvider.Helpers;
using LinqKit;

namespace AutoBuildPredicate.PredicateSearchProvider
{
    internal partial class AutoPredicateBuilder<TEntity> where TEntity : class, new()
    {

   
        internal AutoPredicateBuilder(string assemblyName)
        {
            AssemblyName = assemblyName;
            Item = Expression.Parameter(typeof(TEntity), "entity");
            AutoPredicate = PredicateBuilder.New<TEntity>(true);
        }
        private string AssemblyName { get; }
        internal ExpressionStarter<TEntity> AutoPredicate { get; set; }
        private ParameterExpression Item { get; }
        private object FilterPropertyValue { get; set; }
        private MemberExpression PropertyOrField { get; set; }
        private string EntityName { get; set; }
        private Type FilterPropertyType { get; set; }
        private Type EntityPropertyType { get; set; }


        internal void PredicateCore<TU>(TU filter) where TU : class

        {
            var propertyInfoByPropName = ComputeProperties(filter);
            if (!propertyInfoByPropName.Any()) return;
            {
                foreach (var (key, value) in propertyInfoByPropName)
                {
                    if (!value.InstanceTypeOfProperty.HasProperty(key.PropertyType, value.EntityName, out var entityType))
                        throw new ArgumentException(
                            $"Given property with this type = {key.PropertyType} does not exist for instance: {value.InstanceTypeOfProperty.Name} and type: {entityType}\r\nDid you forget to add NotThisEntityPropertyAttribute or passed wrong type?",
                            nameof(key.Name)); 
                }

                PredicateBuilderByExpressions(propertyInfoByPropName);
            }
        }


    }
}