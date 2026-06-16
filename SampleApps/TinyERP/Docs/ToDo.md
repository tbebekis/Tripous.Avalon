# tERP To-Do

## Next Work

- Prepare tester guide and release documentation.
- Add database setup, reset, and regeneration instructions for testers.
- Add a demo checklist for Sales, Purchases, Stock, Accounting, Finance, Payments, and Settlements.
- Verify master modules with a quick List, Edit, Save smoke test.
- Verify reports and views for Finance Movements, Finance Balances, Stock Movements, Stock Balances, and Journal Entries.
- Continue remaining supporting modules after tester documentation.

## Current Limitations

- `ExchangeRate` is entered manually.
- Price lists and sales documents should currently use the same currency.
- Currency conversion is not performed automatically.
- Payment settlement is amount-based for the first cycle and does not yet allocate exchange-rate differences.
- The first accounting cycle uses fixed posting accounts.

## Future Extensions

- Add a currency-rate table with dated exchange rates.
- Add a service for importing and updating exchange rates from an external provider.
- Resolve the applicable exchange rate by currency and document date.
- Preserve the resolved rate in `Trade.ExchangeRate` as a document snapshot.
- Add stock availability and reservation services.
- Add configurable accounting posting profiles.
