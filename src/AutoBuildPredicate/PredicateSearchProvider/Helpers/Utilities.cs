﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;
using AutoBuildPredicate.PredicateSearchProvider.Models;

namespace AutoBuildPredicate.PredicateSearchProvider.Helpers
{
   internal static class Utilities
    {
        internal static (ExpressionDateTimeInfo dateFromExprInfo, ExpressionDateTimeInfo dateToExprInfo) BuildExpressionDateTimeInfo(DateTimeFromToFilterComplex dateTimeFromToFilter, bool nullable)
        {
            var dateTimeValueDateFrom = dateTimeFromToFilter.DateFrom;
            var dateTimeValueDateTo = dateTimeFromToFilter.DateTo;
            var fromDate = dateTimeValueDateFrom?.DateTime;
            DateTime? toDate = default;
            var fromDateExprType =
                dateTimeValueDateFrom?.ExpressionType ?? CompareExpressionType.GreaterThanOrEqual;
            CompareExpressionType toDateExprType = default;


            if (dateTimeValueDateTo != null)
            {
                toDateExprType = dateTimeValueDateTo.ExpressionType ?? CompareExpressionType.LessThanOrEqual;
                toDate = dateTimeValueDateTo.DateTime;
            }


            ExpressionDateTimeInfo dateToExprInfo = default;
            ConstantExpression fromDateExpressionConstant;
            if (dateTimeFromToFilter.TruncateTime)
            {
                fromDateExpressionConstant = Expression.Constant(fromDate.Value.Date, typeof(DateTime));
            }
            else
            {
                fromDateExpressionConstant = nullable
                    ? Expression.Constant(fromDate, typeof(DateTime?))
                    : Expression.Constant((DateTime)fromDate, typeof(DateTime));
            }

            if (toDate.HasValue)
            {
                ConstantExpression toDateExpressionConstant;
                if (dateTimeFromToFilter.TruncateTime)
                {
                    toDateExpressionConstant = Expression.Constant(toDate.Value.Date, typeof(DateTime));
                }
                else
                {
                    toDateExpressionConstant = nullable
                        ? Expression.Constant(toDate, typeof(DateTime?))
                        : Expression.Constant((DateTime)toDate, typeof(DateTime));
                }

                dateToExprInfo = new ExpressionDateTimeInfo { Constant = toDateExpressionConstant, ExpressionType = toDateExprType, DateTime = toDate };

                //                        (rightToDate, dateToExprType);
            }

            else
            {
                dateToExprInfo = new ExpressionDateTimeInfo();
            }

            var dateFromExprInfo = new ExpressionDateTimeInfo { Constant = fromDateExpressionConstant, ExpressionType = fromDateExprType, DateTime = fromDate };

            return (dateFromExprInfo, dateToExprInfo);
        }
    
        internal static (ExpressionDateTimeInfo dateFromExprInfo, ExpressionDateTimeInfo dateToExprInfo) BuildExpressionDateTimeInfo(DateTimeFromToFilter dateTimeFromToFilter, bool nullable)
        {
            // var dateTimeValueDateFrom = dateTimeFromToFilter.DateFrom;
            // var dateTimeValueDateTo = dateTimeFromToFilter.DateTo;
            var fromDate = dateTimeFromToFilter.DateFrom;
            var toDate = dateTimeFromToFilter.DateTo;
            const CompareExpressionType fromDateExprType = CompareExpressionType.GreaterThanOrEqual;
            CompareExpressionType toDateExprType = default;


            if (toDate != null)
            {
                toDateExprType = CompareExpressionType.LessThanOrEqual;
                toDate = dateTimeFromToFilter.DateTo;
            }


            ExpressionDateTimeInfo dateToExprInfo;


            var fromDateExpressionConstant = nullable
                ? Expression.Constant(fromDate, typeof(DateTime?))
                : Expression.Constant((DateTime)fromDate, typeof(DateTime));
            

            if (toDate.HasValue)
            {
                var toDateExpressionConstant = nullable
                    ? Expression.Constant(toDate, typeof(DateTime?))
                    : Expression.Constant((DateTime)toDate, typeof(DateTime));


                dateToExprInfo = new ExpressionDateTimeInfo { Constant = toDateExpressionConstant, ExpressionType = toDateExprType, DateTime = toDate };
            }

            else
            {
                dateToExprInfo = new ExpressionDateTimeInfo();
            }

            var dateFromExprInfo = new ExpressionDateTimeInfo { Constant = fromDateExpressionConstant, ExpressionType = fromDateExprType, DateTime = fromDate };

            return (dateFromExprInfo, dateToExprInfo);
        }

        internal static string TryAddPath(string entityName, string path)
        {
            var name = entityName;
            if (!string.IsNullOrEmpty(path))
            {
                name = path + "@" + entityName;
            }

            return name;
        }

        internal static object GetInstanceOfNestedEntity(string path, string typeName)
        {
            string assemblyName = typeName.Split(".").First();

            var lastInPath = path.Split('@').Last();

            var qualifiedName =
                Assembly.CreateQualifiedName(assemblyName,
                    $"{typeName}.{lastInPath.Trim('@')}") ?? throw new InvalidOperationException(
                    $"QualifiedName does not find for given parameters: {nameof(typeName)}");


            var type = Type.GetType(qualifiedName);
            var inst = Activator.CreateInstance(
                type ?? throw new InvalidOperationException($"Could not create instance of {qualifiedName}"));

            return inst;
        }

    }
}
