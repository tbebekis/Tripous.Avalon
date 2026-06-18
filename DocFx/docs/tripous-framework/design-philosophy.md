# Design Philosophy

Every software framework is shaped by a set of assumptions.

Some assumptions are explicit.

Others remain hidden beneath layers of code, abstractions and conventions.

Tripous is no exception.

The framework was not created as an academic exercise, nor was it designed to follow a particular architectural trend. Its design philosophy emerged gradually through many years of building real applications, maintaining existing systems and learning from both successful and unsuccessful approaches.

The ideas presented in this article are the principles that guide the framework's design.

## Why Another Framework?

The original motivation behind Tripous was simple.

Data-centric applications contain a great deal of repetition.

Business applications frequently require the same types of functionality:

- Data access
- Validation
- User interfaces
- Lookups
- Configuration
- Commands
- Reporting
- Metadata

Developers often find themselves solving the same problems repeatedly.

The objective of Tripous is not to eliminate application development.

The objective is to eliminate unnecessary repetition while preserving developer control.

## Simplicity Over Complexity

One lesson learned repeatedly over many years is that complexity accumulates.

Every layer of abstraction, every additional framework dependency and every architectural pattern introduces a cost.

Sometimes that cost is justified.

Often it is not.

Tripous generally favors simple solutions over sophisticated ones.

This does not mean simplistic solutions.

It means solutions that remain understandable months or years after they were written.

A framework should help reduce complexity, not create new forms of it.

## Explicit Behavior Over Hidden Magic

Many modern frameworks attempt to automate large portions of application behavior.

While automation can be useful, it often comes with a price.

The more a framework hides from developers, the harder it becomes to understand what is actually happening.

Tripous favors explicit behavior.

Developers should be able to answer questions such as:

- Where does this data come from?
- Why was this object created?
- Which code executed?
- Which SQL statement was sent to the database?

Predictability is considered a feature.

A framework should assist developers, not surprise them.

## SQL Is Not The Enemy

One of the most distinctive characteristics of Tripous is its relationship with relational databases.

Many modern development approaches attempt to hide databases behind object abstractions.

Tripous takes a different view.

Relational databases are powerful systems with their own language and their own strengths.

That language is SQL.

SQL has survived decades of technological change because it remains expressive, efficient and widely understood.

Rather than attempting to replace SQL, Tripous embraces it.

The framework uses metadata, descriptors and application definitions to complement SQL, not to hide it.

Developers remain free to use the full capabilities of relational databases whenever appropriate.

## Declarative Instead of Procedural

Applications contain large amounts of information that describe their own structure.

Examples include:

- Tables
- Fields
- Forms
- Commands
- Modules
- Lookups
- Locators

Traditional development approaches often scatter this information throughout procedural code.

Tripous prefers a declarative approach.

Developers describe the structure of an application using definitions and metadata.

The framework then uses those definitions to construct runtime behavior.

The result is less repetitive code, greater consistency and a clearer description of the application itself.

## Metadata As A First-Class Concept

Metadata is not an auxiliary feature in Tripous.

It is one of the framework's central ideas.

The framework attempts to understand an application through metadata definitions.

A field definition describes a field.

A form definition describes a form.

A module definition describes a module.

The framework then uses those definitions to provide behavior and infrastructure.

This approach allows large parts of an application to be described rather than manually constructed.

## Registries Instead Of Hard-Coding

Another recurring theme throughout the framework is the use of registries.

Applications frequently contain information that needs to be discovered, extended or replaced.

Examples include:

- Modules
- Forms
- Commands
- Lookup sources
- Locators
- Services

Instead of hard-coding references throughout the application, Tripous uses registries to manage such information centrally.

This approach improves discoverability, extensibility and long-term maintainability.

It also makes plugin-based extensions significantly easier to implement.

## Framework As Infrastructure

A framework should provide infrastructure.

It should not attempt to become the application.

Tripous provides mechanisms, services and foundations.

Business rules remain the responsibility of the application itself.

This distinction is important.

Framework code should remain generic.

Application code should remain specific.

When those responsibilities become mixed, both the framework and the application become harder to maintain.

## Long-Term Maintainability

Software often survives much longer than originally expected.

Applications that were supposed to last five years frequently remain in production for fifteen or twenty.

Because of this reality, maintainability is one of the most important design goals of the framework.

Readable code is preferred over clever code.

Stable concepts are preferred over fashionable solutions.

Evolution is preferred over complete rewrites.

The objective is not merely to solve today's problems.

The objective is to create systems that remain understandable years later.

## Technology Changes, Ideas Survive

The history of software development is filled with technologies that once appeared revolutionary.

Many have disappeared.

Others have been replaced.

Throughout its evolution, Tripous has passed through several technological eras.

- Delphi
- Win32
- .NET Framework
- Modern .NET
- Avalonia

The technologies changed.

Many implementation details changed.

The underlying ideas, however, remained remarkably stable.

The framework therefore attempts to focus on concepts rather than trends.

Good ideas tend to outlive the technologies that originally implemented them.

## The Ultimate Goal

The philosophy of Tripous can be summarized by a simple objective.

Help developers build applications that are:

- Understandable
- Maintainable
- Productive
- Predictable

Reduce repetitive work.

Preserve developer control.

Use technology as a tool rather than an objective.

Everything else follows from those principles.

## Related Articles

- [What Is Tripous?](what-is-tripous.md)
- [History and Evolution](history-and-evolution.md)
- [Framework Layers](framework-layers.md)
- [Tripous and Data](tripous-and-data.md)
