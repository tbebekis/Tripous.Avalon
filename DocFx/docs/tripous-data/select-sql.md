# SelectSql

`SelectSql` represents a SQL SELECT statement split into clauses.
It is used when Tripous needs to build, inspect, or extend SELECT statements without treating them as plain opaque strings.

`SelectSqlParser` is the helper that parses a SQL string into those clauses.

Neither class is a full SQL parser.
They support the SELECT patterns Tripous needs for descriptors, filters, locators, list SQL, and generated table SQL.

## Clause-Based SELECT

`SelectSql` stores the major SELECT clauses separately.

```csharp
SelectSql Select = new();

Select.Select = @"
    Customer.Id,
    Customer.Code,
    Customer.Name";

Select.From = @"
    Customer";

Select.Where = @"
    Customer.IsActive = :IsActive";

Select.OrderBy = @"
    Customer.Name";
```

`Text` returns the full statement.

```csharp
string SqlText = Select.Text;
```

The generated SQL includes the clause keywords.

```sql
select
    Customer.Id,
    Customer.Code,
    Customer.Name
from
    Customer
where
    Customer.IsActive = :IsActive
order by
    Customer.Name
```

## Parsing SQL Text

A `SelectSql` can be created from a SQL string.

```csharp
SelectSql Select = new(@"
select
    Id,
    Code,
    Name
from Customer
where IsActive = :IsActive
order by Name");
```

Setting `Text` parses the SQL and fills the clause properties.

```csharp
SelectSql Select = new();

Select.Text = "select * from Customer where IsActive = :IsActive";

string SelectClause = Select.Select;
string FromClause = Select.From;
string WhereClause = Select.Where;
```

`ParseFromTableName()` is a shortcut for simple table selects.

```csharp
SelectSql Select = new();

Select.ParseFromTableName("Customer");
```

The result is equivalent to:

```sql
select * from Customer
```

## Extending WHERE, GROUP BY, HAVING And ORDER BY

`SelectSql` provides helper methods for appending clauses safely.

```csharp
SelectSql Select = new("select * from Customer");

Select.AddToWhere("IsActive = :IsActive");
Select.AddToWhere("Code like :Code");
Select.AddToOrderBy("Name");
```

`AddToWhere()` joins with `and`.
`OrToWhere()` joins with `or`.

```csharp
Select.OrToWhere("IsSystem = :IsSystem");
```

There are similar methods for `GroupBy`, `Having`, and `OrderBy`.

```csharp
Select.AddToGroupBy("CountryId");
Select.AddToHaving("count(*) > 0");
Select.AddToOrderBy("CountryId");
```

These helpers are useful when descriptor, filter, locator, or UI code must add conditions to an existing SELECT.

## User WHERE

`WhereUser` is a separate user-added WHERE fragment.
When `Text` is generated, `WhereUser` is appended to the normal `Where` clause with `and`.

```csharp
SelectSql Select = new("select * from Customer");

Select.Where = "IsActive = :IsActive";
Select.WhereUser = "Name like :Name";

string SqlText = Select.Text;
```

This pattern is useful in locators and filters, where framework SQL and user-entered filtering should remain separate until the final SQL is built.

## Company-Aware SELECTs

When `CompanyAware` is true, `SelectSql` adds a company condition using `DbConfig.CompanyFieldName` and `DbConfig.VariablesPrefix`.

```csharp
SelectSql Select = new("select * from Customer");

Select.CompanyAware = true;

string SqlText = Select.Text;
```

The generated WHERE clause includes a condition like:

```sql
CompanyId = :@CompanyId
```

Later, `SqlValueProviders` replaces `:@CompanyId` with the current company id.

This is one of the mechanisms used for company-aware data.

## Date Range SELECTs

`DateRangeColumn` and `DateRange` can add a fixed date range condition.

```csharp
SelectSql Select = new("select * from SalesInvoice");

Select.DateRangeColumn = "SalesInvoice.InvoiceDate";
Select.DateRange = DateRange.LastMonth;

string SqlText = Select.Text;
```

`DateRangeConstructWhereParams()` creates paired parameters for the range.
`SqlValueProviders` later calls `DateRangeReplaceWhereParams()` to replace those range placeholders with actual date values using the active provider.

This is useful for browser/list SELECTs that are restricted to a predefined date range.

## Connection Name

`ConnectionName` tells Tripous which database connection should execute the SELECT.
When it is empty, it falls back to `DbConfig.DefaultConnectionName`.

```csharp
SelectSql Select = new("select * from Customer");

Select.ConnectionName = "Reports";
```

This matters in multi-connection applications.

## Main Table Name

`GetMainTableName()` tries to return the first table token from the FROM clause.

```csharp
SelectSql Select = new("select * from Customer C");

string TableName = Select.GetMainTableName();
```

This is a convenience helper, not a full SQL analysis feature.
It returns an empty string when the FROM clause starts with a subquery.

## SelectSqlParser

`SelectSqlParser` parses a SELECT statement into:

- `Select`.
- `From`.
- `Where`.
- `GroupBy`.
- `Having`.
- `OrderBy`.

```csharp
SelectSqlParser Parser = SelectSqlParser.Execute(@"
select Id, Name
from Customer
where IsActive = :IsActive
order by Name");

string WhereClause = Parser.Where;
```

The parser skips:

- String literals.
- Line comments.
- Block comments.
- Double-quoted identifiers.
- Bracketed identifiers.
- Backtick-quoted identifiers.
- Clause keywords inside parentheses.

That means nested SELECT statements inside parentheses are handled well enough for Tripous use cases.

## Parser Limits

`SelectSqlParser` is not intended to validate SQL.
It does not understand every SQL dialect feature.
It does not build an abstract syntax tree.

Use it for framework-level clause splitting and SELECT composition.
Do not use it as a general-purpose SQL parser.

Also avoid relying on parse-and-assign behavior when SQL contains unusual quoted field syntax.
`SelectSql.Assign()` exists partly to avoid reparsing when copying another `SelectSql`.

## Where Tripous Uses It

Tripous uses `SelectSql` in several places:

- `TableDef` builds list and row SELECT statements.
- `Locator` extends SELECT statements with user search conditions.
- `TableSet` adapts detail SELECT statements.
- `SqlValueProviders` replaces date range parameters.
- Descriptors store list SQL and filter-related SQL.

The goal is practical composition.
Tripous keeps SQL visible, but `SelectSql` gives the framework a safe place to append framework-generated WHERE, ORDER BY, company, and date-range fragments.
