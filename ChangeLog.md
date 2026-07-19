# Changelog

All notable changes to this project will be documented in this file.

## 2026-07-19

### TinyERP
- Renamed the `Language` table to `SYS_LANG` and linked `SYS_STR_RES` to it through `LanguageId`
- Added `SysStrRes` as the shared system string-resource cache and localizer bridge
- Added automatic insertion of missing English string-resource keys, controlled by system configuration
- Added desktop and web Resource Translations admin forms for editing `SYS_STR_RES`
- Added startup string-resource loading for tERPWeb and language-aware client localization
- Added English and Greek sample string resources and removed the third sample language
- Made application user culture selection use supported languages
- Updated login handling to persist `LastLoginAt`
- Updated desktop and web FactBox buttons so the pane starts hidden and is opened explicitly
- Reworked the desktop startup window so startup dialogs have a visible full-screen owner

### Tripous.Web
- Added WebDesk support used by TinyERP for runtime string-resource packets
- Added a web Resource Translations editor with per-language columns, inline save, delete confirmation, filtering and sorting
- Added main toolbar and menu command integration for the web Resource Translations form

### Tripous.Desktop
- Added a desktop Resource Translations editor backed by the shared resource translation service
- Improved `GroupGrid` header sorting behavior
- Updated data form FactBox command visibility and initial state

## 2026-06-22

### Framework
- Upgraded from Avalonia 11.3.12 to Avalonia 12.0.4
- Upgraded AvaloniaEdit from 11.4.1 to 12.0.0
- Updated clipboard API usage for Avalonia 12
- Replaced obsolete Watermark properties with PlaceholderText
- Replaced obsolete SystemDecorations with WindowDecorations

### Validation
- Full solution builds without warnings or errors
- All sample applications tested successfully
- tERP tested successfully
