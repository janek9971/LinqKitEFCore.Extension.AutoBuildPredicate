using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;
using AutoBuildPredicate.PredicateSearchProvider.Helpers;
using AutoBuildPredicate.PredicateSearchProvider.Models;
using static AutoBuildPredicate.PredicateSearchProvider.Helpers.Utilities;
using CollectionExtensions = AutoBuildPredicate.PredicateSearchProvider.Helpers.CollectionExtensions;

namespace AutoBuildPredicate.PredicateSearchProvider
{
    internal partial class AutoPredicateBuilder<TEntity>
    {
        private void PredicateBuilderByExpressions(
            IDictionary<PropertyInfo, SearchPredicatePropertyInfo> propertyInfoByPropName)
        {
            foreach (var (prop, searchPredicatePropertyInfo) in propertyInfoByPropName)
            {
                PropertyOrField = searchPredicatePropertyInfo.PropertyOrField;
                EntityName = searchPredicatePropertyInfo.EntityName;
                FilterPropertyValue = searchPredicatePropertyInfo.PropertyValue;
                FilterPropertyType = prop.PropertyType;
                EntityPropertyType = searchPredicatePropertyInfo.InstanceTypeOfProperty
                    .GetProperty(EntityName.ToUpper(),
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.PropertyType;
                var predicateBitwiseOperation = searchPredicatePropertyInfo.PredicateBitwiseOperation;

                if (FilterPropertyValue is string _)
                {
                    var expression = StringContainsExpr();

                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }
                else if (FilterPropertyValue is StringFilter stringFilter)
                {
                    var expression = StringFilterContainsExpr(stringFilter);


                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }
                else if (FilterPropertyValue is DateTimeFromToFilterComplex dateTimeFromToFilterComplex)
                {
                    if (dateTimeFromToFilterComplex.DateFrom != null && !dateTimeFromToFilterComplex.DateFrom.DateTime.HasValue)
                        throw new ArgumentNullException(nameof(dateTimeFromToFilterComplex.DateFrom));

                    var expression = DateTimeExpr(dateTimeFromToFilterComplex, Item);

                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }
                else if (FilterPropertyValue is DateTimeFromToFilter dateTimeFromToFilter)
                {
                    if (!dateTimeFromToFilter.DateFrom.HasValue)
                        throw new ArgumentNullException(nameof(dateTimeFromToFilter.DateFrom));

                    var expression = DateTimeExpr(dateTimeFromToFilter, Item);

                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }
                else if (FilterPropertyValue is DateTimeFromToTruncTimeFilter dateTimeFromToTruncTimeFilter)
                {
                    if (!dateTimeFromToTruncTimeFilter.DateFrom.HasValue)
                        throw new ArgumentNullException(nameof(dateTimeFromToFilter.DateFrom));

                    var expression = DateTimeExpr(dateTimeFromToTruncTimeFilter, Item);

                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }
                else if (FilterPropertyValue.GetType().IsGenericType &&
                         FilterPropertyValue.GetType().GetGenericTypeDefinition() == typeof(NumericFilter<>))
                {
                    var expression = NumericFilterExpr();

                    if(expression==null) continue;

                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }
                else if (!searchPredicatePropertyInfo.IsEntityTypeProperty)
                {
                    var expression = CollectionContainsExpr();

                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }

                else
                {
                    var expression = DefaultExpr();

                    AutoPredicate.PredicateByOperationType(predicateBitwiseOperation, expression);
                }
            }
        }

        private Expression<Func<TEntity, bool>> DefaultExpr()
        {
            var right = Expression.Constant(FilterPropertyValue);
            BinaryExpression binaryExpression;
            if (EntityPropertyType.IsNullable())
            {
                var leftUnaryExpression = Expression.Convert(PropertyOrField, FilterPropertyValue.GetType());

                binaryExpression = Expression.Equal(leftUnaryExpression, right);
            }
            else
            {
                binaryExpression = Expression.Equal(PropertyOrField, right);
            }

            var lambda = binaryExpression.LambdaExpressionBuilder<TEntity>(Item);
            return lambda;
        }

        private Expression<Func<TEntity, bool>> CollectionContainsExpr()
        {
            var genericType = FilterPropertyType.GetGenericArguments().First();
            var methodInfo = typeof(CollectionExtensions)
                .GetMethod(nameof(CollectionExtensions.ContainsMethodDelegate),
                    BindingFlags.Public | BindingFlags.Static);

            var valueToEquals = Expression.Constant(FilterPropertyValue);

            Type[] genericArguments = {genericType};
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
            Delegate @delegate = (Delegate) genericMethodInfo.Invoke(null,
                new[] {FilterPropertyValue});

            var methodCallExpression = Expression.Call(null, @delegate.Method, valueToEquals, PropertyOrField);

            var lambda = methodCallExpression.LambdaExpressionBuilder<TEntity>(Item);

            return lambda;
        }

        private Expression<Func<TEntity, bool>> StringContainsExpr()
        {
            var equalsMethodInfo =
                typeof(string).GetMethod(StringSearchOption.Equals.ToString(),
                    new[] {typeof(string)});
            var valueToEquals = Expression.Constant(FilterPropertyValue.ToString().ToLower());

            var call = Expression.Call(PropertyOrField, "ToLower", null);
            var methodCallExpression = Expression.Call(call, equalsMethodInfo, valueToEquals);
            var lambda = methodCallExpression.LambdaExpressionBuilder<TEntity>(Item);

            return lambda;
        }

        private Expression<Func<TEntity, bool>> NumericFilterExpr()
        {
            #region NumericFilter

            var numericFilter = FilterPropertyValue.GetType().GetProperties();
            var value1 = numericFilter.FirstOrDefault()?.GetValue(FilterPropertyValue);
            var value2 = numericFilter.LastOrDefault()?.GetValue(FilterPropertyValue);

            #endregion

            BinaryExpression expr1 = null;

            if (value1 != null)
            {
                #region NumericValue

                var numericValues1 = value1.GetType().GetProperties();
                var numericValue1 = numericValues1.First().GetValue(value1);
                var exprTypeValue1 = (CompareExpressionType)numericValues1.Last().GetValue(value1);

                #endregion

                var value1ExprConst = Expression.Constant(numericValue1);
                expr1 = Expression.MakeBinary(exprTypeValue1.ConvertByName<ExpressionType>(),
                    PropertyOrField,
                    value1ExprConst);
            }

            BinaryExpression expr2 = null;
            if (value2 != null)
            {
                #region NumericValue

                var numericValues2 = value2.GetType().GetProperties();
                var numericValue2 = numericValues2.First().GetValue(value2);
                var exprTypeValue2 =
                    (CompareExpressionType)numericValues2.Last().GetValue(value2);

                #endregion

                var value2ExprConst = Expression.Constant(numericValue2);
                expr2 = Expression.MakeBinary(exprTypeValue2.ConvertByName<ExpressionType>(),
                    PropertyOrField,
                    value2ExprConst);
            }
            Expression<Func<TEntity, bool>> lambda;

            switch (expr1)
            {
                case null when expr2 is null:
                    return null;
                case null:
                    lambda = expr2.LambdaExpressionBuilder<TEntity>(Item);
                    break;
                default:
                    lambda = expr1.LambdaExpressionBuilder<TEntity>(Item, expr2);
                    break;
            }

            return lambda;
        }

        private Expression<Func<TEntity, bool>> StringFilterContainsExpr(StringFilter stringFilter)
        {

            var methodName =
                stringFilter.StringSearchOption.ToString();
            var valueToEquals = Expression.Constant(stringFilter.Str.ToLower());


                var methodInfo = typeof(string).GetMethod(methodName, new[] {typeof(string)});
                var call = Expression.Call(PropertyOrField, "ToLower", null);
                var methodCallExpression = Expression.Call(call, methodInfo, valueToEquals);

            var lambda = methodCallExpression.LambdaExpressionBuilder<TEntity>(Item);
            return lambda;
        }

        private Expression<Func<TEntity, bool>> DateTimeExpr(DateTimeFromToFilterComplex dateTimeFromToFilter,
            ParameterExpression item)
        {
            Expression<Func<TEntity, bool>> lambdaExpr;


            if (EntityPropertyType.IsNullable())
            {
                var (fromDateExpressionInfo, toDateExpressionInfo) =
                    BuildExpressionDateTimeInfo(dateTimeFromToFilter, true);


                MemberExpression memberExpression = dateTimeFromToFilter.TruncateTime
                    ? Expression.Property(Expression.Property(PropertyOrField, "Value"), "Date")
                    : PropertyOrField;

                var ifTrue = memberExpression
                    .GreaterLessThanBuilderExpressions(fromDateExpressionInfo, toDateExpressionInfo,
                        BitwiseOperationExpressions.AndAlso);

              
                var nullOrGreaterLess = Expression.AndAlso(
             Expression.Property(PropertyOrField, "HasValue"),
                    ifTrue);


                lambdaExpr = nullOrGreaterLess.LambdaExpressionBuilder<TEntity>(item);
            }
            else
            {
                var (dateFromExprInfo, dateToExprInfo) = BuildExpressionDateTimeInfo(dateTimeFromToFilter, false);

                var entityPropTruncated = dateTimeFromToFilter.TruncateTime
                    ? Expression.Property(PropertyOrField, "Date")
                    : PropertyOrField;

                var dateTimeExpr =
                    entityPropTruncated.GreaterLessThanBuilderExpressions(dateFromExprInfo, dateToExprInfo,
                        BitwiseOperationExpressions.AndAlso);

                lambdaExpr = dateTimeExpr.LambdaExpressionBuilder<TEntity>(item);
            }

            return lambdaExpr;
        }



        private Expression<Func<TEntity, bool>> DateTimeExpr(DateTimeFromToTruncTimeFilter dateTimeFromToTruncTimeFilter,
            ParameterExpression item)
        {
            Expression<Func<TEntity, bool>> lambdaExpr;


            if (EntityPropertyType.IsNullable())
            {
                var (fromDateExpressionInfo, toDateExpressionInfo) =
                    BuildExpressionDateTimeInfo(dateTimeFromToTruncTimeFilter, true);


                MemberExpression memberExpression =
                    Expression.Property(Expression.Property(PropertyOrField, "Value"), "Date");
              

                var ifTrue = memberExpression
                    .GreaterLessThanBuilderExpressions(fromDateExpressionInfo, toDateExpressionInfo,
                        BitwiseOperationExpressions.AndAlso);
                var nullOrGreaterLess = Expression.AndAlso(
                    Expression.Property(PropertyOrField, "HasValue"),
                    ifTrue);

                lambdaExpr = nullOrGreaterLess.LambdaExpressionBuilder<TEntity>(item);
            }
            else
            {
                var (dateFromExprInfo, dateToExprInfo) = BuildExpressionDateTimeInfo(dateTimeFromToTruncTimeFilter, false);

                var entityPropTruncated = Expression.Property(PropertyOrField, "Date");

                var dateTimeExpr =
                    entityPropTruncated.GreaterLessThanBuilderExpressions(dateFromExprInfo, dateToExprInfo,
                        BitwiseOperationExpressions.AndAlso);

                lambdaExpr = dateTimeExpr.LambdaExpressionBuilder<TEntity>(item);
            }

            return lambdaExpr;
        }

        private Expression<Func<TEntity, bool>> DateTimeExpr(DateTimeFromToFilter dateTimeFromToFilter,
          ParameterExpression item)
        {
            Expression<Func<TEntity, bool>> lambdaExpr;


            if (EntityPropertyType.IsNullable())
            {
                var (fromDateExpressionInfo, toDateExpressionInfo) =
                    BuildExpressionDateTimeInfo(dateTimeFromToFilter, true);

                var ifTrue = PropertyOrField
                    .GreaterLessThanBuilderExpressions(fromDateExpressionInfo, toDateExpressionInfo,
                        BitwiseOperationExpressions.AndAlso);
                var nullOrGreaterLess = Expression.AndAlso(
                    Expression.Property(PropertyOrField, "HasValue"),
                    ifTrue);

                lambdaExpr = nullOrGreaterLess.LambdaExpressionBuilder<TEntity>(item);
            }
            else
            {
                var (dateFromExprInfo, dateToExprInfo) = BuildExpressionDateTimeInfo(dateTimeFromToFilter, false);


                var dateTimeExpr =
                    PropertyOrField.GreaterLessThanBuilderExpressions(dateFromExprInfo, dateToExprInfo,
                        BitwiseOperationExpressions.AndAlso);

                lambdaExpr = dateTimeExpr.LambdaExpressionBuilder<TEntity>(item);
            }

            return lambdaExpr;
        }
    }
}