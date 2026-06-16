---
_layout: landing
---

# Tripous Framework

Tripous is a .NET framework for building data-centric applications, business systems, desktop applications and services.

The project traces its roots back to the mid-1990s, during the Delphi era. Over the years it evolved through several generations of applications, frameworks and development platforms, eventually moving from Delphi to the .NET Framework and later to modern .NET.

Many of the classes, concepts and architectural ideas found in Tripous today have been refined through decades of practical use in commercial software projects. The framework was not designed as an academic exercise or as an implementation of a particular architectural trend. Instead, it grew organically from the requirements of real-world applications and the lessons learned while building them.

The goal of Tripous is simple: provide a solid foundation for building maintainable data-centric applications while keeping developers in full control of their code and application behavior.

## Framework Libraries

### Tripous

The core library of the framework.

Contains utility classes, collections, helper functions, configuration infrastructure, type services, reflection helpers and general-purpose framework facilities used throughout the system.

### Tripous.Data

Provides database access, metadata definitions, lookup infrastructure, data management services and the foundation for building data-centric applications.

### Tripous.Logging

Provides logging infrastructure and related services.

### Tripous.Desktop

Provides the desktop application framework and user interface infrastructure. The current implementation is based on Avalonia UI.

## Design Principles

Tripous is built around a small number of guiding principles:

- Simplicity over complexity
- Explicit behavior over hidden magic
- Declarative configuration where appropriate
- Strong support for data-entry applications
- Reusable business application infrastructure
- Long-term maintainability
- Full developer control

## Documentation

The documentation is divided into two major sections.

### Conceptual Documentation

Explains the architecture, design principles and major framework components.

### API Reference

Reference documentation generated directly from source code and XML comments.

## Sample Applications

### TinyERP

TinyERP is a small ERP application used as a reference implementation, development environment and testing platform for the framework.

## Current Status

Tripous continues to evolve while preserving ideas and concepts that have proven their value through many years of practical use.

The current desktop implementation is based on Avalonia UI through the Tripous.Desktop library, while the core libraries remain independent of any specific user interface technology.