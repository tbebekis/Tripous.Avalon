# Tripous and Data

Data sits at the center of most business applications.

Whether the application is an ERP system, a CRM system, an accounting application, an inventory management system or an internal business tool, most of its activity revolves around data.

Data is created.

Data is modified.

Data is validated.

Data is searched.

Data is reported.

Data is exchanged with other systems.

Because of this reality, the design of the data layer is one of the most important architectural decisions any framework can make.

The design of Tripous is heavily influenced by this observation.

## Data-Centric Applications

For many years software development has been influenced by different architectural trends.

Some focused primarily on user interfaces.

Some focused on object-oriented modeling.

Some focused on service-oriented architectures.

Others focused on distributed systems.

Tripous originated in a different environment.

It originated from the development of business applications.

In such applications, the database is not merely an implementation detail.

The database is one of the application's most valuable assets.

Business rules depend on it.

Reporting depends on it.

Integration depends on it.

Historical data depends on it.

The framework therefore treats data as a first-class concern rather than as a hidden implementation detail.

## Relational Databases Have Endured

The software industry has experienced countless technological shifts.

Programming languages have appeared and disappeared.

User interface technologies have come and gone.

Architectural fashions have changed repeatedly.

Relational databases, however, have remained remarkably stable.

For decades they have provided:

- Structured data storage
- Transactions
- Constraints
- Indexes
- Query optimization
- Data integrity
- Concurrency control

More importantly, they have provided a common language for working with data.

That language is SQL.

## SQL Is One Of The Great Success Stories Of Software

SQL is often treated as something that modern frameworks should hide.

Tripous does not share that view.

SQL is one of the most successful declarative languages ever created.

For decades it has allowed developers to express complex data operations using a concise and readable syntax.

It is understood by database administrators, developers, analysts and reporting tools.

It is portable.

It is mature.

It is expressive.

Most importantly, it describes data operations directly.

When a developer writes a SQL statement, the intent is immediately visible.

The framework therefore embraces SQL rather than attempting to replace it.

## SQL Is A Feature, Not A Problem

Many modern frameworks attempt to place several abstraction layers between applications and relational databases.

Tripous intentionally avoids this approach.

The framework does not assume that SQL is something developers should be protected from.

On the contrary.

A developer who understands SQL possesses a powerful tool that remains valuable regardless of programming language, framework or platform.

Tripous therefore encourages developers to understand their databases and their SQL rather than relying exclusively on framework abstractions.

The goal is not to hide the database.

The goal is to work with it effectively.

## Metadata Complements SQL

Tripous does not advocate writing large applications as collections of unrelated SQL statements.

The framework's approach is different.

Tripous combines:

- SQL
- Metadata
- Definitions
- Registries
- Data modules

Each of these components contributes a different piece of the overall architecture.

SQL describes data operations.

Metadata describes application structure.

Registries organize metadata.

Data modules provide runtime behavior.

Together they form a coherent data architecture.

## Definitions Are Executable Metadata

One of the central ideas of Tripous is that application structure should be described explicitly.

The framework uses definition classes such as:

- `TableDef`
- `FieldDef`
- `ModuleDef`
- `SelectDef`
- `LookupDef`
- `LocatorDef`

These definitions are not documentation.

They are executable metadata.

A definition describes a part of the application in a form that the framework can understand and use.

For example, a `TableDef` describes a database table and its fields.

The framework can then use that definition to generate SQL statements and runtime behavior.

In other words, the metadata is not passive.

It actively participates in the construction of the application.

## Definitions Generate SQL

A common misconception is that metadata-driven frameworks necessarily move away from SQL.

Tripous takes the opposite approach.

Metadata often generates SQL.

For example, the `TableDef` class can generate SQL statements based on its field definitions.

The framework therefore uses metadata to produce SQL rather than to replace it.

This is an important distinction.

The objective is not:

```text
Metadata → No SQL
```

The objective is:

```text
Metadata → SQL → Database
```

The database remains visible.

The SQL remains visible.

The metadata simply helps organize and generate repetitive structures.

The `TableDef` class is a good example of this philosophy.

A table definition contains metadata describing fields, constraints and other database characteristics.

The framework can then generate SQL statements directly from that metadata.

Definitions are therefore not merely descriptive.

They are productive.

They participate in the creation of the database itself.

## The Database Remains Visible

Some architectures attempt to create the illusion that relational databases do not exist.

Tripous does not.

Tables exist.

Fields exist.

Indexes exist.

Constraints exist.

Foreign keys exist.

Transactions exist.

These concepts are fundamental parts of relational systems.

The framework therefore treats them as important concepts rather than hiding them behind layers of abstraction.

Developers are encouraged to understand them.

Applications become easier to maintain when their underlying data structures remain visible and understandable.

## Why Not ORM?

This question naturally arises whenever SQL-centric architectures are discussed.

Object Relational Mapping frameworks solve a real problem.

They can significantly reduce repetitive database code and provide convenient object-oriented access to data.

Tripous does not reject ORM concepts.

In fact, many ideas that later became common in ORM systems were being explored within the Delphi community years before they became mainstream.

The framework simply makes a different architectural trade-off.

Tripous favors:

- Explicit SQL
- Predictable queries
- Database visibility
- Metadata-driven infrastructure
- Declarative application definitions

Rather than attempting to transform relational databases into object graphs, the framework attempts to make relational databases easier to work with while preserving their strengths.

As [Anders Hejlsberg](https://www.artima.com/articles/inappropriate-abstractions) once remarked:

> "You may want to have the illusion that the data is not in a database. You can have that illusion, but it comes at a cost."

Tripous acknowledges that cost and chooses a different trade-off.

Rather than hiding relational databases, the framework attempts to work with them directly while providing metadata-driven infrastructure on top of them.

## Data Modules

Definitions describe structure.

Data modules provide behavior.

A data module is the runtime counterpart of a module definition.

A simplified view of the `DataModule` class illustrates the idea.

```csharp
public abstract class DataModule
{
    public ModuleDef ModuleDef { get; }

    public virtual void Initialize()
    {
    }

    public virtual void Commit()
    {
    }
}
```

A `ModuleDef` describes a business entity.

A `DataModule` implements its runtime behavior.

This separation between description and execution is a recurring design pattern throughout Tripous.

Data modules act as the meeting point between:

- Database structures
- SQL statements
- Metadata definitions
- Application behavior

They provide a structured place for:

- Data loading
- Validation
- Business rules
- Persistence
- Transactions
- Document processing

Metadata describes the application.

Data modules execute it.

## Registries And Metadata

The framework relies heavily on registries to organize data-related definitions.

The `DataRegistry` acts as the central repository for many descriptor types.

Examples include:

- Modules
- Lookups
- Locators
- Document handlers
- Code providers
- Configuration properties

These definitions can be registered during application startup and later retrieved by name.

This architecture allows applications to remain declarative while still supporting dynamic behavior and extensibility.

The framework understands the application's structure through its metadata.

## Data As Architecture

Many systems treat data as something that exists beneath the application.

Tripous views data differently.

Data is not merely storage.

Data is architecture.

The structure of the database influences:

- Business rules
- Reporting
- Security
- Integration
- Application workflows

Because of this, database design deserves the same attention as application design.

A well-designed data model often survives multiple generations of user interfaces and application technologies.

The history of software repeatedly demonstrates this fact.

Applications change.

Databases endure.

## The Goal

The purpose of Tripous is not to replace relational databases.

The purpose is not to hide SQL.

The purpose is not to create another abstraction layer between applications and their data.

The purpose is to make relational databases easier to use while preserving their strengths.

The framework combines SQL, metadata, definitions, registries and runtime services into a coherent architecture that helps developers build data-centric applications.

The database remains visible.

The SQL remains visible.

The architecture remains understandable.

That is a deliberate design choice.

## Related Articles

- [What Is Tripous?](what-is-tripous.md)
- [History and Evolution](history-and-evolution.md)
- [Design Philosophy](design-philosophy.md)
- [Framework Layers](framework-layers.md)
