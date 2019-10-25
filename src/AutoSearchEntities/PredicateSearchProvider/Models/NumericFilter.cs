using System;
using System.ComponentModel.DataAnnotations;
using AutoSearchEntities.PredicateSearchProvider.Helpers;

namespace AutoSearchEntities.PredicateSearchProvider.Models
{
    public class NumericFilter<T> where T : struct
    {
        [Required] public NumericValue<T> Value1 { get; set; }
        public NumericValue<T> Value2 { get; set; }
    }

    public class NumericValue<T> where T : struct
    {
        private T? _value;

        public T? Value
        {
            get => _value;
            set
            {
                if (value.IsNumericType())
                {
                    _value = value;
                }
                else
                {
                    throw new Exception("Invalid type of range, must be numeric");
                }
            }
        }
        public CompareExpressionType ExpressionType { get; set; }
    }

    public enum CompareExpressionType
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }
}
