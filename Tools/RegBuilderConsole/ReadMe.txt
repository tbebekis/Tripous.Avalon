# RegBuilderConsole

Builds the configured projects, loads assemblies containing discoverable types such as enums, runs RegBuilder projects, and copies the generated C# files to their configured output folders.

## Usage

dotnet run --project Tools/RegBuilderConsole -- [options]

## Options

- --project Name
  Executes only the specified RegBuilder project.
  When omitted, all configured RegBuilder projects are executed.

- --configuration Debug|Release
  Defines the build configuration and replaces the {Configuration} token in configured paths.
  The default value is Debug.

- --no-build
  Skips the configured project builds.
  Use this only when the required assemblies are already current.

- --help
  Displays command-line usage and exits.

## Examples

- Execute all projects using Debug:

  dotnet run --project Tools/RegBuilderConsole

- Execute only schema version 2:

  dotnet run --project Tools/RegBuilderConsole -- --project tERP.Version2

- Execute all projects using Release:

  dotnet run --project Tools/RegBuilderConsole -- --configuration Release

- Execute version 2 without building configured projects:

  dotnet run --project Tools/RegBuilderConsole -- --project tERP.Version2 --no-build

## Behavior

- Loads AppSettings.json from the application output folder.
- Builds every configured BuildProjectFilePaths entry unless --no-build is specified.
- Loads and registers every configured AssemblyFilePaths entry.
- Executes all configured projects or the project selected with --project.
- Generates files in a temporary folder.
- Copies only the generated C# files to each configured OutputFolderPath.
- Replaces existing generated files with the new output.
- Does not copy Schema.sql to the destination Registry folder.
- Returns exit code 0 on success.
- Returns exit code 1 on build, configuration, loading, or RegBuilder errors.
