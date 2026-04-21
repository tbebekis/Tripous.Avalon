using System;
using System.Collections;
using System.Data;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Tripous.Data;
using Tripous.Avalon;

using Tests;

namespace DataTest00;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    // ● private fields
    Button fBtnFirst;
    Button fBtnPrev;
    Button fBtnNext;
    Button fBtnLast;
    Button fBtnNew;
    Button fBtnDelete;
    Button fBtnApply;
    ListBox fLstCustomers;
    TextBox fEdtName;
    ComboBox fCboCountry;
    DataGrid fGridCustomers;
    RowSet fCountryRowSet;
    RowSet fCustomerRowSet;
    RowSetView fCountryView;
    RowSetView fCustomerView;
    LookupRegistry fLookups;
    RowSetViewBinder fBinder;
    bool fIsRefreshing;

    // ● event handlers
    async void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
 
    }

    // ● private
    void WindowInitialize()
    { 
        TestData.Initialize();
        
        GetControls();
        InitializeData();
 
        InitializeGrid();
        HookEvents();
        RefreshUi();
 
    }
 
    static public void TestRowSetWithProducts()
    {
     

        // ● create RowSet
        var adapter = new ListSourceAdapter<Product>();
        var rs = new RowSet(TestData.Products, adapter);

        Console.WriteLine($"Rows: {rs.Count}");

        // ● pick first row
        var row = rs[0];
        Console.WriteLine($"Initial Status: {row.RowStatus}");

        // ● modify value
        row.SetValue("Name", row.GetValue("Name") + " (edited)");
        Console.WriteLine($"After SetValue: {row.RowStatus}");

        // ● begin/cancel edit
        row.BeginEdit();
        row.SetValue("Name", "Temp Name");
        Console.WriteLine($"During Edit: {row.GetValue("Name")}");

        row.CancelEdit();
        Console.WriteLine($"After CancelEdit: {row.GetValue("Name")}, Status: {row.RowStatus}");

        // ● begin/commit edit
        row.BeginEdit();
        row.SetValue("Name", "Committed Name");
        row.CommitEdit();
        Console.WriteLine($"After CommitEdit: {row.GetValue("Name")}, Status: {row.RowStatus}");

        // ● new row
        var newRow = rs.NewRow();
        newRow.SetValue("Name", "New Product");
        newRow.SetValue("CategoryId", 1);
        newRow.SetValue("UnitPrice", 100);

        Console.WriteLine($"NewRow Status (before Add): {newRow.RowStatus}");

        rs.AddRow(newRow);
        Console.WriteLine($"NewRow Status (after Add): {newRow.RowStatus}, Rows: {rs.Count}");

        // ● delete row
        rs.DeleteRow(row);
        Console.WriteLine($"After DeleteRow: {row.RowStatus}");

        
        row = rs[1];
        var originalName = (string)row.GetValue("Name");

        row.SetValue("Name", "Something else");
        Console.WriteLine(row.RowStatus); // Modified

        row.SetValue("Name", originalName);
        Console.WriteLine(row.RowStatus); // πρέπει να γίνει Unchanged
        
        Console.WriteLine("Test completed.");
    }
    static public void TestRowSetWithDataTableProducts2()
    {
        // ● init data
        TestData.Initialize();

        // ● create RowSet
        var adapter = new DataTableSourceAdapter();
        var rs = new RowSet(TestData.tblProducts, adapter);

        Console.WriteLine($"Rows: {rs.Count}");

        // ● first row
        var row = rs[0];
        var originalName = (string)row.GetValue("Name");

        Console.WriteLine($"Initial Status: {row.RowStatus}");

        // ● simple modify
        row.SetValue("Name", originalName + " (edited)");
        Console.WriteLine($"After SetValue: {row.RowStatus}");

        // ● edit/cancel
        row.BeginEdit();
        row.SetValue("Name", "Temp Name");
        Console.WriteLine($"During Edit: {row.GetValue("Name")}");

        row.CancelEdit();
        Console.WriteLine($"After CancelEdit: {row.GetValue("Name")}, Status: {row.RowStatus}");

        // ● edit/commit
        row.BeginEdit();
        row.SetValue("Name", "Committed Name");
        row.CommitEdit();
        Console.WriteLine($"After CommitEdit: {row.GetValue("Name")}, Status: {row.RowStatus}");

        // ● back to original
        row.SetValue("Name", originalName);
        Console.WriteLine($"Back To Original: {row.RowStatus}");

        // ● new row
        var newRow = rs.NewRow();
        newRow.SetValue("Id", 9999);
        newRow.SetValue("Name", "New DataTable Product");
        newRow.SetValue("CategoryId", 1);
        newRow.SetValue("UnitPrice", 123.45);

        Console.WriteLine($"NewRow Status (before Add): {newRow.RowStatus}");

        rs.AddRow(newRow);
        Console.WriteLine($"NewRow Status (after Add): {newRow.RowStatus}, Rows: {rs.Count}");

        // ● delete existing row
        rs.DeleteRow(row);
        Console.WriteLine($"After DeleteRow: {row.RowStatus}");

        // ● adapter-level deleted check
        Console.WriteLine($"Adapter IsDeleted: {adapter.IsDeleted(row.DataItem)}");

        Console.WriteLine("Test completed.");
    }
    static public void TestRowSetWithDataTableProducts()
    {
        TestData.Initialize();

        var adapter = new DataTableSourceAdapter();
        var rs = new RowSet(TestData.tblProducts, adapter);

        Console.WriteLine($"Rows: {rs.Count}");

        var row = rs[0];
        var originalName = (string)row.GetValue("Name");

        row.SetValue("Name", originalName + " (edited)");
        Console.WriteLine($"After SetValue: {row.RowStatus}");

        row.BeginEdit();
        row.SetValue("Name", "Temp Name");
        row.CancelEdit();
        Console.WriteLine($"After CancelEdit: {row.GetValue("Name")}, Status: {row.RowStatus}");

        row.BeginEdit();
        row.SetValue("Name", "Committed Name");
        row.CommitEdit();
        Console.WriteLine($"After CommitEdit: {row.GetValue("Name")}, Status: {row.RowStatus}");

        row.SetValue("Name", originalName);
        Console.WriteLine($"Back To Original: {row.RowStatus}");

        var newRow = rs.NewRow();
        newRow.SetValue("Id", 9999);
        newRow.SetValue("Name", "New DataTable Product");
        newRow.SetValue("CategoryId", 1);
        newRow.SetValue("UnitPrice", 123.45);

        rs.AddRow(newRow);
        Console.WriteLine($"NewRow Status: {newRow.RowStatus}, Rows: {rs.Count}");

        rs.DeleteRow(row);
        Console.WriteLine($"After DeleteRow: {row.RowStatus}");
        Console.WriteLine($"Adapter IsDeleted: {adapter.IsDeleted(row.DataItem)}");

        rs.Load();
        Console.WriteLine($"Rows After Reload: {rs.Count}");

        Console.WriteLine("Test completed.");
    }
    static public void TestLookupResolverWithCustomersAndCountries()
    {
        // ● init data
        TestData.Initialize();

        // ● create RowSets
        RowSet countryRowSet = new(TestData.Countries, new ListSourceAdapter<Country>());
        RowSet customerRowSet = new(TestData.Customers, new ListSourceAdapter<Customer>());

        // ● create registry and register lookup source
        LookupRegistry registry = new();
        registry.Register("Countries", countryRowSet);

        // ● create resolver
        LookupResolver resolver = registry.CreateResolver("Countries", "Id", "Name");
        if (resolver == null)
            throw new Exception("Failed to create lookup resolver.");

        // ● test first customer
        Row customerRow = customerRowSet[0];
        object countryId = customerRow.GetValue("CountryId");
        string countryName = resolver.GetDisplayText(countryId);

        Console.WriteLine($"Customer: {customerRow.GetValue("Name")}");
        Console.WriteLine($"CountryId: {countryId}");
        Console.WriteLine($"Resolved Country: {countryName}");

        // ● test direct row resolve
        Row countryRow = resolver.GetRow(countryId);
        Console.WriteLine($"Resolved Row Name: {countryRow?.GetValue("Name")}");

        // ● test invalid key
        string invalidCountryName = resolver.GetDisplayText(999999);
        Console.WriteLine($"Invalid Key Result: '{invalidCountryName}'");

        Console.WriteLine("Lookup test completed.");
    } 
    static public void Test_RowSetView_V1()
    {
        // ● local helpers
        static void Assert(bool Condition, string Message)
        {
            if (!Condition)
                throw new Exception(Message);
        }

        // ● arrange
        TestData.Initialize();

        TestData.tblCountries.AcceptChanges();
        TestData.tblCustomers.AcceptChanges();

        DataTableSourceAdapter Adapter = new();

        RowSet Countries = new(TestData.tblCountries, Adapter);
        RowSet Customers = new(TestData.tblCustomers, Adapter);

        Relation CustomersByCountry = new(
            "CustomersByCountry",
            Countries,
            Customers,
            new[] { Countries.Columns["Id"] },
            new[] { Customers.Columns["CountryId"] }
        );

        RelationContext Context = new(CustomersByCountry);
        RowSetView View = new(Customers)
        {
            RelationContext = Context
        };

        // ● relation filter
        //Context.CurrentMasterRow = Countries[0];   // Greece
        Context.CurrentMasterRow = Countries.FindByKey(Countries, "Id", 1);
        View.Rebuild();

        Assert(View.Count == 3, "Expected 3 customers for Greece.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after rebuild.");
        Assert(Convert.ToInt32(View.CurrentRow["CountryId"]) == 1, "Current row should belong to Greece.");
        
        Console.WriteLine($"View.Count = {View.Count}");
        Console.WriteLine($"CurrentRow = {View.CurrentRow?["Name"]}");
        Console.WriteLine($"MasterRow Country Id = {Context.CurrentMasterRow?["Id"]}");
        Console.WriteLine($"MasterRow Country Name = {Context.CurrentMasterRow?["Name"]}");

        // ● navigation
        Assert(View.First(), "First() failed.");
        Assert(View.CurrentRow != null && Convert.ToInt32(View.CurrentRow["Id"]) == 1, "First() did not move to first row.");

        Assert(View.Next(), "Next() failed.");
        Assert(View.CurrentRow != null && Convert.ToInt32(View.CurrentRow["Id"]) == 2, "Next() did not move to second row.");

        Assert(View.Last(), "Last() failed.");
        Assert(View.CurrentRow != null && Convert.ToInt32(View.CurrentRow["Id"]) == 3, "Last() did not move to last Greece row.");

        Assert(!View.Next(), "Next() should fail at EOF.");
        Assert(View.IsEof, "IsEof should be true at last row.");

        Assert(View.Previous(), "Previous() failed.");
        Assert(View.CurrentRow != null && Convert.ToInt32(View.CurrentRow["Id"]) == 2, "Previous() did not move back.");

        // ● clear current
        View.ClearCurrent();
        Assert(View.CurrentRow == null, "ClearCurrent() failed.");
        Assert(View.IsBof && View.IsEof, "After ClearCurrent(), IsBof and IsEof should both be true.");

        Assert(View.Next(), "Next() after ClearCurrent() failed.");
        Assert(View.CurrentRow != null && Convert.ToInt32(View.CurrentRow["Id"]) == 1, "Next() after ClearCurrent() should move to first.");

        // ● current preservation after rebuild
        View.MoveTo(View[1].Row);
        int CurrentId = Convert.ToInt32(View.CurrentRow["Id"]);

        View.Rebuild();
        Assert(View.CurrentRow != null, "CurrentRow should not be null after rebuild.");
        Assert(Convert.ToInt32(View.CurrentRow["Id"]) == CurrentId, "Current row was not preserved after rebuild.");

        // ● new row and add row
        Row NewRow = View.NewRow();
        NewRow["Id"] = 999;
        NewRow["Name"] = "Test Customer";
        NewRow["CountryId"] = 1;

        View.AddRow(NewRow);

        Assert(View.Count == 4, "AddRow() failed.");
        Assert(View.CurrentRow != null && Convert.ToInt32(View.CurrentRow["Id"]) == 999, "Added row should become current.");
        Assert(View.ContainsRow(NewRow), "View should contain added row.");

        // ● begin / cancel edit
        View.BeginEdit();
        View.CurrentRow["Name"] = "Changed Name";
        View.CancelEdit();

        Assert(Convert.ToString(View.CurrentRow["Name"]) == "Test Customer", "CancelEdit() failed.");

        // ● begin / commit edit
        View.BeginEdit();
        View.CurrentRow["Name"] = "Committed Name";
        View.CommitEdit();

        Assert(Convert.ToString(View.CurrentRow["Name"]) == "Committed Name", "CommitEdit() failed.");

        // ● delete current
        View.Delete();

        Assert(View.Count == 3, "Delete() failed.");
        Assert(!View.ContainsRow(NewRow), "Deleted row should not remain visible.");

        // ● relation change
        Context.CurrentMasterRow = Countries[1];   // Germany
        View.Rebuild();

        Assert(View.Count == 3, "Expected 3 customers for Germany.");
        foreach (RowView RowView in View)
            Assert(Convert.ToInt32(RowView.Row["CountryId"]) == 2, "Relation filter failed for Germany.");

        Console.WriteLine("RowSetView test completed.");
    }
    static public void Test_RowSetView_V1_EdgeCases()
    {
        // ● local helpers
        static void Assert(bool Condition, string Message)
        {
            if (!Condition)
                throw new Exception(Message);
        }
        static Row FindByKey(RowSet rs, string ColumnName, object Value)
        {
            foreach (Row r in rs)
            {
                object v = r[ColumnName];

                if (v == null && Value == null)
                    return r;

                if (v != null && v.Equals(Value))
                    return r;
            }

            return null;
        }

        // ● arrange
        TestData.Initialize();

        TestData.tblCountries.AcceptChanges();
        TestData.tblCustomers.AcceptChanges();

        DataTableSourceAdapter Adapter = new();

        RowSet Countries = new(TestData.tblCountries, Adapter);
        RowSet Customers = new(TestData.tblCustomers, Adapter);

        Relation CustomersByCountry = new(
            "CustomersByCountry",
            Countries,
            Customers,
            new[] { Countries.Columns["Id"] },
            new[] { Customers.Columns["CountryId"] }
        );

        RelationContext Context = new(CustomersByCountry);
        RowSetView View = new(Customers)
        {
            RelationContext = Context
        };

        Context.CurrentMasterRow = FindByKey(Countries, "Id", 1); // Greece
        View.Rebuild();

        Assert(View.Count == 3, "Expected 3 customers for Greece.");

        // ● MoveTo(Row)
        Row Beta = FindByKey(Customers, "Id", 2);
        Assert(Beta != null, "Beta row not found.");
        Assert(View.MoveTo(Beta), "MoveTo(Beta) failed.");
        Assert(ReferenceEquals(View.CurrentRow, Beta), "CurrentRow should be Beta.");

        // ● MoveTo(Row) for non-visible row
        Row GermanyCustomer = FindByKey(Customers, "Id", 4);
        Assert(GermanyCustomer != null, "Germany customer row not found.");
        Assert(!View.MoveTo(GermanyCustomer), "MoveTo(non-visible row) should fail.");
        Assert(ReferenceEquals(View.CurrentRow, Beta), "CurrentRow should remain unchanged after failed MoveTo().");

        // ● ClearCurrent + navigation recovery
        View.ClearCurrent();
        Assert(View.CurrentRow == null, "ClearCurrent() failed.");
        Assert(View.IsBof && View.IsEof, "ClearCurrent() should set both IsBof and IsEof to true.");

        Assert(View.Next(), "Next() after ClearCurrent() failed.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after Next().");
        Assert(Convert.ToInt32(View.CurrentRow["Id"]) == 1, "Next() after ClearCurrent() should go to first row.");

        View.ClearCurrent();
        Assert(View.Previous(), "Previous() after ClearCurrent() failed.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after Previous().");
        Assert(Convert.ToInt32(View.CurrentRow["Id"]) == 3, "Previous() after ClearCurrent() should go to last row.");

        // ● DeleteRow(Row) - delete middle row, current should move to next visible
        Row Alpha = FindByKey(Customers, "Id", 1);
        Beta = FindByKey(Customers, "Id", 2);
        Row Aegean = FindByKey(Customers, "Id", 3);

        Assert(View.MoveTo(Beta), "MoveTo(Beta) failed before delete.");
        View.DeleteRow(Beta);

        Assert(View.Count == 2, "DeleteRow(Beta) failed.");
        Assert(!View.ContainsRow(Beta), "Deleted Beta row should not remain visible.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after deleting middle row.");
        Assert(ReferenceEquals(View.CurrentRow, Aegean), "After deleting middle row, current should move to next visible row.");

        // ● DeleteRow(Row) - delete last visible row, current should move to previous visible
        View.DeleteRow(Aegean);

        Assert(View.Count == 1, "DeleteRow(Aegean) failed.");
        Assert(!View.ContainsRow(Aegean), "Deleted Aegean row should not remain visible.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after deleting last row.");
        Assert(ReferenceEquals(View.CurrentRow, Alpha), "After deleting last visible row, current should move to previous visible row.");

        // ● DeleteRow(Row) - delete only remaining row, current should become null
        View.DeleteRow(Alpha);

        Assert(View.Count == 0, "DeleteRow(Alpha) failed.");
        Assert(View.CurrentRow == null, "CurrentRow should be null when no visible rows remain.");
        Assert(View.IsBof && View.IsEof, "Empty view should have IsBof and IsEof true.");

        // ● current preservation when current row becomes non-visible because of relation change
        TestData.Initialize();

        TestData.tblCountries.AcceptChanges();
        TestData.tblCustomers.AcceptChanges();

        Countries = new RowSet(TestData.tblCountries, Adapter);
        Customers = new RowSet(TestData.tblCustomers, Adapter);
        CustomersByCountry = new Relation(
            "CustomersByCountry",
            Countries,
            Customers,
            new[] { Countries.Columns["Id"] },
            new[] { Customers.Columns["CountryId"] }
        );
        Context = new RelationContext(CustomersByCountry);
        View = new RowSetView(Customers)
        {
            RelationContext = Context
        };

        Context.CurrentMasterRow = FindByKey(Countries, "Id", 1); // Greece
        View.Rebuild();

        Beta = FindByKey(Customers, "Id", 2);
        Assert(View.MoveTo(Beta), "MoveTo(Beta) failed before relation change.");
        Assert(ReferenceEquals(View.CurrentRow, Beta), "CurrentRow should be Beta before relation change.");

        Context.CurrentMasterRow = FindByKey(Countries, "Id", 2); // Germany
        View.Rebuild();

        Assert(View.Count == 3, "Expected 3 customers for Germany.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after relation change.");
        Assert(Convert.ToInt32(View.CurrentRow["CountryId"]) == 2, "CurrentRow should belong to Germany after relation change.");
        Assert(Convert.ToInt32(View.CurrentRow["Id"]) == 4, "When previous current becomes invisible, current should fall to first visible row.");

        Console.WriteLine("RowSetView edge case test completed.");
    }
    static public void Test_RowSetView_Relation_AutoRefresh()
    {
        // ● local helpers
        static void Assert(bool Condition, string Message)
        {
            if (!Condition)
                throw new Exception(Message);
        }
        static Row FindByKey(RowSet rs, string ColumnName, object Value)
        {
            foreach (Row r in rs)
            {
                object v = r[ColumnName];

                if (v == null && Value == null)
                    return r;

                if (v != null && v.Equals(Value))
                    return r;
            }

            return null;
        }

        // ● arrange
        TestData.Initialize();

        TestData.tblCountries.AcceptChanges();
        TestData.tblCustomers.AcceptChanges();

        DataTableSourceAdapter Adapter = new();

        RowSet Countries = new(TestData.tblCountries, Adapter);
        RowSet Customers = new(TestData.tblCustomers, Adapter);

        Relation CustomersByCountry = new(
            "CustomersByCountry",
            Countries,
            Customers,
            new[] { Countries.Columns["Id"] },
            new[] { Customers.Columns["CountryId"] }
        );

        RelationContext Context = new(CustomersByCountry);
        RowSetView View = new(Customers)
        {
            RelationContext = Context
        };

        int RebuildCount = 0;
        View.Rebuilt += (s, e) => RebuildCount++;

        // ● initial state - Greece
        Context.CurrentMasterRow = FindByKey(Countries, "Id", 1);

        Assert(View.Count == 3, "Expected 3 customers for Greece.");
        foreach (RowView rv in View)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 1, "Row should belong to Greece.");

        // ● change to Germany (should auto-rebuild)
        Context.CurrentMasterRow = FindByKey(Countries, "Id", 2);

        Assert(View.Count == 3, "Expected 3 customers for Germany.");
        foreach (RowView rv in View)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 2, "Row should belong to Germany.");

        // ● change to France
        Context.CurrentMasterRow = FindByKey(Countries, "Id", 4);

        Assert(View.Count == 3, "Expected 3 customers for France.");
        foreach (RowView rv in View)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 4, "Row should belong to France.");

        // ● set null master (detail should be empty)
        Context.CurrentMasterRow = null;

        Assert(View.Count == 0, "When master is null, detail should be empty.");

        // ● sanity: ensure rebuilds actually happened
        Assert(RebuildCount >= 3, "Expected multiple rebuilds triggered by context changes.");

        Console.WriteLine("RowSetView relation auto-refresh test completed.");
    }
    static public void Test_RowSetView_Filter()
    {
        // ● local helpers
        static void Assert(bool Condition, string Message)
        {
            if (!Condition)
                throw new Exception(Message);
        }

        // ● arrange
        TestData.Initialize();
        TestData.tblCustomers.AcceptChanges();

        DataTableSourceAdapter Adapter = new();
        RowSet Customers = new(TestData.tblCustomers, Adapter);
        RowSetView View = new(Customers);

        Assert(View.Count == 24, "Expected 24 customers initially.");

        // ● apply filter
        View.Filter = r => Convert.ToInt32(r["CountryId"]) == 1;

        Assert(View.Count == 3, "Expected 3 customers for CountryId = 1 after filter.");
        foreach (RowView rv in View)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 1, "Filtered row should belong to CountryId = 1.");

        // ● current preservation under filter change
        Row Current = View.CurrentRow;
        Assert(Current != null, "CurrentRow should not be null after filtering.");

        View.Filter = r => Convert.ToInt32(r["CountryId"]) == 2;

        Assert(View.Count == 3, "Expected 3 customers for CountryId = 2 after filter change.");
        foreach (RowView rv in View)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 2, "Filtered row should belong to CountryId = 2.");

        Assert(View.CurrentRow != null, "CurrentRow should not be null after filter change.");
        Assert(Convert.ToInt32(View.CurrentRow["CountryId"]) == 2, "CurrentRow should belong to CountryId = 2 after filter change.");

        // ● clear filter
        View.Filter = null;

        Assert(View.Count == 24, "Expected 24 customers after clearing filter.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after clearing filter.");

        // ● filter to empty
        View.Filter = r => false;

        Assert(View.Count == 0, "Expected empty view after false filter.");
        Assert(View.CurrentRow == null, "CurrentRow should be null when filter returns no rows.");
        Assert(View.IsBof && View.IsEof, "Empty filtered view should have IsBof and IsEof true.");

        Console.WriteLine("RowSetView filter test completed.");
    }
    static public void Test_RowSetView_Sort()
    {
        // ● local helpers
        static void Assert(bool Condition, string Message)
        {
            if (!Condition)
                throw new Exception(Message);
        }

        // ● arrange
        TestData.Initialize();
        TestData.tblCustomers.AcceptChanges();

        DataTableSourceAdapter Adapter = new();
        RowSet Customers = new(TestData.tblCustomers, Adapter);
        RowSetView View = new(Customers);

        Assert(View.Count == 24, "Expected 24 customers initially.");

        // ● ascending by Name
        View.Sort = (A, B) => string.Compare(
            Convert.ToString(A["Name"]),
            Convert.ToString(B["Name"]),
            StringComparison.OrdinalIgnoreCase
        );

        Assert(View.Count == 24, "Expected 24 customers after ascending sort.");

        string PrevName = null;
        foreach (RowView rv in View)
        {
            string Name = Convert.ToString(rv.Row["Name"]);
            if (PrevName != null)
                Assert(string.Compare(PrevName, Name, StringComparison.OrdinalIgnoreCase) <= 0, "Ascending sort failed.");

            PrevName = Name;
        }

        // ● current preservation after sort change
        Row TargetRow = null;
        foreach (RowView rv in View)
        {
            if (Convert.ToInt32(rv.Row["Id"]) == 10)
            {
                TargetRow = rv.Row;
                break;
            }
        }

        Assert(TargetRow != null, "Target row not found.");
        Assert(View.MoveTo(TargetRow), "MoveTo(TargetRow) failed.");
        Assert(ReferenceEquals(View.CurrentRow, TargetRow), "CurrentRow should be target row before sort change.");

        // ● descending by Name
        View.Sort = (A, B) => string.Compare(
            Convert.ToString(B["Name"]),
            Convert.ToString(A["Name"]),
            StringComparison.OrdinalIgnoreCase
        );

        Assert(View.Count == 24, "Expected 24 customers after descending sort.");
        Assert(View.CurrentRow != null, "CurrentRow should not be null after sort change.");
        Assert(ReferenceEquals(View.CurrentRow, TargetRow), "Current row should be preserved after sort change.");

        PrevName = null;
        foreach (RowView rv in View)
        {
            string Name = Convert.ToString(rv.Row["Name"]);
            if (PrevName != null)
                Assert(string.Compare(PrevName, Name, StringComparison.OrdinalIgnoreCase) >= 0, "Descending sort failed.");

            PrevName = Name;
        }

        // ● sort with filter together
        View.Filter = r => Convert.ToInt32(r["CountryId"]) == 1;
        Assert(View.Count == 3, "Expected 3 customers after filter.");

        PrevName = null;
        foreach (RowView rv in View)
        {
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 1, "Filter + sort failed on CountryId.");
            string Name = Convert.ToString(rv.Row["Name"]);
            if (PrevName != null)
                Assert(string.Compare(PrevName, Name, StringComparison.OrdinalIgnoreCase) >= 0, "Filter + descending sort failed.");

            PrevName = Name;
        }

        // ● clear sort
        View.Sort = null;
        Assert(View.Count == 3, "Expected 3 customers after clearing sort with filter still active.");

        // ● clear filter too
        View.Filter = null;
        Assert(View.Count == 24, "Expected 24 customers after clearing filter.");

        Console.WriteLine("RowSetView sort test completed.");
    }
    static public void Test_RowSetView_MasterDetail_WithMasterView()
    {
        // ● local helpers
        static void Assert(bool Condition, string Message)
        {
            if (!Condition)
                throw new Exception(Message);
        }

        // ● arrange
        TestData.Initialize();

        TestData.tblCountries.AcceptChanges();
        TestData.tblCustomers.AcceptChanges();

        DataTableSourceAdapter Adapter = new();

        RowSet Countries = new(TestData.tblCountries, Adapter);
        RowSet Customers = new(TestData.tblCustomers, Adapter);

        RowSetView MasterView = new(Countries);
        RowSetView DetailView = new(Customers);

        Relation CustomersByCountry = new(
            "CustomersByCountry",
            Countries,
            Customers,
            new[] { Countries.Columns["Id"] },
            new[] { Customers.Columns["CountryId"] }
        );

        RelationContext Context = new(CustomersByCountry);
        Context.MasterView = MasterView;
        DetailView.RelationContext = Context;

        // ● initial sync from master current row
        Assert(MasterView.CurrentRow != null, "MasterView.CurrentRow should not be null.");
        Assert(Convert.ToInt32(MasterView.CurrentRow["Id"]) == 1, "Expected first master row to be Greece.");
        Assert(Context.CurrentMasterRow != null, "Context.CurrentMasterRow should not be null after MasterView assignment.");
        Assert(ReferenceEquals(Context.CurrentMasterRow, MasterView.CurrentRow), "Context.CurrentMasterRow should track MasterView.CurrentRow.");

        Assert(DetailView.Count == 3, "Expected 3 customers for Greece.");
        foreach (RowView rv in DetailView)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 1, "Detail row should belong to Greece.");

        // ● move master to Germany
        Assert(MasterView.Next(), "MasterView.Next() failed.");
        Assert(MasterView.CurrentRow != null, "MasterView.CurrentRow should not be null after Next().");
        Assert(Convert.ToInt32(MasterView.CurrentRow["Id"]) == 2, "Expected second master row to be Germany.");
        Assert(ReferenceEquals(Context.CurrentMasterRow, MasterView.CurrentRow), "Context should track master after Next().");

        Assert(DetailView.Count == 3, "Expected 3 customers for Germany.");
        foreach (RowView rv in DetailView)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 2, "Detail row should belong to Germany.");

        // ● move master to France
        Assert(MasterView.MoveTo(MasterView[3].Row), "MoveTo(France) failed.");
        Assert(MasterView.CurrentRow != null, "MasterView.CurrentRow should not be null after MoveTo().");
        Assert(Convert.ToInt32(MasterView.CurrentRow["Id"]) == 4, "Expected master row to be France.");
        Assert(ReferenceEquals(Context.CurrentMasterRow, MasterView.CurrentRow), "Context should track master after MoveTo().");

        Assert(DetailView.Count == 3, "Expected 3 customers for France.");
        foreach (RowView rv in DetailView)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 4, "Detail row should belong to France.");

        // ● clear master current
        MasterView.ClearCurrent();

        Assert(MasterView.CurrentRow == null, "MasterView.CurrentRow should be null after ClearCurrent().");
        Assert(Context.CurrentMasterRow == null, "Context.CurrentMasterRow should be null after master ClearCurrent().");
        Assert(DetailView.Count == 0, "DetailView should be empty when master current is null.");

        // ● recover with master navigation
        Assert(MasterView.First(), "MasterView.First() failed.");
        Assert(MasterView.CurrentRow != null, "MasterView.CurrentRow should not be null after First().");
        Assert(Convert.ToInt32(MasterView.CurrentRow["Id"]) == 1, "Expected master row to return to Greece.");
        Assert(ReferenceEquals(Context.CurrentMasterRow, MasterView.CurrentRow), "Context should track master after First().");

        Assert(DetailView.Count == 3, "Expected 3 customers for Greece after recovery.");
        foreach (RowView rv in DetailView)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 1, "Detail row should belong to Greece after recovery.");

        // ● disconnect master view
        Context.MasterView = null;

        Assert(Context.CurrentMasterRow == null, "Context.CurrentMasterRow should be null after disconnect.");
        Assert(DetailView.Count == 0, "DetailView should be empty after disconnect.");

        // ● reconnect master view
        Context.MasterView = MasterView;

        Assert(Context.CurrentMasterRow != null, "Context.CurrentMasterRow should not be null after reconnect.");
        Assert(ReferenceEquals(Context.CurrentMasterRow, MasterView.CurrentRow), "Context should track master after reconnect.");
        Assert(DetailView.Count == 3, "Expected 3 customers after reconnect.");
        foreach (RowView rv in DetailView)
            Assert(Convert.ToInt32(rv.Row["CountryId"]) == 1, "Detail row should belong to Greece after reconnect.");

        Console.WriteLine("RowSetView master-detail integration test completed.");
    }
    
// ● private methods
    void GetControls()
    {
        fBtnFirst = this.FindControl<Button>("btnFirst");
        fBtnPrev = this.FindControl<Button>("btnPrev");
        fBtnNext = this.FindControl<Button>("btnNext");
        fBtnLast = this.FindControl<Button>("btnLast");
        fBtnNew = this.FindControl<Button>("btnNew");
        fBtnDelete = this.FindControl<Button>("btnDelete");
        fBtnApply = this.FindControl<Button>("btnApply");
        fLstCustomers = this.FindControl<ListBox>("lstCustomers");
        fEdtName = this.FindControl<TextBox>("edtName");
        fCboCountry = this.FindControl<ComboBox>("cboCountry");
        fGridCustomers = this.FindControl<DataGrid>("gridCustomers");
    }
    void InitializeData()
    {
        fCountryRowSet = RowSet.Create(TestData.Countries, new ListSourceAdapter<Country>());
        fCustomerRowSet = RowSet.Create(TestData.Customers, new ListSourceAdapter<Customer>());

        fCountryView = new RowSetView(fCountryRowSet);
        fCustomerView = new RowSetView(fCustomerRowSet);

        fLookups = new LookupRegistry();
        fLookups.Register("Countries", fCountryRowSet);

        fBinder = new RowSetViewBinder(fCustomerView, fLookups);
        fBinder.Bind(fEdtName, "Name");
        fBinder.Bind(fCboCountry, "Countries", "CountryId", "Name", "Id");
        fBinder.Bind(fLstCustomers, "Name");
    }
 
    void InitializeGrid()
    {
        fGridCustomers.AutoGenerateColumns = false;
        fGridCustomers.Columns.Clear();
        fGridCustomers.ItemsSource = fCustomerView;

        fGridCustomers.Columns.Add(new DataGridTextColumn()
        {
            Header = "Id",
            Binding = new Binding("Row[Id]")
        });
        fGridCustomers.Columns.Add(new DataGridTextColumn()
        {
            Header = "Name",
            Binding = new Binding("Row[Name]")
        });
        fGridCustomers.Columns.Add(new DataGridTextColumn()
        {
            Header = "CountryId",
            Binding = new Binding("Row[CountryId]")
        });
    }
    void HookEvents()
    {
        fBtnFirst.Click += (s, e) => fCustomerView.First();
        fBtnPrev.Click += (s, e) => fCustomerView.Previous();
        fBtnNext.Click += (s, e) => fCustomerView.Next();
        fBtnLast.Click += (s, e) => fCustomerView.Last();
        fBtnNew.Click += (s, e) => DoNewCustomer();
        fBtnDelete.Click += (s, e) => DoDeleteCustomer();
        fBtnApply.Click += (s, e) => ApplyCurrentCustomer();

        fLstCustomers.SelectionChanged += (s, e) =>
        {
            if (fIsRefreshing)
                return;

            if (fLstCustomers.SelectedItem is RowView RowView)
                fCustomerView.CurrentRowView = RowView;
        };

        fGridCustomers.SelectionChanged += (s, e) =>
        {
            if (fIsRefreshing)
                return;

            if (fGridCustomers.SelectedItem is RowView RowView)
                fCustomerView.CurrentRowView = RowView;
        };

        fCustomerView.CurrentRowViewChanged += (s, e) => RefreshUi();
        fCustomerView.Rebuilt += (s, e) => RefreshUi();
    }
    void RefreshUi()
    {
        if (fIsRefreshing)
            return;

        fIsRefreshing = true;
        try
        {
            fBinder.Refresh();
            fGridCustomers.ItemsSource = null;
            fGridCustomers.ItemsSource = fCustomerView;
            fGridCustomers.SelectedItem = fCustomerView.CurrentRowView;
            RefreshButtons();
        }
        finally
        {
            fIsRefreshing = false;
        }
    } 
 
    void RefreshGridSelection()
    {
        fGridCustomers.SelectedItem = fCustomerView.CurrentRowView;
    }
 
    void RefreshButtons()
    {
        bool HasRow = fCustomerView.CurrentRow != null;
        fBtnDelete.IsEnabled = HasRow;
        fBtnApply.IsEnabled = HasRow;
    }
    RowView FindCountryRowView(object CountryId)
    {
        foreach (RowView Item in fCountryView)
        {
            if (Equals(Item.Row["Id"], CountryId))
                return Item;
        }

        return null;
    }
    int GetSelectedCountryId()
    {
        if (fCboCountry.SelectedItem is RowView CountryRowView)
            return Convert.ToInt32(CountryRowView.Row["Id"]);

        return 0;
    }
    int GetNextCustomerId()
    {
        int Result = 0;

        foreach (Row Row in fCustomerRowSet)
            Result = Math.Max(Result, Convert.ToInt32(Row["Id"]));

        return Result + 1;
    }
    void DoNewCustomer()
    {
        Row Row = fCustomerView.NewRow();

        Row["Id"] = GetNextCustomerId();
        Row["Name"] = "New Customer";
        Row["CountryId"] = 1;

        fCustomerView.AddRow(Row);
    }
    void DoDeleteCustomer()
    {
        if (fCustomerView.CurrentRow == null)
            return;

        fCustomerView.Delete();
    }
    void ApplyCurrentCustomer()
    {
        Row Row = fCustomerView.CurrentRow;
        if (Row == null)
            return;

        Row.BeginEdit();
        try
        {
            Row["Name"] = fEdtName.Text;
            Row["CountryId"] = GetSelectedCountryId();
            Row.CommitEdit();
        }
        catch
        {
            Row.CancelEdit();
            throw;
        }

        fCustomerView.Refresh();
    }

    
    
    // ● construction
    public MainWindow()
    {
        InitializeComponent();
 
        this.Loaded += (s, e) =>
        {
            if (IsWindowInitialized)
                return;
            WindowInitialize();
            IsWindowInitialized = true;
        };
    }
}