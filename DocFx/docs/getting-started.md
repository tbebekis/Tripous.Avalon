# Getting Started

This section explains how to build and run applications based on the Tripous Framework.

## Requirements

- .NET 10 SDK
- Git
- A supported database system
- An IDE such as Rider or Visual Studio

## Clone the Repository

Clone the repository and move into the solution folder.

```bash
git clone https://github.com/tbebekis/Tripous.Avalon
cd Tripous.Avalon
```

## Build the Solution

Restore packages and build the solution.

```bash
dotnet restore Tripous.Avalon.sln
dotnet build Tripous.Avalon.sln
```

For a faster first check, build only the core projects you are working with instead of the full solution.

## Explore TinyERP

The recommended way to become familiar with the framework is to study and run the TinyERP sample application.

TinyERP is a sample application that demonstrates the Tripous declarative model and application structure. It is not intended to be copied as an application template without adaptation.

TinyERP demonstrates:

- Application registration
- Modules
- Forms
- Data modules
- Descriptors
- Database access
- Lookups and locators

## Next Steps

After building and running TinyERP, continue with:

- [What Is Tripous?](tripous-framework/what-is-tripous.md)
- [Design Philosophy](tripous-framework/design-philosophy.md)
- [Framework Layers](tripous-framework/framework-layers.md)
- [Tripous.Data](tripous-data/overview.md)
- [Manual Application Declaration](manual-declaration/overview.md)
