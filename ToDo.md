# Tripous.Avalon To-Do And Roadmap

## Documentation

- Continue XML documentation cleanup in small batches.
- Add conceptual documentation for Tripous.Avalon architecture and main subsystems.
- Document data-module rules and UI-layer boundaries.
- Document `Tripous.Desktop` form, grid, locator, and toolbar infrastructure.
- Consider RegBuilder `ListFields:` metadata for controlling generated list SELECT columns.
- If `ListFields:` is added, require `Id`, allow top fields, enum display columns and join aliases, and require `FilterFields` to be a subset of final list fields.
- Later apply XML documentation cleanup to tERP types.

## User Interface Preferences

- Add dynamic user-selected list filters.
- Display available List SELECT fields through an add-filter command.
- Store filter field names, data types, operators, and values per user and form.
- Allow users to show, hide, resize, and reorder list grid columns.
- Store list grid layouts per user, form, and List SELECT.
- Use versioned JSON preference models stored in `SYS_INI`.
- Ignore or remove preferences for fields that no longer exist.

## Controls And Components

- Review whether a dedicated `LookUpBox` is still needed after the current lookup combo box and locator work.
- Continue polishing `LocatorBox`.
- Add `GroupGrid`.
- Add `PivotGrid`.

## Extensibility

- Design and implement a plugin system.
