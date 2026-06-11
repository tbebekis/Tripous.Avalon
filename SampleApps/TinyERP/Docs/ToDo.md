# tERP To-Do

## Next Work

- Test the full Sales Order to Sales Delivery Note transformation.
- Define partial delivery behavior and remaining quantities.
- Create stock movements when posting a Sales Delivery Note.
- Apply `Warehouse.AllowNegativeStock` during stock posting.
- Preserve source document and source line references.
- Add cancellation and reversal workflows.

## Current Limitations

- `ExchangeRate` is entered manually.
- Price lists and sales documents should currently use the same currency.
- Currency conversion is not performed automatically.
- Purchase-specific defaults, pricing, validation, and posting are not implemented.
- Document transformation, cancellation, and reversal are not implemented.

## Future Extensions

- Add a currency-rate table with dated exchange rates.
- Add a service for importing and updating exchange rates from an external provider.
- Resolve the applicable exchange rate by currency and document date.
- Preserve the resolved rate in `Trade.ExchangeRate` as a document snapshot.
- Add stock availability and reservation services.
- Add financial and accounting posting handlers.
