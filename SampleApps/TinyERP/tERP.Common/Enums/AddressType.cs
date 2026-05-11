namespace tERP.Common;

[RegistryType]
public enum AddressType
{
    None = 0,
    Main = 1,
    Billing = 2,
    Shipping = 3,
    Other = 4
}

[RegistryType]
public enum ProductType
{
    None = 0,
    Goods = 1,
    Service = 2,
    RawMaterial = 3
}

[RegistryType]
public enum ContactType
{
    None = 0,
    Person = 1,
    Accounting = 2,
    Sales = 3,
    Support = 4,
    Other = 5
}

[RegistryType]
public enum TradeType
{
    None = 0,
    Sales = 1,
    Purchases = 2,
    Warehouse = 3,
    Financial = 4,
    Accounting = 5,
}

[RegistryType]
public enum WarehouseType
{
    None = 0,
    Main = 1,
    Store = 2,
    Transit = 3,
    Production = 4,
    Scrap = 5,
    Virtual = 6,
}

[RegistryType]
public enum ProjectStatus
{
    None = 0,
    Draft = 1,
    Active = 2,
    Suspended = 3,
    Completed = 4,
    Cancelled = 5,
}

[RegistryType]
public enum DocStatus
{
    None = 0,
    Draft = 1,
    PendingApproval = 2,
    Posted = 3,
    Completed = 4,
    Cancelled = 5,
    Closed = 6,
}