using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;
using AutoBuildPredicate.PredicateSearchProvider.Helpers;
using JetBrains.Annotations;

namespace AutoBuildPredicate.PredicateSearchProvider.Models
{
    public class NumericFilter<T> : IEquatable<NumericFilter<T>>
    {
        [Required] public NumericValue<T> Value1 { get; set; }
        [CanBeNull] public NumericValue<T> Value2 { get; set; }

        public bool Equals(NumericFilter<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value1, other.Value1) && Equals(Value2, other.Value2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NumericFilter<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value1 != null ? Value1.GetHashCode() : 0) * 397) ^ (Value2 != null ? Value2.GetHashCode() : 0);
            }
        }

        public static bool operator ==(NumericFilter<T> left, NumericFilter<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NumericFilter<T> left, NumericFilter<T> right)
        {
            return !Equals(left, right);
        }
    }


    public class NumericValue<T> : IEquatable<NumericValue<T>>
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (!typeof(T).IsNullable())
                {
                    throw new Exception("Must be nullable type");
                }

                _value = (T)(object) value;
                if (_value.GetType().IsNumericType())
                {
                    _value = value;
                }
                else
                {
                    throw new Exception("Invalid type of range, must be numeric");
                }
            }
        }

        [Required] public CompareExpressionType ExpressionType { get; set; } = CompareExpressionType.Equal;


        public bool Equals(NumericValue<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(_value, other._value) && ExpressionType == other.ExpressionType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NumericValue<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(_value) * 397) ^ (int) ExpressionType;
            }
        }

        public static bool operator ==(NumericValue<T> left, NumericValue<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NumericValue<T> left, NumericValue<T> right)
        {
            return !Equals(left, right);
        }
    }
}
