# AppForm And AppFormDialog

`AppForm` is the base class for desktop forms that are hosted inside a tab page or a modal window.

`DataForm` derives from `AppForm`, but `AppForm` can also be used directly for dashboards, command pages and other non-CRUD screens.

## AppForm

`AppForm` is an Avalonia `UserControl`.

It provides:

- a form context;
- a title;
- a lifecycle;
- tab hosting support;
- modal dialog support;
- close handling;
- escape key handling;
- modal result handling.

## Lifecycle

The important lifecycle methods are:

- `Setup()`, called after the context is assigned and before initialization.
- `FormInitializing()`, called just before initialization.
- `FormInitialize()`, used to initialize controls and commands.
- `FormInitialized()`, called after initialization.
- `Start()`, called after the form has a parent and initialization is complete.
- `Closing()`, called just before closing.
- `Closed()`, called after closing.

Example:

```csharp
/// <summary>
/// Displays an application dashboard.
/// </summary>
public partial class DashboardForm : AppForm
{
    // ● protected
    /// <summary>
    /// Initializes controls and commands.
    /// </summary>
    protected override void FormInitialize()
    {
        CreateToolBar();
    }
    /// <summary>
    /// Loads dashboard data after the form is ready.
    /// </summary>
    protected override async Task Start()
    {
        await RefreshDashboard();
    }
}
```

Use `FormInitialize()` for control setup. Use `Start()` for work that needs the form to already be attached to its parent.

## FormContext

`FormContext` is the object passed to an `AppForm` when it is opened.

It contains:

- `FormId`, a unique form id inside the host.
- `ClassName`, the form class name.
- `DisplayMode`, tab or dialog.
- `Caller`, the control that opened the form.
- `ParentControl`, the hosting `TabItem` or `Window`.
- `Title`, the form title.
- `Options`, `Params` and `Tag`, optional caller data.
- `ModalResult` and `ResultData`, modal return data.

Example:

```csharp
FormContext Context = FormContext.Create(
    typeof(DashboardForm),
    FormDisplayMode.TabItem,
    Ui.MainWindow);
```

The context creates the form through `TypeStore`.

```csharp
AppForm Form = Context.CreateForm();
```

## Tab Hosting

`AppFormPagerHandler` hosts `AppForm` instances in a `TabControl`.

```csharp
FormContext Context = FormContext.Create(
    typeof(DashboardForm),
    FormDisplayMode.TabItem,
    Ui.MainWindow);

AppForm Form = PagerHandler.ShowAppForm(Context);
```

The handler:

- reuses an already open form with the same `FormId`;
- creates a `TabItem`;
- assigns the context parent control;
- calls `Form.Setup(Context)`;
- selects the tab.

`CloseForm()` removes the parent tab item. Middle-click close is supported when `ClosableByUser` is true.

## Modal Hosting

`AppFormDialog` hosts an `AppForm` inside a modal `Window`.

```csharp
FormContext Context = FormContext.Create(
    typeof(DashboardForm),
    FormDisplayMode.Dialog,
    Ui.MainWindow);

Context = await AppFormDialog.ShowModal(Context);
```

`AppFormDialog` sets:

- `DisplayMode` to `Dialog`;
- the owner window;
- the parent control;
- default dialog size;
- the dialog title.

It then creates the form and calls `Setup()`.

## Modal Result

When a form is displayed modally, setting `ModalResult` closes the containing window.

```csharp
ModalResult = ModalResult.Ok;
```

Before closing, `AppForm` copies the result to `Context.ModalResult` and calls `PassResultBack()`.

```csharp
/// <summary>
/// Passes dialog result data back to the caller.
/// </summary>
protected override void PassResultBack()
{
    Context.ResultData = SelectedItem;
}
```

The caller can then inspect the context.

```csharp
FormContext Context = await AppFormDialog.ShowModal(FormContext);

if (Context.Result)
{
    object ResultData = Context.ResultData;
}
```

## Keyboard Handling

`AppForm` listens to key down events.

Override `ProcessKeyDown()` for custom shortcuts.

```csharp
/// <summary>
/// Handles form-level shortcuts.
/// </summary>
protected override bool ProcessKeyDown(KeyEventArgs e)
{
    if (e.Key == Key.F5)
    {
        Refresh();
        return true;
    }

    return base.ProcessKeyDown(e);
}
```

By default, `Escape` cancels and closes a modal form.

## Title

`TitleText` updates the parent tab header or the parent window title.

```csharp
TitleText = "Dashboard";
```

This lets the same form work correctly in both tab and modal display modes.
