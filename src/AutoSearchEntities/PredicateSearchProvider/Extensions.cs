using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace AutoSearchEntities.PredicateSearchProvider
{
    internal static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> LambdaExpressionBuilder<T>(this Expression binaryExpression, ParameterExpression parameterExpression)
        {
            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameterExpression);
        }
        public static Expression GreaterLessThanBuilderExpressions(this MemberExpression leftExpression,
            Expression rightFromDate,
            [CanBeNull] Expression rightToDate)
        {
            var greaterThanOrEqualBody = Expression.GreaterThanOrEqual(leftExpression, rightFromDate);

            if (rightToDate == null) return greaterThanOrEqualBody;
            var lessThanOrEqualBody = Expression.LessThanOrEqual(leftExpression, rightToDate);

            return Expression.AndAlso(greaterThanOrEqualBody, lessThanOrEqualBody);
        }
    }

    internal static class StringExtensions
    {
       
        public static MemberExpression GetPropertyOrField(this string key, ParameterExpression item)
        {
            MemberExpression propertyOrField = null;

            Expression current = item;

            foreach (var part in key.Split('.'))
            {
                propertyOrField = Expression.PropertyOrField(current, part.ToUpper());
                // subsequent parts go to selector itself: "x.Parameter.AnotherParameter"
                current = propertyOrField;
            }


            return propertyOrField;
        }
    }
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null;

        public static bool HasProperty(this Type obj, Type propertyType, string propertyName)
            //            PropertyInfo propertyInfo)
        {
            var propertyOfObj = obj.GetProperty(propertyName.ToUpper().Replace("_", string.Empty));

            if (propertyOfObj == null)
            {
                return false;
            }


            var objType = Nullable.GetUnderlyingType(propertyOfObj.PropertyType) ?? propertyOfObj.PropertyType;
            //            Type propertyType;

            if (propertyType == typeof(DateTimeFromToFilter))
            {
                propertyType = typeof(DateTime);
            }
            else
            {
                propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            }


            return objType == propertyType;
        }
    }
}
