# Documents

## Purpose

The document data modules provide the business logic for transactional documents in tERP.

The current implementation focuses on commercial documents and especially the Sales Order. The same hierarchy is intended to support sales, purchases, posting, cancellation, document transformation, stock operations, and accounting integration.

## Data Module Hierarchy

```text
DataModule (Tripous.Data)
    AppDataModule (tERP.Data)
        DocumentDataModule
            TradeDataModule
                SalesDataModule
                    SalesOrderDataModule
                    SalesDeliveryNoteDataModule
                    SalesInvoiceDataModule
                    SalesCreditNoteDataModule
                    SalesReturnDataModule
                    SalesCancellationDataModule
                PurchaseDataModule
                    PurchaseOrderDataModule
                    PurchaseDeliveryNoteDataModule
                    PurchaseInvoiceDataModule
                    PurchaseCreditNoteDataModule
                    PurchaseReturnDataModule
                    PurchaseCancellationDataModule
```

`DocumentDataModule` provides the common document infrastructure.

`TradeDataModule` adds commercial calculations, pricing, taxes, document discounts, totals, and validation.

`SalesDataModule` adds sales defaults, customer address snapshots, sales pricing validation, and sales line defaults.

The concrete sales and purchase data modules currently act as document-type entry points. Document-specific behavior can be added by overriding the virtual methods of their base classes.

## Document Types And Number Series

`DocumentDataModule.Initialize()` creates the document context from the registered module.

`AssignCodeProviderDef()` resolves:

- `DocumentType`
- `DraftCodeProviderDef`
- `FinalCodeProviderDef`

`GetCodeProviderDef()` returns the draft provider during normal editing and the final provider while `IsPosting` is true.

`GetFinalCodeProviderDefFromDocumentType()` resolves the final number series configured for the current document type.

`AssignCodeValue()` replaces the draft code with the next final code while posting. The final number is assigned inside the same database transaction that saves the document.

## Document Handlers

`DocumentDataModule.Initialize()` creates the handler registered for the current module.

`CreateDocumentContext()` creates the `DocumentContext` passed to the handler.

The handler hierarchy for trade documents is:

```text
DocumentHandler
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

## Posting

`DocumentDataModule.Post()`:

- Creates the document context.
- Enables `IsPosting`.
- Calls `DocumentHandler.Validate()`.
- Calls `DocumentHandler.Post()`.
- Calls `Commit()`.
- Restores the previous posting values if posting fails.
- Disables `IsPosting` in a `finally` block.

`TradeDocumentHandler.Validate()` permits posting only for an unlocked, non-cancelled draft document.

`TradeDocumentHandler.Post()` assigns:

- `TradeStatusId = Posted`
- `PostingDate`
- `PostedAt`
- `PostedBy`
- `IsLocked = true`

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

## Detail Line Order

`DocumentDataModule.NewRowAdded()` assigns `DisplayOrder` to new detail rows.

The value starts at `10` and increases by `10`.

`DocumentDataModule.Initialize()` sorts detail views by:

- `DisplayOrder`
- `Id`

## Sales Line Defaults

`SalesDataModule.NewRowAdded()` assigns `SalesDefaults.DefaultQuantity` to new trade lines.

`TradeDataModule.NewRowAdded()` copies the document warehouse to detail rows containing a `WarehouseId` field.

## Customer Snapshots

`SalesDataModule.ColumnChanged()` calls `CopyPersonAddresses()` when `PersonId` changes.

`CopyPersonAddresses()` loads the customer addresses and copies billing and shipping values to the `Trade` row.

These fields are document snapshots. Later changes to the customer addresses do not alter an existing document.

Changing the customer also reloads the tax business group, resolves prices again, and recalculates the document.

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
- Missing-price validation.
- Save and reopen without monetary rounding drift.

## Current Limitations

- `ExchangeRate` is entered manually.
- Automatic currency-rate retrieval is not implemented.
- Currency conversion between price-list and document currencies is not implemented.
- Related pricing-field changes replace a manually entered unit price.
- Purchase-specific defaults, pricing, and validation are not implemented.
- Posting, cancellation, and document transformation workflows are not implemented.

## Planned Posting Workflow

The intended posting workflow will use `DocumentDataModule.IsPosting` and the final number-series provider.

Posting is expected to:

- Validate the document.
- Perform the final calculation.
- Assign the final document code.
- Set posting date, user, status, and audit values.
- Commit the document.
- Create the required stock, financial, or accounting records.
- Prevent later editing where appropriate.

The exact posting responsibilities will be documented here when implemented.
