
/*---------------------------------------------------
Table: DocumentType
Module: DocumentType
Group: Documents
IsLookup
-----------------------------------------------------  
Defines document types and their posting behavior.

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

    NumberSeriesId @NVARCHAR(40) @NULL,                -- Lookup

    HandlerClass @NVARCHAR(256) @NULL,                  -- IDocumentHandler full class name

    IsActive @BOOL default 1 @NOT_NULL,
    IsSystem @BOOL default 0 @NOT_NULL,                 -- system defined and protected type
    AllowManualNumber @BOOL default 0 @NOT_NULL,
    AutoComplete @BOOL default 0 @NOT_NULL,

    AffectsStock @BOOL default 0 @NOT_NULL,
    AffectsFinancial @BOOL default 0 @NOT_NULL,
    AffectsAccounting @BOOL default 0 @NOT_NULL,

    StockDirection int default 0 @NOT_NULL,
    FinancialDirection int default 0 @NOT_NULL,
    AccountingDirection int default 0 @NOT_NULL,

    IsCancellation @BOOL default 0 @NOT_NULL,
    CancellationTargetId @NVARCHAR(40) @NULL,          -- Lookup  -- what document type may cancel

    PrintTemplate @NVARCHAR(96) @NULL,
    ReportName @NVARCHAR(96) @NULL,

    DisplayOrder int default 0 @NOT_NULL,

    Color @NVARCHAR(32) @NULL,                          -- ui display color
    IconName @NVARCHAR(96) @NULL,                       -- ui icon

    Remarks @NBLOB_TEXT @NULL,

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT UQ_{TableName}_Name UNIQUE (Name),

    FOREIGN KEY (NumberSeriesId) REFERENCES SYS_NUMBER_SERIES(Id),
    FOREIGN KEY (CancellationTargetId) REFERENCES DocumentType(Id)
    )


/*---------------------------------------------------
Table: Trade
Group: Sales
Module: Trade
-----------------------------------------------------
Commercial document header.

Used for sales and purchase documents:
orders, delivery notes, invoices, returns and cancellations.
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup
    Code @NVARCHAR(40) @NOT_NULL,                       -- Code TR-DRAFT-YYYY-XXXXXX TRADE-DRAFT

    TradeStatusId int default 0 @NOT_NULL,              -- Enum TradeStatus
    TaxTreatmentId int default 1 @NOT_NULL,             -- Enum TaxTreatment

    TradeDate @DATE @NOT_NULL,
    PostingDate @DATE @NULL,
    DeliveryDate @DATE @NULL,
    DueDate @DATE @NULL,

    ExternalRef @NVARCHAR(96) @NULL,                    -- e.g. "Related to Order 123", "Your ref: PO-456"

    PersonId @NVARCHAR(40) @NOT_NULL,                   -- Locator Person -- Customer, Supplier, etc
    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup

    SalesPersonId @NVARCHAR(40) @NULL,                  -- Lookup Person
    ProjectId @NVARCHAR(40) @NULL,                      -- Lookup
    CostCenterId @NVARCHAR(40) @NULL,                   -- Lookup
    BranchId @NVARCHAR(40) @NULL,                       -- Lookup

    CurrencyId @NVARCHAR(40) @NOT_NULL,                 -- Lookup
    ExchangeRate @DECIMAL default 1 @NOT_NULL,          -- Exchange Rate for base currency

    PaymentMethodId @NVARCHAR(40) @NULL,                -- Lookup
    PaymentTermId @NVARCHAR(40) @NULL,                  -- Lookup

    BillingName @NVARCHAR(96) @NULL,
    BillingAddressLine1 @NVARCHAR(128) @NULL,
    BillingAddressLine2 @NVARCHAR(128) @NULL,
    BillingCity @NVARCHAR(64) @NULL,
    BillingPostalCode @NVARCHAR(20) @NULL,
    BillingCountryId @NVARCHAR(40) @NULL,               -- Lookup

    ShippingName @NVARCHAR(96) @NULL,
    ShippingAddressLine1 @NVARCHAR(128) @NULL,
    ShippingAddressLine2 @NVARCHAR(128) @NULL,
    ShippingCity @NVARCHAR(64) @NULL,
    ShippingPostalCode @NVARCHAR(20) @NULL,
    ShippingCountryId @NVARCHAR(40) @NULL,              -- Lookup

    SourceId @NVARCHAR(40) @NULL,                       -- Locator Trade
    CancelsTradeId @NVARCHAR(40) @NULL,                 -- Locator Trade
    CancelledByTradeId @NVARCHAR(40) @NULL,             -- Locator Trade

    LinesAmount @DECIMAL default 0 @NOT_NULL,           -- sum of lines before header discounts/charges/taxes
    DiscountPercent @DECIMAL default 0 @NOT_NULL,       -- Header Discount %
    DiscountAmount @DECIMAL default 0 @NOT_NULL,
    DiscountReason @NVARCHAR(256) @NULL,

    ChargesAmount @DECIMAL default 0 @NOT_NULL,

    NetAmount @DECIMAL default 0 @NOT_NULL,             -- = LinesAmount - DiscountAmount + ChargesAmount
    VatAmount @DECIMAL default 0 @NOT_NULL,
    TotalAmount @DECIMAL default 0 @NOT_NULL,

    IsLocked @BOOL default 0 @NOT_NULL,                 -- Lock document from editing
    IsCancelled @BOOL default 0 @NOT_NULL,

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  -- Lookup AppUser
    ModifiedAt @DATE_TIME @NULL,
    ModifiedBy @NVARCHAR(40) @NULL,                     -- Lookup AppUser
    PostedAt @DATE_TIME @NULL,
    PostedBy @NVARCHAR(40) @NULL,                       -- Lookup AppUser
    CancelledAt @DATE_TIME @NULL,
    CancelledBy @NVARCHAR(40) @NULL,                    -- Lookup AppUser

    Remarks @NVARCHAR(512) @NULL,                       -- internal
    Comments @NVARCHAR(512) @NULL,                      -- customer visible                      

    CONSTRAINT UQ_{TableName}_DocumentType_Code UNIQUE (DocumentTypeId, Code),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),

    FOREIGN KEY (PersonId) REFERENCES Person(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (SalesPersonId) REFERENCES Person(Id),
    FOREIGN KEY (ProjectId) REFERENCES Project(Id),
    FOREIGN KEY (CostCenterId) REFERENCES CostCenter(Id),
    FOREIGN KEY (BranchId) REFERENCES Branch(Id),

    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),

    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethod(Id),
    FOREIGN KEY (PaymentTermId) REFERENCES PaymentTerm(Id),

    FOREIGN KEY (BillingCountryId) REFERENCES Country(Id),
    FOREIGN KEY (ShippingCountryId) REFERENCES Country(Id),

    FOREIGN KEY (SourceId) REFERENCES Trade(Id),
    FOREIGN KEY (CancelsTradeId) REFERENCES Trade(Id),
    FOREIGN KEY (CancelledByTradeId) REFERENCES Trade(Id),

    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (PostedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (CancelledBy) REFERENCES AppUser(Id)
    )



/*---------------------------------------------------
Table: TradeTax
-----------------------------------------------------
Hidden detail table.

Stores VAT summary lines per VAT rate for a Trade document.
Generated and maintained by TradeDataModule.
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    TradeId @NVARCHAR(40) @NOT_NULL,                    -- Master
    VatRateId @NVARCHAR(40) @NOT_NULL,                  -- Lookup

    VatRatePercent @DECIMAL default 0 @NOT_NULL,        -- Snapshot of the percent at production time

    NetAmount @DECIMAL default 0 @NOT_NULL,
    VatAmount @DECIMAL default 0 @NOT_NULL,
    TotalAmount @DECIMAL default 0 @NOT_NULL,

    CONSTRAINT UQ_{TableName}_Trade_VatRate UNIQUE (TradeId, VatRateId),

    FOREIGN KEY (TradeId) REFERENCES Trade(Id),
    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id)
    )



/*---------------------------------------------------
Table: TradeLine
Master: Trade
-----------------------------------------------------
Commercial document line.
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL primary key,

    TradeId @NVARCHAR(40) @NOT_NULL,                    -- Master

    LineNo int @NOT_NULL,

    LineTypeId int default 1 @NOT_NULL,                 -- Enum TradeLineType

    ProductId @NVARCHAR(40) @NULL,                      -- Locator Product

    ProductCode @NVARCHAR(40) @NULL,                    -- Snapshot
    ProductName @NVARCHAR(128) @NULL,                   -- Snapshot

    Description @NVARCHAR(256) @NULL,                   -- Snapshot

    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup (line override)

    UnitOfMeasureId @NVARCHAR(40) @NULL,                -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NULL,              -- Snapshot

    UnitRatio @DECIMAL default 1 @NOT_NULL,             -- Snapshot ratio to primary unit

    Quantity @DECIMAL default 0 @NOT_NULL,
    PrimaryUnitQuantity @DECIMAL default 0 @NOT_NULL,

    ReservedQuantity @DECIMAL default 0 @NOT_NULL,
    ExecutedQuantity @DECIMAL default 0 @NOT_NULL,

    VatRateId @NVARCHAR(40) @NULL,                      -- Lookup
    VatRatePercent @DECIMAL default 0 @NOT_NULL,        -- Snapshot

    UnitPrice @DECIMAL default 0 @NOT_NULL,

    GrossAmount @DECIMAL default 0 @NOT_NULL,           -- Quantity * UnitPrice

    DiscountPercent @DECIMAL default 0 @NOT_NULL,
    DiscountAmount @DECIMAL default 0 @NOT_NULL,

    NetUnitPrice @DECIMAL default 0 @NOT_NULL,          -- Display/convenience value

    NetAmount @DECIMAL default 0 @NOT_NULL,             -- GrossAmount - DiscountAmount
    VatAmount @DECIMAL default 0 @NOT_NULL,
    TotalAmount @DECIMAL default 0 @NOT_NULL,

    SourceTradeLineId @NVARCHAR(40) @NULL,              -- Locator TradeLine

    CONSTRAINT UQ_{TableName}_Trade_LineNo UNIQUE (TradeId, LineNo),

    FOREIGN KEY (TradeId) REFERENCES Trade(Id),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),

    FOREIGN KEY (VatRateId) REFERENCES VatRate(Id),

    FOREIGN KEY (SourceTradeLineId) REFERENCES TradeLine(Id)
    )




/*---------------------------------------------------
Table: StockTrade
Group: Inventory
Module: StockTrade
Form: Default
-----------------------------------------------------
Warehouse transaction document.

Used for pure stock operations such as:
- warehouse transfer
- stock count adjustment
- destruction / write-off
- internal stock correction

Posting this document produces StockMovement rows.
It does not represent sales or purchases.
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup -- controls numbering, posting behavior and movement direction
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup -- main/source warehouse
    ToWarehouseId @NVARCHAR(40) @NULL,                  -- Lookup -- destination warehouse, used only for transfers

    Code @NVARCHAR(40) @NOT_NULL,                       -- Code STK-DRAFT-YYYY-XXXXXX STOCK_TRADE_DRAFT
    DocumentDate @DATE @NOT_NULL,
    PostingDate @DATE @NULL,                            -- date used for generated stock movements
    StatusId int @NOT_NULL,                             -- Enum TradeStatus -- Draft, Posted, Cancelled

    TotalCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,       -- total internal stock cost value posted by this document

    Remarks @NVARCHAR(512) @NULL,                       -- internal notes

    IsLocked @BOOL DEFAULT 0 @NOT_NULL,
    IsCancelled @BOOL DEFAULT 0 @NOT_NULL,

    CancelsStockTradeId @NVARCHAR(40) @NULL,            -- Locator StockTrade -- original document cancelled by this one
    CancelledByStockTradeId @NVARCHAR(40) @NULL,        -- Locator StockTrade -- reverse/cancellation document

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  -- Lookup AppUser
    ModifiedAt @DATE_TIME @NULL,
    ModifiedBy @NVARCHAR(40) @NULL,                     -- Lookup AppUser
    PostedAt @DATE_TIME @NULL,
    PostedBy @NVARCHAR(40) @NULL,                       -- Lookup AppUser
    CancelledAt @DATE_TIME @NULL,
    CancelledBy @NVARCHAR(40) @NULL,                    -- Lookup AppUser

    CONSTRAINT UQ_{TableName}_DocumentType_Code UNIQUE (DocumentTypeId, Code),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (ToWarehouseId) REFERENCES Warehouse(Id),

    FOREIGN KEY (CancelsStockTradeId) REFERENCES StockTrade(Id),
    FOREIGN KEY (CancelledByStockTradeId) REFERENCES StockTrade(Id),

    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (PostedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (CancelledBy) REFERENCES AppUser(Id)
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
    LineNo int @NOT_NULL,

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    ProductCode @NVARCHAR(40) @NOT_NULL,                -- product code snapshot
    ProductName @NVARCHAR(128) @NOT_NULL,               -- product name snapshot

    WarehouseId @NVARCHAR(40) @NULL,                    -- Lookup -- optional source warehouse override

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,            -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NOT_NULL,          -- unit of measure snapshot
    UnitRatio @DECIMAL DEFAULT 1 @NOT_NULL,             -- converts line quantity to primary/base quantity

    Quantity @DECIMAL DEFAULT 0 @NOT_NULL,              -- always positive; direction is determined by DocumentType
    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- Quantity * UnitRatio

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,              -- internal stock cost per primary unit
    CostAmount @DECIMAL DEFAULT 0 @NOT_NULL,            -- PrimaryQuantity * UnitCost

    SourceTradeLineId @NVARCHAR(40) @NULL,              -- optional source commercial line
    SourceStockTradeLineId @NVARCHAR(40) @NULL,         -- optional source stock line, e.g. reversal/copy/adjustment flow

    Remarks @NVARCHAR(512) @NULL,                       -- internal line notes

    CONSTRAINT UQ_{TableName}_StockTrade_LineNo UNIQUE (StockTradeId, LineNo),

    FOREIGN KEY (StockTradeId) REFERENCES StockTrade(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),
    FOREIGN KEY (SourceTradeLineId) REFERENCES TradeLine(Id),
    FOREIGN KEY (SourceStockTradeLineId) REFERENCES StockTradeLine(Id)
    )

/*---------------------------------------------------
Table: StockMovement
Group: Inventory
Module: StockMovement
  
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

    ProductId @NVARCHAR(40) @NOT_NULL,                  -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,                -- Lookup

    MovementDate @DATE @NOT_NULL,                       -- stock ledger date
    Direction int @NOT_NULL,                            -- 1=in, -1=out

    Quantity @DECIMAL DEFAULT 0 @NOT_NULL,              -- always positive, in movement unit
    PrimaryQuantity @DECIMAL DEFAULT 0 @NOT_NULL,       -- quantity in product primary unit

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,            -- Lookup
    UnitOfMeasureName @NVARCHAR(40) @NOT_NULL,          -- unit of measure snapshot
    UnitRatio @DECIMAL DEFAULT 1 @NOT_NULL,             -- converts Quantity to PrimaryQuantity

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,              -- internal stock cost per primary unit at movement time
    CostAmount @DECIMAL DEFAULT 0 @NOT_NULL,            -- PrimaryQuantity * UnitCost

    SourceModule @NVARCHAR(64) @NOT_NULL,               -- source module name, e.g. Trade or StockTrade
    SourceTable @NVARCHAR(64) @NOT_NULL,                -- source line table, e.g. TradeLine or StockTradeLine
    SourceId @NVARCHAR(40) @NOT_NULL,                   -- source line Id

    DocumentTypeId @NVARCHAR(40) @NOT_NULL,             -- Lookup -- source document type
    DocumentCode @NVARCHAR(40) @NOT_NULL,               -- source document code snapshot
    DocumentDate @DATE @NOT_NULL,                       -- source document date snapshot

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                  -- Lookup AppUser

    CONSTRAINT CHK_{TableName}_Direction CHECK (Direction IN (1, -1)),
    CONSTRAINT CHK_{TableName}_Quantity CHECK (Quantity >= 0),
    CONSTRAINT CHK_{TableName}_PrimaryQuantity CHECK (PrimaryQuantity >= 0),

    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id)
    )

/*---------------------------------------------------
Table: StockBalance
Group: Inventory
Module: StockBalance 

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
Group: Inventory
Module: StockCount
-----------------------------------------------------
Physical inventory count document.

Used to verify actual warehouse quantities against system quantities and produce inventory adjustment movements.

After posting, the document generates StockMovement records for quantity differences and becomes immutable.

Used for:
- periodic inventory counts
- cycle counts
- stock corrections
- inventory reconciliation
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                     -- Code [SC-YYYY-XXXXXX] [STOCK_COUNT]

    WarehouseId @NVARCHAR(40) @NOT_NULL,              -- Lookup

    CountDate @DATE @NOT_NULL,

    StatusId int DEFAULT 0 @NOT_NULL,                 -- Enum TradeStatus

    Remarks @NBLOB_TEXT @NULL,

    CancelledDocumentId @NVARCHAR(40) @NULL,
    CancellationDocumentId @NVARCHAR(40) @NULL,

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                -- Lookup AppUser
    ModifiedAt @DATE_TIME @NULL,
    ModifiedBy @NVARCHAR(40) @NULL,                   -- Lookup AppUser

    FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id),
    FOREIGN KEY (CancelledDocumentId) REFERENCES StockCount(Id),
    FOREIGN KEY (CancellationDocumentId) REFERENCES StockCount(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id)
    )

/*---------------------------------------------------
Table: StockCountLine
Master: StockCount
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

    StockCountId @NVARCHAR(40) @NOT_NULL,             -- Master

    LineNo int @NOT_NULL,

    ProductId @NVARCHAR(40) @NOT_NULL,                -- Locator Product

    ProductCode @NVARCHAR(40) @NOT_NULL,
    ProductName @NVARCHAR(96) @NOT_NULL,

    UnitOfMeasureId @NVARCHAR(40) @NOT_NULL,          -- Lookup

    SystemQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    CountedQuantity @DECIMAL DEFAULT 0 @NOT_NULL,
    DifferenceQuantity @DECIMAL DEFAULT 0 @NOT_NULL,

    UnitCost @DECIMAL DEFAULT 0 @NOT_NULL,
    DifferenceCostAmount @DECIMAL DEFAULT 0 @NOT_NULL,

    Remarks @NVARCHAR(512) @NULL,

    FOREIGN KEY (StockCountId) REFERENCES StockCount(Id),
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    FOREIGN KEY (UnitOfMeasureId) REFERENCES UnitOfMeasure(Id),

    CONSTRAINT UQ_{TableName}_LineNo UNIQUE (StockCountId, LineNo)
    )

/*---------------------------------------------------
Table: StockReservation
Group: Inventory
Module: StockReservation

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

    ProductId @NVARCHAR(40) @NOT_NULL,                 -- Locator Product
    WarehouseId @NVARCHAR(40) @NOT_NULL,              -- Lookup

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
Group: Finance
Module: FinanceMovement

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
    CreatedBy @NVARCHAR(40) @NOT_NULL,               -- Lookup AppUser

    CONSTRAINT CHK_{TableName}_Direction CHECK (Direction IN (1, -1)),
    CONSTRAINT CHK_{TableName}_Amount CHECK (Amount >= 0),

    FOREIGN KEY (CashAccountId) REFERENCES CashAccount(Id),
    FOREIGN KEY (CompanyBankAccountId) REFERENCES CompanyBankAccount(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id),
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id)
    )

/*---------------------------------------------------
Table: FinanceBalance
Group: Finance
Module: FinanceBalance

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
Group: Accounting
Module: Account
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
Group: Accounting
Module: JournalEntry
-----------------------------------------------------
Represents one accounting journal entry.

A journal entry is the accounting document that records one balanced double-entry transaction.

Each JournalEntry contains two or more JournalEntryLine records.

The total debit amount of all lines must always equal the total credit amount.

Journal entries may be entered manually or generated automatically by posting business documents.

Typical sources include:
- sales invoices
- purchase invoices
- customer receipts
- supplier payments
- inventory adjustments
- asset depreciation

After posting, a journal entry becomes immutable.

Corrections are made through reversal journal entries, never by editing or deleting posted records.
----------------------------------------------------*/
CREATE TABLE {TableName} (
                             Id @NVARCHAR(40) @NOT_NULL PRIMARY KEY,

    Code @NVARCHAR(40) @NOT_NULL,                     -- Code [JE-YYYY-XXXXXX] [JOURNAL_ENTRY]

    EntryDate @DATE @NOT_NULL,

    StatusId int DEFAULT 0 @NOT_NULL,                 -- Enum TradeStatus

    TotalDebit @DECIMAL DEFAULT 0 @NOT_NULL,
    TotalCredit @DECIMAL DEFAULT 0 @NOT_NULL,

    SourceModule @NVARCHAR(64) @NULL,
    SourceTable @NVARCHAR(64) @NULL,
    SourceId @NVARCHAR(40) @NULL,

    DocumentTypeId @NVARCHAR(40) @NULL,               -- Lookup
    DocumentCode @NVARCHAR(40) @NULL,
    DocumentDate @DATE @NULL,

    Remarks @NBLOB_TEXT @NULL,                        -- LargeMemo

    CancelledDocumentId @NVARCHAR(40) @NULL,
    CancellationDocumentId @NVARCHAR(40) @NULL,

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,                -- Lookup AppUser
    ModifiedAt @DATE_TIME @NULL,
    ModifiedBy @NVARCHAR(40) @NULL,                   -- Lookup AppUser

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),
    CONSTRAINT CHK_{TableName}_Totals CHECK (TotalDebit = TotalCredit),

    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
    FOREIGN KEY (CancelledDocumentId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CancellationDocumentId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id)
    )

/*---------------------------------------------------
Table: JournalEntryLine
Master: JournalEntry
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

    LineNo int @NOT_NULL,

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

    CONSTRAINT UQ_{TableName}_LineNo UNIQUE (JournalEntryId, LineNo),

    CONSTRAINT CHK_{TableName}_DebitAmount CHECK (DebitAmount >= 0),
    CONSTRAINT CHK_{TableName}_CreditAmount CHECK (CreditAmount >= 0),

    FOREIGN KEY (JournalEntryId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (AccountId) REFERENCES Account(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currency(Id)
    )

/*---------------------------------------------------
Table: Asset
Group: Assets
Module: Asset
-----------------------------------------------------
Represents a fixed asset owned by the company.

Assets are long-term resources used by the business and are subject to depreciation over their useful life.

Examples:
- vehicles
- computers
- machinery
- furniture
- office equipment

An asset may generate depreciation records during its lifetime and may eventually be sold, disposed, or scrapped.

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

    Code @NVARCHAR(40) @NOT_NULL,                  -- Code [AST-XXXXXX] [ASSET]
    Name @NVARCHAR(96) @NOT_NULL,

    AssetCategoryId @NVARCHAR(40) @NOT_NULL,       -- Lookup
    AssetLocationId @NVARCHAR(40) @NULL,           -- Lookup

    StatusId int DEFAULT 1 @NOT_NULL,              -- Enum AssetStatus

    AcquisitionDate @DATE @NOT_NULL,
    InServiceDate @DATE @NULL,

    AcquisitionCost @DECIMAL @NOT_NULL,

    DepreciationMethodId @NVARCHAR(40) @NOT_NULL,  -- Lookup

    UsefulLifeMonths int @NOT_NULL,

    SalvageValue @DECIMAL DEFAULT 0 @NOT_NULL,

    AccumulatedDepreciation @DECIMAL DEFAULT 0 @NOT_NULL,
    BookValue @DECIMAL DEFAULT 0 @NOT_NULL,

    SerialNumber @NVARCHAR(96) @NULL,

    SupplierId @NVARCHAR(40) @NULL,                -- Locator Supplier

    Remarks @NBLOB_TEXT @NULL,                     -- LargeMemo

    CreatedAt @DATE_TIME @NOT_NULL,
    CreatedBy @NVARCHAR(40) @NOT_NULL,             -- Lookup AppUser
    ModifiedAt @DATE_TIME @NULL,
    ModifiedBy @NVARCHAR(40) @NULL,                -- Lookup AppUser

    CONSTRAINT UQ_{TableName}_Code UNIQUE (Code),

    FOREIGN KEY (AssetCategoryId) REFERENCES AssetCategory(Id),
    FOREIGN KEY (AssetLocationId) REFERENCES AssetLocation(Id),
    FOREIGN KEY (DepreciationMethodId) REFERENCES AssetDepreciationMethod(Id),
    FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id),
    FOREIGN KEY (ModifiedBy) REFERENCES AppUser(Id)
    )

/*---------------------------------------------------
Table: AssetDepreciationLine
Master: Asset
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
    CreatedBy @NVARCHAR(40) @NOT_NULL,                -- Lookup AppUser

    FOREIGN KEY (AssetId) REFERENCES Asset(Id),
    FOREIGN KEY (JournalEntryId) REFERENCES JournalEntry(Id),
    FOREIGN KEY (CreatedBy) REFERENCES AppUser(Id)
    )