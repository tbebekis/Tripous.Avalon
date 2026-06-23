# Tripous Web Bundler

Bundles Tripous Web source fragments into runtime JavaScript or CSS files.

- Source files are grouped by immediate subfolder under the source root.
- Each subfolder produces a bundle with the same name, e.g. `tp/` produces `tp.js`.
- Files inside each group are sorted by the numeric fragment order at the start of the file name, then by file name.
- `tp/00-core.js` produces `tp.js`.
- `tp-UI/00-core.js` produces `tp-UI.js`.
- `tp-Data/00-core.js` produces `tp-Data.js`.

Example:

```bash
dotnet run --project Tools/TripousWebBundler -- \
  --source WebApps/WebApp/wwwroot/js-src \
  --output WebApps/WebApp/wwwroot/tp/js \
  --extension js
```

Dry run:

```bash
dotnet run --project Tools/TripousWebBundler -- \
  --source WebApps/WebApp/wwwroot/js-src \
  --output WebApps/WebApp/wwwroot/tp/js \
  --extension js \
  --dry-run
```
