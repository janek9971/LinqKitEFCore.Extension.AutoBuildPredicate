using System;
using AutoBuildPredicate;
using AutoBuildPredicate.PredicateSearchProvider.CustomUtilities.Enums;
using AutoBuildPredicate.PredicateSearchProvider.Models;
using JetBrains.Annotations;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var predicateEngine = new PredicateEngine<Product>();
            var test = new ProductFilter()
            {
                PackingDay = new DateTimeFromToFilter()
                {
                    DateFrom = new DateTimeValue()
                    {
                        DateTime = DateTime.Now,
                        ExpressionType = CompareExpressionType.LessThanOrEqual
                    },
                    DateTo = new DateTimeValue()
                    {
                        DateTime = null
                        // ExpressionType = CompareExpressionType.GreaterThanOrEqual
                    },
                    TruncateTime = false
                },
                // Test = new DateTimeFromToFilter()
                // {
                //     DateTo = new DateTimeValue()
                //     {
                //         DateTime = DateTime.Today,
                //
                //     }
                // }
            };
            var myExpressionStarter = predicateEngine.PredicateByFilter(test);
        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCategory { get; set; }
        public decimal ProductPrice { get; set; }
        public double ProductWeight { get; set; }
        public int Units { get; set; }
        public DateTime? PackingDay { get; set; }

    }

    public class ProductFilter
    {
        public NumericFilter<int?> ProductID { get; set; }
        public StringFilter ProductCategory { get; set; }
        public NumericFilter<decimal?> ProductPrice { get; set; }
        public NumericFilter<double?> ProductWeight { get; set; }
        public int? Units { get; set; }
        public DateTimeFromToFilter PackingDay { get; set; }
    }
}
