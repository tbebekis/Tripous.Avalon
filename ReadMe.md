# Tripous.Avalon
### A Data-Centric UI Framework for Avalonia

**Tripous.Avalon** is not just another UI library; it's a bridge between complex business data and the Avalonia UI ecosystem. It is designed for developers who build **data-heavy, enterprise-grade applications** where business logic, data integrity, and runtime dynamics are the top priorities.

![Demo App](/Docs/DataApp00.gif)

## The Core Problem
Modern UI frameworks excel at static XAML-based design but often leave the developer struggling with:
* **Manual Synchronization:** Keeping Grids, Forms, and Details in sync as the user navigates.
* **Schema Agnosticism:** Managing `DataTables` and `POCO Lists` with the same codebase.
* **Data Integrity:** Handling Master-Detail relationships, automatic Foreign Key assignments, and Cascade Deletes.
* **Runtime Flexibility:** Binding and configuring UI components programmatically without bloated XAML.

## Our Mission: The "BindingSource" Pattern
We solve these problems by introducing the **BindingSource** pattern, a centralized "data controller" that acts as the single source of truth for your UI. Instead of binding controls directly to raw data, you bind them to a `BindingSource`, which handles:
- **Currency Management:** Tracking the "Current" record across all bound controls.
- **Filtering & Searching:** Smart partial matches and wildcard support.
- **Relational Logic:** Automatic Master-Detail synchronization and state management.
- **Lifecycle Management:** CRUD operations with cancelable events for business validation.

## Key Features

* **Unified Data-Binding:** Use the same C# code to manage and data-bind `System.Data.DataTable` and `IList<T>` to Avalonia controls.
* **One-Line Binding:** Bind any Avalonia control (TextBox, CheckBox, ComboBox, DatePicker, DataGrid, etc.) with a single method call.
* **Advanced Lookup Support:** Effortless binding for lookup controls with automatic synchronization between ID values and display text.
* **Lifecycle Events:** Hook into the data flow with cancelable events for validation and business logic. 
---

## Documentation Index

Explore the core features of the framework:

### [Introduction to BindingSource](/Docs/IntroToBindingSource.md)
* Learn the basics of creating a `BindingSource` from Lists or DataTables.
* Understanding the `fAllRows` (Data) vs `fRows` (View) architecture.

### [Master-Detail Relationships](/Docs/MasterDetail.md)
* Setting up fluent relationships with `AddDetail()`.
* Understanding `BindingRelation` and automatic synchronization.
* **Advanced Data Integrity:** Auto-key assignment and **Recursive Cascade Delete**.
* Real-time search using `SetFilter()`.
* Case-insensitive "Contains" and Wildcard (`*`) support.
 

### [Roadmap & Future Goals](/Docs/Roadmap.md)
- **Full Expression Parser:** Support for complex filters with logical operators (e.g., `(Status == 'Pending' AND Total > 100) OR Priority == 1`).
- **Dynamic Predicates:** Support for custom filtering functions defined at runtime.
- **GetRequiredPropertyNames():** A new method in `IDataSource` that returns only the fields necessary for the current filter or relation, improving performance in lazy-loading scenarios.
- **The Locator System:** Advanced search and select composite controls.

---

## Quick Vision Snippet

```csharp
// Define Master and Detail
var bsMaster = BindingSource.FromTable(dtCustomers);
var bsDetail = BindingSource.FromTable(dtOrders);

// Bind UI controls dynamically
bsMaster.Bind(myGridMaster, true);
bsDetail.Bind(myGridDetail, true);

// Connect them in one line
bsMaster.AddDetail("CustOrders", "Id", bsDetail, "CustomerId");
bsMaster.ActivateDetails(true); 

// Everything else (Sync, Filtering, Cascade Delete) is handled by the framework.