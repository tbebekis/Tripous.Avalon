# Extension Methods

Tripous Core includes a set of extension methods for common framework tasks.
They keep descriptor, registry, UI, and data code shorter by centralizing repeated low-level operations.

This article is a map of the most useful groups.
The API reference contains the complete method list.

## String Helpers

`StringExtensions` contains case-insensitive comparisons, quoting helpers, numeric parsing, diacritic removal, and title formatting helpers.

```csharp
bool Same = "Customer".IsSameText("customer");
bool Starts = "CustomerName".StartsWithText("customer");
bool Contains = "CustomerName".ContainsText("name");
```

These helpers are used throughout Tripous when names, aliases, and registry identifiers should be compared without casing differences.

```csharp
if (Field.Name.IsSameText("Code"))
{
    // Handle the Code field.
}
```

`SplitToWords()` is important for readable default titles.

```csharp
string Title = "CustomerName".SplitToWords();
```

The result is:

```text
Customer Name
```

`ToPlural()` is used by descriptor title helpers when a list or module needs a plural title.

```csharp
string Title = "Customer".ToPlural().SplitToWords();
```

## List Helpers

`ListExtensions` contains helpers for validation, moving items, formatting lists as text, file persistence, and splitting sequences into chunks.

```csharp
bool IsValid = Items.IsValidIndex(Index);
bool Moved = Items.Move(Index, Down: true);
```

These are useful in UI or descriptor editor scenarios where a user can reorder items.

```csharp
if (Columns.CanMove(Index, Down: false))
    Columns.Move(Index, Down: false);
```

String lists can be converted to display text.

```csharp
string Text = Names.CommaText();
```

And enumerable values can be split into fixed-size chunks.

```csharp
List<List<string>> Chunks = Names.Split(100);
```

## Dictionary Helpers

`DictionaryExtensions` provides typed accessors for dictionaries and hashtable-style values.

```csharp
string Name = Values.AsString("Name");
int Count = Values.AsInteger("Count");
bool Enabled = Values.AsBoolean("Enabled");
DateTime Date = Values.AsDateTime("CreatedAt");
```

This is useful when values come from loosely typed sources, such as dynamic properties, configuration dictionaries, or data payloads.

There are also helpers for text and XML persistence.

```csharp
string Text = Dictionary.DicToText();

Dictionary.TextToDic(Text);
```

`ValuesToRow()` copies dictionary values to a `DataRow`.

```csharp
Values.ValuesToRow(Row);
```

## Type Helpers

`TypeExtensions` helps with reflection and type classification.

```csharp
bool HasCtor = ClassType.HasDefaultConstructor();
bool IsNumeric = DataType.IsNumeric();
bool IsDateTime = DataType.IsDateTime();
```

It also contains object creation helpers.

```csharp
object Instance = ClassType.Create();
```

Property helpers are useful when infrastructure code needs to inspect or read public properties by name.

```csharp
PropertyInfo Property = ClassType.FindPublicProperty("Name");
object Value = ClassType.GetPublicPropertyValue(Instance, "Name");
```

Tripous uses this kind of helper in descriptor, JSON, and dynamic infrastructure code.

## Assembly Helpers

`AssemblyExtensions` provides path and safe type loading helpers.

```csharp
string Folder = Assembly.GetFolder();
string FileName = Assembly.GetFileName();
Type[] Types = Assembly.GetTypesSafe();
```

`GetTypesSafe()` is especially important for scanning assemblies.
It returns the available types even when some types cannot be loaded.

`AppAssemblies` and `TypeStore` depend on this kind of safe assembly scanning.

## Date And Time Helpers

`DateTimeExtensions` provides file-name formatting, week calculations, and day boundaries.

```csharp
string FileNamePart = DateTime.Now.ToFileName();
DateTime Start = DateTime.Today.StartOfDay();
DateTime End = DateTime.Today.EndOfDay();
int Week = DateTime.Today.GetWeekNumber();
```

These helpers are small, but they avoid repeated formatting and boundary code.

`DateTimeFormatTypeExtensions` maps a `DateTimeFormatType` value to a culture-aware format string.

```csharp
string Format = DateTimeFormatType.Date.GetFormatString(CultureInfo.CurrentCulture);
```

## Stream Helpers

`StreamExtensions` contains helpers for copying and reading streams.

```csharp
Source.CopyAllTo(Destination);
byte[] Data = Stream.ToArray();
```

Use these when a stream must be copied completely or converted to bytes.

## Exception Helpers

`ExceptionExtensions` converts exceptions into readable text.

```csharp
string Text = Ex.GetErrorText();
string FullText = Ex.GetErrorTextFull();
```

Use the short form for user-facing or log summaries.
Use the full form when nested exception detail is needed for diagnostics.

## When To Use Extension Methods

Use Tripous extension methods when they express an existing framework convention.

- Case-insensitive name comparison.
- Descriptor title formatting.
- Safe assembly type loading.
- Typed dictionary reads.
- Collection item reordering.
- Stream copying.
- Exception formatting.

Avoid adding one-off extension methods for behavior that belongs in a domain class or service.
Extension methods are most valuable when they describe a repeated infrastructure operation.
