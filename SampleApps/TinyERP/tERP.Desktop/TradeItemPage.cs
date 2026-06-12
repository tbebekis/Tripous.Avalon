namespace tERP.Desktop;

public class TradeItemPage: ItemPage
{
    // ● protected
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

        List<GridColumnBinding> Bindings = DetailInfo.Grid.GetInfoList();
        foreach (GridColumnBinding Binding in Bindings)
            Binding.GridColumn.IsVisible = false;

        for (int Index = 0; Index < FieldNames.Count; Index++)
        {
            string FieldName = FieldNames[Index];
            GridColumnBinding Binding = Bindings.FirstOrDefault(Item =>
            {
                string Name = !string.IsNullOrWhiteSpace(Item.DisplayFieldName) ? Item.DisplayFieldName : Item.FieldName;
                return Name.IsSameText(FieldName);
            });

            if (Binding == null)
                continue;

            Binding.GridColumn.IsVisible = true;
            Binding.GridColumn.DisplayIndex = Index;
        }
    }

    // ● construction
    public TradeItemPage()
    {
    }

    // ● public
    public override void Bind(int ColumnCount)
    {
        base.Bind(ColumnCount);
        ApplyTradeLineGridFields();
    }
}
