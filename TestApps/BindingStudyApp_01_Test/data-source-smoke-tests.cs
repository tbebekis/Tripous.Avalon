using System.ComponentModel;
using System.Data;
using Tripous.Data;

namespace BindingStudyApp_01_Test;

public class DataSourceSmokeTests
{
    // ● private
    DataTable CreateCustomerTable()
    {
        DataTable Table = new("Customer");
        Table.Columns.Add("Id", typeof(int));
        Table.Columns.Add("Code", typeof(string));
        Table.Columns.Add("Name", typeof(string));
        Table.Columns.Add("IsActive", typeof(bool));
        Table.Rows.Add(1, "CUST-001", "Acme Stores", true);
        Table.Rows.Add(2, "CUST-002", "Northwind Traders", true);
        Table.Rows.Add(3, "CUST-003", "Contoso Retail", false);
        return Table;
    }
    DataTable CreateOrderTable()
    {
        DataTable Table = new("Order");
        Table.Columns.Add("Id", typeof(int));
        Table.Columns.Add("CustomerId", typeof(int));
        Table.Columns.Add("Code", typeof(string));
        Table.Columns.Add("Amount", typeof(decimal));
        Table.Rows.Add(1, 1, "ORD-001", 120.50m);
        Table.Rows.Add(2, 1, "ORD-002", 340.00m);
        Table.Rows.Add(3, 2, "ORD-003", 90.25m);
        Table.Rows.Add(4, 3, "ORD-004", 840.90m);
        return Table;
    }
    List<CustomerPoco> CreateCustomerPocoList()
    {
        return new()
        {
            new CustomerPoco { Id = 1, Code = "POCO-001", Name = "Poco One", IsActive = true },
            new CustomerPoco { Id = 2, Code = "POCO-002", Name = "Poco Two", IsActive = false },
            new CustomerPoco { Id = 3, Code = "POCO-003", Name = "Poco Three", IsActive = true }
        };
    }
    List<OrderPoco> CreateOrderPocoList()
    {
        return new()
        {
            new OrderPoco { Id = 1, CustomerId = 1, Code = "ORD-001", Amount = 120.50m },
            new OrderPoco { Id = 2, CustomerId = 1, Code = "ORD-002", Amount = 340.00m },
            new OrderPoco { Id = 3, CustomerId = 2, Code = "ORD-003", Amount = 90.25m },
            new OrderPoco { Id = 4, CustomerId = 3, Code = "ORD-004", Amount = 840.90m }
        };
    }

    // ● public
    [Fact]
    public void DataTableSource_LoadsRowsAndNavigates()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());

        Assert.Equal("Customer", Source.Name);
        Assert.Equal(3, Source.Count);
        Assert.Equal(0, Source.Position);
        Assert.Equal("CUST-001", Source.Current.AsString("Code"));

        Source.MoveNext();

        Assert.Equal(1, Source.Position);
        Assert.Equal("CUST-002", Source.Current.AsString("Code"));
        Assert.False(Source.IsBof);
        Assert.False(Source.IsEof);
    }
    [Fact]
    public void DataViewSource_UsesViewFilterAndSort()
    {
        DataTable Table = CreateCustomerTable();
        DataView View = new(Table);
        View.RowFilter = "IsActive = true";
        View.Sort = "Code DESC";
        DataSource Source = DataSource.FromDataView(View);

        Assert.Equal("Customer", Source.Name);
        Assert.Equal(2, Source.Count);
        Assert.Equal("CUST-002", Source.Current.AsString("Code"));

        Source.MoveNext();

        Assert.Equal("CUST-001", Source.Current.AsString("Code"));
    }
    [Fact]
    public void DataSource_SetFilter_FiltersDataTableRows()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());

        Source.SetFilter("Name", "North");

        Assert.Single(Source.Rows);
        Assert.Equal("Northwind Traders", Source.Current.AsString("Name"));

        Source.CancelFilter();

        Assert.Equal(3, Source.Count);
    }
    [Fact]
    public void DataSource_SetFilter_SupportsStringPrefix()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());

        Source.SetFilter("Code", "CUST-00*");

        Assert.Equal(3, Source.Count);
    }
    [Fact]
    public void DataSource_SetFilter_FiltersPocoRows()
    {
        DataSource Source = DataSource.FromList(CreateCustomerPocoList());

        Source.SetFilter("Name", "Two");

        Assert.Single(Source.Rows);
        Assert.Equal("Poco Two", Source.Current.AsString("Name"));
    }
    [Fact]
    public void DataSource_SetFilter_RefreshesAfterPocoChange()
    {
        List<CustomerPoco> List = CreateCustomerPocoList();
        DataSource Source = DataSource.FromList(List);

        Source.SetFilter("Name", "Acme");
        List[1].Name = "Acme Poco";

        Assert.Single(Source.Rows);
        Assert.Equal("Acme Poco", Source.Current.AsString("Name"));
    }
    [Fact]
    public void DataSource_SetFilter_RefreshesAfterRowChange()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());

        Source.SetFilter("Name", "Acme");
        Source.AllRows[1]["Name"] = "Acme Branch";

        Assert.Equal(2, Source.Count);
    }
    [Fact]
    public void DataSourceList_FindsAndGetsByName()
    {
        DataSourceList List = new();
        DataSource Customers = DataSource.FromTable(CreateCustomerTable());
        DataSource Orders = DataSource.FromTable(CreateOrderTable());

        List.Add(Customers);
        List.Add(Orders);

        Assert.Same(Customers, List.Find("customer"));
        Assert.Same(Orders, List.Get("ORDER"));
        Assert.Null(List.Find("Missing"));
        Assert.Throws<ApplicationException>(() => List.Get("Missing"));
    }
    [Fact]
    public void DataSourceList_RejectsInvalidItems()
    {
        DataSourceList List = new();
        DataSource Customers = DataSource.FromTable(CreateCustomerTable());
        DataSource DuplicateCustomers = DataSource.FromTable(CreateCustomerTable(), "customer");
        DataSource Unnamed = DataSource.FromList(CreateCustomerPocoList());

        List.Add(Customers);

        Assert.Throws<ArgumentNullException>(() => List.Add(null));
        Assert.Throws<ApplicationException>(() => List.Add(Unnamed));
        Assert.Throws<ApplicationException>(() => List.Add(DuplicateCustomers));
    }
    [Fact]
    public void DataSourceList_AssignsAndClearsOwner()
    {
        object Owner = new();
        DataSourceList List = new() { Owner = Owner };
        DataSource Customers = DataSource.FromTable(CreateCustomerTable());
        DataSource Orders = DataSource.FromTable(CreateOrderTable());

        List.Add(Customers);

        Assert.Same(Owner, Customers.Owner);

        List[0] = Orders;

        Assert.Null(Customers.Owner);
        Assert.Same(Owner, Orders.Owner);

        List.Remove(Orders);

        Assert.Null(Orders.Owner);
    }
    [Fact]
    public void DataSourceList_ClearClearsOwner()
    {
        object Owner = new();
        DataSourceList List = new() { Owner = Owner };
        DataSource Customers = DataSource.FromTable(CreateCustomerTable());
        DataSource Orders = DataSource.FromTable(CreateOrderTable());

        List.Add(Customers);
        List.Add(Orders);
        List.Clear();

        Assert.Null(Customers.Owner);
        Assert.Null(Orders.Owner);
    }
    [Fact]
    public void DataSourceList_OwnerChangeUpdatesExistingItems()
    {
        object Owner = new();
        object NewOwner = new();
        DataSourceList List = new() { Owner = Owner };
        DataSource Customers = DataSource.FromTable(CreateCustomerTable());
        DataSource Orders = DataSource.FromTable(CreateOrderTable());

        List.Add(Customers);
        List.Add(Orders);
        List.Owner = NewOwner;

        Assert.Same(NewOwner, Customers.Owner);
        Assert.Same(NewOwner, Orders.Owner);

        List.Clear();

        Assert.Null(Customers.Owner);
        Assert.Null(Orders.Owner);
    }
    [Fact]
    public void DataSourceRow_ChangesUnderlyingDataRowView()
    {
        DataTable Table = CreateCustomerTable();
        DataSource Source = DataSource.FromTable(Table);

        Source.Current["Code"] = "EDIT-001";
        Source.Current["IsActive"] = false;

        Assert.Equal("EDIT-001", Table.Rows[0]["Code"]);
        Assert.False((bool)Table.Rows[0]["IsActive"]);
    }
    [Fact]
    public void ExternalDataRowChange_RefreshesDataSourceRow()
    {
        DataTable Table = CreateCustomerTable();
        DataSource Source = DataSource.FromTable(Table);
        bool Changed = false;
        bool SourceChanged = false;

        Source.Current.PropertyChanged += (Sender, Args) =>
        {
            if (Args.PropertyName == "Code")
                Changed = true;
        };
        Source.Changed += (Sender, Args) =>
        {
            if (Args.FieldName == "Code")
                SourceChanged = true;
        };

        Table.Rows[0]["Code"] = "ROW-001";

        Assert.True(Changed);
        Assert.True(SourceChanged);
        Assert.Equal("ROW-001", Source.Current.AsString("Code"));
    }
    [Fact]
    public void AddAndDeleteRows_UpdateCountAndCurrent()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        DataSourceRow Row = Source.AddNew();
        Row["Id"] = 4;
        Row["Code"] = "CUST-004";
        Row["Name"] = "New Customer";

        Assert.Equal(4, Source.Count);
        Assert.Equal("CUST-004", Source.Current.AsString("Code"));

        bool Deleted = Source.DeleteCurrent();

        Assert.True(Deleted);
        Assert.Equal(3, Source.Count);
    }
    [Fact]
    public void MasterDetail_FiltersDetailRowsByCurrentMaster()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");

        Assert.Equal(2, Orders.Count);
        Assert.All(Orders.Rows, Row => Assert.Equal(1, Row.AsInteger("CustomerId")));

        Customers.MoveNext();

        Assert.Single(Orders.Rows);
        Assert.Equal(2, Orders.Current.AsInteger("CustomerId"));
    }
    [Fact]
    public void DetailsActive_DisablesDetailRefresh()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");

        Customers.ActivateDetails(false);
        Customers.MoveNext();

        Assert.Equal(2, Orders.Count);
        Assert.All(Orders.Rows, Row => Assert.Equal(1, Row.AsInteger("CustomerId")));

        Customers.ActivateDetails(true);

        Assert.Single(Orders.Rows);
        Assert.Equal(2, Orders.Current.AsInteger("CustomerId"));
    }
    [Fact]
    public void AddDetail_CreatesRelationNameFromSourceNames()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        DataSourceRelation Relation = Customers.AddDetail(Orders, "Id", "CustomerId");

        Assert.Equal("Customers_TO_Orders", Relation.Name);
        Assert.Equal("Customers_TO_Orders", Relation.ToString());
        Assert.Equal("Customers", Customers.ToString());
    }
    [Fact]
    public void RemoveDetail_RemovesByChild()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");

        Customers.RemoveDetail(Orders);

        Assert.Empty(Customers.Details);
        Assert.Empty(Customers.Relations);
        Assert.Null(Orders.Master);
        Assert.Equal(4, Orders.Count);
    }
    [Fact]
    public void RemoveDetail_RemovesByName()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");

        Customers.RemoveDetail("orders");

        Assert.Empty(Customers.Details);
        Assert.Empty(Customers.Relations);
        Assert.Null(Orders.Master);
        Assert.Equal(4, Orders.Count);
    }
    [Fact]
    public void AddDetail_RequiresChild()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");

        Assert.Throws<ArgumentNullException>(() => Customers.AddDetail(null, "Id", "CustomerId"));
    }
    [Fact]
    public void AddDetail_RequiresMasterName()
    {
        DataTable Table = CreateCustomerTable();
        Table.TableName = string.Empty;
        DataSource Customers = DataSource.FromTable(Table);
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");

        Assert.Throws<ApplicationException>(() => Customers.AddDetail(Orders, "Id", "CustomerId"));
    }
    [Fact]
    public void AddDetail_RequiresDetailName()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataTable Table = CreateOrderTable();
        Table.TableName = string.Empty;
        DataSource Orders = DataSource.FromTable(Table);

        Assert.Throws<ApplicationException>(() => Customers.AddDetail(Orders, "Id", "CustomerId"));
    }
    [Fact]
    public void AddDetail_RejectsSelfDetail()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");

        Assert.Throws<ApplicationException>(() => Customers.AddDetail(Customers, "Id", "Id"));
    }
    [Fact]
    public void AddDetail_RequiresParentFields()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");

        Assert.Throws<ArgumentException>(() => Customers.AddDetail(Orders, Array.Empty<string>(), new[] { "CustomerId" }));
    }
    [Fact]
    public void AddDetail_RequiresChildFields()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");

        Assert.Throws<ArgumentException>(() => Customers.AddDetail(Orders, new[] { "Id" }, Array.Empty<string>()));
    }
    [Fact]
    public void AddDetail_RequiresSameFieldCount()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");

        Assert.Throws<ArgumentException>(() => Customers.AddDetail(Orders, new[] { "Id", "Code" }, new[] { "CustomerId" }));
    }
    [Fact]
    public void AddDetail_RejectsDuplicateChild()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");

        Assert.Throws<ApplicationException>(() => Customers.AddDetail(Orders, "Id", "CustomerId"));
    }
    [Fact]
    public void AddDetail_RejectsChildWithExistingMaster()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource OtherCustomers = DataSource.FromTable(CreateCustomerTable(), "OtherCustomers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        OtherCustomers.AddDetail(Orders, "Id", "CustomerId");

        Assert.Throws<ApplicationException>(() => Customers.AddDetail(Orders, "Id", "CustomerId"));
    }
    [Fact]
    public void CascadeDeleteRule_RestrictBlocksMasterDelete()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");
        Customers.CascadeDeleteRule = CascadeDeleteRule.Restrict;

        bool Deleted = Customers.DeleteCurrent();

        Assert.False(Deleted);
        Assert.Equal(3, Customers.Count);
    }
    [Fact]
    public void CascadeDeleteRule_CascadeDeletesDetailRows()
    {
        DataSource Customers = DataSource.FromTable(CreateCustomerTable(), "Customers");
        DataSource Orders = DataSource.FromTable(CreateOrderTable(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");
        Customers.CascadeDeleteRule = CascadeDeleteRule.Cascade;

        bool Deleted = Customers.DeleteCurrent();

        Assert.True(Deleted);
        Assert.Equal(2, Customers.Count);
        Assert.DoesNotContain(Orders.AllRows, Row => Row.AsInteger("CustomerId") == 1);
    }
    [Fact]
    public void ListProvider_LoadsAndUpdatesPocoItems()
    {
        List<CustomerPoco> List = CreateCustomerPocoList();
        DataSource Source = DataSource.FromList(List);
        bool Changed = false;

        Source.Current.PropertyChanged += (Sender, Args) =>
        {
            if (Args.PropertyName == "Code")
                Changed = true;
        };

        List[0].Code = "POCO-EDIT";

        Assert.Equal(3, Source.Count);
        Assert.True(Changed);
        Assert.Equal("POCO-EDIT", Source.Current.AsString("Code"));
    }
    [Fact]
    public void PocoMasterDetail_FiltersDetailRowsByCurrentMaster()
    {
        DataSource Customers = DataSource.FromList(CreateCustomerPocoList(), "Customers");
        DataSource Orders = DataSource.FromList(CreateOrderPocoList(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");

        Assert.Equal(2, Orders.Count);
        Assert.All(Orders.Rows, Row => Assert.Equal(1, Row.AsInteger("CustomerId")));

        Customers.MoveNext();

        Assert.Single(Orders.Rows);
        Assert.Equal(2, Orders.Current.AsInteger("CustomerId"));
    }
    [Fact]
    public void PocoCascadeDeleteRule_RestrictBlocksMasterDelete()
    {
        DataSource Customers = DataSource.FromList(CreateCustomerPocoList(), "Customers");
        DataSource Orders = DataSource.FromList(CreateOrderPocoList(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");
        Customers.CascadeDeleteRule = CascadeDeleteRule.Restrict;

        bool Deleted = Customers.DeleteCurrent();

        Assert.False(Deleted);
        Assert.Equal(3, Customers.Count);
        Assert.Equal(4, Orders.AllRows.Count);
    }
    [Fact]
    public void PocoCascadeDeleteRule_CascadeDeletesDetailRows()
    {
        DataSource Customers = DataSource.FromList(CreateCustomerPocoList(), "Customers");
        DataSource Orders = DataSource.FromList(CreateOrderPocoList(), "Orders");
        Customers.AddDetail(Orders, "Id", "CustomerId");
        Customers.CascadeDeleteRule = CascadeDeleteRule.Cascade;

        bool Deleted = Customers.DeleteCurrent();

        Assert.True(Deleted);
        Assert.Equal(2, Customers.Count);
        Assert.DoesNotContain(Orders.AllRows, Row => Row.AsInteger("CustomerId") == 1);
    }
    [Fact]
    public void PositionEvents_FireAndCanCancelMove()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        bool Changing = false;
        bool Changed = false;

        Source.PositionChanging += (Sender, Args) =>
        {
            if (!Changing)
            {
                Changing = true;
                Assert.Equal(0, Args.OldPosition);
                Assert.Equal(1, Args.NewPosition);
            }
        };
        Source.PositionChanged += (Sender, Args) =>
        {
            if (!Changed)
            {
                Changed = true;
                Assert.Equal(0, Args.OldPosition);
                Assert.Equal(1, Args.NewPosition);
            }
        };

        bool Moved = Source.MoveNext();

        Assert.True(Moved);
        Assert.True(Changing);
        Assert.True(Changed);
        Assert.Equal(1, Source.Position);

        Source.PositionChanging += (Sender, Args) => Args.Cancel = true;

        Moved = Source.MoveNext();

        Assert.True(Moved);
        Assert.Equal(1, Source.Position);
    }
    [Fact]
    public void ChangeEvents_FireAndCanCancelValueChange()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        bool Changing = false;
        bool Changed = false;

        Source.Changing += (Sender, Args) =>
        {
            if (!Changing)
            {
                Changing = true;
                Assert.Equal("Code", Args.FieldName);
                Assert.Equal("CUST-001", Args.OldValue);
                Assert.Equal("EDIT-001", Args.NewValue);
            }
        };
        Source.Changed += (Sender, Args) =>
        {
            Changed = true;
            Assert.Equal("Code", Args.FieldName);
            Assert.Equal("EDIT-001", Args.NewValue);
        };

        Source.Current["Code"] = "EDIT-001";

        Assert.True(Changing);
        Assert.True(Changed);
        Assert.Equal("EDIT-001", Source.Current.AsString("Code"));

        Source.Changing += (Sender, Args) => Args.Cancel = true;
        Source.Current["Code"] = "BLOCKED";

        Assert.Equal("EDIT-001", Source.Current.AsString("Code"));
    }
    [Fact]
    public void AddDeleteEvents_FireAndCanCancel()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        bool Adding = false;
        bool Added = false;
        bool Deleting = false;
        bool Deleted = false;

        Source.Adding += (Sender, Args) => Adding = true;
        Source.Added += (Sender, Args) => Added = true;
        Source.Deleting += (Sender, Args) => Deleting = true;
        Source.Deleted += (Sender, Args) => Deleted = true;

        DataSourceRow Row = Source.AddNew();

        Assert.True(Adding);
        Assert.True(Added);
        Assert.Equal(4, Source.Count);

        bool Result = Source.DeleteCurrent();

        Assert.True(Result);
        Assert.True(Deleting);
        Assert.True(Deleted);
        Assert.Equal(3, Source.Count);

        Source.Adding += (Sender, Args) => Args.Cancel = true;
        Row = Source.NewRow();
        Source.AddRow(Row);

        Assert.Equal(3, Source.Count);

        Source.Deleting += (Sender, Args) => Args.Cancel = true;
        Result = Source.DeleteCurrent();

        Assert.False(Result);
        Assert.Equal(3, Source.Count);
    }
    [Fact]
    public void LoadClearEvents_FireAndCanCancel()
    {
        DataSource Source = DataSource.FromTable(CreateCustomerTable());
        bool Loading = false;
        bool Loaded = false;
        bool Clearing = false;
        bool Cleared = false;

        Source.Loading += (Sender, Args) => Loading = true;
        Source.Loaded += (Sender, Args) => Loaded = true;
        Source.Clearing += (Sender, Args) => Clearing = true;
        Source.Cleared += (Sender, Args) => Cleared = true;

        Source.Load();
        Source.Clear();

        Assert.True(Loading);
        Assert.True(Loaded);
        Assert.True(Clearing);
        Assert.True(Cleared);
        Assert.True(Source.IsEmpty);

        Source.Loading += (Sender, Args) => Args.Cancel = true;
        Source.Load();

        Assert.True(Source.IsEmpty);
    }

    public class CustomerPoco: INotifyPropertyChanged
    {
        // ● private fields
        int fId;
        string fCode;
        string fName;
        bool fIsActive;

        // ● private
        void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        // ● properties
        public int Id
        {
            get => fId;
            set
            {
                fId = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string Code
        {
            get => fCode;
            set
            {
                fCode = value;
                OnPropertyChanged(nameof(Code));
            }
        }
        public string Name
        {
            get => fName;
            set
            {
                fName = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public bool IsActive
        {
            get => fIsActive;
            set
            {
                fIsActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        // ● events
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class OrderPoco: INotifyPropertyChanged
    {
        // ● private fields
        int fId;
        int fCustomerId;
        string fCode;
        decimal fAmount;

        // ● private
        void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        // ● properties
        public int Id
        {
            get => fId;
            set
            {
                fId = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public int CustomerId
        {
            get => fCustomerId;
            set
            {
                fCustomerId = value;
                OnPropertyChanged(nameof(CustomerId));
            }
        }
        public string Code
        {
            get => fCode;
            set
            {
                fCode = value;
                OnPropertyChanged(nameof(Code));
            }
        }
        public decimal Amount
        {
            get => fAmount;
            set
            {
                fAmount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        // ● events
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
