# What Is Tripous?

Tripous is a framework for building data-centric applications, business systems, desktop applications and services.

At first glance, it may appear to be just another collection of .NET libraries. In reality, it is the result of a long evolution that spans multiple generations of development tools, programming languages and software architectures.

The origins of Tripous can be traced back to the Delphi era of the mid-1990s. Over the years, the ideas, patterns and components that eventually became Tripous were used, tested, revised and improved through many real-world projects. Some concepts survived multiple technology transitions, from Delphi to the .NET Framework and eventually to modern .NET.

Because of this history, Tripous was not designed around a particular trend, methodology or architectural fashion. It was shaped by practical experience and by the requirements of real applications.

The primary goal of the framework is to simplify the development of data-centric systems while preserving full developer control over application behavior.

## Data-Centric by Design

Tripous was created primarily for applications that work with data.

Examples include:

- Business applications
- ERP systems
- CRM systems
- Inventory systems
- Financial applications
- Administrative systems
- Service applications
- Internal enterprise tools

Such applications typically spend most of their time performing operations related to data:

- Retrieving information
- Editing information
- Validating information
- Displaying information
- Searching information
- Reporting information

Tripous focuses on making these activities simple, consistent and maintainable.

## A Different Approach

Many modern frameworks attempt to hide complexity behind layers of abstraction.

Tripous follows a different philosophy.

The framework tries to provide useful abstractions without hiding what is actually happening.

Developers are encouraged to understand their applications, their databases and their business rules rather than relying on invisible framework behavior.

This philosophy can be summarized as:

- Explicit behavior over hidden magic
- Simplicity over complexity
- Practicality over fashion
- Understanding over automation

## Declarative Application Development

One of the central ideas behind Tripous is declarative application development.

Instead of manually constructing every aspect of an application through procedural code, developers describe the structure of the application using definitions and metadata.

These definitions describe concepts such as:

- Modules
- Tables
- Fields
- Forms
- Lookups
- Locators
- Commands

The framework then uses these declarations to construct much of the application's runtime behavior.

This approach reduces repetitive code while maintaining clarity and developer control.

## Framework Layers

The framework is organized into several major layers.

### Tripous

The core library.

Contains utility classes, collections, helper functions, configuration infrastructure, reflection services and other foundational components.

### Tripous.Data

Provides data access infrastructure, metadata definitions, lookup services, schema management, data modules and related functionality.

### Tripous.Logging

Provides logging infrastructure.

### Tripous.Desktop

Provides the desktop application layer.

The current implementation is based on Avalonia UI.

## SQL Instead of ORM

One design decision often surprises developers.

Tripous does not use an Object Relational Mapping (ORM) framework.

Instead, it embraces SQL as the primary language for database access.

This decision was not made because ORM technology was unknown or unavailable.

On the contrary, the ideas behind modern ORM frameworks were already being explored and discussed many years before they became mainstream.

After many years of experimentation and practical experience, the conclusion was simple:

SQL remains one of the clearest, most expressive and most efficient ways to communicate with relational databases.

Tripous therefore combines SQL with metadata, descriptors and application definitions rather than attempting to hide relational databases behind object models.

## A Framework Built Through Time

Most frameworks are products of a particular moment in time.

Tripous is different.

It carries ideas that have survived several decades of software development, multiple programming languages and multiple platform transitions.

Some technologies came and went.

Some architectural trends appeared and disappeared.

The underlying problems, however, remained the same:

How do we build software that is understandable, maintainable and useful?

Tripous represents one answer to that question.

It is not the only answer.

It is simply the answer that emerged from many years of practical software development experience.

## Related Articles

- [History and Evolution](history-and-evolution.md)
- [Design Philosophy](design-philosophy.md)
- [Framework Layers](framework-layers.md)
- [Tripous and Data](tripous-and-data.md)
- [Tripous.Data Overview](../tripous-data/overview.md)
