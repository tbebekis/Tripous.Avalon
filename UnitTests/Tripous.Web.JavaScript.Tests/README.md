# Tripous Web JavaScript Tests

This folder contains Vitest tests for the generated Tripous Web browser bundles.

- Source JavaScript remains under `WebApps/WebApp/wwwroot/js-src`.
- Tests load the generated bundles from `WebApps/WebApp/wwwroot/tp/js`.
- Run the WebApp build or bundler before running these tests, so the generated bundles are current.
- `node_modules/`, `coverage/`, and `.vitest/` are ignored.
- `package.json`, `package-lock.json`, test files, and helper files are tracked.

Commands:

```bash
cd UnitTests/Tripous.Web.JavaScript.Tests
npm install
npm run test
```
