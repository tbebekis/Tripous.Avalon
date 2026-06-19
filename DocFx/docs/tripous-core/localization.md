# Texts And ILocalizer

`Texts` is the small localization gateway used by Tripous.
Core, Data, and Desktop code ask `Texts` for display text, titles, captions, and error messages.

`ILocalizer` is the application-side extension point.
An application may assign `Texts.Current` to an object that knows how to translate keys.

## Default Behavior

When no localizer is assigned, `Texts` returns a readable default.

```csharp
string Title = Texts.L("CustomerName");
```

By default, `Texts.SplitKeys` is `true`.
That means keys are split into words when no translation exists.

```csharp
string Title = Texts.L("CustomerName");
```

The fallback value is:

```text
Customer Name
```

This keeps the application usable even before real translation resources exist.

## Explicit Defaults

Use the overload with a default value when the fallback text should be controlled.

```csharp
string Caption = Texts.L("CustomerName", "Customer");
```

If no localizer exists, the default value is used.
When `Texts.Current` is assigned, the localizer fully controls the returned value.

Tripous uses this pattern in definitions and validation messages.

```csharp
Sys.Throw(Texts.GS("E_TableDef_NoFieldsDefined", "TableDef fields are not defined."));
```

## L And GS

`L()` and `GS()` both return localized text.
They currently use the same lookup behavior.

```csharp
string Title = Texts.L("Customer");
string Message = Texts.GS("E_Customer_NotFound", "Customer not found.");
```

The names help express intent:

- Use `L()` for normal labels, captions, and titles.
- Use `GS()` for general strings, especially messages.

## Localizer Implementation

An application can provide its own localizer by implementing `ILocalizer`.

```csharp
/// <summary>
/// Provides dictionary-based localization.
/// </summary>
public class DictionaryLocalizer : ILocalizer
{
    // ● private fields
    private readonly Dictionary<string, string> fTexts = new(StringComparer.OrdinalIgnoreCase);

    // ● public
    /// <summary>
    /// Adds or replaces a localized text.
    /// </summary>
    public void Add(string Key, string Text)
    {
        fTexts[Key] = Text;
    }
    /// <summary>
    /// Returns the localized text for a key.
    /// </summary>
    public string GetText(string Key)
    {
        return fTexts.TryGetValue(Key, out string Text) ? Text : Key.SplitToWords();
    }
}
```

The application assigns it during startup.

```csharp
DictionaryLocalizer Localizer = new();

Localizer.Add("CustomerName", "Customer");
Localizer.Add("E_Customer_NotFound", "Customer not found.");

Texts.Current = Localizer;
```

After that, all Tripous code that calls `Texts.L()` or `Texts.GS()` uses the application localizer.
The localizer should provide its own fallback for missing keys, because `Texts` does not apply the default value after `Texts.Current` is assigned.

## Where Tripous Uses It

Descriptors use `Texts` to turn title keys into display text.

```csharp
public virtual string Title => Texts.L(TitleKey);
```

Data table and field definitions use it for readable titles.

```csharp
string Title = Texts.GS(TitleKey, TitleKey);
```

Desktop binding code uses it for grid column captions.

```csharp
string Caption = Texts.L(Column.Caption);
```

Registry and validation code use it for error messages.

```csharp
Sys.Throw(Texts.GS("E_ModuleDef_NoTopTable", "ModuleDef has no top table."));
```

This keeps localization out of the lower-level classes.
Those classes only know about keys; the application decides how keys become final text.

## Double Underscores

After lookup, `Texts` replaces double underscores with spaces.

```csharp
string Caption = Texts.L("Customer__Name");
```

The result is:

```text
Customer Name
```

This is useful when a key needs to preserve a visual space marker while still being a single identifier-like string.

## When To Use It

Use `Texts` for user-facing strings that may need translation or central control.

- Form titles.
- Field captions.
- Grid column captions.
- Command captions.
- Validation messages.
- Descriptor title keys.

Do not use it for internal identifiers, registry names, table names, or field names.
Those are structural values and should stay stable.
