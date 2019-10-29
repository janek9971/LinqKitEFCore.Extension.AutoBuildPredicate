using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoSearchEntities.PredicateSearchProvider.CustomUtilities.Attributes;
using AutoSearchEntities.PredicateSearchProvider.Models;
using JetBrains.Annotations;
using LinqKit;

namespace AutoSearchEntities.PredicateSearchProvider.Helpers
{
    internal static class EnumExtensions
    {
        public static TEnum ConvertByName<TEnum>(this Enum source, bool ignoreCase = false) where TEnum : struct
        {
            // if limited by lack of generic enum constraint
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("enumeration type required.");
            }

            if (!Enum.TryParse(source.ToString(), ignoreCase, out TEnum result))
            {
                throw new Exception("conversion failure.");
            }

            return result;
        }
    }

    internal static class CollectionExtensions
    {
        public static Func<T, bool> ContainsMethod<T>(this ICollection<T> collection, Delegates type)
        {
            Func<T, bool> del = collection.Contains<T>;
            return del;
        }
    }

    internal static class ExpressionExtensions
    {
        public static ExpressionStarter<T> PredicateByOperationType<T>(this ExpressionStarter<T> predicate,
            BitwiseOperation operation,
            Expression<Func<T, bool>> expr)
        {
            switch (operation)
            {
                case BitwiseOperation.And:
                    predicate = predicate.And(expr);
                    break;
                case BitwiseOperation.Or:
                    predicate = predicate.Or(expr);
                    break;
            }

            return predicate;
        }

        public static Expression ChooseExpressionType(this Expression expr1,
            BitwiseOperationExpressions operationExpressions,
            Expression expr2 = null)
        {
            Expression binaryExpression;
            if (expr2 != null)
            {
                switch (operationExpressions)
                {
                    case BitwiseOperationExpressions.And:
                        binaryExpression = Expression.And(expr1, expr2);
                        break;
                    case BitwiseOperationExpressions.Or:
                        binaryExpression = Expression.Or(expr1, expr2);
                        break;
                    case BitwiseOperationExpressions.AndAlso:
                        binaryExpression = Expression.AndAlso(expr1, expr2);
                        break;
                    case BitwiseOperationExpressions.OrElse:
                        binaryExpression = Expression.AndAlso(expr1, expr2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(operationExpressions), operationExpressions, null);
                }
            }
            else
            {
                binaryExpression = expr1;
            }

            return binaryExpression;
        }

        public static Expression<Func<T, bool>> LambdaExpressionBuilder<T>(this Expression expr1,
            ParameterExpression parameterExpression, BinaryExpression expr2 = null,
            BitwiseOperationExpressions bitwiseOperationExpressions = BitwiseOperationExpressions.AndAlso)
        {
            Expression binaryExpression = expr1.ChooseExpressionType(bitwiseOperationExpressions, expr2);

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameterExpression);
        }

        public static Expression GreaterLessThanBuilderExpressions(this MemberExpression leftExpression,
            (Expression expr, CompareExpressionType? exprType) rightFromDate,
            (Expression expr, CompareExpressionType? exprType) rightToDate,
            BitwiseOperationExpressions bitwiseOperationExpressions)
        {
            var (expression, compareExpressionType) = rightFromDate;
            var greaterThanOrEqualBody = Expression.MakeBinary(compareExpressionType.ConvertByName<ExpressionType>(),
                leftExpression,
                expression);

            var (expr, exprType) = rightToDate;
            if (expr == null) return greaterThanOrEqualBody;
            var lessThanOrEqualBody = Expression.MakeBinary(exprType.ConvertByName<ExpressionType>(),
                leftExpression,
                expr);

            return greaterThanOrEqualBody.ChooseExpressionType(bitwiseOperationExpressions, lessThanOrEqualBody);
        }
        public static MemberExpression GetPropertyOrField(this ParameterExpression item, string key)
        {
            MemberExpression propertyOrField = null;

            Expression current = item;

            foreach (var part in key.Split('@'))
            {
                //NotAMemberOfType
                try
                {
                    propertyOrField = Expression.PropertyOrField(current, part.ToUpper());
                    current = propertyOrField;
                }
                catch (ArgumentException)
                {
                    var message = FormattableString.Invariant(
                        $"{part} is not a member of type {current.Type}\r\nDid you enter the property name correctly?");
                    throw new ArgumentException(message);
                }
            }


            return propertyOrField;
        }
    }



    internal static class TypeExtensions
    {
        public static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null;

        public static bool IsGenericList(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType)
                {
                    if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        return true;
                    }

                    else if (@interface.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        return true;
                    }

                    else if (@interface.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
                    {
                        return true;
                    }

                    else if (@interface.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
                    {
                        return true;
                    }
                }
            }

            return false;
            //            return (type.GetInterface("IEnumerable") != null);
        }

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
            else if (propertyType == typeof(StringFilter))
            {
                propertyType = typeof(string);
            }
            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(NumericFilter<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType.GetGenericArguments().First()) ??
                               propertyType;
            }
            else if (propertyType.IsGenericList())
            {
                var genericArguments = propertyType.GetGenericArguments();
                if (genericArguments.Length > 1)
                {
                    throw new Exception("Only one generic argument is possible");
                }

                propertyType = genericArguments.First();
            }
            else
            {
                propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            }


            return objType == propertyType;
        }


        public static bool IsNumericType(this Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (typeCode)
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
    }

}