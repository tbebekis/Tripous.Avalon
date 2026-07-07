namespace tERP.Desktop;

/// <summary>
/// Item page that applies trade line grid layout defaults.
/// </summary>
public class TradeItemPage: ItemPage
{
    // ● protected
    /// <summary>
    /// Applies configured field visibility and order to the trade line grid.
    /// </summary>
    protected virtual void ApplyTradeLineGridFields()
    {
        if (Context.Module is not SalesDataModule && Context.Module is not PurchaseDataModule)
            return;

        AppDefaultProperties AppDefaultProperties = Config.GetObjectValue<AppDefaultProperties>(DataLib.SAppDefaultProperties);
        List<string> FieldNames = Context.Module is PurchaseDataModule
            ? AppDefaultProperties.Purchase.TradeLineGridFields
            : AppDefaultProperties.Sales.TradeLineGridFields;
        UiDetailTableInfo DetailInfo = Context.TopTableUiInfo.DetailList.FirstOrDefault(Item => Item.TableDef.Name.IsSameText("TradeLine"));
        if (DetailInfo?.Grid == null || FieldNames == null)
            return;

        List<GroupGridColumnBinding> Bindings = DetailInfo.Grid.GetInfoList();
        foreach (GroupGridColumnBinding Binding in Bindings)
            DetailInfo.Grid.SetColumnVisible(Binding.GridColumn, false);

        for (int Index = 0; Index < FieldNames.Count; Index++)
        {
            string FieldName = FieldNames[Index];
            GroupGridColumnBinding Binding = Bindings.FirstOrDefault(Item =>
            {
                string Name = !string.IsNullOrWhiteSpace(Item.DisplayFieldName) ? Item.DisplayFieldName : Item.FieldName;
                return Name.IsSameText(FieldName);
            });

            if (Binding == null)
                continue;

            DetailInfo.Grid.SetColumnVisible(Binding.GridColumn, true);
            DetailInfo.Grid.MoveColumn(Binding.GridColumn, Index);
        }
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TradeItemPage()
    {
    }

    // ● public
    /// <summary>
    /// Binds the item page and applies trade line grid settings.
    /// </summary>
    public override void Bind(int ColumnCount)
    {
        base.Bind(ColumnCount);
        ApplyTradeLineGridFields();
    }
}
