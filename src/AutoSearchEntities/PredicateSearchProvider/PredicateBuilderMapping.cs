using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoSearchEntities.PredicateSearchProvider.CustomExpressionProviders;
using AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Attributes;
using AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Enums;
using AutoSearchEntities.PredicateSearchProvider.Helpers;
using AutoSearchEntities.PredicateSearchProvider.Models;
using LinqKit;

// ReSharper disable AssignNullToNotNullAttribute

#pragma warning disable 1591

namespace AutoSearchEntities.PredicateSearchProvider
{
    internal static class PredicateBuilderMapping<TEntity> where TEntity : class, new()
    {
        //        private const string AssemblyName = "EntityModel";
        //        private const string TypeNamePrototype = "EntityModel.AntModels";

        internal static ExpressionStarter<TEntity> PredicateCore<TU>(TU filter, ParameterExpression item)
            where TU : class
        {
            var predicate = PredicateBuilder.New<TEntity>(true);


            var propertyInfoByPropName = ComputeProperties(filter, item);
            if (!propertyInfoByPropName.Any()) return predicate;
            {
                var temp = new Dictionary<PropertyInfo, SearchPredicatePropertyInfo>(propertyInfoByPropName);
                foreach (var (key, value) in temp)
                {

                    if (!value.InstanceTypeOfProperty.HasProperty(key.PropertyType, value.EntityName))
                        throw new ArgumentException($"Given property does not exist for instance: {value.InstanceTypeOfProperty.Name}", nameof(key.Name));//TODO TEST
//                        propertyInfoByPropName.Remove(key); 
                }

                predicate.And(PredicateBuilderByExpressions(propertyInfoByPropName, item));
            }
            return predicate;
        }

        internal static List<Expression<Func<TEntity, bool>>> GetCustomExpressions<TU>(TU filter,
            ParameterExpression item) where TU : class, ICustomExpressions<TEntity>
        {
            var expressionsBuilder = new ModelExpressions<TEntity>.ExpressionsBuilder(item);
            filter?.Expressions(expressionsBuilder);

            return expressionsBuilder.Build().GetExpressions();
        }

//        public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        private static Expression<Func<TEntity, bool>> PredicateBuilderByExpressions(
            IDictionary<PropertyInfo, SearchPredicatePropertyInfo> propertyInfoByPropName, ParameterExpression item)

        {
            var predicate = PredicateBuilder.New<TEntity>(true);

            //  foreach (var (propertyInfo, searchPredicatePropertyInfo) in propertyInfoByPropName)
            foreach (var (_, searchPredicatePropertyInfo) in propertyInfoByPropName)
            {
                var propertyOrField = searchPredicatePropertyInfo.PredicateBuilderParams.PropertyOrField;
//                var propType = propertyInfo.PropertyType;
                var entityName = searchPredicatePropertyInfo.EntityName;
                var propertyValue = searchPredicatePropertyInfo.PropertyValue;

                var propertyType = new TEntity().GetType().GetProperty(entityName.ToUpper())?.PropertyType;


                if (propertyValue is string _)
                {
//                    var methodName =
//                        searchPredicatePropertyInfo.PredicateBuilderParams.StringSearchMethod.ToString();

                    var equalsMethodInfo =
                        typeof(string).GetMethod(StringSearchOption.Equals.ToString(), new[] {typeof(string), typeof(StringComparison)});
                    var valueToEquals = Expression.Constant(propertyValue);
                    var comparisonType = Expression.Constant(StringComparison.InvariantCultureIgnoreCase);

                    var methodCallExpression = Expression.Call(propertyOrField, equalsMethodInfo, valueToEquals,
                        comparisonType);

                    predicate = predicate.And(Expression.Lambda<Func<TEntity, bool>>(methodCallExpression, item));
                }
                else if (propertyValue is StringFilter stringFilter)
                {
                    var methodName =
                        stringFilter.StringSearchOption.ToString();
                    var equalsMethodInfo =
                        typeof(string).GetMethod(methodName, new[] { typeof(string), typeof(StringComparison) });
                    var valueToEquals = Expression.Constant(stringFilter.Str);
                    var comparisonType = Expression.Constant(stringFilter.StringComparison);

                    var methodCallExpression = Expression.Call(propertyOrField, equalsMethodInfo, valueToEquals,
                        comparisonType);

                    predicate = predicate.And(Expression.Lambda<Func<TEntity, bool>>(methodCallExpression, item));
                }
                else if (propertyValue is DateTimeFromToFilter properties)
                {
//                    var propertiesDateTime = propertyValue.GetType().GetProperties();

                    var dateTimeValueDateFrom = properties.DateFrom;
//                        (DateTimeValue) propertiesDateTime[0].GetValue(propertyValue);
                    var dateTimeValueDateTo = properties.DateTo;



                    var fromDate = dateTimeValueDateFrom.DateTime;
                    var toDate = dateTimeValueDateTo.DateTime;

                    var rightFromDate = Expression.Constant(fromDate.Value.Date);
                    ConstantExpression rightToDate = null;

                    if (toDate.HasValue)
                        rightToDate = Expression.Constant(toDate.Value.Date);


                    var dateFromInfoTuple = (rightFromDate, dateTimeValueDateFrom.ExpressionType);
                    var dateToInfoTuple = (rightToDate, dateTimeValueDateTo.ExpressionType);
                    if (propertyType.IsNullable())

                    {
                        ConditionalExpression LeftConditionalExpr()
                        {
                            var ifTrue = Expression.Property(Expression.Property(propertyOrField, "Value"), "Date")
                                .GreaterLessThanBuilderExpressions(dateFromInfoTuple, dateToInfoTuple);

                            var ifFalse = propertyOrField.GreaterLessThanBuilderExpressions(
                                (Expression.Constant(fromDate, typeof(DateTime?)), dateTimeValueDateFrom.ExpressionType),
                                (Expression.Constant(toDate, typeof(DateTime?)), dateTimeValueDateFrom.ExpressionType));

                            var conditionalExpression =
                                Expression.Condition(Expression.Property(propertyOrField, "HasValue"), ifTrue, ifFalse);

                            return conditionalExpression;
                        }

                        var lambdaExpr = LeftConditionalExpr().LambdaExpressionBuilder<TEntity>(item);

                        predicate = predicate.And(lambdaExpr);
                    }
                    else
                    {
                        var entityPropTruncated = Expression.Property(propertyOrField, "Date");


                        var dateTimeExpr =
                            entityPropTruncated.GreaterLessThanBuilderExpressions(dateFromInfoTuple, dateToInfoTuple);

                        var lambdaExpr = dateTimeExpr.LambdaExpressionBuilder<TEntity>(item);
                        predicate = predicate.And(lambdaExpr);
                    }
                }
                else if (propertyValue.GetType() == typeof(NumericFilter<>))
                {
                    var rangeProperties = propertyValue.GetType().GetProperties();
                    var lessThan =  rangeProperties[0].GetValue(propertyValue);
                    var greaterThan = rangeProperties[1].GetValue(propertyValue);

                    var lessThanValues = lessThan.GetType().GetProperties();
                    var lessThanValue = lessThanValues[0].GetValue(propertyValue);
                    var exprTypeLessThan = (CompareExpressionType)lessThanValues[1].GetValue(propertyValue);

                    var greaterThanValues = greaterThan.GetType().GetProperties();
                    var greaterThanValue = greaterThanValues[0].GetValue(propertyValue);
                    var exprTypeGreaterThan =
                        (CompareExpressionType)greaterThanValues[1].GetValue(propertyValue);

                    var lessThanExprConst = Expression.Constant(lessThanValue);
                    var greaterThanExprConst = Expression.Constant(greaterThanValue);
                    //                            var left = Expression.GreaterThan(propertyOrField, greaterThanExprConst);

                    if (propertyOrField.GetType() != lessThanValue.GetType() ||
                        propertyOrField.GetType() != greaterThanValue.GetType())
                    {
                        throw new Exception("Wrong type");
                    }

                    var left = Expression.MakeBinary(exprTypeGreaterThan.ConvertByName<ExpressionType>(),
                        propertyOrField,
                        greaterThanExprConst);

                    var right = Expression.MakeBinary(exprTypeLessThan.ConvertByName<ExpressionType>(),
                        propertyOrField,
                        lessThanExprConst);
                    //                            var right = Expression.LessThan(propertyOrField, lessThanExprConst);


                    var and = Expression.AndAlso(left, right);
                    var lambda = and.LambdaExpressionBuilder<TEntity>(item);

                    predicate = predicate.And(lambda);
                }
                else
                {
                        var right = Expression.Constant(propertyValue);
                        if (propertyType.IsNullable())
                        {
                            var leftUnaryExpression = Expression.Convert(propertyOrField, propertyValue.GetType());

                            var equalBody = Expression.Equal(leftUnaryExpression, right);
//                                var lambdaExpr = Expression.Lambda<Func<TEntity, bool>>(equalBody, item);
                            predicate = predicate.And(equalBody.LambdaExpressionBuilder<TEntity>(item));
                        }
                        else
                        {
                            var equalBody = Expression.Equal(propertyOrField, right);
//                                var lambdaExpr = Expression.Lambda<Func<TEntity, bool>>(equalBody, item);
                            predicate = predicate.And(equalBody.LambdaExpressionBuilder<TEntity>(item));
                        }
                    
                }
            }

            return predicate;
        }

        private static IDictionary<PropertyInfo, SearchPredicatePropertyInfo> ComputeProperties<TU>(TU filter,
            ParameterExpression item) where TU : class
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
                if (Attribute.IsDefined(property, typeof(NotEntityPropertyAttribute))) continue;

                if (property?.GetValue(filter) == null) continue;


                var propertyName = property.Name.Replace("_", string.Empty);
                var entityName = propertyName;

                var searchPredicatePropertyInfo =
                    new SearchPredicatePropertyInfo {PredicateBuilderParams = new PredicateBuilderParams()};
                object inst = new TEntity();

                string path=string.Empty;
                if (Attribute.IsDefined(property, typeof(CustomSearchPath)))
                {
                    var fieldPathSearchAttribute = property.GetCustomAttribute<CustomSearchPath>();
                    path = fieldPathSearchAttribute.SearchPath;

                    if (!string.IsNullOrEmpty(path))
                    {
                        var entityModelName = fieldPathSearchAttribute.AssemblyName;
                        var qualifiedName =
                            Assembly.CreateQualifiedName(entityModelName,
                                $"{entityModelName}.{fieldPathSearchAttribute.TypeName}.{path.Trim('.')}");

                        var type = Type.GetType(qualifiedName);
                        inst = Activator.CreateInstance(type);
                    }
                    
                }

                if (Attribute.IsDefined(property, typeof(AdditionalSearchOptions)))
                {
                    var fieldPathSearchAttribute = property.GetCustomAttribute<AdditionalSearchOptions>();

                    entityName = fieldPathSearchAttribute.EntityPropertyName ?? propertyName;

                    var name = path + entityName;
                    searchPredicatePropertyInfo.PredicateBuilderParams.PropertyOrField = name.GetPropertyOrField(item);
//                    searchPredicatePropertyInfo.PredicateBuilderParams.StringSearchMethod =
//                        fieldPathSearchAttribute.StringSearchType;
                }
                else
                {
                    searchPredicatePropertyInfo.PredicateBuilderParams.PropertyOrField =
                        entityName.GetPropertyOrField(item);
//                    searchPredicatePropertyInfo.PredicateBuilderParams.StringSearchMethod =
//                        StringMethods.Equals;
                }

                searchPredicatePropertyInfo.EntityName = entityName;
                searchPredicatePropertyInfo.InstanceTypeOfProperty = inst.GetType();


                var value = property.GetValue(filter, null);

                switch (value)
                {
                    //TODO TO CHECK
//                    case string _ when value.Equals(string.Empty):  
                    case ICollection<TU> collection when !collection.Any():
                    case IEnumerable<TU> enumerable when !enumerable.Any():
                    case DateTime _ when value.Equals(default(DateTime)):
                        continue;
//                    case DateTimeFromToFilter _:
//                    {
//                        searchPredicatePropertyInfo.PropertyValue = value;
//                        propertyInfoByPropName.Add(property, searchPredicatePropertyInfo);
//                        break;
//                    }

                    default:
                        searchPredicatePropertyInfo.PropertyValue = value;
                        propertyInfoByPropName.Add(property, searchPredicatePropertyInfo);
                        break;
                }
            }

            return propertyInfoByPropName;
        }


    }
}