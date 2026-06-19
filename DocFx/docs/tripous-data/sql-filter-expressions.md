# SQL Filter Expressions

`SqlFilterExpressionDef` describes inline filter tags embedded inside a SELECT statement.
It is different from `SqlFilterDef`.

`SqlFilterDef` is structured metadata registered in code.
`SqlFilterExpressionDef` is for SELECT text written by a user or consultant, where the SQL itself declares which runtime filter values may be requested.

At execution time the SELECT text is inspected.
The tags can be used to create a dialog where the end user enters the actual filter values.

## Basic Idea

A consultant may write a SELECT like this:

```sql
select
    Id,
    Name,
    Amount
from Customer
where Name like '%[[string:Customer Name]]%'
  and Amount >= [[dec:Minimum Amount]]
```

The `[[...]]` parts are filter expressions.
They are not database parameters.
They are prompts embedded in the SELECT statement.

## Simple Types

Syntax:

```text
[[string:Label]] or just [[Label]]
[[int:Label]]
[[dec:Label]]
```

Examples:

```sql
Name = '[[string:Customer Name]]'
Name = '[[Customer Name]]'
Total = [[int:Min Items]]
Amount > [[dec:Min Amount]]
```

Decimal can be used with any numeric non-integer column, such as decimal, float, or double.

## Dates

Syntax:

```text
[[date:Custom:Label]]
[[date:Range]]
```

Examples:

```sql
Date >= [[date:Custom:From Date]]
Date = [[date:Today]]
Date >= [[date:LastMonth]] and Date <= [[date:Today]]
Date >= [[date:Custom:From Date]] and Date <= [[date:Today]]
```

`Custom` is literal and requires a label.
A range is a predefined literal and returns an auto-computed value.

Available ranges:

- `Today`.
- `Yesterday`.
- `LastWeek`.
- `LastMonth`.
- `LastTwoMonths`.
- `LastThreeMonths`.
- `LastSemester`.
- `LastYear`.
- `LastTwoYears`.

## Lookups

Syntax:

```text
[[lookup:string:Label]]
[[lookup:int:Label]]

[[lookup:string:multi:Label]]
[[lookup:int:multi:Label]]

[[lookup:string:Label:SelectStatement]]
[[lookup:int:Label:SelectStatement]]

[[lookup:string:multi:Label:SelectStatement]]
[[lookup:int:multi:Label:SelectStatement]]
```

Examples:

```sql
CountryId = [[lookup:int:Country]]
CountryId = [[lookup:int:Country:select Id from Country]]
CountryId in ([[lookup:int:multi:Country]])
CountryId in ([[lookup:int:multi:Country:select Id from Country]])
```

`multi` means that at runtime the user may select multiple values.

The SELECT statement is mandatory.
It may be provided inline:

```text
[[lookup:string:Customers:select Id from Customer]]
```

If the inline SELECT statement is omitted, it should be provided in the design-time UI.
The first column returned by the SELECT statement is the value used in the query.

Example lookup SELECTs:

```sql
select Id from Country
select Id, Name from Branch
```

## Enums

Syntax:

```text
[[enum:string:Label]]
[[enum:int:Label]]
[[enum:dec:Label]]

[[enum:string:multi:Label]]
[[enum:int:multi:Label]]
[[enum:dec:multi:Label]]

[[enum:string:Label:ConstantList]]
[[enum:int:Label:ConstantList]]
[[enum:dec:Label:ConstantList]]

[[enum:string:multi:Label:ConstantList]]
[[enum:int:multi:Label:ConstantList]]
[[enum:dec:multi:Label:ConstantList]]
```

Examples:

```sql
Status = [[enum:string:Status]]
Status = [[enum:string:Status:Pending;Completed;Canceled]]
Grade in ([[enum:dec:multi:Expected Grades]])
Grade in ([[enum:dec:multi:Expected Grades:1.5;2.0;2.5;3.0]])
```

`multi` means that at runtime the user may select multiple values.

The constant list is mandatory.
It is a list of constants separated by semicolon and may be provided inline:

```text
[[enum:string:Status:Pending;Completed;Canceled]]
```

If the inline constant list is omitted, it should be provided in the design-time UI.

Example constant lists:

```text
Pending;Completed;Cancelled
1.5;2.0;2.5;3.0
```

Use `int` or `dec` modifiers to avoid SQL quotes.
Use `multi` to present a selection list and generate comma-separated values.

## Parsing

`SqlFilterExpressionDef` parses a single raw tag.

```csharp
SqlFilterExpressionDef Expression = new();

Expression.RawTag = "[[date:Custom:From Date]]";

string Label = Expression.Label;
DateRange DateRange = Expression.DateRange;
```

If the tag is invalid, `HasErrors` becomes true.

```csharp
if (Expression.HasErrors)
    string Errors = Expression.Errors;
```

Important parsed properties include:

- `ExpressionType`.
- `Label`.
- `DateRange`.
- `IsMultiple`.
- `IsNumeric`.
- `Text`.
- `Statement`.
- `LookUpSelectSqlText`.

## More Examples

String examples:

```sql
Name = '[[string:Customer Name]]'
Name = '[[Customer Name]]'
Name like '%[[string:Customer Part]]%'
```

Integer example:

```sql
Value = [[int:Total Items]]
```

Decimal example:

```sql
Value = [[dec:Total Amount]]
```

Date examples:

```sql
OrderDate = [[date:Custom:Order Date]]
OrderDate = [[date:Today]]
OrderDate >= [[date:LastWeek]] and OrderDate <= [[date:Today]]
```

Lookup examples:

```sql
CountryId = [[lookup:string:Country]]
CountryId in ([[lookup:string:multi:Country]])
```

Enum examples:

```sql
OrderStatus = [[enum:string:Order Status]]
OrderStatus in ([[enum:string:multi:Order Status]])
ExpectedGrade >= [[enum:dec:Expected Grades]]
```
