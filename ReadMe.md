# Tripous.Avalon

Tripous.Avalon is a cross-platform .NET application framework for building structured desktop business applications.

It combines the power of Avalonia UI with a metadata-driven architecture focused on data management, business modules, automatic user interface generation, and rapid application development.

The project is the latest evolution of ideas and technologies that have been developed and refined over many years through several generations of the Tripous framework.

## What Is Tripous?

Tripous is not a UI toolkit.

Tripous is an application framework.

Its goal is to provide the infrastructure required by data-centric desktop applications so developers can focus on business logic instead of repeatedly implementing the same application plumbing.

The framework provides:

* Metadata-based application definition
* Declarative database schema registration
* Data modules and business modules
* Automatic form generation
* Lookup and locator systems
* SQL-oriented data access
* Master-detail data management
* Business document workflows
* Cross-platform desktop deployment

The framework is designed primarily for applications such as:

* ERP systems
* CRM systems
* Inventory management
* Accounting applications
* Internal business tools
* Administrative systems
* Data-entry intensive applications

## Why Avalonia?

Avalonia UI provides a modern cross-platform desktop foundation for .NET applications.

Tripous.Avalon uses Avalonia as its presentation layer while providing a much higher-level application architecture above it.

In simple terms:

Avalonia provides windows, controls, layouts, styling and rendering.

Tripous provides application structure, metadata, business modules, data modules, automatic forms, lookups, locators, database integration and workflow infrastructure.

The relationship is similar to how traditional enterprise frameworks were built on top of desktop UI frameworks in the past.

## Design Philosophy

The framework follows several core principles:

* Simplicity over complexity
* Explicit behavior over hidden magic
* SQL as a first-class citizen
* Metadata over repetitive code
* Reusability through descriptors and registries
* Productivity without sacrificing control
* Long-term maintainability

Tripous intentionally avoids excessive abstraction and favors deterministic behavior that can be understood, debugged and extended.

## Architecture Overview

The framework is organized into several major layers.

### Tripous

Core utilities, descriptors, registries, infrastructure services and common functionality.

### Tripous.Data

Database access, schema registration, SQL generation, data modules, lookups, locators and data-related services.

### Tripous.Logging

Logging infrastructure and diagnostics.

### Tripous.Desktop

Avalonia-based desktop framework including forms, controls, application shell, menus, toolbars, navigation and automatic UI generation.

## Metadata-Driven Development

One of the central ideas behind Tripous is that applications should be described declaratively whenever possible.

Database tables, modules, forms, lookups, locators and many other application elements are registered through metadata descriptors.

This allows large parts of an application to be generated automatically while remaining fully customizable.

The result is a development model that combines the productivity of RAD tools with the flexibility of modern .NET development.

## Documentation

Project documentation is available through the DocFX documentation site.

The documentation includes:

* Conceptual documentation
* Architecture guides
* Framework reference
* API documentation
* Tutorials and examples

## Sample Applications

The repository contains several sample applications demonstrating different aspects of the framework, from simple desktop applications to larger business-oriented systems.

The TinyERP sample application demonstrates how a complete business application can be built using the framework.

## Project Status

Tripous.Avalon is an active long-term project.

The framework is currently focused on:

* Framework stabilization
* Documentation
* Sample applications
* Automated testing
* Public releases

## License

Tripous.Avalon is licensed under the Tripous.Avalon Community License v1.0.

See LICENSE.txt for details.
