namespace tERP.Data;

public partial class RegistryVersion1: RegistryVersion
{
    // ● public
    public override void RegisterCodeProviders()
    {
        DataRegistry.AddOrGetCodeProvider("BillOfMaterial");
        DataRegistry.AddOrGetCodeProvider("CashAccount");
        DataRegistry.AddOrGetCodeProvider("Company");
        DataRegistry.AddOrGetCodeProvider("FixedAsset");
        DataRegistry.AddOrGetCodeProvider("PersonAddress");
        DataRegistry.AddOrGetCodeProvider("Product");
        DataRegistry.AddOrGetCodeProvider("Project");
        DataRegistry.AddOrGetCodeProvider("SalesPerson");
        DataRegistry.AddOrGetCodeProvider("Warehouse");
        DataRegistry.AddOrGetCodeProvider("WarehouseLocation");
    }

    public override void AddCodeProviderPatterns(Dictionary<string, string> Patterns)
    {
        Patterns["BillOfMaterial"] = "BOM-XXXXXX";
        Patterns["CashAccount"] = "CASH-XXXXXX";
        Patterns["Company"] = "XXXXXX";
        Patterns["FixedAsset"] = "AST-XXXXXX";
        Patterns["PersonAddress"] = "ADR-XXXXXX";
        Patterns["Product"] = "XXXXXX";
        Patterns["Project"] = "YYYY-XXXX";
        Patterns["SalesPerson"] = "XXXX";
        Patterns["Warehouse"] = "WH-XXXXXX";
        Patterns["WarehouseLocation"] = "LOC-XXXXXX";
    }
}