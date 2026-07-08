# Tripous.Avalon Roadmap

## Current Status

- Tripous.Avalon v1.0.0 has been released.
- The first public release cycle is closed.
- The solution has been upgraded from Avalonia 11.3.12 to Avalonia 12.0.4.
- AvaloniaEdit has been upgraded from 11.4.1 to 12.0.0.
- All sample applications and tERP have been smoke-tested after the Avalonia upgrade.
- The `GroupGrid` completion checkpoint was reached successfully and checked repeatedly.
- `GroupGrid` lives in the separate `Tripous.Avalonia.Controls` project.
- `GroupGrid` is now the Tripous data-aware grid foundation.
- The old Avalonia `DataGrid` was removed completely from the Tripous desktop path.
- The `Locator2` transition is complete; the new locator architecture is now the normal `Locator` architecture.
- The old Desktop Locator was replaced by the new Locator runtime, UI, and `LocatorBox`.

## Documentation

- Continue XML documentation cleanup in small batches.
- Later apply XML documentation cleanup to tERP types.
- Add tester guide and release documentation for tERP.
- Keep DocFX conceptual documentation aligned with framework changes.
- Consider RegBuilder `ListFields:` metadata for controlling generated list SELECT columns.
- If `ListFields:` is added, require `Id`, allow top fields, enum display columns and join aliases, and require `FilterFields` to be a subset of final list fields.

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
- Continue polishing the new `LocatorBox` where real usage reveals details.
- Continue polishing `GroupGrid` where real usage reveals details.
- Add `PivotGrid`.

## Completed Milestones

- Complete the `GroupGrid`.
- Status: Completed.
- Includes in-place editors, editor host architecture, navigation, selection, grouping, summaries, filtering, sorting, virtualization where needed, documentation, demos, and sample applications.
- Replace the Desktop Locator.
- Status: Completed.
- Replaced the old Desktop Locator with the new Locator runtime, UI, definition, request, context, result architecture, and `LocatorBox`.
- Replace Avalonia `DataGrid`.
- Status: Completed.
- Migrated forms, lookup dialogs, document editors, tests, and performance tuning to `GroupGrid`.

## Planned Milestones

- Implement the Web Locator.
- Status: Planned.
- Implement the Locator architecture on the Web platform.
- Includes lookup dialogs, incremental search, popup selector, callbacks, and integration with the new Web runtime.
- The goal is a common Desktop and Web Locator architecture.
- Complete `Tripous.Web` and `tERP.Web`.
- Status: Planned.
- Includes desktop-like shell, pages, dialogs, toolbars, data-aware controls, ajax runtime, document editing, transactions, reporting, and full `tERP.Web` functionality.
- Release the next Tripous.Avalon version.
- Status: Planned.
- Includes stabilization, documentation, demos, samples, release notes, and GitHub Release.

## tERP

- Collect tester feedback from zipped Linux and Windows executables.
- Add tester guide and release notes.
- Continue remaining supporting modules and smoke tests.

## Extensibility

- Design and implement a plugin system.
