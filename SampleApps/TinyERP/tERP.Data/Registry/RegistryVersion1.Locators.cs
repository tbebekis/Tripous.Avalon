namespace tERP.Data;

public partial class RegistryVersion1: RegistryVersion
{
    // ● public
    public override void RegisterLocators()
    {
        DataRegistry.AddOrGetLocator("Country", "Country", "Id", FormName: "Country");
        DataRegistry.AddOrGetLocator("Customer", "Person", "Id", FormName: "Person");
        DataRegistry.AddOrGetLocator("Person", "Person", "Id", FormName: "Person");
        DataRegistry.AddOrGetLocator("Product", "Product", "Id", FormName: "Product");
    }
}