/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Edits the tERP application default properties configuration object.
/// </summary>
[TypeStore]
public partial class AppDefaultPropertiesEditor : UserControl, IConfigEditor
{
    // ● private fields
    readonly Dictionary<string, List<LookupOption>> fLookupLists = new();
    readonly Dictionary<ComboBox, string> fPropertyNames = new();
    readonly Dictionary<ComboBox, bool> fComboBoxIsSales = new();
    readonly string[] fRequiredLineGridFields = ["ProductCode", "Quantity", "UnitPrice", "TotalAmount"];
    readonly string[] fAvailableLineGridFields = [
        "DisplayOrder",
        "LineTypeId",
        "ProductCode",
        "ProductName",
        "TaxProductGroupId",
        "Description",
        "WarehouseId",
        "UnitOfMeasureId",
        "UnitOfMeasureName",
        "Quantity",
        "PrimaryUnitQuantity",
        "ReservedQuantity",
        "ExecutedQuantity",
        "InvoicedQuantity",
        "ReturnedQuantity",
        "CreditedQuantity",
        "UnitPrice",
        "GrossAmount",
        "DiscountPercent",
        "DiscountAmount",
        "DocumentDiscountAmount",
        "NetUnitPrice",
        "NetAmount",
        "TaxPercent",
        "TaxAmount",
        "TotalAmount",
    ];
    AppDefaultProperties fDefaults = new();
    NumericUpDown fSalesDefaultQuantity;
    CheckBox fSalesAllowZeroUnitPrice;
    TextBox fSalesPriceResolverClassName;
    TextBox fSalesTaxResolverClassName;
    NumericUpDown fPurchaseDefaultQuantity;
    CheckBox fPurchaseAllowZeroUnitPrice;
    TextBox fPurchasePriceResolverClassName;
    TextBox fPurchaseTaxResolverClassName;
    string fCurrentLineGridKind = "Sales";
    bool fLoadingControls;

    // ● private methods
    void AddLookupList(string Key, string TableName)
    {
        MemTable Table = Config.Store.Select($"select Id, Code, Name from {TableName} order by Code");
        List<LookupOption> Items = new();
        foreach (DataRow Row in Table.Rows)
            Items.Add(new LookupOption(Row.AsString("Id"), Row.AsString("Code"), Row.AsString("Name")));
        fLookupLists[Key] = Items;
    }
    void LoadLookupLists()
    {
        fLookupLists.Clear();
        AddLookupList("WarehouseId", "Warehouse");
        AddLookupList("CostCenterId", "CostCenter");
        AddLookupList("BranchId", "CompanyBranch");
        AddLookupList("CurrencyId", "Currency");
        AddLookupList("PaymentMethodId", "PaymentMethod");
        AddLookupList("PaymentTermId", "PaymentTerm");
        AddLookupList("PriceListTypeId", "PriceListType");
        AddLookupList("TaxBusinessGroupId", "TaxBusinessGroup");
        AddLookupList("OriginTaxJurisdictionId", "TaxJurisdiction");
        AddLookupList("DestinationTaxJurisdictionId", "TaxJurisdiction");
    }
    TextBlock CreateLabel(string Text)
    {
        TextBlock Result = new();
        Result.Text = Text;
        Result.VerticalAlignment = VerticalAlignment.Center;
        return Result;
    }
    Grid CreateRow(string LabelText, Control Control)
    {
        Grid Result = new();
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(220)));
        Result.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        Result.Children.Add(CreateLabel(LabelText));
        Grid.SetColumn(Control, 1);
        Result.Children.Add(Control);
        return Result;
    }
    ComboBox CreateLookupComboBox(string PropertyName, bool IsSales)
    {
        ComboBox Result = new();
        Result.ItemsSource = fLookupLists[PropertyName];
        fPropertyNames[Result] = PropertyName;
        fComboBoxIsSales[Result] = IsSales;
        return Result;
    }
    TextBox CreateTextBox()
    {
        TextBox Result = new();
        return Result;
    }
    NumericUpDown CreateQuantityBox()
    {
        NumericUpDown Result = new();
        Result.Minimum = 0;
        Result.Increment = 1;
        return Result;
    }
    CheckBox CreateCheckBox()
    {
        CheckBox Result = new();
        return Result;
    }
    void AddDefaultRows(StackPanel Panel, bool IsSales)
    {
        Panel.Children.Clear();
        Panel.Children.Add(CreateRow("Warehouse", CreateLookupComboBox("WarehouseId", IsSales)));
        Panel.Children.Add(CreateRow("Cost Center", CreateLookupComboBox("CostCenterId", IsSales)));
        Panel.Children.Add(CreateRow("Branch", CreateLookupComboBox("BranchId", IsSales)));
        Panel.Children.Add(CreateRow("Currency", CreateLookupComboBox("CurrencyId", IsSales)));
        Panel.Children.Add(CreateRow("Payment Method", CreateLookupComboBox("PaymentMethodId", IsSales)));
        Panel.Children.Add(CreateRow("Payment Term", CreateLookupComboBox("PaymentTermId", IsSales)));
        Panel.Children.Add(CreateRow("Price List Type", CreateLookupComboBox("PriceListTypeId", IsSales)));
        Panel.Children.Add(CreateRow("Tax Business Group", CreateLookupComboBox("TaxBusinessGroupId", IsSales)));
        Panel.Children.Add(CreateRow("Origin Tax Jurisdiction", CreateLookupComboBox("OriginTaxJurisdictionId", IsSales)));
        Panel.Children.Add(CreateRow("Destination Tax Jurisdiction", CreateLookupComboBox("DestinationTaxJurisdictionId", IsSales)));
        NumericUpDown DefaultQuantity = CreateQuantityBox();
        CheckBox AllowZeroUnitPrice = CreateCheckBox();
        TextBox PriceResolverClassName = CreateTextBox();
        TextBox TaxResolverClassName = CreateTextBox();
        Panel.Children.Add(CreateRow("Default Quantity", DefaultQuantity));
        Panel.Children.Add(CreateRow("Allow Zero Unit Price", AllowZeroUnitPrice));
        Panel.Children.Add(CreateRow("Price Resolver Class", PriceResolverClassName));
        Panel.Children.Add(CreateRow("Tax Resolver Class", TaxResolverClassName));
        if (IsSales)
        {
            fSalesDefaultQuantity = DefaultQuantity;
            fSalesAllowZeroUnitPrice = AllowZeroUnitPrice;
            fSalesPriceResolverClassName = PriceResolverClassName;
            fSalesTaxResolverClassName = TaxResolverClassName;
        }
        else
        {
            fPurchaseDefaultQuantity = DefaultQuantity;
            fPurchaseAllowZeroUnitPrice = AllowZeroUnitPrice;
            fPurchasePriceResolverClassName = PriceResolverClassName;
            fPurchaseTaxResolverClassName = TaxResolverClassName;
        }
    }
    void SetComboValue(ComboBox ComboBox, string Id)
    {
        ComboBox.SelectedItem = null;
        if (ComboBox.ItemsSource is IEnumerable<LookupOption> Items)
            ComboBox.SelectedItem = Items.FirstOrDefault(Item => Item.Id == Id);
    }
    string GetComboValue(ComboBox ComboBox)
    {
        if (ComboBox.SelectedItem is LookupOption Item)
            return Item.Id;
        return string.Empty;
    }
    object GetDefaults(bool IsSales) => IsSales ? fDefaults.Sales : fDefaults.Purchase;
    void LoadDefaultValues(bool IsSales)
    {
        object Defaults = GetDefaults(IsSales);
        foreach (var Pair in fPropertyNames)
        {
            bool IsSalesCombo = fComboBoxIsSales[Pair.Key];
            if (IsSalesCombo != IsSales)
                continue;
            PropertyInfo PI = Defaults.GetType().GetProperty(Pair.Value);
            SetComboValue(Pair.Key, PI.GetValue(Defaults) as string);
        }
        if (IsSales)
        {
            fSalesDefaultQuantity.Value = fDefaults.Sales.DefaultQuantity;
            fSalesAllowZeroUnitPrice.IsChecked = fDefaults.Sales.AllowZeroUnitPrice;
            fSalesPriceResolverClassName.Text = fDefaults.Sales.PriceResolverClassName;
            fSalesTaxResolverClassName.Text = fDefaults.Sales.TaxResolverClassName;
        }
        else
        {
            fPurchaseDefaultQuantity.Value = fDefaults.Purchase.DefaultQuantity;
            fPurchaseAllowZeroUnitPrice.IsChecked = fDefaults.Purchase.AllowZeroUnitPrice;
            fPurchasePriceResolverClassName.Text = fDefaults.Purchase.PriceResolverClassName;
            fPurchaseTaxResolverClassName.Text = fDefaults.Purchase.TaxResolverClassName;
        }
    }
    void SaveDefaultValues(bool IsSales)
    {
        object Defaults = GetDefaults(IsSales);
        foreach (var Pair in fPropertyNames)
        {
            bool IsSalesCombo = fComboBoxIsSales[Pair.Key];
            if (IsSalesCombo != IsSales)
                continue;
            PropertyInfo PI = Defaults.GetType().GetProperty(Pair.Value);
            PI.SetValue(Defaults, GetComboValue(Pair.Key));
        }
        if (IsSales)
        {
            fDefaults.Sales.DefaultQuantity = fSalesDefaultQuantity.Value ?? 0;
            fDefaults.Sales.AllowZeroUnitPrice = fSalesAllowZeroUnitPrice.IsChecked == true;
            fDefaults.Sales.PriceResolverClassName = fSalesPriceResolverClassName.Text ?? string.Empty;
            fDefaults.Sales.TaxResolverClassName = fSalesTaxResolverClassName.Text ?? string.Empty;
        }
        else
        {
            fDefaults.Purchase.DefaultQuantity = fPurchaseDefaultQuantity.Value ?? 0;
            fDefaults.Purchase.AllowZeroUnitPrice = fPurchaseAllowZeroUnitPrice.IsChecked == true;
            fDefaults.Purchase.PriceResolverClassName = fPurchasePriceResolverClassName.Text ?? string.Empty;
            fDefaults.Purchase.TaxResolverClassName = fPurchaseTaxResolverClassName.Text ?? string.Empty;
        }
    }
    List<string> GetSelectedLineGridFields()
    {
        List<string> Result = new();
        foreach (object Item in lstSelected.Items)
            Result.Add(Item.ToString());
        return Result;
    }
    void SetSelectedLineGridFields(IEnumerable<string> Fields)
    {
        List<string> Selected = Fields.ToList();
        lstSelected.ItemsSource = Selected;
        lstAvailable.ItemsSource = fAvailableLineGridFields.Where(Field => !Selected.Contains(Field)).ToList();
    }
    void SaveCurrentLineGridFields()
    {
        if (fCurrentLineGridKind == "Sales")
            fDefaults.Sales.TradeLineGridFields = GetSelectedLineGridFields();
        else
            fDefaults.Purchase.TradeLineGridFields = GetSelectedLineGridFields();
    }
    void LoadCurrentLineGridFields()
    {
        if (fCurrentLineGridKind == "Sales")
            SetSelectedLineGridFields(fDefaults.Sales.TradeLineGridFields);
        else
            SetSelectedLineGridFields(fDefaults.Purchase.TradeLineGridFields);
    }
    void ChangeLineGridKind()
    {
        if (fLoadingControls)
            return;
        SaveCurrentLineGridFields();
        fCurrentLineGridKind = cboLineGridKind.SelectedItem as string ?? "Sales";
        LoadCurrentLineGridFields();
    }
    void MoveField(ListBox Source, ListBox Target)
    {
        if (Source.SelectedItem == null)
            return;
        List<string> SourceItems = Source.Items.Cast<object>().Select(Item => Item.ToString()).ToList();
        List<string> TargetItems = Target.Items.Cast<object>().Select(Item => Item.ToString()).ToList();
        string ItemText = Source.SelectedItem.ToString();
        SourceItems.Remove(ItemText);
        TargetItems.Add(ItemText);
        Source.ItemsSource = SourceItems;
        Target.ItemsSource = TargetItems;
        Target.SelectedItem = ItemText;
    }
    void MoveSelectedField(int Direction)
    {
        if (lstSelected.SelectedItem == null)
            return;
        List<string> Items = lstSelected.Items.Cast<object>().Select(Item => Item.ToString()).ToList();
        string ItemText = lstSelected.SelectedItem.ToString();
        int Index = Items.IndexOf(ItemText);
        int NewIndex = Index + Direction;
        if (NewIndex < 0 || NewIndex >= Items.Count)
            return;
        Items.RemoveAt(Index);
        Items.Insert(NewIndex, ItemText);
        lstSelected.ItemsSource = Items;
        lstSelected.SelectedItem = ItemText;
    }
    void ValidateLineGridFields(string Title, List<string> Fields)
    {
        foreach (string Field in fRequiredLineGridFields)
        {
            if (!Fields.Contains(Field))
                throw new TripousException($"{Title} line grid fields must contain '{Field}'.");
        }
    }
    void SetupLineGridTab()
    {
        cboLineGridKind.ItemsSource = new string[] { "Sales", "Purchases" };
        cboLineGridKind.SelectedItem = "Sales";
        ToolTip.SetTip(btnAddField, "Add Field");
        ToolTip.SetTip(btnRemoveField, "Remove Field");
        ToolTip.SetTip(btnMoveFieldUp, "Move Up");
        ToolTip.SetTip(btnMoveFieldDown, "Move Down");
        cboLineGridKind.SelectionChanged += (Sender, Args) => ChangeLineGridKind();
        btnAddField.Click += (Sender, Args) => MoveField(lstAvailable, lstSelected);
        btnRemoveField.Click += (Sender, Args) => MoveField(lstSelected, lstAvailable);
        btnMoveFieldUp.Click += (Sender, Args) => MoveSelectedField(-1);
        btnMoveFieldDown.Click += (Sender, Args) => MoveSelectedField(1);
    }
    void LoadControls()
    {
        fLoadingControls = true;
        fPropertyNames.Clear();
        fComboBoxIsSales.Clear();
        LoadLookupLists();
        AddDefaultRows(pnlSales, IsSales: true);
        AddDefaultRows(pnlPurchase, IsSales: false);
        LoadDefaultValues(IsSales: true);
        LoadDefaultValues(IsSales: false);
        fCurrentLineGridKind = "Sales";
        cboLineGridKind.SelectedItem = "Sales";
        fLoadingControls = false;
        LoadCurrentLineGridFields();
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public AppDefaultPropertiesEditor()
    {
        InitializeComponent();
        SetupLineGridTab();
    }

    // ● public methods
    /// <summary>
    /// Loads a configuration value into the editor.
    /// </summary>
    public void LoadValue(ConfigPropertyDef Def, string Value)
    {
        fDefaults = string.IsNullOrWhiteSpace(Value) ? new AppDefaultProperties() : Json.Deserialize<AppDefaultProperties>(Value);
        LoadControls();
    }
    /// <summary>
    /// Returns the edited configuration value.
    /// </summary>
    public string SaveValue()
    {
        SaveDefaultValues(IsSales: true);
        SaveDefaultValues(IsSales: false);
        SaveCurrentLineGridFields();
        ValidateLineGridFields("Sales", fDefaults.Sales.TradeLineGridFields);
        ValidateLineGridFields("Purchases", fDefaults.Purchase.TradeLineGridFields);
        return Json.Serialize(fDefaults);
    }

    class LookupOption
    {
        // ● constructors
        public LookupOption(string Id, string Code, string Name)
        {
            this.Id = Id;
            this.Code = Code;
            this.Name = Name;
        }

        // ● public methods
        public override string ToString() => $"{Code} - {Name}";

        // ● properties
        public string Id { get; }
        public string Code { get; }
        public string Name { get; }
    }
}
