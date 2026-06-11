
/*---------------------------------------------------
Table: DocumentType 
Module: DocumentType DocumentTypeDataModule
Group: Documents
IsLookup
FieldGroups: Posting, Cancellation, Output, Appearance, Notes
-----------------------------------------------------
Defines document types and their posting behavior.

A document type controls numbering, posting handlers, stock effects,
financial effects, accounting effects, cancellation behavior, and
optional output templates.

Examples:
    SAL-INV     Sales Invoice
    PUR-INV     Purchase Invoice
    RETAIL      Retail Receipt
    SAL-CREDIT  Sales Credit Note
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,

    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,

    TradeTypeId int @NOT_NULL,                         -- Enum TradeType

    NumberSeriesId @NVARCHAR(40) @NOT_NULL,            -- Lookup
    ModuleName @NVARCHAR(96) @NOT_NULL,                -- Lookup ModuleName ClassName:DocumentModuleLookupSource -- Module Name 

    IsActive @BOOL default 1 @NOT_NULL,
    IsSystem @BOOL default 0 @NOT_NULL,                -- system defined and protected type
    AllowManualNumber @BOOL default 0 @NOT_NULL,
    AutoComplete @BOOL default 0 @NOT_NULL,

    AffectsStock @BOOL default 0 @NOT_NULL,            -- Group Posting
    AffectsFinancial @BOOL default 0 @NOT_NULL,        -- Group Posting
    AffectsAccounting @BOOL default 0 @NOT_NULL,       -- Group Posting

    StockDirection int default 0 @NOT_NULL,            -- Group Posting
    FinancialDirection int default 0 @NOT_NULL,        -- Group Posting
    AccountingDirection int default 0 @NOT_NULL,       -- Group Posting

    IsCancellation @BOOL default 0 @NOT_NULL,          -- Group Cancellation
    CancellationTargetId @NVARCHAR(40) @NULL,          -- Lookup; Group Cancellation -- what document type may cancel

    PrintTemplate @NVARCHAR(96) @NULL,                 -- Group Output
    ReportName @NVARCHAR(96) @NULL,                    -- Group Output

    DisplayOrder int default 0 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                         -- Group Appearance -- ui display color
    IconName @NVARCHAR(96) @NULL,                      -- Group Appearance -- ui icon

    Remarks @NBLOB_TEXT @NULL,                         -- LargeMemo; Group Notes

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (NumberSeriesId) REFERENCES SYS_NUMBER_SERIES(Id),
    FOREIGN KEY (CancellationTargetId) REFERENCES DocumentType(Id)
    )



/*---------------------------------------------------
Table: Trade

Module: SalesOrder SalesOrderDataModule
Group: Sales
ItemPage: TradeItemPage
DetailOrder: Trade=TradeLine, TradeTax
DetailOrder: TradeLine=TradeLineTax
Code: Draft SO-YYYY-XXXXXX 

Module: SalesDeliveryNote SalesDeliveryNoteDataModule
Group: Sales
ItemPage: TradeItemPage
Code: Draft SDN-YYYY-XXXXXX

Module: SalesInvoice SalesInvoiceDataModule
Group: Sales
ItemPage: TradeItemPage
Code: Draft SINV-YYYY-XXXXXX  

Module: SalesCreditNote SalesCreditNoteDataModule
Group: Sales
ItemPage: TradeItemPage
Code: Draft SCN-YYYY-XXXXXX

Module: SalesReturn SalesReturnDataModule
Group: Sales
ItemPage: TradeItemPage
Code: Draft SRET-YYYY-XXXXXX

Module: SalesCancellation SalesCancellationDataModule
Group: Sales
ItemPage: TradeItemPage
Code: Draft SCAN-YYYY-XXXXXX

Module: PurchaseOrder PurchaseOrderDataModule
Group: Purchases
ItemPage: TradeItemPage
Code: Draft PO-YYYY-XXXXXX  

Module: PurchaseDeliveryNote PurchaseDeliveryNoteDataModule
Group: Purchases
ItemPage: TradeItemPage
Code: Draft PDN-YYYY-XXXXXX

Module: PurchaseInvoice PurchaseInvoiceDataModule
Group: Purchases
ItemPage: TradeItemPage
Code: Draft PINV-YYYY-XXXXXX 

Module: PurchaseCreditNote PurchaseCreditNoteDataModule
Group: Purchases
ItemPage: TradeItemPage
Code: Draft PCN-YYYY-XXXXXX

Module: PurchaseReturn PurchaseReturnDataModule
Group: Purchases
ItemPage: TradeItemPage
Code: Draft PRET-YYYY-XXXXXX

Module: PurchaseCancellation PurchaseCancellationDataModule
Group: Purchases
ItemPage: TradeItemPage
Code: Draft PCAN-YYYY-XXXXXX
  
FieldGroups: Billing, Shipping, Amounts, Audit, Organization, Notes 
 
-----------------------------------------------------
Commercial document header.

Used as the shared storage table for sales and purchase documents.

Supported business document types include:
- orders
- delivery notes
- invoices
- credit notes
- returns
- cancellation documents

Each declared module represents a specific business view over this table,
with its own menu command, form, module registration, document types,
validation rules, and posting behavior.

Document-specific behavior is determined by the selected DocumentType and
its associated handler implementation.

Line details are stored in TradeLine.
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,             

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup; [Hidden]
    Code @NVARCHAR(40) @NOT_NULL,                       -- Code; [ReadOnlyUI]
    TradeTypeId int default 0 @NOT_NULL,                -- [Hidden]

    TradeStatusId int default 0 @NOT_NULL,              -- Enum TradeStatus; [ReadOnlyUI]
    TaxBusinessGroupId @NVARCHAR(40) @NULL,             -- Lookup -- Tax classification copied from Person and stored as a document snapshot
    OriginTaxJurisdictionId @NVARCHAR(40) @NULL,        -- Lookup -- Jurisdiction resolved from the company or branch address
    DestinationTaxJurisdictionId @NVARCHAR(40) @NULL,   -- Lookup -- Jurisdiction resolved from the transaction address or selected as an override

    TradeDate @DATE @NOT_NULL,                          -- 
    PostingDate @DATE @NULL,                            -- [ReadOnlyUI]
    DeliveryDate @DATE @NULL,                           -- 
    DueDate @DATE @NULL,                                -- 

    ExternalRef @NVARCHAR(96) @NULL,                    -- e.g. "Related to Order 123", "Your ref: PO-456"

    PersonId @NVARCHAR(40) @NOT_NULL,                   -- Locator Person -- Customer, Supplier, etc
    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup

    SalesPersonId @NVARCHAR(40) @NULL,                  -- Lookup Person; Group Organization
    ProjectId @NVARCHAR(40) @NULL,                      -- Lookup; Group Organization
    CostCenterId @NVARCHAR(40) @NULL,                   -- Lookup; Group Organization
    BranchId @NVARCHAR(40) @NULL,                       -- Lookup; Group Organization

    PriceListTypeId @NVARCHAR(40) @NULL,                -- Lookup -- Price list type stored as a document snapshot
    CurrencyId @NVARCHAR(40) @NOT_NULL,                 -- Lookup
    ExchangeRate @DECIMAL default 1 @NOT_NULL,          -- Exchange Rate for base currency

    PaymentMethodId @NVARCHAR(40) @NULL,                -- Lookup
    PaymentTermId @NVARCHAR(40) @NULL,                  -- Lookup

    BillingName @NVARCHAR(96) @NULL,                    -- Group Billing
    BillingAddressLine1 @NVARCHAR(128) @NULL,           -- Group Billing
    BillingAddressLine2 @NVARCHAR(128) @NULL,           -- Group Billing
    BillingCity @NVARCHAR(64) @NULL,                    -- Group Billing
    BillingRegion @NVARCHAR(64) @NULL,                  -- Group Billing -- State, province, or administrative region
    BillingPostalCode @NVARCHAR(20) @NULL,              -- Group Billing
    BillingCountryId @NVARCHAR(40) @NULL,               -- Lookup; Group Billing

    ShippingName @NVARCHAR(96) @NULL,                   -- Group Shipping
    ShippingAddressLine1 @NVARCHAR(128) @NULL,          -- Group Shipping
    ShippingAddressLine2 @NVARCHAR(128) @NULL,          -- Group Shipping
    ShippingCity @NVARCHAR(64) @NULL,                   -- Group Shipping
    ShippingRegion @NVARCHAR(64) @NULL,                 -- Group Shipping -- State, province, or administrative region
    ShippingPostalCode @NVARCHAR(20) @NULL,             -- Group Shipping
    ShippingCountryId @NVARCHAR(40) @NULL,              -- Lookup; Group Shipping

    SourceId @NVARCHAR(40) @NULL,                       -- Locator Trade; [Hidden]
    CancelsTradeId @NVARCHAR(40) @NULL,                 -- Locator Trade; [Hidden]
    CancelledByTradeId @NVARCHAR(40) @NULL,             -- Locator Trade; [Hidden]

    LinesAmount @DECIMAL default 0 @NOT_NULL,           -- Group Amounts; [ReadOnlyUI] -- sum of lines before header discounts/charges/taxes
    DiscountPercent @DECIMAL default 0 @NOT_NULL,       -- Group Amounts -- Header Discount %
    DiscountAmount @DECIMAL default 0 @NOT_NULL,        -- Group Amounts
    DiscountReason @NVARCHAR(256) @NULL,                -- Group Amounts

    ChargesAmount @DECIMAL default 0 @NOT_NULL,         -- Group Amounts

    NetAmount @DECIMAL default 0 @NOT_NULL,             -- Group Amounts; [ReadOnlyUI] -- = LinesAmount - DiscountAmount + ChargesAmount
    TaxAmount @DECIMAL default 0 @NOT_NULL,             -- Group Amounts; [ReadOnlyUI] -- Total tax amount from all line tax components
    TotalAmount @DECIMAL default 0 @NOT_NULL,           -- Group Amounts; [ReadOnlyUI]

    IsLocked @BOOL default 0 @NOT_NULL,                 -- [ReadOnlyUI] -- Lock document from editing
    IsCancelled @BOOL default 0 @NOT_NULL,              -- [ReadOnlyUI]

    CreatedAt @DATE_TIME @NOT_NULL,                     -- Group Audit; [ReadOnlyUI]
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  --  Lookup SYS_APP_USER; Group Audit; [ReadOnlyUI]
    ModifiedAt @DATE_TIME @NULL,                        -- Group Audit; [ReadOnlyUI]
    ModifiedBy @NVARCHAR(40) @NULL,                     --  Lookup SYS_APP_USER; Group Audit; [ReadOnlyUI]
    PostedAt @DATE_TIME @NULL,                          -- Group Audit; [ReadOnlyUI]
    PostedBy @NVARCHAR(40) @NULL,                       --  Lookup SYS_APP_USER; Group Audit; [ReadOnlyUI]
    CancelledAt @DATE_TIME @NULL,                       -- Group Audit; [ReadOnlyUI]
    CancelledBy @NVARCHAR(40) @NULL,                    --  Lookup SYS_APP_USER; Group Audit; [ReadOnlyUI]

    Remarks @NVARCHAR(512) @NULL,                       -- Memo; Group Notes -- internal
    Comments @NVARCHAR(512) @NULL,                      -- Memo; Group Notes -- customer visible

    CONSTRAINT UQ_{TableName}_DocumentType_Code UNIQUE (DocumentTypeId, Code),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (TaxBusinessGroupId) REFERENCES TaxBusinessGroup(Id),
    FOREIGN KEY (OriginTaxJurisdictionId) REFERENCES TaxJurisdiction(Id),
    FOREIGN KEY (DestinationTaxJurisdictionId) REFERENCES TaxJurisdiction(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (SalesPersonId) REFERENCES Person(Id),
    FOREIGN KEY (ProjectId) REFERENCES Project(Id),
    FOREIGN KEY (CostCenterId) REFERENCES CostCenter(Id),
    FOREIGN KEY (BranchId) REFERENCES CompanyBranch(Id),

    FOREIGN KEY (PriceListTypeId) REFERENCES PriceListType(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),

    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethod(Id),
    FOREIGN KEY (PaymentTermId) REFERENCES PaymentTerm(Id),

    FOREIGN KEY (BillingCountryId) REFERENCES Country(Id),
    FOREIGN KEY (ShippingCountryId) REFERENCES Country(Id),

    FOREIGN KEY (SourceId) REFERENCES Trade(Id),
    FOREIGN KEY (CancelsTradeId) REFERENCES Trade(Id),
    FOREIGN KEY (CancelledByTradeId) REFERENCES Trade(Id),

    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (PostedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (CancelledBy) REFERENCES  SYS_APP_USER(Id)
    )



/*---------------------------------------------------
Table: TradeTax
-----------------------------------------------------
Hidden detail table.

Stores tax summary rows per applied tax rule for a Trade document.

The table is generated from TradeLineTax and maintained by
TradeDataModule. It supports both a single European VAT component
and multiple United States sales tax components.
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,              -- -- Primary identifier

    TradeId @NVARCHAR(40) @NOT_NULL,                     -- Master -- Owning commercial document
    TaxRuleId @NVARCHAR(40) @NOT_NULL,                   -- Lookup -- Applied tax rule
    TaxRateId @NVARCHAR(40) @NOT_NULL,                   -- Lookup -- Applied tax rate
    TaxRatePercent @DECIMAL_(9,4) default 0 @NOT_NULL,   -- Snapshot TaxRate.Percent -- Percentage at calculation time

    TaxableAmount @DECIMAL default 0 @NOT_NULL,          -- -- Sum of taxable amounts for this rule
    TaxAmount @DECIMAL default 0 @NOT_NULL,              -- -- Sum of tax amounts for this rule
    TotalAmount @DECIMAL default 0 @NOT_NULL,            -- -- TaxableAmount plus TaxAmount

    CONSTRAINT UQ_{TableName}_Trade_TaxRule UNIQUE (TradeId, TaxRuleId),

    FOREIGN KEY (TradeId) REFERENCES Trade(Id),
    FOREIGN KEY (TaxRuleId) REFERENCES TaxRule(Id),
    FOREIGN KEY (TaxRateId) REFERENCES TaxRate(Id)
    )



/*---------------------------------------------------
Table: TradeLine
-----------------------------------------------------
Commercial document line.

Stores commercial values and the final aggregate tax result.
Individual tax components are stored in TradeLineTax so a line may
contain one VAT component or multiple sales tax components.
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,              -- Primary identifier

    TradeId @NVARCHAR(40) @NOT_NULL,                     -- Master; [ReadOnlyUI] -- Owning commercial document

    DisplayOrder int @NOT_NULL,                          -- User-visible line order

    LineTypeId int default 1 @NOT_NULL,                  -- Enum TradeLineType -- Item or Service

    ProductId @NVARCHAR(40) @NULL,                       -- Locator Product
    ProductCode @NVARCHAR(40) @NULL,                     -- Snapshot Product.Code
    ProductName @NVARCHAR(128) @NULL,                    -- Snapshot Product.Name
    TaxProductGroupId @NVARCHAR(40) @NULL,               -- Lookup -- Tax classification copied from Product and stored as a line snapshot

    Description @NVARCHAR(256) @NULL,                    -- Commercial line description

    WarehouseId @NVARCHAR(40) @NULL,                     -- Lookup -- Optional line-level warehouse override

    UnitOfMeasureId @NVARCHAR(40) @NULL,                 -- Lookup -- Transaction unit of measure
    UnitOfMeasureName @NVARCHAR(40) @NULL,               -- Snapshot UnitOfMeasure.Name; [ReadOnlyUI]
    UnitRatio @DECIMAL default 1 @NOT_NULL,              -- [ReadOnlyUI] -- Ratio to the product primary unit

    Quantity @DECIMAL default 0 @NOT_NULL,               -- Quantity expressed in UnitOfMeasureId
    PrimaryUnitQuantity @DECIMAL default 0 @NOT_NULL,    -- [ReadOnlyUI] -- Quantity converted to the product primary unit

    ReservedQuantity @DECIMAL default 0 @NOT_NULL,       -- [ReadOnlyUI] -- Quantity reserved by warehouse processes
    ExecutedQuantity @DECIMAL default 0 @NOT_NULL,       -- [ReadOnlyUI] -- Quantity already executed or fulfilled

    TaxPercent @DECIMAL_(9,4) default 0 @NOT_NULL,       -- [ReadOnlyUI] -- Aggregate effective percentage of all tax components
    IsTaxExempt @BOOL default 0 @NOT_NULL,               -- [ReadOnlyUI] -- Indicates that the resolved tax treatment is exempt
    IsReverseCharge @BOOL default 0 @NOT_NULL,           -- [ReadOnlyUI] -- Indicates that tax liability shifts to the recipient

    UnitPrice @DECIMAL default 0 @NOT_NULL,              -- Price per selected unit before discounts and taxes

    GrossAmount @DECIMAL default 0 @NOT_NULL,            -- [ReadOnlyUI] -- Quantity multiplied by UnitPrice

    DiscountPercent @DECIMAL default 0 @NOT_NULL,        -- Line discount percentage
    DiscountAmount @DECIMAL default 0 @NOT_NULL,         -- Line discount monetary value

    NetUnitPrice @DECIMAL default 0 @NOT_NULL,           -- [ReadOnlyUI] -- Unit price after line discount

    NetAmount @DECIMAL default 0 @NOT_NULL,              -- [ReadOnlyUI] -- GrossAmount minus DiscountAmount
    DocumentDiscountAmount @DECIMAL default 0 @NOT_NULL, -- [ReadOnlyUI] -- Allocated share of the document discount
    TaxAmount @DECIMAL default 0 @NOT_NULL,              -- [ReadOnlyUI] -- Sum of all TradeLineTax components
    TotalAmount @DECIMAL default 0 @NOT_NULL,            -- [ReadOnlyUI] -- NetAmount minus DocumentDiscountAmount plus TaxAmount

    SourceTradeLineId @NVARCHAR(40) @NULL,               -- Locator TradeLine; [ReadOnlyUI] -- Source line for copied or corrective documents

    FOREIGN KEY (TradeId) REFERENCES Trade(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (TaxProductGroupId) REFERENCES TaxProductGroup(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),
    FOREIGN KEY (SourceTradeLineId) REFERENCES TradeLine(Id)
    )



/*---------------------------------------------------
Table: TradeLineTax
-----------------------------------------------------
Hidden subdetail table containing the individual tax components of a
commercial document line.

European VAT normally produces one row. United States sales tax may
produce multiple rows for state, county, city, or special district
taxes. Values are stored as snapshots so posted documents remain
unchanged when setup records are later edited.
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL primary key,              -- -- Primary identifier

    TradeLineId @NVARCHAR(40) @NOT_NULL,                 -- Master -- Owning commercial document line
    TaxRuleId @NVARCHAR(40) @NOT_NULL,                   -- Lookup -- Rule that produced this component
    TaxRateId @NVARCHAR(40) @NOT_NULL,                   -- Lookup -- Rate selected by the rule
    TaxJurisdictionId @NVARCHAR(40) @NOT_NULL,           -- Lookup -- Geographic authority that imposed this component
    TaxClauseId @NVARCHAR(40) @NULL,                     -- Lookup -- Legal explanation for exemption or special treatment

    SequenceNo int default 0 @NOT_NULL,                  -- -- Calculation order for compound tax components
    TaxCalculationTypeId int default 1 @NOT_NULL,        -- Enum TaxCalculationType -- Snapshot of the calculation method
    TaxRatePercent @DECIMAL_(9,4) default 0 @NOT_NULL,   -- Snapshot TaxRate.Percent -- Percentage at calculation time
    TaxableAmount @DECIMAL default 0 @NOT_NULL,          -- -- Amount on which this tax component was calculated
    TaxAmount @DECIMAL default 0 @NOT_NULL,              -- -- Calculated monetary value of this tax component

    IsExempt @BOOL default 0 @NOT_NULL,                  -- -- Snapshot indicating an exempt component
    IsReverseCharge @BOOL default 0 @NOT_NULL,           -- -- Snapshot indicating recipient tax liability
    TaxClauseText @NVARCHAR(512) @NULL,                  -- -- Snapshot of the printed legal explanation

    CONSTRAINT UQ_{TableName}_TradeLine_TaxRule UNIQUE (TradeLineId, TaxRuleId),

    FOREIGN KEY (TradeLineId) REFERENCES TradeLine(Id),
    FOREIGN KEY (TaxRuleId) REFERENCES TaxRule(Id),
    FOREIGN KEY (TaxRateId) REFERENCES TaxRate(Id),
    FOREIGN KEY (TaxJurisdictionId) REFERENCES TaxJurisdiction(Id),
    FOREIGN KEY (TaxClauseId) REFERENCES TaxClause(Id)
    )



/*---------------------------------------------------
Table: StockTrade 
Module: StockTrade StockTradeDataModule
Group: Inventory
Form: Default
FieldGroups: Warehouses, Dates, Relations, Status, Audit, Notes
-----------------------------------------------------
Warehouse transaction document.

Used for inventory-only operations that do not involve
customers, suppliers, receivables, or payables.

Examples:
- warehouse transfer
- stock count adjustment
- stock write-off
- internal stock correction

Posting this document generates StockMovement rows and updates
inventory balances. It does not represent a commercial transaction.
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup -- controls numbering, posting behavior and movement direction
    Code @NVARCHAR(40) @NOT_NULL,                       -- Code Draft STK-YYYY-XXXXXX StockTrade
    TradeTypeId int default 0 @NOT_NULL,                -- [Hidden]
    
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup; Group Warehouses -- main/source warehouse
    ToWarehouseId @NVARCHAR(40) @NULL,                  -- Lookup; Group Warehouses -- destination warehouse, used only for transfers
 
    DocumentDate @DATE @NOT_NULL,                       -- Group Dates
    PostingDate @DATE @NULL,                            -- Group Dates -- date used for generated stock movements

    StatusId int @NOT_NULL,                             -- Enum TradeStatus

    TotalCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,       -- total internal stock cost value posted by this document

    Remarks @NVARCHAR(512) @NULL,                       -- Memo; Group Notes -- internal notes

    IsLocked @BOOL DEFAULT 0 @NOT_NULL,                 -- Group Status
    IsCancelled @BOOL DEFAULT 0 @NOT_NULL,              -- Group Status

    CancelsStockTradeId @NVARCHAR(40) @NULL,            -- Locator StockTrade; Group Relations -- original document cancelled by this one
    CancelledByStockTradeId @NVARCHAR(40) @NULL,        -- Locator StockTrade; Group Relations -- reverse/cancellation document

    CreatedAt @DATE_TIME @NOT_NULL,                     -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  --  Lookup SYS_APP_USER; Group Audit
    ModifiedAt @DATE_TIME @NULL,                        -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                     --  Lookup SYS_APP_USER; Group Audit
    PostedAt @DATE_TIME @NULL,                          -- Group Audit
    PostedBy @NVARCHAR(40) @NULL,                       --  Lookup SYS_APP_USER; Group Audit
    CancelledAt @DATE_TIME @NULL,                       -- Group Audit
    CancelledBy @NVARCHAR(40) @NULL,                    --  Lookup SYS_APP_USER; Group Audit

    CONSTRAINT UQ_{TableName}_DocumentType_Code UNIQUE (DocumentTypeId, Code),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (ToWarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (CancelsStockTradeId) REFERENCES StockTrade(Id),
    FOREIGN KEY (CancelledByStockTradeId) REFERENCES StockTrade(Id),

    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (PostedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (CancelledBy) REFERENCES  SYS_APP_USER(Id)
    )



/*---------------------------------------------------
Table: StockTradeLine
-----------------------------------------------------
Stock transaction document line.

Represents the intended stock operation for one product.
Posting StockTradeLine rows produces immutable StockMovement rows.

Examples:
- one transfer line produces one OUT movement and one IN movement
- one write-off line produces one OUT movement
- one positive adjustment line produces one IN movement
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    StockTradeId @NVARCHAR(40) @NOT_NULL,               -- Master
    DisplayOrder int @NOT_NULL,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    ProductCode @NVARCHAR(40) @NOT_NULL,                -- Snapshot Product.Code  
    ProductName @NVARCHAR(128) @NOT_NULL,               -- Snapshot Product.Name  

    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup -- optional source warehouse override

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,            -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NOT_NULL,          -- Snapshot UnitOfMeasure.Name
    UnitRatio @DECIMAL DEFAULT 1 @NOT_NULL,             -- ratio to primary unit, ProductUnitOfMeasure.Ratio, converts line quantity to primary/base quantity

    Quantity @DECIMAL DEFAULT 0 @NOT_NULL,              -- always positive, direction is determined by DocumentType
    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- Quantity * UnitRatio

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,              -- internal stock cost per primary unit
    CostAmount @DECIMAL DEFAULT 0 @NOT_NULL,            -- PrimaryQuantity * UnitCost

    SourceTradeLineId @NVARCHAR(40) @NULL,              -- optional source commercial line
    SourceStockTradeLineId @NVARCHAR(40) @NULL,         -- optional source stock line, e.g. reversal/copy/adjustment flow

    Remarks @NVARCHAR(512) @NULL,                       -- internal line notes

    FOREIGN KEY (StockTradeId) REFERENCES StockTrade(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),
    FOREIGN KEY (SourceTradeLineId) REFERENCES TradeLine(Id),
    FOREIGN KEY (SourceStockTradeLineId) REFERENCES StockTradeLine(Id)
    )

/*---------------------------------------------------
Table: StockMovement 
Module: StockMovement StockMovementDataModule
Group: Inventory
  
IsReadOnly
NotUiVisible
-----------------------------------------------------
Immutable stock ledger movement.

Produced only by posting business documents:
- Trade / TradeLine for sales and purchases
- StockTrade / StockTradeLine for warehouse operations

Each row represents one physical stock effect in one warehouse.
Quantities are always positive.
Direction is stored separately.
-----------------------------------------------------
The central stock ledger of the ERP system.

All inventory changes are recorded here as immutable movement rows.

StockMovement is the single source of truth for:
- current stock balances
- stock history
- inventory valuation
- stock audits and traceability

Rows are generated only by posting business documents and are never edited or deleted.

Typical sources include:
- sales and purchase documents
- warehouse transfers
- stock adjustments
- write-offs and destructions

Current stock quantities are calculated from StockMovement, not from document tables.  
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    TradeTypeId int default 0 @NOT_NULL,                -- Enum; [ReadOnlyUI]
    
    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup

    MovementDate @DATE @NOT_NULL,                       -- stock ledger date
    Direction int @NOT_NULL,                            -- 1=in, -1=out

    Quantity @DECIMAL DEFAULT 0 @NOT_NULL,              -- always positive, in movement unit
    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- quantity in product primary unit

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,            -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NOT_NULL,          -- Snapshot UnitOfMeasure.Name
    UnitRatio @DECIMAL DEFAULT 1 @NOT_NULL,             -- ratio to primary unit, ProductUnitOfMeasure.Ratio, converts line quantity to primary/base quantity

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,              -- internal stock cost per primary unit at movement time
    CostAmount @DECIMAL DEFAULT 0 @NOT_NULL,            -- PrimaryQuantity * UnitCost

    SourceModule @NVARCHAR(64) @NOT_NULL,               -- source module name, e.g. Trade or StockTrade
    SourceTable @NVARCHAR(64) @NOT_NULL,                -- source line table, e.g. TradeLine or StockTradeLine
    SourceId @NVARCHAR(40) @NOT_NULL,                   -- source line Id

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup -- source document type
    DocumentCode @NVARCHAR(40) @NOT_NULL,               -- source document code snapshot
    DocumentDate @DATE @NOT_NULL,                       -- source document date snapshot    

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  --  Lookup SYS_APP_USER

    CONSTRAINT CHK_{TableName}_Direction CHECK (Direction IN (1, -1)),
    CONSTRAINT CHK_{TableName}_Quantity CHECK (Quantity >= 0),
    CONSTRAINT CHK_{TableName}_PrimaryQuantity CHECK (PrimaryQuantity >= 0),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id)
    )

/*---------------------------------------------------
Table: StockBalance 
Module: StockBalance StockBalanceDataModule 
Group: Inventory

IsReadOnly
NotUiVisible
-----------------------------------------------------
Cached current stock balance per Product / Warehouse.

StockMovement remains the single source of truth.
StockBalance is maintained during posting/reversal and can always be rebuilt from StockMovement.
  
Maintains the current on-hand inventory balance per Product and Warehouse.

This table is a performance cache and not the authoritative inventory ledger.

The authoritative source of inventory data is StockMovement. All quantities, costs, and stock valuations originate from StockMovement records.

StockBalance is maintained automatically during document posting and cancellation and may be fully rebuilt at any time from StockMovement.

Users never insert, edit, or delete rows in this table directly.

Used for:
- current stock availability
- inventory valuation
- fast stock queries and reporting

One row exists per Product/Warehouse combination.  
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup

    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- current stock in product primary unit
    TotalCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,       -- current total stock value
    AverageUnitCost @DECIMAL DEFAULT 0 @NOT_NULL,       -- TotalCostAmount / PrimaryQuantity

    LastMovementDate @DATE @NULL,
    LastMovementId @NVARCHAR(40) @NULL,

    CONSTRAINT UQ_{TableName}_Product_Warehouse UNIQUE (ProductId, WarehouseId),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (LastMovementId) REFERENCES StockMovement(Id)
    )


/*---------------------------------------------------
Table: StockCount 
Module: StockCount StockCountDataModule
Group: Inventory
FieldGroups: Relations, Audit, Notes
-----------------------------------------------------
Physical inventory count document.

Used to record the results of a physical warehouse inventory count
and reconcile actual quantities against system quantities.

After posting, the document generates StockMovement records for
quantity differences and becomes immutable.

Used for:
- periodic inventory counts
- cycle counts
- stock corrections
- inventory reconciliation
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                     -- Code Draft SC-YYYY-XXXXXX StockCount
    DocumentTypeId @NVARCHAR(40) @NOT_NULL,           -- Lookup -- controls numbering, posting behavior and movement direction
    TradeTypeId int default 0 @NOT_NULL,              -- [Hidden]
    
    WarehouseId @NVARCHAR(40) @NOT_NULL,              -- Lookup

    CountDate @DATE @NOT_NULL,

    StatusId int DEFAULT 0 @NOT_NULL,                 -- Enum TradeStatus

    Remarks @NBLOB_TEXT @NULL,                        -- LargeMemo; Group Notes

    CancelledDocumentId @NVARCHAR(40) @NULL,          -- Locator StockCount; Group Relations
    CancellationDocumentId @NVARCHAR(40) @NULL,       -- Locator StockCount; Group Relations

    CreatedAt @DATE_TIME @NOT_NULL,                   -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,                --  Lookup SYS_APP_USER; Group Audit
    ModifiedAt @DATE_TIME @NULL,                      -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                   --  Lookup SYS_APP_USER; Group Audit

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (CancelledDocumentId) REFERENCES StockCount(Id),
    FOREIGN KEY (CancellationDocumentId) REFERENCES StockCount(Id),
    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES  SYS_APP_USER(Id)
    )



/*---------------------------------------------------
Table: StockCountLine
-----------------------------------------------------
Inventory count line.

Stores both the system quantity and the physically counted quantity for a product.

At posting time, the difference between the counted quantity and the system quantity is converted into inventory adjustment StockMovement records.

Positive differences generate inbound movements.
Negative differences generate outbound movements.
  
Inventory count lines represent a snapshot of warehouse stock at the time the count begins.

During the counting process no inventory movements should be posted for the warehouse being counted.

The SystemQuantity value is captured at count time and is not recalculated during posting.

At posting time, the difference between the counted quantity and the recorded system quantity is converted into inventory adjustment StockMovement records.  
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    StockCountId @NVARCHAR(40) @NOT_NULL,               -- Master

    DisplayOrder int @NOT_NULL,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    ProductCode @NVARCHAR(40) @NOT_NULL,                -- Snapshot Product.Code 
    ProductName @NVARCHAR(96) @NOT_NULL,                -- Snapshot Product.Name

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,          -- Lookup

    SystemQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    CountedQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    DifferenceQuantity @DECIMAL DEFAULT 0 @NOT_NULL,

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,
    DifferenceCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,

    Remarks @NVARCHAR(512) @NULL,

    FOREIGN KEY (StockCountId) REFERENCES StockCount(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id) 
    )

/*---------------------------------------------------
Table: StockReservation 
Module: StockReservation StockReservationDataModule
Group: Inventory

NotUiVisible
IsReadOnly
-----------------------------------------------------
Represents reserved inventory created by commercial documents, usually sales orders.

A reservation does not move stock and does not create StockMovement records.

It reduces available quantity but leaves on-hand quantity unchanged.

StockReservation is created, updated, released, or cancelled automatically by Trade posting and document conversion flows.

The authoritative source document is traced through SourceModule, SourceTable, SourceId and SourceLineId.

Used for:
- sales order reservations
- available stock calculation
- preventing over-allocation
- linking ordered quantities to later deliveries or invoices
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup

    ReservedQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    ExecutedQuantity @DECIMAL DEFAULT 0 @NOT_NULL,

    SourceModule @NVARCHAR(96) @NOT_NULL,
    SourceTable @NVARCHAR(96) @NOT_NULL,
    SourceId @NVARCHAR(40) @NOT_NULL,
    SourceLineId @NVARCHAR(40) @NOT_NULL,

    CreatedAt @DATE_TIME @NOT_NULL,

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),

    CONSTRAINT UQ_{TableName}_SourceLine UNIQUE (SourceLineId)
    )

/*---------------------------------------------------
Table: FinanceMovement 
Module: FinanceMovement FinanceMovementDataModule
Group: Finance

NotUiVisible
IsReadOnly
-----------------------------------------------------
The central financial ledger of the ERP system.

All cash and bank account changes are recorded here as immutable movement rows.

FinanceMovement is the single source of truth for:
- cash balances
- bank balances
- financial transaction history
- cash flow reporting
- financial audits and traceability

Rows are generated only by posting business documents and are never edited or deleted.

Typical sources include:
- customer receipts
- supplier payments
- cash deposits
- cash withdrawals
- bank transfers
- financial adjustments

A FinanceMovement represents one financial effect on one cash account or one bank account.

Current balances are calculated from FinanceMovement, not from document tables.

Cancellation is performed through reversal movements that preserve the complete audit trail.
  
A financial movement affects exactly one financial account.

A movement may belong either to a CashAccount or to a CompanyBankAccount, but never to both simultaneously.

This rule ensures that every movement has a single financial destination and prevents double counting of financial balances.

Transfers between cash and bank accounts are represented by separate movement rows, one for each affected account.  
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

   
    TradeTypeId int default 0 @NOT_NULL,              -- Enum; [ReadOnlyUI]
    MovementDate @DATE @NOT_NULL,

    CashAccountId @NVARCHAR(40) @NULL,                -- Lookup
    CompanyBankAccountId @NVARCHAR(40) @NULL,         -- Lookup

    Direction int @NOT_NULL,                          -- 1=in, -1=out

    Amount @DECIMAL DEFAULT 0 @NOT_NULL,

    CurrencyId @NVARCHAR(40) @NOT_NULL,              -- Lookup
    ExchangeRate @DECIMAL DEFAULT 1 @NOT_NULL,

    SourceModule @NVARCHAR(64) @NOT_NULL,
    SourceTable @NVARCHAR(64) @NOT_NULL,
    SourceId @NVARCHAR(40) @NOT_NULL,

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,          -- Lookup
    DocumentCode @NVARCHAR(40) @NOT_NULL,
    DocumentDate @DATE @NOT_NULL,

    Remarks @NVARCHAR(512) @NULL,

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,               --  Lookup SYS_APP_USER

    CONSTRAINT CHK_{TableName}_Direction CHECK (Direction IN (1, -1)),
    CONSTRAINT CHK_{TableName}_Amount CHECK (Amount >= 0),

    FOREIGN KEY (CashAccountId) REFERENCES CashAccount(Id),
    FOREIGN KEY (CompanyBankAccountId) REFERENCES CompanyBankAccount(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id)
    )

/*---------------------------------------------------
Table: FinanceBalance
Module: FinanceBalance FinanceBalanceDataModule
Group: Finance

NotUiVisible
IsReadOnly
-----------------------------------------------------
Maintains the current balance of cash and bank accounts.

This table is a performance cache and not the authoritative financial ledger.

The authoritative source of financial data is FinanceMovement. All balances, cash flow information, and financial history originate from FinanceMovement records.

FinanceBalance is maintained automatically during document posting and cancellation and may be fully rebuilt at any time from FinanceMovement.

Users never insert, edit, or delete rows in this table directly.

Used for:
- current cash balances
- current bank balances
- cash availability
- fast financial queries and reporting

One row exists per financial account.
  
A balance row belongs to exactly one financial account.

A row may reference either a CashAccount or a CompanyBankAccount, but never both simultaneously.  
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    CashAccountId @NVARCHAR(40) @NULL,                -- Lookup
    CompanyBankAccountId @NVARCHAR(40) @NULL,         -- Lookup

    Balance @DECIMAL DEFAULT 0 @NOT_NULL,

    LastMovementDate @DATE @NULL,
    LastMovementId @NVARCHAR(40) @NULL,

    FOREIGN KEY (CashAccountId) REFERENCES CashAccount(Id),
    FOREIGN KEY (CompanyBankAccountId) REFERENCES CompanyBankAccount(Id),
    FOREIGN KEY (LastMovementId) REFERENCES FinanceMovement(Id)
    )


/*---------------------------------------------------
Table: Account 
Module: Account AccountDataModule
Group: Accounting
-----------------------------------------------------
Defines general ledger accounts used by the accounting subsystem.

Accounts form the chart of accounts of the company.

Each accounting line references one Account.

Accounts may be organized hierarchically through ParentAccountId.

Only posting accounts should be used in JournalEntryLine records.
Parent/group accounts are used only for structure and reporting.

AccountType defines the accounting nature of the account:
- Asset
- Liability
- Equity
- Revenue
- Expense

NormalBalance defines the natural side of the account:
- Asset and Expense accounts normally have Debit balance
- Liability, Equity and Revenue accounts normally have Credit balance

The accounting subsystem uses accounts to record balanced double-entry transactions.

Examples:
- Cash
- Bank
- Customers
- Suppliers
- Sales Revenue
- Purchase Expenses
- VAT Payable
- VAT Receivable
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,
    Name @NVARCHAR(96) @NOT_NULL,

    ParentAccountId @NVARCHAR(40) @NULL,              -- Lookup Account

    AccountTypeId int @NOT_NULL,                      -- Enum AccountType
    NormalBalanceId int @NOT_NULL,                    -- Enum NormalBalance

    IsPosting @BOOL DEFAULT 1 @NOT_NULL,
    IsActive @BOOL DEFAULT 1 @NOT_NULL,

    Remarks @NBLOB_TEXT @NULL,                        -- LargeMemo

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (ParentAccountId) REFERENCES Account(Id)
    )




/*---------------------------------------------------
Table: JournalEntry  
Module: JournalEntry JournalEntryDataModule
Group: Accounting
FieldGroups: Source, Document, Relations, Audit, Notes
-----------------------------------------------------
Represents one accounting journal entry.

A journal entry records one balanced double-entry accounting transaction.

Each JournalEntry contains two or more JournalEntryLine records.
The total debit amount of all lines must always equal the total credit amount.

Journal entries may be entered manually or generated automatically by
posting business documents.

Typical sources include:
- sales invoices
- purchase invoices
- customer receipts
- supplier payments
- inventory adjustments
- asset depreciation

After posting, a journal entry becomes immutable.

Corrections are performed through reversal journal entries rather than
editing or deleting posted records.
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                     -- Code Draft JE-YYYY-XXXXXX JournalEntry

    EntryDate @DATE @NOT_NULL,

    StatusId int DEFAULT 0 @NOT_NULL,                 -- Enum TradeStatus

    TotalDebit @DECIMAL DEFAULT 0 @NOT_NULL,
    TotalCredit @DECIMAL DEFAULT 0 @NOT_NULL,

    SourceModule @NVARCHAR(64) @NULL,                 -- Group Source
    SourceTable @NVARCHAR(64) @NULL,                  -- Group Source
    SourceId @NVARCHAR(40) @NULL,                     -- Group Source

    DocumentTypeId @NVARCHAR(40) @NULL,               -- Lookup; Group Document
    DocumentCode @NVARCHAR(40) @NULL,                 -- Group Document
    DocumentDate @DATE @NULL,                         -- Group Document
    TradeTypeId int default 0 @NOT_NULL,              -- Enum; [Hidden]

    Remarks @NBLOB_TEXT @NULL,                        -- LargeMemo; Group Notes

    CancelledDocumentId @NVARCHAR(40) @NULL,          -- Locator JournalEntry; Group Relations
    CancellationDocumentId @NVARCHAR(40) @NULL,       -- Locator JournalEntry; Group Relations

    CreatedAt @DATE_TIME @NOT_NULL,                   -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,                --  Lookup SYS_APP_USER; Group Audit
    ModifiedAt @DATE_TIME @NULL,                      -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                   --  Lookup SYS_APP_USER; Group Audit

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT CHK_{TableName}_Totals CHECK (TotalDebit = TotalCredit),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CancelledDocumentId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CancellationDocumentId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES  SYS_APP_USER(Id)
    )

/*---------------------------------------------------
Table: JournalEntryLine
-----------------------------------------------------
Represents one accounting line within a journal entry.

Each line affects exactly one accounting account.

A line may contain either a debit amount or a credit amount, but never both simultaneously.

The sum of all debit lines must equal the sum of all credit lines of the parent JournalEntry.

JournalEntryLine records form the accounting ledger of the ERP system.

Every financial event is ultimately represented by one or more journal entry lines.

Examples:

Sales Invoice
    Customer Account      Debit
    Sales Revenue         Credit
    VAT Payable           Credit

Customer Receipt
    Cash Account          Debit
    Customer Account      Credit

Purchase Invoice
    Expense Account       Debit
    VAT Receivable        Debit
    Supplier Account      Credit

Posted lines are immutable.

Corrections are performed through reversal journal entries rather than direct modification of existing lines.
  
Exactly one of DebitAmount or CreditAmount should be greater than zero.

A journal line should never contain both a debit amount and a credit amount simultaneously.

A line with both amounts equal to zero is considered invalid.  
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    JournalEntryId @NVARCHAR(40) @NOT_NULL,          -- Master

    DisplayOrder int @NOT_NULL,

    AccountId @NVARCHAR(40) @NOT_NULL,               -- Lookup

    DebitAmount @DECIMAL DEFAULT 0 @NOT_NULL,
    CreditAmount @DECIMAL DEFAULT 0 @NOT_NULL,

    CurrencyId @NVARCHAR(40) @NULL,                  -- Lookup
    ExchangeRate @DECIMAL DEFAULT 1 @NOT_NULL,

    ReferenceNo @NVARCHAR(40) @NULL,
    Remarks @NVARCHAR(512) @NULL,

    SourceModule @NVARCHAR(64) @NULL,
    SourceTable @NVARCHAR(64) @NULL,
    SourceId @NVARCHAR(40) @NULL, 

    CONSTRAINT CHK_{TableName}_DebitAmount CHECK (DebitAmount >= 0),
    CONSTRAINT CHK_{TableName}_CreditAmount CHECK (CreditAmount >= 0),

    FOREIGN KEY (JournalEntryId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (AccountId) REFERENCES Account(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id)
    )



/*---------------------------------------------------
Table: Asset 
Module: Asset AssetDataModule
Group: Assets
FieldGroups: Classification, Acquisition, Depreciation, Supplier, Audit, Notes
-----------------------------------------------------
Represents a fixed asset owned by the company.

Assets are long-term resources used by the business and are subject to
depreciation over their useful life.

Examples:
- vehicles
- computers
- machinery
- furniture
- office equipment

An asset may generate depreciation records during its lifetime and may
eventually be sold, disposed, or scrapped.

The asset module is responsible for tracking:
- acquisition cost
- depreciation
- accumulated depreciation
- book value
- asset lifecycle events

Current BookValue is calculated as:

AcquisitionCost - AccumulatedDepreciation
----------------------------------------------------*/
CREATE TABLE {TableName} (
    Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                  -- Code AST-XXXXXX Asset
    Name @NVARCHAR(96) @NOT_NULL,

    AssetCategoryId @NVARCHAR(40) @NOT_NULL,       -- Lookup; Group Classification
    AssetLocationId @NVARCHAR(40) @NULL,           -- Lookup; Group Classification

    StatusId int DEFAULT 1 @NOT_NULL,              -- Enum AssetStatus

    AcquisitionDate @DATE @NOT_NULL,               -- Group Acquisition
    InServiceDate @DATE @NULL,                     -- Group Acquisition

    AcquisitionCost @DECIMAL @NOT_NULL,            -- Group Acquisition

    DepreciationMethodId @NVARCHAR(40) @NOT_NULL,  -- Lookup; Group Depreciation
    UsefulLifeMonths int @NOT_NULL,                -- Group Depreciation
    SalvageValue @DECIMAL DEFAULT 0 @NOT_NULL,     -- Group Depreciation

    AccumulatedDepreciation @DECIMAL DEFAULT 0 @NOT_NULL, -- Group Depreciation
    BookValue @DECIMAL DEFAULT 0 @NOT_NULL,        -- Group Depreciation

    SerialNumber @NVARCHAR(96) @NULL,              -- Group Classification

    SupplierId @NVARCHAR(40) @NULL,                -- Locator Supplier; Group Supplier

    Remarks @NBLOB_TEXT @NULL,                     -- LargeMemo; Group Notes

    CreatedAt @DATE_TIME @NOT_NULL,                -- Group Audit
    CreatedBy @NVARCHAR(40) @NOT_NULL,             --  Lookup SYS_APP_USER; Group Audit
    ModifiedAt @DATE_TIME @NULL,                   -- Group Audit
    ModifiedBy @NVARCHAR(40) @NULL,                --  Lookup SYS_APP_USER; Group Audit

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),

    FOREIGN KEY (AssetCategoryId) REFERENCES AssetCategory(Id),
    FOREIGN KEY (AssetLocationId) REFERENCES AssetLocation(Id),
    FOREIGN KEY (DepreciationMethodId) REFERENCES AssetDepreciationMethod(Id),
    FOREIGN KEY (SupplierId) REFERENCES ProductSupplier(Id),
    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES  SYS_APP_USER(Id)
    )



/*---------------------------------------------------
Table: AssetDepreciationLine 
-----------------------------------------------------
Represents one depreciation event of a fixed asset.

Each row records the depreciation amount calculated for a specific accounting period.

Depreciation records are immutable and form the depreciation history of the asset.

Depreciation lines are used to:
- calculate accumulated depreciation
- calculate current book value
- provide auditability of asset valuation
- generate accounting journal entries

One Asset may have many depreciation lines during its useful life.

A depreciation line may be linked to the JournalEntry generated for the same depreciation event.

Corrections are performed through reversal records and reversal journal entries rather than direct modification of existing depreciation records.

The parent Asset stores AccumulatedDepreciation and BookValue as cached current values, but the authoritative depreciation history is this table.
----------------------------------------------------*/
CREATE TABLE {TableName} (
   Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    AssetId @NVARCHAR(40) @NOT_NULL,                  -- Master

    DepreciationDate @DATE @NOT_NULL,

    DepreciationAmount @DECIMAL DEFAULT 0 @NOT_NULL,
    AccumulatedDepreciation @DECIMAL DEFAULT 0 @NOT_NULL,
    BookValueAfter @DECIMAL DEFAULT 0 @NOT_NULL,

    JournalEntryId @NVARCHAR(40) @NULL,               -- Lookup

    Remarks @NVARCHAR(512) @NULL,

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                --  Lookup SYS_APP_USER

    FOREIGN KEY (AssetId) REFERENCES Asset(Id),
    FOREIGN KEY (JournalEntryId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CreatedBy) REFERENCES  SYS_APP_USER(Id)
    )
