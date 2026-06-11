# tERP To-Do

## Sales Order

- Define when automatic repricing occurs.
- Decide whether a manually edited `UnitPrice` remains fixed.
- Regenerate the version 2 schema and registry files after adding `TradeLine.DocumentDiscountAmount`.
- Complete save, reopen, edit, delete, and recalculation tests.
- Implement posting after pricing, discounts, validation, and reopen tests are complete.

## Current Limitations

- `ExchangeRate` is entered manually.
- Price lists and sales documents should currently use the same currency.
- Currency conversion is not performed automatically.

## Future Extensions

- Add a currency-rate table with dated exchange rates.
- Add a service for importing and updating exchange rates from an external provider.
- Resolve the applicable exchange rate by currency and document date.
- Preserve the resolved rate in `Trade.ExchangeRate` as a document snapshot.
