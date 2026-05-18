# Automatic Code Generation Specification
## Tripous / tERP
### Number Series & Code Provider Engine

## Purpose

The system provides automatic generation of business codes during inserts.

Examples:

```text
SO-2026-000001
SO-2026-05-000123
INV-2026-Q2-000045
PAY-26-W20-0007
```

Code generation is driven by:

- NumberSeries table
- FieldDef.CodeProvider property
- Code pattern engine
- Reset periods

Generation occurs automatically during `DataModule.Insert()`.

---

## NumberSeries Table

```sql
CREATE TABLE NumberSeries (
    Id              @NVARCHAR(40) @NOT_NULL primary key,
    Code            @NVARCHAR(40) @NOT_NULL,
    Name            @NVARCHAR(96) @NOT_NULL,
    Pattern         @NVARCHAR(64) @NOT_NULL,
    ResetPeriodId   integer default 0 @NOT_NULL, -- Enum
    NextNumber      integer default 1 @NOT_NULL,
    LastResetValue  @NVARCHAR(16) @NULL,
    IsActive        @BOOL default 1 @NOT_NULL,

    CONSTRAINT UQ_NumberSeries_Code UNIQUE (Code),
    CONSTRAINT UQ_NumberSeries_Name UNIQUE (Name)
)
```

---

## FieldDef

```csharp
public string CodeProvider { get; set; }
```

Example:

```sql
Code @NVARCHAR(40) @NOT_NULL -- Code [SALES_ORDER]
```

or

```csharp
Field.CodeProvider = "SALES_ORDER";
```

The provider name maps to:

```text
NumberSeries.Code
```

Example:

```text
SALES_ORDER
PURCHASE_ORDER
CUSTOMER
SUPPLIER
```

---

## ResetPeriod Enum

```csharp
public enum ResetPeriod
{
    None = 0,
    Year = 1,
    Semester = 2,
    Quarter = 3,
    Month = 4,
    Week = 5,
    Day = 6,
}
```

Meaning:

```text
None      -> never reset
Year      -> reset every year
Semester  -> reset every semester
Quarter   -> reset every quarter
Month     -> reset every month
Week      -> reset every week
Day       -> reset every day
```

---

## Pattern Syntax

Pattern controls final code generation.

Example:

```text
SO-YYYY-XXXXXX
```

Generated:

```text
SO-2026-000001
```

Pattern may contain:

```text
YYYY
YY
MM
DD
Q
S
WW
X
```

Everything else is literal text.

Example:

```text
INV-YYYY-MM-XXXXXX
```

Result:

```text
INV-2026-05-000123
```

## The X Token

Pattern must contain at least one `X` token.

There is no limit on how many `X` tokens a pattern may contain.

`X` tokens define the numeric part of the generated code and determine its maximum numeric capacity.

The total number of `X` tokens is used, regardless of separators or literal characters.

Examples:

```text
XXXX
```

Numeric digits:

```text
4
```

Maximum value:

```text
9999
```

---

```text
XXX-XXX
```

Numeric digits:

```text
6
```

Maximum value:

```text
999999
```

---

```text
SO-YYYY-XXXXXXXX
```

Generated:

```text
SO-2026-00000123
```

If the next generated number exceeds the numeric capacity defined by the pattern, generation fails and an error is raised.

Example:

```text
Pattern: XXX-XXX
Current number: 1000000
```

Result:

```text
Error: Number exceeds pattern capacity.
```
 

## Tokens

### Year

```text
YYYY -> 2026
YY   -> 26
```

Example:

```text
SO-YYYY-XXXXXX
```

Result:

```text
SO-2026-000001
```

---

### Month

```text
MM -> 01..12
```

Example:

```text
INV-YYYY-MM-XXXXX
```

Result:

```text
INV-2026-05-00001
```

---

### Day

```text
DD -> 01..31
```

Example:

```text
PAY-YYYY-MM-DD-XXXX
```

Result:

```text
PAY-2026-05-18-0001
```

---

### Quarter

```text
Q -> 1..4
```

Example:

```text
INV-YYYY-Q-XXXXX
```

Results:

```text
INV-2026-Q1-00001
INV-2026-Q2-00001
INV-2026-Q3-00001
INV-2026-Q4-00001
```

---

### Semester

```text
S -> 1..2
```

Example:

```text
UNI-YYYY-S-XXXXX
```

Results:

```text
UNI-2026-S1-00001
UNI-2026-S2-00001
```

---

### Week

ISO week:

```text
WW -> 01..53
```

Example:

```text
PAY-YY-WW-XXXX
```

Result:

```text
PAY-26-W20-0001
```

---

## Numeric Token X

X represents numeric positions.

Only X characters participate.

Everything else is ignored.

Examples:

```text
XXXX      -> 4 digits
XXXXXX    -> 6 digits

XXX-XXX   -> 6 digits
XX-XX-X   -> 5 digits
```

Generated:

```text
000001
000123
012345
```

Padding uses:

```text
0
```

always.

---

## Reset Validation Rules

Pattern must contain tokens compatible with ResetPeriod.

### None

No requirements.

---

### Year

Required:

```text
YYYY
or
YY
```

Valid:

```text
SO-YYYY-XXXXXX
SO-YY-XXXXXX
```

Invalid:

```text
SO-XXXXXX
```

---

### Semester

Required:

```text
YYYY or YY
AND
S
```

Valid:

```text
SO-YYYY-S-XXXXXX
```

Invalid:

```text
SO-YYYY-XXXXXX
```

---

### Quarter

Required:

```text
YYYY or YY
AND
Q
```

Valid:

```text
INV-YYYY-Q-XXXXX
```

---

### Month

Required:

```text
YYYY or YY
AND
MM
```

Valid:

```text
SO-YYYY-MM-XXXXXX
```

Invalid:

```text
SO-YYYY-XXXXXX
```

---

### Week

Required:

```text
YYYY or YY
AND
WW
```

Valid:

```text
PAY-YY-WW-XXXX
```

---

### Day

Required:

```text
YYYY or YY
MM
DD
```

Valid:

```text
PAY-YYYY-MM-DD-XXXX
```

Invalid:

```text
PAY-YYYY-MM-XXXX
```

---

## Reset Value Calculation

Internal reset values:

```text
Year      -> 2026
Semester  -> 2026-S1
Quarter   -> 2026-Q2
Month     -> 2026-05
Week      -> 2026-W20
Day       -> 2026-05-18
```

Stored in:

```text
LastResetValue
```

If current value differs:

```text
NextNumber = 1
LastResetValue = CurrentValue
```

---

## Insert Behavior

During:

```csharp
DataModule.Insert()
```

system checks:

```csharp
FieldDef.CodeProvider
```

If:

```text
CodeProvider != null
```

and field value is empty:

```text
Generate code
```

Generated value is assigned automatically.

Manual values remain unchanged.

---

## UI Rules

Fields using:

```csharp
FieldDef.CodeProvider
```

should become:

```csharp
FieldDef.IsReadOnlyUi = true;
```

unless explicitly overridden.

---

## Examples

Sales Orders:

```text
Pattern: SO-YYYY-XXXXXX
Reset: Year
```

Generated:

```text
SO-2026-000001
SO-2026-000002
```

---

Invoices:

```text
Pattern: INV-YYYY-MM-XXXXX
Reset: Month
```

Generated:

```text
INV-2026-05-00001
INV-2026-05-00002

INV-2026-06-00001
```

---

University:

```text
Pattern: UNI-YYYY-S-XXXXX
Reset: Semester
```

Generated:

```text
UNI-2026-S1-00001
UNI-2026-S2-00001
```

---

Weekly payments:

```text
Pattern: PAY-YY-WW-XXXX
Reset: Week
```

Generated:

```text
PAY-26-W20-0001
PAY-26-W20-0002
```

---