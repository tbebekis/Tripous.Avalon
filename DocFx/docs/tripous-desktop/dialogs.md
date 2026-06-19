# Dialogs

Tripous.Desktop provides a few dialog patterns for common modal workflows.
They range from simple message boxes to full `DataForm` instances hosted inside an `AppFormDialog`.

## Main Types

- `MessageBox`, for information, errors, and Yes/No questions.
- `DialogWindow`, the base class for custom modal dialogs.
- `DialogInfo`, the runtime context and result holder for `DialogWindow`.
- `ModalResult`, the OK/Cancel/None result enum.
- `InputBox`, a simple text input dialog.
- `AppFormDialog`, a modal host for `AppForm` and `DataForm`.
- `DataFormContext.ShowFormModal()`, the shortcut for opening registered data forms modally.

## MessageBox

`MessageBox` covers simple user messages.

```csharp
await MessageBox.Info("Saved.", this);
await MessageBox.Error("Save failed.", this);
```

Exceptions can be passed directly.

```csharp
try
{
    Save();
}
catch (Exception e)
{
    await MessageBox.Error(e, this);
}
```

Use `YesNo()` for confirmation.

```csharp
bool Confirmed = await MessageBox.YesNo(
    "Delete selected row?",
    this);

if (!Confirmed)
    return;
```

The caller control is used to find the owner window.
When no caller is provided, dialog helpers usually fall back to `Ui.MainWindow`.

## DialogWindow

`DialogWindow` is the base class for custom modal dialogs.
It provides a lifecycle and a shared `DialogInfo` object.

Override these methods in a custom dialog:

- `WindowInitialize()`, for one-time setup.
- `ItemToControls()`, for loading input data into controls.
- `ControlsToItem()`, for saving controls back to result data.

`ModalResult` closes the dialog when it becomes `Ok` or `Cancel`.

```csharp
this.ModalResult = ModalResult.Ok;
```

## DialogInfo

`DialogInfo` stores the modal dialog context.

Important members are:

- `Dialog`, the actual dialog window.
- `Parent`, the owner window.
- `Caller`, the control that opened the dialog.
- `InputData`, the input object passed to the dialog.
- `ResultData`, the object returned by the dialog.
- `ModalResult`, the final modal result.
- `Result`, true when `ModalResult` is `Ok`.
- `Params`, custom parameter dictionary.

Use `DialogWindow.ShowModal<T>()` to show a dialog and get its `DialogInfo`.

```csharp
DialogInfo Info = await DialogWindow.ShowModal<MyDialog>(
    InputData,
    this);

if (Info.Result)
    UseResult(Info.ResultData);
```

## InputBox

`InputBox` is a small `DialogWindow` for a single text value.

```csharp
InputBoxData Data = await InputBox.ShowModal(
    "Enter code",
    "",
    this);

if (Data.Result)
    UseCode(Data.Value);
```

`InputBoxData` carries the message, the input value, and the final `DialogInfo`.

## AppFormDialog

`AppFormDialog` hosts an `AppForm` inside a modal `Window`.
It is used when an existing form should be shown modally instead of in a tab or page.

```csharp
FormContext Context = FormContext.Create("Customer", this);
Context = await AppFormDialog.ShowModal(Context);
```

`AppFormDialog.ShowModal()` sets:

- `Context.DisplayMode` to `Dialog`.
- `Context.ParentControl` to the dialog window.
- the owner window from `Context.Caller`.
- the dialog title from `Context.Title`.

Then it creates the form and calls `Form.Setup(Context)`.

## DataForm Modal Dialogs

For registered data forms, prefer `DataFormContext.ShowFormModal()`.

```csharp
DataFormContext Context = await DataFormContext.ShowFormModal(
    "Customer",
    DataFormAction.Edit,
    CustomerId,
    this);

if (Context.Result)
    RefreshCustomer();
```

This is used by reference context menus for Show List, Edit, and Add operations.

Common start actions are:

- `DataFormAction.List`.
- `DataFormAction.Edit`.
- `DataFormAction.Insert`.

The optional row id is used by edit and other row-specific actions.

## File Dialogs

`Ui` provides simple wrappers for Avalonia storage picker dialogs.

```csharp
string Path = await Ui.OpenFileDialog(this, "json", "txt");
```

```csharp
string Path = await Ui.SaveFileDialog(this, "json");
```

The extension list is converted to file picker filters.
The helpers return the selected local path or no path when the user cancels.

## Specialized Dialogs

Tripous.Desktop also includes specialized dialogs such as:

- `LoginDialog`.
- `FirstRunDialog`.
- `ConfigDialog`.
- `DbConnectionEditDialog`.
- `RegBuilderProjectDialog`.

These follow the same modal principles, but carry domain-specific data and validation.

## Practical Notes

- Use `MessageBox` only from the UI layer.
- Do not show dialogs from data modules.
- Pass the caller control when available so the owner window is correct.
- Use `DialogWindow` and `DialogInfo` for custom modal dialogs.
- Use `DataFormContext.ShowFormModal()` for registered data forms.
