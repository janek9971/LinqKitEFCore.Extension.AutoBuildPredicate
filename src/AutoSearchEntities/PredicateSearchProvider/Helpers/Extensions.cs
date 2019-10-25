using System;
using System.Linq.Expressions;
using AutoSearchEntities.PredicateSearchProvider.Models;
using JetBrains.Annotations;

namespace AutoSearchEntities.PredicateSearchProvider.Helpers
{

    internal static class Extensions
    {
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(Nullable.GetUnderlyingType(o.GetType())))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static TEnum ConvertByName<TEnum>(this Enum source, bool ignoreCase = false) where TEnum : struct
        {
            // if limited by lack of generic enum constraint
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("enumeration type required.");
            }

            TEnum result;
            if (!Enum.TryParse(source.ToString(), ignoreCase, out result))
            {
                throw new Exception("conversion failure.");
            }

            return result;
        }
    }
    internal static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> LambdaExpressionBuilder<T>(this Expression binaryExpression, ParameterExpression parameterExpression)
        {
            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameterExpression);
        }
        public static Expression GreaterLessThanBuilderExpressions(this MemberExpression leftExpression,
            (Expression expr, CompareExpressionType exprType) rightFromDate,
            [CanBeNull] (Expression expr, CompareExpressionType exprType) rightToDate)
        {
            var (expression, compareExpressionType) = rightFromDate;
            var greaterThanOrEqualBody = Expression.MakeBinary(compareExpressionType.ConvertByName<ExpressionType>(),
                leftExpression,
                expression);

//            Expression.GreaterThanOrEqual(leftExpression, rightFromDate.expr);

            var (expr, exprType) = rightToDate;
            if (expr == null) return greaterThanOrEqualBody;
            var lessThanOrEqualBody = Expression.MakeBinary(exprType.ConvertByName<ExpressionType>(),
                leftExpression,
                expr);
            ;

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
            }


            return propertyOrField;
        }
    }
    internal static class TypeExtensions
    {
        public static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null;

        public static bool HasProperty(this Type obj, Type propertyType, string propertyName)
        {
            var propertyOfObj = obj.GetProperty(propertyName.ToUpper().Replace("_", string.Empty));

            if (propertyOfObj == null)
            {
                return false;
            }


            var objType = Nullable.GetUnderlyingType(propertyOfObj.PropertyType) ?? propertyOfObj.PropertyType;

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
