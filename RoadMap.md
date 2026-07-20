# Tripous.Avalon Roadmap

## Current Status

- Tripous.Avalon v2.0.0 has been released.
- The second public release cycle is closed.
- The solution has been upgraded from Avalonia 11.3.12 to Avalonia 12.0.4.
- AvaloniaEdit has been upgraded from 11.4.1 to 12.0.0.
- All sample applications and tERP have been smoke-tested after the Avalonia upgrade.
- The `GroupGrid` completion checkpoint was reached successfully and checked repeatedly.
- `GroupGrid` lives in the separate `Tripous.Avalonia.Controls` project.
- `GroupGrid` is now the Tripous data-aware grid foundation.
- `PivotGrid` v1 was added to `Tripous.Avalonia.Controls` as a framework-neutral custom-rendered pivot grid.
- The old Avalonia `DataGrid` was removed completely from the Tripous desktop path.
- The `Locator2` transition is complete; the new locator architecture is now the normal `Locator` architecture.
- The old Desktop Locator was replaced by the new Locator runtime, UI, and `LocatorBox`.
- TinyERP now has shared `SYS_STR_RES` localization infrastructure for desktop and web.
- TinyERP includes desktop and web administrator forms for editing system string resources.
- tERPWeb now includes command tree views, database explorer, interactive SQL, WebDesk data forms and resource translation editing.

## Documentation

- Continue XML documentation cleanup in small batches.
- Later apply XML documentation cleanup to tERP types.
- Add tester guide and release documentation for tERP.
- Keep DocFX conceptual documentation aligned with framework changes.
- Add DocFX pages for `SysStrRes`, `SYS_LANG`, `SYS_STR_RES` and TinyERP multilingual administration.
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
- Continue polishing `PivotGrid` where real usage reveals details.

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
- Add `PivotGrid`.
- Status: Completed.
- Added a framework-neutral custom-rendered pivot grid with POCO/DataTable/DataView adapters, row and column axes, measures, aggregates, filtering, sorting, drag/drop field layout, totals, settings, export, demos, and unit tests.

## Planned Milestones

- Implement the Web Locator.
- Status: Planned.
- Implement the Locator architecture on the Web platform.
- Includes lookup dialogs, incremental search, popup selector, callbacks, and integration with the new Web runtime.
- The goal is a common Desktop and Web Locator architecture.
- Stabilize `Tripous.Web` and `tERP.Web` runtime parity.
- Status: Planned.
- Includes desktop and web parity checks, runtime hardening, workflow polish, document editing, transactions, and reporting follow-up.
- Prepare the next Tripous.Avalon maintenance release.
- Status: Planned.
- Includes stabilization, documentation, demos, samples, release notes, and GitHub Release.

## tERP

- Collect tester feedback from zipped Linux and Windows executables.
- Add tester guide and release notes.
- Continue remaining supporting modules and smoke tests.
- Continue web UI parity checks after each desktop feature lands.

## Extensibility

- Design and implement a plugin system.
