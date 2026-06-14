# tERP To-Do

## Next Work

- Complete manual UI verification for generated Journal Entries, Finance Movements, and Finance Balances.
- Design payment documents for customer receipts and supplier payments.
- Add payments and settlements.
- Continue remaining stock modules after finance, accounting, and payment basics.

## Current Limitations

- `ExchangeRate` is entered manually.
- Price lists and sales documents should currently use the same currency.
- Currency conversion is not performed automatically.
- Payments and settlements are not implemented.
- Cash and bank finance movements are reserved for payment documents.
- The first accounting cycle uses fixed posting accounts.

## Future Extensions

- Add a currency-rate table with dated exchange rates.
- Add a service for importing and updating exchange rates from an external provider.
- Resolve the applicable exchange rate by currency and document date.
- Preserve the resolved rate in `Trade.ExchangeRate` as a document snapshot.
- Add stock availability and reservation services.
- Add configurable accounting posting profiles.
