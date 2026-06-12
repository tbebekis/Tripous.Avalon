/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the business purpose of a postal address.
/// </summary>
[TypeStore]
public enum AddressType
{
    /// <summary>No address purpose is specified.</summary>
    None = 0,
    /// <summary>The primary general-purpose address.</summary>
    Main = 1,
    /// <summary>The address used for billing and invoicing.</summary>
    Billing = 2,
    /// <summary>The address used for shipping and delivery.</summary>
    Shipping = 3,
    /// <summary>An address with another business purpose.</summary>
    Other = 4
}

/// <summary>
/// Defines the commercial and inventory nature of a product.
/// </summary>
[TypeStore]
public enum ProductType
{
    /// <summary>No product type is specified.</summary>
    None = 0,
    /// <summary>A tangible item that can be sold, purchased, or stocked.</summary>
    Goods = 1,
    /// <summary>An intangible service provided or purchased.</summary>
    Service = 2,
    /// <summary>A material consumed in production or assembly.</summary>
    RawMaterial = 3
}

/// <summary>
/// Defines the business role of a contact entry.
/// </summary>
[TypeStore]
public enum ContactType
{
    /// <summary>No contact role is specified.</summary>
    None = 0,
    /// <summary>A general personal contact.</summary>
    Person = 1,
    /// <summary>A contact for accounting and payment matters.</summary>
    Accounting = 2,
    /// <summary>A contact for sales and commercial matters.</summary>
    Sales = 3,
    /// <summary>A contact for technical or customer support.</summary>
    Support = 4,
    /// <summary>A contact with another business role.</summary>
    Other = 5
}

/// <summary>
/// Defines the operational purpose of a warehouse.
/// </summary>
[TypeStore]
public enum WarehouseType
{
    /// <summary>No warehouse type is specified.</summary>
    None = 0,
    /// <summary>The primary company warehouse.</summary>
    Main = 1,
    /// <summary>A retail or branch store warehouse.</summary>
    Store = 2,
    /// <summary>A temporary warehouse for goods in transit.</summary>
    Transit = 3,
    /// <summary>A warehouse used for production materials and output.</summary>
    Production = 4,
    /// <summary>A warehouse for damaged, rejected, or scrap items.</summary>
    Scrap = 5,
    /// <summary>A logical warehouse without a physical storage location.</summary>
    Virtual = 6,
}

/// <summary>
/// Defines the lifecycle status of a project.
/// </summary>
[TypeStore]
public enum ProjectStatus
{
    /// <summary>No project status is specified.</summary>
    None = 0,
    /// <summary>The project is being prepared and has not started.</summary>
    Draft = 1,
    /// <summary>The project is currently active.</summary>
    Active = 2,
    /// <summary>The project is temporarily paused.</summary>
    Suspended = 3,
    /// <summary>The project has finished successfully.</summary>
    Completed = 4,
    /// <summary>The project was cancelled before completion.</summary>
    Cancelled = 5,
}

/// <summary>
/// Defines the data type of a product attribute value.
/// </summary>
[TypeStore]
public enum ProductAttributeType
{
    /// <summary>No attribute data type is specified.</summary>
    None = 0,
    /// <summary>A free-form text value.</summary>
    Text = 1, 
    /// <summary>A whole-number value.</summary>
    Integer = 2, 
    /// <summary>A decimal numeric value.</summary>
    Decimal = 3, 
    /// <summary>A value selected from predefined options.</summary>
    Option = 4,
}

/// <summary>
/// Defines the business domain and direction of a trade document.
/// </summary>
[TypeStore]
public enum TradeType
{
    /// <summary>No trade type is specified.</summary>
    None = 0,
    /// <summary>A customer-facing sales transaction.</summary>
    Sales = 1,
    /// <summary>A supplier-facing purchase transaction.</summary>
    Purchases = 2,
    /// <summary>An internal warehouse or inventory transaction.</summary>
    Warehouse = 3,
    /// <summary>A financial transaction involving money or balances.</summary>
    Financial = 4,
    /// <summary>An accounting transaction recorded in the ledger.</summary>
    Accounting = 5,
}

/// <summary>
/// Defines the lifecycle status of a trade document.
/// </summary>
[TypeStore]
public enum TradeStatus
{
    /// <summary>No document status is specified.</summary>
    None = 0,
    /// <summary>The document is editable and has not been posted.</summary>
    Draft = 1,
    /// <summary>The document is finalized, posted, and locked.</summary>
    Posted = 2,
    /// <summary>The document has been cancelled.</summary>
    Cancelled = 3,
    /// <summary>The document has been fully executed or fulfilled.</summary>
    Completed = 4,
}

/// <summary>
/// Defines the geographic level represented by a tax jurisdiction.
/// </summary>
[TypeStore]
public enum TaxJurisdictionType
{
    /// <summary>No jurisdiction type is specified.</summary>
    None = 0,
    /// <summary>A sovereign country.</summary>
    Country = 1,
    /// <summary>A state, province, or equivalent administrative region.</summary>
    State = 2,
    /// <summary>A county or equivalent subdivision of a state.</summary>
    County = 3,
    /// <summary>A city or municipality.</summary>
    City = 4,
    /// <summary>A special local tax authority or district.</summary>
    Special = 5,
    /// <summary>A tax territory containing multiple countries, such as the European Union.</summary>
    TaxZone = 6,
}

/// <summary>
/// Defines the family of indirect tax represented by a tax rate.
/// </summary>
[TypeStore]
public enum TaxType
{
    /// <summary>No indirect tax family is specified.</summary>
    None = 0,
    /// <summary>Value Added Tax, commonly used in Europe and many other countries.</summary>
    Vat = 1,
    /// <summary>Sales tax, commonly imposed by United States state and local authorities.</summary>
    SalesTax = 2,
    /// <summary>Goods and Services Tax, used in countries such as Canada and Australia.</summary>
    Gst = 3,
    /// <summary>Another indirect tax family not represented by the standard values.</summary>
    Other = 4,
}

/// <summary>
/// Defines how a tax rule calculates its monetary tax component.
/// </summary>
[TypeStore]
public enum TaxCalculationType
{
    /// <summary>No tax calculation method is specified.</summary>
    None = 0,
    /// <summary>Calculates tax as a percentage of the taxable amount.</summary>
    Percentage = 1,
    /// <summary>Calculates tax on the taxable amount including previously calculated tax components.</summary>
    TaxOnTax = 2,
}

/// <summary>
/// Legacy document tax treatment retained until the generated modules
/// and TradeDataModule are migrated to the tax rule model.
/// </summary>
[TypeStore]
public enum TaxTreatment
{
    /// <summary>No legacy tax treatment is specified.</summary>
    None = 0,
    /// <summary>The document is subject to normal domestic taxation.</summary>
    Normal = 1,
    /// <summary>The document is exempt from tax.</summary>
    Exempt = 2,
    /// <summary>The document concerns a transaction with a non-EU country.</summary>
    ThirdCountry = 3,
    /// <summary>The document concerns an intra-community EU transaction.</summary>
    IntraCommunity = 4,
}

/// <summary>
/// Defines the commercial nature of a trade document line.
/// </summary>
[TypeStore]
public enum TradeLineType
{
    /// <summary>No line type is specified.</summary>
    None = 0,
    /// <summary>A line representing a tangible product or material.</summary>
    Item = 1,
    /// <summary>A line representing a service.</summary>
    Service = 2,
}

/// <summary>
/// Defines the method used to calculate inventory cost.
/// </summary>
[TypeStore]
public enum StockCostingMethod
{
    /// <summary>No stock costing method is specified.</summary>
    None = 0,
    /// <summary>Uses the continuously recalculated weighted average cost.</summary>
    MovingAverage = 1,
    /// <summary>Issues the oldest available stock cost first.</summary>
    Fifo = 2,
    /// <summary>Issues the newest available stock cost first.</summary>
    Lifo = 3,
    /// <summary>Uses a predefined standard unit cost.</summary>
    StandardCost = 4,
}

/// <summary>
/// Defines the financial statement category of an account.
/// </summary>
[TypeStore]
public enum AccountType
{
    /// <summary>No account type is specified.</summary>
    None = 0,

    /// <summary>
    /// Resources owned by the company.
    /// Examples: Cash, Bank, Customers, Inventory, Fixed Assets.
    /// </summary>
    Asset = 1,

    /// <summary>
    /// Obligations owed by the company.
    /// Examples: Suppliers, Loans, Taxes Payable.
    /// </summary>
    Liability = 2,

    /// <summary>
    /// Owners' equity.
    /// Examples: Share Capital, Retained Earnings.
    /// </summary>
    Equity = 3,

    /// <summary>
    /// Income generated by the company.
    /// Examples: Sales Revenue, Service Revenue.
    /// </summary>
    Revenue = 4,

    /// <summary>
    /// Costs and expenses incurred by the company.
    /// Examples: Purchases, Salaries, Rent, Utilities.
    /// </summary>
    Expense = 5,
}

/// <summary>
/// Defines whether increases to an account are normally recorded as debits or credits.
/// </summary>
[TypeStore]
public enum NormalBalance
{
    /// <summary>No normal balance is specified.</summary>
    None = 0,

    /// <summary>
    /// Debit-nature account.
    /// Typical for Assets and Expenses.
    /// </summary>
    Debit = 1,

    /// <summary>
    /// Credit-nature account.
    /// Typical for Liabilities, Equity and Revenue.
    /// </summary>
    Credit = 2,
}

/// <summary>
/// Defines the lifecycle status of a fixed asset.
/// </summary>
[TypeStore]
public enum AssetStatus
{
    /// <summary>No asset status is specified.</summary>
    None = 0,
    /// <summary>The asset record is being prepared.</summary>
    Draft = 1,
    /// <summary>The asset is owned and currently in use.</summary>
    Active = 2,
    /// <summary>The asset has been removed from active use.</summary>
    Disposed = 3,
    /// <summary>The asset was disposed of through a sale.</summary>
    Sold = 4,
    /// <summary>The asset was discarded or destroyed as scrap.</summary>
    Scrapped = 5,
}

/// <summary>
/// Defines custom commands available for document forms.
/// </summary>
[TypeStore]
public enum DocumentAction
{
    /// <summary>No document action is specified.</summary>
    None = 0,
    /// <summary>Finalizes, posts, and locks the current document.</summary>
    Post = 1,
    /// <summary>Creates a Sales Delivery Note from the current Sales Order.</summary>
    CreateDeliveryNote = 2,
}
