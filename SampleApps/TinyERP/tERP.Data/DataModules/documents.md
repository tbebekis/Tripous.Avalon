# Documents

## Purpose

The document data modules provide the business logic for transactional documents in tERP.

The current implementation covers sales, purchases, stock documents, manual journal entries, payments, posting, cancellation, document transformation, stock operations, finance movements, and accounting integration.

Data modules do not show UI, modal dialogs, message boxes, or wait for user interaction. They may be used from services or other non-UI callers. UI feedback belongs to forms and other desktop-layer classes.

## Current Status

- The main tERP document cycle is complete for the current checkpoint.
- Sales, Purchases, Stock Trade, Journal Entries, partner finance movements, Payments, Payment Cancellations, and Payment Settlements are implemented.
- Build and all known unit tests succeeded at the previous checkpoint.
- Remaining work is tester guide, release documentation, supporting-module smoke tests, and future extensions.

## Data Module Hierarchy

```text
DataModule (Tripous.Data)
    AppDataModule (tERP.Data)
        DocumentDataModule
            JournalEntryDataModule
            PaymentDataModule
            StockCountDataModule
            StockTradeDataModule
            TradeDataModule
                SalesDataModule
                    SalesOrderDataModule
                    SalesStockDataModule
                        SalesDeliveryNoteDataModule
                        SalesReturnDataModule
                    SalesInvoiceDataModule
                    SalesCreditNoteDataModule
                    SalesCancellationDataModule
                PurchaseDataModule
                    PurchaseOrderDataModule
                    PurchaseStockDataModule
                        PurchaseDeliveryNoteDataModule
                        PurchaseReturnDataModule
                    PurchaseInvoiceDataModule
                    PurchaseCreditNoteDataModule
                    PurchaseCancellationDataModule
```

`DocumentDataModule` provides the common document infrastructure.

`JournalEntryDataModule` handles manual accounting entries and validates debit and credit balance.

`PaymentDataModule` handles customer receipts, supplier payments, and their cancellation documents.

`StockCountDataModule` handles inventory counts and stock adjustments.

`StockTradeDataModule` handles standalone stock receipts, issues, transfers, and stock cancellations.

`TradeDataModule` adds commercial calculations, pricing, taxes, document discounts, totals, and validation.

`SalesDataModule` adds sales defaults, customer address snapshots, sales pricing validation, and sales line defaults.

`SalesStockDataModule` adds sales stock movement posting for delivery and return documents.

`PurchaseDataModule` adds purchase defaults, supplier address snapshots, purchase pricing validation, and purchase line defaults.

`PurchaseStockDataModule` adds purchase stock movement posting for delivery and return documents.

The concrete document modules act as document-type entry points and add document-specific transformation, posting, and cancellation behavior where needed.

## Document Types And Number Series

`DocumentDataModule.Initialize()` creates the document context from the registered module.

`AssignCodeProviderDef()` resolves:

- `DocumentType`
- `DraftCodeProviderDef`
- `FinalCodeProviderDef`

`GetCodeProviderDef()` returns the draft provider during normal editing and the final provider while `IsPosting` is true.

`GetFinalCodeProviderDefFromDocumentType()` resolves the final number series configured for the current document type.

`AssignCodeValue()` replaces the draft code with the next final code while posting. The final number is assigned inside the same database transaction that saves the document.

The current document modules are:

- Sales: `SalesOrder`, `SalesDeliveryNote`, `SalesInvoice`, `SalesCreditNote`, `SalesReturn`, `SalesCancellation`
- Purchases: `PurchaseOrder`, `PurchaseDeliveryNote`, `PurchaseInvoice`, `PurchaseCreditNote`, `PurchaseReturn`, `PurchaseCancellation`
- Stock: `StockTrade`, `StockCount`
- Accounting: `JournalEntry`
- Finance: `CustomerReceipt`, `CustomerReceiptCancellation`, `SupplierPayment`, `SupplierPaymentCancellation`

## Document Handlers

`DocumentDataModule.Initialize()` creates the handler registered for the current module.

`CreateDocumentContext()` creates the `DocumentContext` passed to the handler.

The handler hierarchy is:

```text
DocumentHandler
    JournalEntryDocumentHandler
    PaymentDocumentHandler
    StockCountDocumentHandler
    StockTradeDocumentHandler
    TradeDocumentHandler
        SalesDocumentHandler
            SalesOrderDocumentHandler
            SalesDeliveryNoteDocumentHandler
            SalesInvoiceDocumentHandler
            SalesCreditNoteDocumentHandler
            SalesReturnDocumentHandler
            SalesCancellationDocumentHandler
        PurchaseDocumentHandler
            PurchaseOrderDocumentHandler
            PurchaseDeliveryNoteDocumentHandler
            PurchaseInvoiceDocumentHandler
            PurchaseCreditNoteDocumentHandler
            PurchaseReturnDocumentHandler
            PurchaseCancellationDocumentHandler
```

The base handlers contain shared behavior. Concrete handlers are extension points for document-specific posting logic.

`TradeDocumentHandler`, `PaymentDocumentHandler`, `StockTradeDocumentHandler`, `StockCountDocumentHandler`, and `JournalEntryDocumentHandler` all enforce draft, unlocked, non-cancelled posting rules for their document families.

## Posting

`DocumentDataModule.Post()`:

- Creates the document context.
- Enables `IsPosting`.
- Calls `DocumentHandler.Validate()`.
- Calls `DocumentHandler.Post()`.
- Calls `Commit()`.
- Restores the previous posting values if posting fails.
- Disables `IsPosting` in a `finally` block.

Each document handler permits posting only for an unlocked, non-cancelled draft document in its own document family.

`TradeDocumentHandler.Post()` assigns:

- `TradeStatusId = Posted`
- `PostingDate`
- `PostedAt`
- `PostedBy`
- `IsLocked = true`

`PaymentDocumentHandler.Post()` assigns:

- `StatusId = Posted`
- `PostingDate`
- `PostedAt`
- `PostedBy`
- `IsLocked = true`

`StockTradeDocumentHandler.Post()` assigns:

- `StatusId = Posted`
- `PostingDate`
- `PostedAt`
- `PostedBy`
- `IsLocked = true`

`StockCountDocumentHandler.Post()` assigns:

- `StatusId = Posted`
- `CountDate`
- `PostedAt`
- `PostedBy`
- `IsLocked = true`

`JournalEntryDocumentHandler.Post()` assigns:

- `StatusId = Posted`
- `PostedAt`
- `PostedBy`
- `IsLocked = true`

The final document code is assigned by `DocumentDataModule.AssignCodeValue()` inside the commit transaction.

Before an existing document is saved or posted, `DocumentDataModule` locks and reloads its persisted row inside the commit transaction.

The commit is rejected when:

- The document no longer exists.
- Its `ModifiedAt` value changed after the module loaded it.
- A stale module attempts to save a document that is now locked.
- A stale module attempts to post a document that is no longer a draft.
- The persisted document is cancelled or locked before posting.

This provides optimistic concurrency for document headers and prevents an open stale module from overwriting a document changed by another user.

`DocumentDataForm` asks for confirmation and calls `DocumentDataModule.Post()`.

After posting:

- The document cannot be edited.
- The Save and Post commands are disabled.
- The Add New command remains enabled so the user can immediately start the next document.
- Bound controls and detail grids are read-only.
- Detail Add/Delete commands and their keyboard shortcuts are disabled.

`DocumentDataModule.CheckCanCommit()` also rejects changes to locked documents, independently of the UI.

## Sales Order Lifecycle

tERP currently uses a simplified Sales Order lifecycle.

- There is no separate `Released` status.
- A posted Sales Order represents an approved or released order.
- Only posted Sales Orders can be transformed into Sales Delivery Notes.
- A Sales Order becomes completed when all line quantities are fully delivered.
- Posting a Sales Order does not create stock movements.
- Stock movements are created when the related Sales Delivery Note is posted.

This is a deliberate simplification for the sample application. A future version may introduce a separate release and approval workflow.

## Document Defaults

`DocumentDataModule.SetDefaultValues()` assigns `TradeTypeId` from the current `DocumentType`.

`TradeDataModule.SetDefaultValues()` assigns the following values to a new `Trade` row:

- `DocumentTypeId`
- `TradeStatusId = Draft`
- `TradeDate`
- `ExchangeRate = 1`

`SalesDataModule.SetDefaultValues()` assigns configured sales defaults:

- `WarehouseId`
- `CostCenterId`
- `BranchId`
- `PriceListTypeId`
- `CurrencyId`
- `PaymentMethodId`
- `PaymentTermId`
- `TaxBusinessGroupId`
- `OriginTaxJurisdictionId`
- `DestinationTaxJurisdictionId`

The values are obtained from `SalesDefaults` and fall back to the corresponding `DataLib` default providers.

`PurchaseDataModule.SetDefaultValues()` assigns configured purchase defaults:

- `WarehouseId`
- `CostCenterId`
- `BranchId`
- `PriceListTypeId`
- `CurrencyId`
- `PaymentMethodId`
- `PaymentTermId`
- `TaxBusinessGroupId`
- `OriginTaxJurisdictionId`
- `DestinationTaxJurisdictionId`

The values are obtained from `PurchaseDefaults` and fall back to the corresponding `DataLib` default providers.

`PaymentDataModule.SetDefaultValues()` assigns:

- `DocumentTypeId`
- `StatusId = Draft`
- `PartnerTradeTypeId`
- `PaymentDate`
- `ExchangeRate = 1`
- `CurrencyId`
- `PaymentMethodId`
- `CompanyBankAccountId`

`JournalEntryDataModule.SetDefaultValues()` assigns:

- `DocumentTypeId`
- `StatusId = Draft`
- `EntryDate`
- `TradeTypeId = Accounting`
- `IsLocked = false`
- `IsCancelled = false`

`StockTradeDataModule` and `StockCountDataModule` assign draft status, document dates, warehouse defaults, and lock/cancellation flags needed by their stock posting flows.

## Detail Line Order

`DocumentDataModule.NewRowAdded()` assigns `DisplayOrder` to new detail rows.

The value starts at `10` and increases by `10`.

`DocumentDataModule.Initialize()` sorts detail views by:

- `DisplayOrder`
- `Id`

## Trade Line Defaults

`SalesDataModule.NewRowAdded()` assigns `SalesDefaults.DefaultQuantity` to new trade lines.

`PurchaseDataModule.NewRowAdded()` assigns `PurchaseDefaults.DefaultQuantity` to new trade lines.

`TradeDataModule.NewRowAdded()` copies the document warehouse to detail rows containing a `WarehouseId` field.

## Business Partner Address Snapshots

`SalesDataModule.ColumnChanged()` calls `CopyPersonAddresses()` when `PersonId` changes.

`CopyPersonAddresses()` loads the customer addresses and copies billing and shipping values to the `Trade` row.

`PurchaseDataModule` applies the same behavior when the supplier changes.

These fields are document snapshots. Later changes to the customer addresses do not alter an existing document.

Changing the customer or supplier also reloads the tax business group, resolves prices again, and recalculates the document.

## Price List Snapshot

`Trade.PriceListTypeId` stores the selected price-list type as a document snapshot.

New sales documents receive the value from `SalesDefaults.PriceListTypeId`.

`CreatePriceResolveArgs()` uses the document value rather than reading the current application default. This ensures that reopening a document uses its original pricing context.

## Price Resolution

`CreatePriceResolver()` creates the configured `IPriceResolver`.

`CreatePriceResolveArgs()` provides:

- Trade type
- Price-list type
- Customer
- Product
- Unit of measure
- Quantity
- Trade date
- Currency

`ResolveLinePriceResult()` calls the resolver.

`ResolveLinePrice()` assigns the resolved `UnitPrice`. If no price is found, it preserves the current manual value.

`GetTaxExclusiveUnitPrice()` converts a tax-inclusive list price to a tax-exclusive line price.

`ResolvePrices()` resolves prices for every active trade line.

## Automatic Repricing

`TradeDataModule.ColumnChanged()` performs automatic repricing when the following values change:

- Customer
- Trade date
- Trade type
- Price-list type
- Currency
- Product
- Tax product group
- Unit of measure
- Quantity

A manual `UnitPrice` remains editable. A later pricing-field change replaces it only when an applicable price is resolved.

Automatic currency conversion is not implemented. The price list and the document must currently use the same currency.

## Line Calculations

`CalculateLine()` calculates:

- `PrimaryUnitQuantity`
- `GrossAmount`
- `DiscountPercent`
- `DiscountAmount`
- `NetUnitPrice`
- `NetAmount`

The formulas are:

- `GrossAmount = Quantity * UnitPrice`
- `DiscountAmount = GrossAmount * DiscountPercent`
- `NetAmount = GrossAmount - DiscountAmount`
- `NetUnitPrice = NetAmount / Quantity`

During live editing, the changed discount field is authoritative:

- Changing `DiscountPercent` recalculates `DiscountAmount`.
- Changing `DiscountAmount` recalculates `DiscountPercent`.

During the final calculation in `CheckCanCommit()`, monetary `DiscountAmount` values are authoritative. This prevents percentage rounding from changing saved monetary values during save and reopen.

## Document Discount

`CalculateDocumentDiscount()` calculates and allocates the document discount.

`Trade.DiscountPercent` and `Trade.DiscountAmount` represent the document-level discount.

`TradeLine.DocumentDiscountAmount` stores the allocated share for each line.

The allocation rules are:

- Only active lines with a positive `NetAmount` participate.
- Allocation is proportional to each line's `NetAmount`.
- The rounding remainder is assigned to the last eligible line.
- The sum of all `DocumentDiscountAmount` values equals `Trade.DiscountAmount`.

The line discount and document discount remain separate:

- `TradeLine.DiscountAmount` is the line discount.
- `TradeLine.DocumentDiscountAmount` is the allocated document discount.

## Tax Resolution

`CreateTaxResolver()` creates the `ITaxResolver` implementation configured by `SalesDefaults.TaxResolverClassName`.

`CreateTaxResolveArgs()` builds the complete tax context for each trade line.

Tax resolution uses:

- Trade type and trade date
- Customer tax business group
- Product tax product group
- Origin tax jurisdiction
- Destination tax jurisdiction
- Origin branch address
- Destination billing or shipping address
- Taxable amount

`LoadTaxBusinessGroupId()` loads the tax classification assigned to the selected customer.

`LoadOriginAddress()` loads the company branch address used as the tax origin.

`GetDestinationAddress()` uses the shipping snapshot when a shipping country exists. Otherwise, it uses the billing snapshot.

`TaxResolver.Resolve()` performs the following steps:

- Loads active tax jurisdictions.
- Resolves explicit origin and destination jurisdiction identifiers.
- Falls back to address-based jurisdiction resolution when an explicit identifier is not available.
- Builds each jurisdiction path through its parent hierarchy.
- Loads active tax rules for the tax business group, tax product group, trade type, and trade date.
- Matches rules against the resolved origin and destination jurisdiction paths.
- Gives precedence to applicable jurisdiction-specific rules.
- Calculates tax components in priority order.
- Supports percentage and tax-on-tax calculation types.
- Supports exempt and reverse-charge rules.
- Returns the aggregate tax percent, tax amount, resolved jurisdictions, and component list.

Address-based jurisdiction matching considers:

- Country
- Region
- Postal-code pattern

The most specific matching jurisdiction is selected.

`CalculateLineTax()` stores the resolved origin and destination jurisdictions on the document and stores the aggregate result on the trade line.

Validation rejects a non-zero taxable line when:

- A tax business group is missing.
- A tax product group is missing.
- Origin or destination jurisdiction cannot be resolved.
- No applicable tax rule is found.

## Taxable Amount And Taxes

The taxable amount of a line is:

`NetAmount - DocumentDiscountAmount`

`CreateTaxResolver()` creates the configured `ITaxResolver`.

`CreateTaxResolveArgs()` provides:

- Document and line identifiers
- Document type and trade type
- Trade date
- Customer
- Tax business group
- Product and tax product group
- Origin and destination jurisdictions
- Origin and destination addresses
- Taxable amount

`CalculateLineTax()` resolves and stores:

- Aggregate tax percent
- Tax exemption state
- Reverse-charge state
- Tax amount
- Line total

Resolved origin and destination jurisdictions replace the document values. An empty tax result does not clear an explicitly selected jurisdiction.

The line total is:

`NetAmount - DocumentDiscountAmount + TaxAmount`

The tax fields are calculated values and are read-only in the UI. Different tax rates are tested by selecting products with different tax product groups.

In the sample data:

- `Coffee Machine` uses the standard tax product group.
- `Espresso Beans` uses the reduced tax product group.

## Tax Snapshots

`ReplaceLineTaxRows()` replaces the generated `TradeLineTax` rows of a trade line.

Each row stores the applied rule, rate, jurisdiction, taxable amount, tax amount, exemption information, reverse-charge information, and clause text.

`CalculateTaxSummary()` rebuilds `TradeTax` by grouping active `TradeLineTax` rows by tax rule.

`DeleteOrphanLineTaxRows()` removes tax component rows whose trade line has been deleted.

The tax tables are calculation snapshots. Reopening a saved document does not depend on reconstructing historical tax results from current setup data.

`TradeTax` and `TradeLineTax` are hidden detail tables and are not displayed as editable grids.

## Document Totals

`CalculateTotals()` calculates:

- `LinesAmount`
- `NetAmount`
- `TaxAmount`
- `TotalAmount`

The formulas are:

- `LinesAmount = Sum of TradeLine.NetAmount`
- `NetAmount = LinesAmount - DiscountAmount + ChargesAmount`
- `TaxAmount = Sum of TradeLine.TaxAmount`
- `TotalAmount = NetAmount + TaxAmount`

## Calculation Sequence

`Calculate(DataRow, string, string)` performs the complete calculation sequence:

- Calculate all active lines.
- Calculate and allocate the document discount.
- Calculate line taxes.
- Rebuild the tax summary.
- Calculate document totals.

The parameterless `Calculate()` uses monetary discount amounts as authoritative values.

`TradeLine_RowDeleted()` recalculates the document after deleting a line.

`ColumnChanged()` recalculates the affected pricing, line, tax, discount, and total values during editing.

## Validation

`TradeDataModule.CheckCanCommit()` performs the final calculation and calls `Validate()` before `TableSet.Commit()`.

`Validate()` collects all validation errors and throws one `TripousBusinessException`.

All sales and purchase documents require a complete billing address snapshot:

- Name
- Address line 1
- City
- Postal code
- Country

Sales and purchase Orders, Delivery Notes, and Returns also require a complete shipping address snapshot with the same fields.

`ValidateLine()` checks:

- Tax business group
- Tax product group
- Origin tax jurisdiction
- Destination tax jurisdiction
- Applicable tax rule

`SalesDataModule.ValidateLine()` additionally checks:

- `UnitPrice` is not zero when `SalesDefaults.AllowZeroUnitPrice` is false.

`TripousBusinessException` is displayed as an expected business message by `DesktopExceptionHandler`, without the generic unexpected-error text or technical details.

## Detail Grid Behavior

The trade-line grid fields are configured by `SalesDefaults.TradeLineGridFields`.

The grid supports:

- Add button
- Delete button
- Keyboard shortcuts
- Automatic selection of a newly added row
- Selection of the next or previous row after deletion
- Recalculation after line deletion

`DataViewItemsSource` synchronizes add, delete, and move operations incrementally. Cell changes do not reload the entire collection, preserving the selected row and editor focus.

## Tested Scenarios

- Standard and reduced VAT lines in the same document.
- Document discount allocation across different tax rates.
- Document discount entered as percent and amount.
- Rounding remainder allocation.
- Deleting the last line.
- Recalculation after deletion.
- Manual unit price when no price-list entry exists.
- Save and reopen without monetary rounding drift.
- Posting with final number assignment and audit fields.
- Reopening a posted document as read-only.

## UI Verification

The Sales and Purchase document flows were manually verified with a newly created sample database:

- Partial Order to Delivery Note transformations.
- Partial Delivery Note to Return transformations.
- Correct remaining quantities after each transformation.
- Correct behavior while the original source form remains open.
- No confirmation dialog when the source has no remaining quantity.
- Supplier selection copies billing and shipping address snapshots.
- Purchase transformations preserve supplier, address, and tax context.
- Destination tax jurisdiction is available in transformed Purchase documents.
- Address and tax validation prevents incomplete documents from being saved.
- Partial Sales Delivery Note to Sales Invoice transformations.
- Partial Purchase Delivery Note to Purchase Invoice transformations.
- A Return between two partial Invoices does not alter the remaining invoice quantity.
- The original Delivery Note form can remain open throughout partial invoicing.
- No confirmation dialog is shown when no invoice quantity remains.
- Sales and Purchase Invoices do not create stock movements.
- Partial Sales Invoice to Sales Credit Note transformations with quantities `4 + 6`.
- Partial Purchase Invoice to Purchase Credit Note transformations with quantities `4 + 6`.
- Credit Note transformations preserve partner, address, tax, and pricing context.
- The original Invoice form can remain open throughout partial crediting.
- No confirmation dialog is shown when no credit quantity remains.
- Sales and Purchase Credit Notes do not create stock movements.
- Sales Invoice to Sales Cancellation posting.
- Purchase Invoice to Purchase Cancellation posting.
- Cancellation transformations preserve partner, address, tax, pricing, line, and total context.
- Cancellation posting changes the source Invoice status to `Cancelled`.
- Cancellation posting releases the source Delivery Note invoiced quantity.
- A new Invoice created after Cancellation receives the restored quantity `10`.
- Sales and Purchase Cancellations do not create stock movements.
- Sales stock was verified as opening `50`, delivery `-10`, return `+3`, final balance `43`.
- Purchase stock was verified as opening `300`, delivery `+10`, return `-3`, final balance `307`.
- Purchase Credit Notes left the `Espresso Beans` stock balance unchanged at `310` after a Purchase Delivery Note of `10`.
- Sales Cancellation left the `Laptop Computer 14` stock balance unchanged at `40` after a Sales Delivery Note of `10`.
- Purchase Cancellation left the `Espresso Beans` stock balance unchanged at `310` after a Purchase Delivery Note of `10`.
- Customer Receipt posting creates partner and bank Finance Movements and a matching accounting Journal Entry.
- Supplier Payment posting creates partner and bank Finance Movements and a matching accounting Journal Entry.
- Customer Receipt Cancellation reverses customer and bank balances and marks the original payment cancelled.
- Supplier Payment Cancellation reverses supplier and bank balances and marks the original payment cancelled.

## Current Limitations

- `ExchangeRate` is entered manually.
- Automatic currency-rate retrieval is not implemented.
- Currency conversion between price-list and document currencies is not implemented.
- A pricing-field change replaces a manual unit price only when an applicable price is found.
- Payment settlement is amount-based for the first cycle and does not yet allocate exchange-rate differences.

## Tester Release TODO

- Add a tester guide with database setup, sample data notes, main workflows, expected outcomes, and known limitations.
- Add a demo checklist for Sales, Purchases, Stock, Accounting, Finance, Payments, and Settlements.
- Add initial database creation on application startup, with a user confirmation question before creating the database.
- Add an easy database regeneration command or UI action for testers.
- Add an application settings dialog for database path and basic application options.
- Verify master modules with a quick List, Edit, Save smoke test.
- Verify reports and views for Finance Movements, Finance Balances, Stock Movements, Stock Balances, and Journal Entries.
- Document that tERP is a sample ERP used to demonstrate Tripous.Avalon capabilities, not a production ERP.
- Document reset and regeneration steps for testers.
- Document known functional limits before tester delivery.

## Sales Order Transformation

The implementation supports partial delivery:

- The open module is treated as an in-memory snapshot rather than the current database authority.
- A temporary source module reloads the header, lookup display values, and lines from the database immediately before transformation.
- The transformed document is calculated and validated before its modal form is opened.
- Only a posted Sales Order can be transformed.
- The new Sales Delivery Note remains unsaved and opens in insert mode.
- `Trade.SourceId` references the Sales Order.
- `TradeLine.SourceTradeLineId` references the source order line.
- Delivery quantity is `Quantity - ExecutedQuantity`.
- Header and line business snapshots are copied.
- IDs, codes, statuses, audit fields, calculated amounts, and tax snapshots are regenerated.
- Multiple Sales Delivery Notes can be created from one Sales Order.
- Posting validates each delivery quantity against the remaining source quantity.
- Posting updates source line `ExecutedQuantity` in the same transaction.
- Posting the final remaining quantities sets the Sales Order status to `Completed`.
- Posting creates immutable outgoing `StockMovement` rows in the same transaction.
- Posting updates `StockBalance` quantity, total cost, average unit cost, and last movement.
- Outgoing movements use the current moving-average unit cost.
- Posting rejects negative stock unless `Warehouse.AllowNegativeStock` is enabled.
- Stock movement, stock balance, delivery posting, and source-order updates share the same transaction.

The same database-snapshot rule applies to:

- Purchase Order to Purchase Delivery Note.
- Sales Delivery Note to Sales Return.
- Purchase Delivery Note to Purchase Return.

Final posting validation repeats quantity, status, cancellation, and stock checks inside the database transaction using locked rows.

## Executed Quantity Semantics

`TradeLine.ExecutedQuantity` represents operational fulfillment or return quantity:

- On Sales Order lines, it is the quantity transformed into posted Sales Delivery Notes.
- On Purchase Order lines, it is the quantity transformed into posted Purchase Delivery Notes.
- On Sales Delivery Note lines, it is the quantity transformed into posted Sales Returns.
- On Purchase Delivery Note lines, it is the quantity transformed into posted Purchase Returns.

Invoicing is an independent process and must not reuse `ExecutedQuantity`. Delivery Note lines require a separate invoiced quantity so returns and invoices can progress independently.

## Invoice Transformation

Posted Sales and Purchase Delivery Notes can be transformed into draft Invoices:

- Sales Delivery Note to Sales Invoice.
- Purchase Delivery Note to Purchase Invoice.
- `Trade.SourceId` references the source Delivery Note.
- `TradeLine.SourceTradeLineId` references the source Delivery Note line.
- Invoice quantity is `Quantity - InvoicedQuantity`.
- Multiple partial Invoices can be created from one Delivery Note.
- Posting validates invoice quantities against the current database values using locked source rows.
- Posting updates source line `InvoicedQuantity` in the same transaction.
- `ExecutedQuantity` remains dedicated to Returns and is not changed by Invoice posting.
- Returns and Invoices can progress independently from the same Delivery Note.
- Partner, addresses, prices, discounts, tax context, and business snapshots are copied.
- The transformed Invoice is calculated and validated before its modal form is opened.
- The Delivery Note form checks the current invoicing remainder before showing confirmation.
- Invoice posting does not create stock movements because stock was already moved by the Delivery Note.
- Invoice posting creates the accounting Journal Entry.
- Invoice posting creates the partner Finance Movement and updates Finance Balance.

## Credit Note Transformation

Posted Sales and Purchase Invoices can be transformed into draft Credit Notes:

- Sales Invoice to Sales Credit Note.
- Purchase Invoice to Purchase Credit Note.
- `Trade.SourceId` references the source Invoice.
- `TradeLine.SourceTradeLineId` references the source Invoice line.
- Credit quantity is `Quantity - CreditedQuantity`.
- Multiple partial Credit Notes can be created from one Invoice.
- Posting validates credit quantities against the current database values using locked source rows.
- Posting updates source line `CreditedQuantity` in the same transaction.
- Partner, addresses, prices, discounts, tax context, and business snapshots are copied.
- The transformed Credit Note is calculated and validated before its modal form is opened.
- The Invoice form checks the current credit remainder before showing confirmation.
- Credit Note posting does not create stock movements. Physical returns remain separate Return documents.
- Sales Credit Notes reverse the financial and accounting direction of Sales Invoices.
- Purchase Credit Notes reverse the financial and accounting direction of Purchase Invoices.
- Credit Note posting creates the reversing accounting Journal Entry.
- Credit Note posting creates the reversing partner Finance Movement and updates Finance Balance.

## Invoice Cancellation

Posted Sales and Purchase Invoices can create full Cancellation documents:

- Sales Invoice to Sales Cancellation.
- Purchase Invoice to Purchase Cancellation.
- `Trade.CancelsTradeId` references the cancelled Invoice.
- The cancelled Invoice stores the Cancellation identifier in `Trade.CancelledByTradeId`.
- The cancelled Invoice receives `TradeStatusId = Cancelled`, `IsCancelled`, `CancelledAt`, and `CancelledBy`.
- Cancellation documents preserve the complete Invoice context, quantities, prices, discounts, taxes, and totals.
- Partial or modified Cancellation documents are rejected during posting.
- Posting validates the Invoice and all source lines again using locked database rows.
- An Invoice can be cancelled only once.
- An Invoice with posted Credit Notes cannot be cancelled.
- Cancellation posting subtracts the Invoice quantities from the source Delivery Note `InvoicedQuantity`.
- Released Delivery Note quantities can be invoiced again.
- Cancellation posting and all source updates share the same database transaction.
- Cancellation documents do not create stock movements because Invoice documents do not move stock.
- Sales and Purchase Cancellations reverse the financial and accounting direction of their Invoices.
- Cancellation posting creates the reversing accounting Journal Entry.
- Cancellation posting creates the reversing partner Finance Movement and updates Finance Balance.

## Accounting Posting

The accounting implementation is intentionally small and fixed for the first cycle.

Manual Journal Entries:

- A Journal Entry must have two or more active lines.
- Each line must contain exactly one debit or credit amount.
- Debit and credit totals must balance.
- Only active posting accounts can be used.
- Posting changes `StatusId` to `Posted`, assigns the final code, stores `PostedAt` and `PostedBy`, and locks the entry.
- Posted and locked Journal Entries cannot be edited.
- The Journal Entry desktop form uses the standard document Post action.

Automatic Journal Entries are created when posting accounting-affecting documents:

- Sales Invoices debit customer receivables and credit sales revenue plus VAT payable.
- Purchase Invoices debit purchases plus VAT receivable and credit supplier payables.
- Sales and Purchase Credit Notes create the reverse accounting direction of their source Invoice.
- Sales and Purchase Cancellations create a full reversing Journal Entry.
- Customer Receipts debit cash or bank and credit customers.
- Supplier Payments debit suppliers and credit cash or bank.
- Customer Receipt and Supplier Payment Cancellations create full reversing Journal Entries.
- A Cancellation Journal Entry links to the original generated Journal Entry.
- The original generated Journal Entry is marked cancelled after the reversing entry succeeds.
- Journal Entry creation is part of the same database transaction as document posting.

The first-cycle fixed accounts are:

- `10-3000` Customers
- `20-1000` Suppliers
- `70-1000` Sales Revenue
- `60-1000` Purchases
- `20-2000` VAT Payable
- `10-5000` VAT Receivable
- `10-1000` Cash
- `10-2000` Bank

## Finance Posting

The first-cycle finance implementation tracks partner, cash, and bank balances.

Automatic Finance Movements are created when posting finance-affecting documents:

- Sales Invoices create positive customer balance movements.
- Purchase Invoices create negative supplier balance movements.
- Sales and Purchase Credit Notes create reversing partner movements.
- Sales and Purchase Cancellations create full reversing partner movements.
- Customer Receipts create a negative customer movement and a positive cash or bank movement.
- Supplier Payments create a positive supplier movement and a negative cash or bank movement.
- Customer Receipt and Supplier Payment Cancellations create reversing partner and cash or bank movements.
- A Cancellation Finance Movement links to the original generated Finance Movement.
- Finance Movement creation and Finance Balance update are part of the same database transaction as document posting.

Finance Balance is a cache maintained from Finance Movement:

- Balances are grouped by `TradeTypeId`, `CurrencyId`, and financial owner.
- Current partner balances use `PersonId`.
- Cash balances use `CashAccountId`.
- Bank balances use `CompanyBankAccountId`.

## Payment Posting

Customer Receipts and Supplier Payments use `PaymentDataModule`.

Payment posting creates:

- One partner Finance Movement.
- One cash or bank Finance Movement.
- Finance Balance updates for both financial owners.
- One automatic accounting Journal Entry.

Customer Receipt posting:

- Reduces the customer Sales balance.
- Increases the selected cash or bank Financial balance.
- Debits Cash or Bank.
- Credits Customers.

Supplier Payment posting:

- Reduces the supplier Purchases balance.
- Decreases the selected cash or bank Financial balance.
- Debits Suppliers.
- Credits Cash or Bank.

Posted invoice forms can create payment documents:

- Sales Invoice provides `Create Customer Receipt`.
- Purchase Invoice provides `Create Supplier Payment`.
- The created payment uses the invoice's current open finance movement amount.
- The created payment copies partner, currency, exchange rate, source code, and an initial settlement row.

Payment settlement rows link payments to open trade Finance Movements:

- A settlement can reference only trade Finance Movements.
- The settlement movement must belong to the same partner, currency, and trade type.
- The movement direction must match the payment document type.
- Cancelled movements cannot be settled.
- The settled amount cannot exceed the open movement amount.
- When settlement rows exist, the payment header amount is adjusted on save to match the settlement total.
- Invalid settlement rows are validated before header amount adjustment, so an over-settlement is blocked instead of being normalized into the header amount.
- If no settlement rows exist, the payment amount remains manually entered and becomes unapplied.
- Cancellation documents cannot have settlement rows.
- Payment cancellation forms disable adding settlement rows in the settlement detail grid.
- `PaymentSettlement.FinanceMovementId` uses a payment-aware locator that filters open trade movements by the payment header partner, currency, trade type, and financial direction.

Payment forms:

- Show an information message when the payment header amount is adjusted to the settlement total.
- Attempt posting for draft editable documents even when pending UI changes exist, so validation errors are shown to the user instead of silently disabling the post command.

## Payment Cancellation

Posted payments are cancelled through separate cancellation documents:

- Customer Receipt to Customer Receipt Cancellation.
- Supplier Payment to Supplier Payment Cancellation.
- `Payment.CancelledPaymentId` references the cancelled payment from the cancellation document.
- The cancelled payment stores the cancellation identifier in `Payment.CancellationPaymentId`.
- The cancelled payment receives `StatusId = Cancelled`, `IsCancelled`, `CancelledAt`, and `CancelledBy`.
- Cancellation forms copy source payment display fields such as partner and cancelled payment code for immediate UI display.

Payment cancellation posting:

- Creates reversing partner and cash or bank Finance Movements.
- Links reversing Finance Movements to the original payment Finance Movements.
- Updates Finance Balance in the same posting transaction.
- Creates a reversing accounting Journal Entry.
- Links the reversing Journal Entry to the original generated Journal Entry.
- Marks the original generated Journal Entry as cancelled.
- Reopens the original trade movement open amount because posted settlements on cancelled payments are ignored.

## Stock Count

The Stock Count document supports initial stock and later inventory adjustments:

- `SystemQuantity` captures the current warehouse balance when a product is selected.
- `CountedQuantity` is the physical quantity entered by the user.
- `DifferenceQuantity` is `CountedQuantity - SystemQuantity`.
- Posting creates a stock movement only when the difference is not zero.
- Positive differences use the unit cost entered on the count line.
- Negative differences use the current moving-average unit cost.
- Posting updates `StockBalance` to the counted quantity.
- Posting is rejected when stock changed after the count was entered.
- A zero difference posts without creating a stock movement.
- Multi-line adjustments are atomic and roll back together when any line fails.
- Stock movement, stock balance, and Stock Count posting share the same transaction.
- The Stock Count desktop form exposes the standard document Post action.

## Stock Transactions

The Stock Transaction document supports the inventory operations required by the tiny ERP:

- Transfer moves stock between two warehouses and creates one outgoing and one incoming movement.
- Receipt increases stock using the unit cost entered on the document line.
- Issue decreases stock using the current moving-average unit cost.
- Transfer uses the source warehouse moving-average cost for both movements.
- Destination warehouse valuation is recalculated using moving-average costing.
- Alternative product units are converted using `ProductUnitOfMeasure.Ratio`.
- Receipt and Issue lines may override the header warehouse.
- Transfer lines use the header source and destination warehouses.
- Negative stock is rejected unless the source warehouse permits it.
- Posting updates `StockMovement` and `StockBalance` in the document transaction.
- Posted documents are locked and cannot be posted or edited again.
- Cancellation creates a complete reversing Stock Transaction.
- Cancellation preserves all source products, quantities, units, and posted costs.
- Modified or partial cancellations are rejected during posting.
- The source document is marked cancelled only after the reversing movements succeed.
- A Stock Transaction can be cancelled only once.
- A cancellation document cannot itself be cancelled.
- The desktop form provides a `Create Stock Cancellation` toolbar action.

## Purchase Stock Posting

- New purchase documents receive configured defaults for warehouse, cost center, branch, price list, currency, payment, and tax fields.
- New purchase lines receive the configured default quantity.
- Purchase pricing, tax resolvers, zero-price validation, and trade-line grid fields use `PurchaseDefaults`.
- A posted Purchase Order can be transformed into a draft Purchase Delivery Note.
- The Purchase Order desktop form provides a `Create Purchase Delivery Note` toolbar button.
- The button asks for confirmation, creates the draft document, and opens it in a modal form.
- Transformation copies only each line's remaining quantity.
- Posting a transformed Purchase Delivery Note updates source line `ExecutedQuantity`.
- The Purchase Order remains posted while quantities remain and becomes completed after full receipt.
- Receipt quantities cannot exceed the remaining source quantities.
- Posting changes the document status to posted, assigns the final code, records posting metadata, and locks the document.
- Purchase Delivery Note posting creates incoming `StockMovement` rows.
- Stock quantity is converted to primary units using `Quantity × UnitRatio`.
- Incoming unit cost is the net line cost after line and document discounts, divided by primary quantity.
- Movement cost remains equal to the net line cost and is not recalculated from the rounded unit cost.
- Incoming posting increases `StockBalance` quantity and total cost.
- `AverageUnitCost` is recalculated using moving-average costing.
- Purchase Return posting creates outgoing movements using the current moving-average unit cost.
- A posted Purchase Delivery Note can create a draft Purchase Return for its remaining returnable quantities.
- The Purchase Delivery Note desktop form provides a `Create Purchase Return` toolbar button.
- The button checks the current database quantities before asking for confirmation.
- When no quantity remains, it displays the business message without showing the confirmation dialog.
- Otherwise, it asks for confirmation, creates the draft return, and opens it in a modal form.
- Posting a transformed Purchase Return increases `ExecutedQuantity` on each source delivery line.
- A return quantity cannot exceed the remaining received quantity.
- The source Purchase Delivery Note remains posted after partial or complete returns.
- Purchase Return posting rejects negative stock unless the warehouse allows it.
- When outgoing posting reduces stock to zero, total cost and average unit cost are set to zero.
- Stock movement, stock balance, and purchase document posting share the same transaction.
- A failure on any line rolls back all stock movements, balance updates, and document posting changes.
- A posted purchase stock document cannot be posted again, preventing duplicate stock movements.

## Sales Return Stock Posting

- Sales Return posting creates incoming stock movements.
- Returned stock uses the warehouse's current moving-average unit cost.
- Posting increases stock quantity and total cost while preserving the moving-average cost.
- A posted Sales Delivery Note can create a draft Sales Return for its remaining quantities.
- The Sales Delivery Note desktop form provides a `Create Sales Return` toolbar button.
- The button checks the current database quantities before asking for confirmation.
- When no quantity remains, it displays the business message without showing the confirmation dialog.
- Otherwise, it asks for confirmation, creates the draft return, and opens it in a modal form.
- Sales Return lines reference their source Sales Delivery Note lines through `SourceTradeLineId`.
- Posting a transformed Sales Return increases `ExecutedQuantity` on each source delivery line.
- A return quantity cannot exceed the remaining delivered quantity.
- The source Sales Delivery Note remains posted after partial or complete returns.
- Stock movements, balance updates, and document posting share the same transaction.
- A failure on any line rolls back all movements, balances, and posting changes.
