---
_layout: landing
---

# Tripous Framework Documentation

Tripous is a .NET framework for building data-centric applications, business systems, desktop applications and service-ready data layers.

The framework combines explicit C# registration, database metadata, SQL-first data access, application descriptors, reusable data modules and an Avalonia-based desktop layer.

## Start Here

- [Introduction](docs/introduction.md)
- [Getting Started](docs/getting-started.md)
- [Overview](docs/tripous-framework/what-is-tripous.md)
- [Sample Applications](docs/sample-applications.md)
- [Screenshots](docs/screenshots.md)

## Application Declaration

Tripous applications are built from descriptors.

Descriptors define modules, tables, fields, forms, lookups, locators, select definitions, code providers and configuration properties.

- [Manual Application Declaration](docs/manual-declaration/overview.md)
- [Automatic Application Declaration](docs/automatic-declaration/overview.md)

Manual declaration and generated declaration use the same runtime model.

The [Registration Builder](docs/automatic-declaration/reg-builder.md) generates the same declarations a developer can write by hand.

Tripous supports six RDBMS engines: SQL Server, SQLite, PostgreSQL, MySQL, Firebird and Oracle.

## Sample Applications

The repository includes progressively larger samples.

- `01-hello-tripous` shows the smallest desktop shell.
- `02-notes` adds SQLite, one table, one module and one form.
- `03-todo` adds lookups, filters and configuration.
- `04-mini-crm` demonstrates manual master/detail registration, locators and code providers.
- `05-password-manager` demonstrates services, encrypted fields and vault locking.
- `TinyERP` is the larger automatic declaration sample built with the Registration Builder.

## Framework Libraries

- [Tripous Core](docs/tripous-core/overview.md)
- [Tripous.Data](docs/tripous-data/overview.md)
- [Tripous.Desktop](docs/tripous-desktop/overview.md)
- [API Reference](api/)

## Core Ideas

- SQL-first database access.
- Six supported RDBMS engines: SQL Server, SQLite, PostgreSQL, MySQL, Firebird and Oracle.
- Explicit descriptors instead of hidden conventions.
- Declarative metadata where it helps.
- Manual and generated registration share the same model.
- Data modules contain business/data behavior.
- UI feedback belongs in the UI layer.
- Core and data libraries can be used without a desktop UI.
- The desktop layer currently uses Avalonia UI.

## Current Status

Tripous is evolving documentation and samples around the same foundation used by the framework libraries.

The current focus is conceptual documentation, sample applications and clearer guidance for manual and automatic registration.
