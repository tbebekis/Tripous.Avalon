# Tripous.Avalon
### Dynamic Data-Binding & Management for Avalonia UI

![Demo App](DataApp00.gif)

**Tripous.Avalon** is a lightweight, high-performance library designed to simplify data management and UI synchronization in Avalonia UI applications. It provides a robust abstraction layer between your UI and your data sources, whether they are **DataTables** or **Generic Lists**.

## The Philosophy: Runtime Dynamics over Static XAML

The majority of Avalonia and WPF documentation focuses heavily on **Design-time binding via XAML**. While this works for simple, static forms, it often falls short in complex, professional-grade applications. 

In the real world, enterprise applications require **Dynamic Binding**:
* UI layouts that are generated or modified at **runtime**.
* Data sources and schemas that aren't known until the application is executing.
* The need to bind, unbind, or re-bind controls programmatically without bloating the XAML with complex converters.

**Tripous.Avalon** is built specifically for this. It moves the binding logic into a centralized, programmable `DataSource` object, allowing for a flexible and maintainable codebase where the UI responds dynamically to the data structure.

## Filling the Gap: Why Tripous.Avalon?

Surprisingly, modern UI frameworks often lack a native, high-level mechanism to handle common data entry requirements out-of-the-box. Developers are usually left to manually implement:
1. **Currency Management:** Tracking the "Current" record across multiple synchronized controls (TextBoxes, Grids, etc.).
2. **Standardized Notifications:** A unified way to listen for `OnAdding`, `OnChanging`, or `OnDeleting` events across any data type (List or Table).
3. **Lookup Logic:** The complex task of mapping IDs to Display Names in ComboBoxes and ListBoxes at runtime.

**Tripous.Avalon** fills this gap by providing a ready-to-use engine that treats a `List<T>` or a `DataTable` with the same first-class respect, offering a rich event lifecycle and simplified API that empowers the developer to build data-heavy applications faster.

## Key Features

* **Unified IDataLink:** Use the same C# code to manage `System.Data.DataTable` and `IList<T>`.
* **One-Line Binding:** Bind any Avalonia control (TextBox, CheckBox, ComboBox, DatePicker, DataGrid, etc.) with a single method call.
* **Advanced Lookup Support:** Effortless binding for lookup controls with automatic synchronization between ID values and display text.
* **Lifecycle Events:** Hook into the data flow with cancelable events for validation and business logic.
* **UX Enhancements:** Built-in keyboard navigation support (e.g., Enter as Tab, F4 for Dropdowns).

## Quick Example (The Runtime Way)

```csharp
// 1. Initialize from any source
var dsCustomer = DataSource.FromList(myCustomerList); 
var dsCountry = DataSource.FromList(myCountryList);

// 2. Bind dynamically at Runtime in your Window/UserControl
void SetupUI() {
    // Basic fields
    dsCustomer.Bind(txtName, "FullName");
    dsCustomer.Bind(numCredit, "CreditLimit");
    dsCustomer.Bind(chkIsActive, "IsActive");
    
    // Bind a Lookup ComboBox (CountryId -> CountryName mapping)
    dsCustomer.Bind(cboCountry, dsCountry, "CountryName", "Id", "CountryId");
    
    // Bind a DataGrid with automatic column generation
    dsCustomer.Bind(myMainGrid, true);
}

void Insert()
{
    DataSourceRow Row = dsCustomer.Add();
    Row["Id"] = edtNewId.GetText();
    Row["Name"] = edtNewName.GetText();
    Row["CountryId"] = edtNewCountryId.GetText();
}

void Delete()
{
    dsCustomer.Delete();
}
```



## Roadmap: This is Just the Beginning

**Tripous.Avalon** is an evolving framework. This initial release provides the core Data Management foundation, but much more is planned to transform it into a full-featured Enterprise UI suite:

* **The Locator Control:** A sophisticated composite search control for large datasets (Code/Description boxes with Dialog/Dropdown search modes).
* **Runtime UI Customization:** Tools to allow end-users to modify Labels, Titles, and Visibility of controls at runtime.
* **Navigation & Status Bars:** Pre-built controls for record navigation (First, Prior, Next, Last) and data status reporting.
* **User Scripting Integration:** Support for runtime business logic scripts.

---

### License
This project is open-source and available for use in both commercial and personal projects.
