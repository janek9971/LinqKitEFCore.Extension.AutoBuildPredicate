# LinqKitEFCore.Extension.AutoBuildPredicate

This .NET core extension is free and easy to use to search entities much easier, without null checking and building big expression trees.

Library contains:
* SearchPredicateEngine,
* Attributes,
* Custom model classes



# How it works?

To use, you just have to instantiate the engine:

```c#

var predicateEngine = new PredicateEngine<TEntity>();

var myExpressionStarter = predicateEngine.PredicateByFilter(filterModel)

IQueryable<YourReturnType> = context.TEntity.Where(myExpressionStarter);

```

Before doing it you have to define class "filterModel"

The simplest way is just put properties with the same names as in an entity class. 
It is important that all properties are nullable types, otherwise if you have integer property and you does not initialize, the library will add an expression to predicate where property equals to zero.
In filter class you can use all Built-in types, for DateTime you have to use library class "DateTimeFromToFilter"

### Example:

EntityClass

```c#
   public class Product
    {
        public int ProductID { get; set; }
        public string ProductCategory { get; set; }
        public decimal ProductPrice { get; set; }
        public double ProductWeight { get; set; }
        public int Units { get; set; }
    }
```

FilterClass

```c#
   public class ProductFilter
    {
        public int? ProductID { get; set; }
        public string ProductCategory { get; set; }
        public decimal? ProductPrice { get; set; }
        public double? ProductWeight { get; set; }
        public int? Units { get; set; }
    }
```

If you pass empty filter, it will return all objects in entity of type Product.
Providing the property value will be compared to the entity property by operator **"=="** for primitives and **"Equals"** for string.

Of course you can pass anonymous class instead of creating filterClass but I strongly adviste to use it to a small number of filtering properties.

</br>

## Extend filter class by built-in "Custom model classes" and "Custom attributes"

### Custom model classes:

* #### StringFilter<br />
     With StringFilter you can pass StringSearchOption(Equals, Contains, StartsWith, EndsWith), where default value is "Equals" and
StringComparison where default value is none

* #### NumericFilter<br />
    With NumericFilter you can pass generic Value1 (Required) or/and  generic Value2 and set CompareExpressionType(Equal, GreaterThan...).
This values have to be nullable type!
* #### DateTimeFromToFilter<br />
    With DateTimeFromToFilter you can pass **DateFrom** (Required) or/and  **DateTo** and set CompareExpressionType(Equal, GreaterThan...).
* CollectionFilter<br />
    This class accepts generic List, it is used to build predicate : 
    ```c#
    entity => Collection.Contains(entity.Property) 
    ```
### Custom attributes:
 * #### EntityPropertyName<br />
     This attribute overrides filter class property name if you can't use explicitly same name as in entity class.
 
 * #### CustomSearchPath<br />
    In this attribute you can pass a nested searchPath, but only for one-to-one entities. This option require to pass AssemblyName and TypeName of nested entity to CustomSearchPathAttribute or by constructor of PredicateEngine: 
 ```c# 
    var predicateEngine = new PredicateEngine<TEntity>("myAssemblyName", "myTypeName");
 ```   
   **AssemblyName** and **TypeName** setted by CustomSearchPathAttribute will always be overriden by passing them via constructor but only for that property where attribute was set. 
   <br />

* #### NotThisEntityProperty<br />
   This attribute is used when in filter class want to use property which is not a member of entity passed to engine. You can use it when path to that property is complicated so you can't use CustomSearchPath. Then you can define expression like this:
```c#
var myExpressionStarter = predicateEngine.PredicateByFilter(filterModel);
myExpressionStarter = myExpressionStarter.And(entity=>entity.<yourCustomExpr>)
```

* #### PredicateBitwiseOperation<br />
   This attribute sets the bitwise operation for predicate builder. By using it you can define how each property will be combined in predicate. **And** and **Or** shall be construed respectively as **AndALso** and **OrElse** bitwise operators. Default value is set to **And**.


## Examples with custom utilities

### #1 Example

```c#
   public class Product
    {
        public int ProductID { get; set; }
        public string ProductCategory { get; set; }
        public decimal ProductPrice { get; set; }
        public double ProductWeight { get; set; }
        public int Units { get; set; }
        public DateTime PackingDay {get;set;}
    }
```
```c#
 public class ProductFilter
    {
        public NumericFilter<int?> ProductID { get; set; }
        public StringFilter ProductCategory { get; set; }
        public NumericFilter<decimal?> ProductPrice { get; set; }
        public NumericFilter<double?> ProductWeight { get; set; }
        public int? Units { get; set; }
        public DateTimeFromToFilter PackingDay { get; set; }
     }
```
   ##### JSON for ProductFilter
```json
{
   "productID":{
      "value1":{
         "value":120,
         "expressionType":"GreaterThanOrEqual"
      },
      "value2":{
         "value":180,
         "expressionType":"LessThanOrEqual"
      }
   },
   "productCategory":{
      "str":"toy",
      "stringSearchOption":"Contains",
      "stringComparison":"InvariantCultureIgnoreCase"
   },
   "productPrice":{
      "value1":{
         "value":250,
         "expressionType":"Equal"
      },
      "value2":{
         "value":287,
         "expressionType":"Equal"
      }
   },
   "productWeight":{
      "value1":{
         "value":500,
         "expressionType":"GreaterThanOrEqual"
      }
   },
   "Units":20,
   "packingDay":{
      "dateFrom":{
         "dateTime":"2019-10-25T09:00:00",
         "expressionType":"GreaterThanOrEqual"
      },
      "dateTo":{
         "dateTime":"2019-10-25T15:00:00",
         "expressionType":"LessThanOrEqual"
      },
      "truncateTime":"false"
   }
}
```
   ##### It is equivalent to

```c#
{
	entity => (

			(
				(
					(		
						(
							((entity.ProductID >= 120) Or (entity.ProductID <= 180)) 
									
							AndAlso entity.ProductCategory.Contains("toy", InvariantCultureIgnoreCase)
						) 
						AndAlso ((entity.ProductPrice == 250) Or (entity.ProductPrice == 287))
					) 
					AndAlso (entity.ProductWeight >= 500)
				) 
			    AndAlso (entity.Units == 20)
			) 
		    AndAlso ((entity.PackingDay >= 25.10.2019 09:00:00) AndAlso (entity.PackingDay <= 25.10.2019 15:00:00))
	)
}
```  
<br />

_More examples with attributes coming soon._

<br />
<br />
This is my first public code as nupkg, so I am fully open to any suggestions and changes :)