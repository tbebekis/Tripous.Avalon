# SQL Filter Panel

`SqlFilterPanelHandler` creates the filter controls shown in a `DataForm` list page.
It is the Desktop UI layer for structured `SqlFilterDef` and `SqlFilterDefs` filters.

This is different from `SqlFilterExpressionDef`.
`SqlFilterDef` filters are registered metadata.
`SqlFilterExpressionDef` is used for inline filter tags embedded in SQL text.

## Main Types

- `SqlFilterPanelHandler` creates controls, collects values, clears values, and produces SQL WHERE text.
- `SqlFilterInfo` keeps the UI controls associated with one `SqlFilterDef`.
- `SqlFilterDef` describes one structured filter.
- `SqlFilterDefs` is the filter definition list used by a select.
- `SelectDef` supplies the filter definitions for a list SELECT.

## DataForm Integration

`DataForm` creates a filter panel handler during startup.

```csharp
FilterPanelHandler = new SqlFilterPanelHandler(pnlFilters);
```

When the selected list `SelectDef` changes, the form creates the filter controls.
If the select does not already define filters, Tripous can infer them from the SELECT schema.

```csharp
SqlFilterDefs FilterDefs;

if (SelectDef.FilterDefs == null || SelectDef.FilterDefs.Count == 0)
    FilterDefs = SelectDef.DefineFilters(Module.Name, Module.Store);
else
    FilterDefs = SelectDef.FilterDefs;

SelectDef.ValidateBooleanFilterTypes(Module.Name, Module.Store, FilterDefs);
FilterPanelHandler.CreateFilterControls(FilterDefs);
```

`GetSavedFilterValues()` is a hook where a form may apply user-saved filter values before controls are created.

## Created Controls

For each `SqlFilterDef`, the handler creates:

- a boolean operator combo box with `And` and `Or`.
- a label from `FilterDef.Title`.
- a condition operator combo box.
- one value control.
- a second value control for `Between`.

The value control depends on `FilterDataType`.

- Boolean filters use a combo box with All, True, and False.
- Date and date-time filters use `DatePicker`.
- Other filters use `TextBox`.

String filters support:

- `Equal`.
- `Contains`.
- `StartsWith`.
- `EndsWith`.

Non-string filters support:

- `Equal`.
- `GreaterOrEqual`.
- `LessOrEqual`.
- `Between`.

Boolean filters hide the condition operator and use `Equal` internally.

## Collecting Values

`CollectValues()` reads the current UI values and returns only active filters.
A filter is active when it has enough values to produce a condition.

```csharp
SqlFilterDefs ActiveFilters = FilterPanelHandler.CollectValues();
```

Rules:

- empty text boxes are ignored.
- date filters with no date are ignored.
- boolean All is ignored.
- `Between` is included only when both values exist.
- non-between filters use only `Value`.

The collected filters are clones of the original definitions, so the base metadata remains unchanged.

## Producing SQL WHERE

`GetWhere()` collects active filters and formats them as inline SQL WHERE text.

```csharp
string Where = FilterPanelHandler.GetWhere();
```

`DataForm.ListSelect()` wraps the selected SQL and applies that WHERE text.

```csharp
string SqlText = SelectDef.SqlText;
string Where = FilterPanelHandler.GetWhere();

if (!string.IsNullOrWhiteSpace(Where))
    SqlText = $"select * from ({SqlText}) X where {Where}";

Module.ListSelect(SqlText);
```

This means list filters operate on the SELECT result columns.
For joined or aliased columns, make sure the filter field name matches the visible SELECT output or the expression intended by the select definition.

## Clearing Filters

The list toolbar includes a Clear Filter button.
It calls `SqlFilterPanelHandler.Clear()`.

```csharp
FilterPanelHandler.Clear();
```

Clear resets:

- text boxes to empty text.
- date pickers to no selected date.
- boolean operator combo boxes to `And`.
- condition operator combo boxes to `Equal`.
- boolean value combo boxes to All.
- second value controls to hidden.

## Boolean Filters

Boolean filters need special care because many Tripous databases store booleans as integer values.
Before creating the panel, `DataForm` calls `SelectDef.ValidateBooleanFilterTypes()`.

```csharp
SelectDef.ValidateBooleanFilterTypes(
    Module.Name,
    Module.Store,
    FilterDefs);
```

This checks that boolean filters are compatible with the SELECT schema.

## Typical Flow

The usual runtime flow is:

- the user selects a list SELECT.
- `DataForm` loads or defines `SqlFilterDefs`.
- `SqlFilterPanelHandler` creates the filter controls.
- the user enters filter values.
- the user executes Refresh List.
- `DataForm` asks the panel for WHERE text.
- the list SELECT is wrapped and executed.

## Practical Notes

- Use this panel for structured select filters.
- Use `SqlFilterExpressionDef` only for inline filter tags inside SQL text.
- Define filter field names carefully when the SELECT uses joins or aliases.
- Use `GetSavedFilterValues()` for per-user defaults.
- Keep filter UI logic in Desktop and filter metadata in Tripous.Data.
