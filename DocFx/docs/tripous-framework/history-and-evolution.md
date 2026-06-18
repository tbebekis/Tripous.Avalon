# History and Evolution

The history of Tripous did not begin with .NET, Avalonia, GitHub, or even C#.

Its roots go back to a different era of software development, when desktop applications dominated the industry, internet connections were slow, source code was exchanged through mailing lists, and developers gathered in newsgroups rather than social networks.

To understand Tripous, it is useful to understand the environment in which many of its ideas were born.

## The Delphi Years

The story begins in the mid-1990s with Borland Delphi.

At the time, Delphi was one of the most productive development environments available. It combined a fast native compiler, a rich component library and a language that was both powerful and easy to understand.

Many of the ideas that later became part of Tripous were originally explored while building business applications using Delphi.

The problems were familiar:

- Data entry forms
- Database access
- Business rules
- Configuration management
- Reusable application infrastructure
- Reporting
- User interface generation

As applications grew larger, the need for reusable frameworks became increasingly apparent.

The goal was not simply to write code.

The goal was to write less code by creating reusable solutions to recurring problems.

## The Rise of Open Source Delphi

The late 1990s and early 2000s were a fascinating period for the Delphi community.

The modern open-source ecosystem did not yet exist.

GitHub did not exist.

Stack Overflow did not exist.

Most collaboration happened through:

- Newsgroups
- Mailing lists
- Personal websites
- SourceForge projects

One of the most influential community efforts of that era was Project JEDI.

The JEDI project became one of the largest and most ambitious Delphi open-source initiatives ever created.

Its objective was simple:

Create high-quality reusable libraries and tools for Delphi developers.

Over time the project produced several important libraries, including the famous JCL (JEDI Code Library) and JVCL (JEDI Visual Component Library).

Thousands of developers used these libraries in commercial and non-commercial software projects around the world.

The JEDI community was not merely a software repository.

It was also a place where developers exchanged ideas, discussed architecture, reviewed code and experimented with new approaches to software development.

Many of the concepts that later influenced Tripous were refined during those years.

## The Obiwan Experiment

One of the most ambitious JEDI subprojects was Obiwan.

Obiwan was created during a period when developers were actively searching for better ways to bridge the gap between object-oriented programming and relational databases.

Today such technologies are commonly referred to as Object Relational Mapping frameworks, or ORMs.

At the time, however, the terminology was less standardized.

The Delphi community often used the term Object Persistence Framework (OPF).

The goal was to allow business objects to be stored and retrieved from relational databases while minimizing repetitive database code.

Obiwan explored ideas that would later become common throughout the software industry:

- Metadata-driven development
- Object persistence
- Mapping definitions
- Separation of business objects and storage mechanisms
- Declarative application design

Although Obiwan itself did not become a mainstream framework, the experience gained from working on such ideas proved invaluable.

More importantly, it demonstrated a lesson that would influence future decisions:

Technology changes rapidly, but good ideas survive.

Many years later, concepts such as metadata, descriptors, registries and declarative definitions would still be present inside Tripous, even though the original technologies had changed completely.

## Learning From Frameworks

During the following years many frameworks appeared and disappeared.

Some were successful.

Some were not.

Some introduced useful concepts.

Others introduced unnecessary complexity.

Like many developers of that era, the author experimented extensively with different approaches to application architecture.

Various techniques were explored:

- Object persistence frameworks
- Metadata systems
- Declarative architectures
- Code generation
- Framework-based development
- Component-oriented design

Some ideas were adopted.

Others were rejected.

Over time a simple principle emerged:

A framework should help developers understand their applications, not hide them.

This principle eventually became one of the foundations of Tripous.

## Moving To .NET

The arrival of Microsoft's .NET platform represented one of the largest technological shifts in modern software development.

For many Delphi developers, including the author of Tripous, this transition required a complete re-evaluation of existing tools, libraries and development practices.

The migration began in the early years of .NET and eventually became permanent.

During this period several generations of the Tripous framework were developed.

Many Delphi concepts were reimplemented for the .NET world.

Others were redesigned completely.

The move to .NET provided access to:

- A modern runtime environment
- Reflection
- Generics
- Improved tooling
- Rich class libraries
- Cross-language interoperability

At the same time, many of the original design goals remained unchanged.

The framework still aimed to simplify the development of business applications and data-centric systems.

## The ORM Question

One of the most significant architectural decisions in the evolution of Tripous concerns database access.

Many modern application frameworks are built around ORM technologies.

During the 2000s and 2010s ORM frameworks became increasingly popular and eventually became the default choice for many development teams.

Tripous took a different path.

This was not due to lack of familiarity with ORM concepts.

On the contrary, object persistence and object-relational mapping had been studied and discussed for many years, beginning long before they became mainstream technologies.

After extensive experimentation, a different conclusion was reached.

Relational databases already provide an extremely powerful language:

SQL.

SQL is expressive.

SQL is efficient.

SQL is universally understood by database professionals.

Rather than attempting to hide relational databases behind object abstractions, Tripous embraces SQL while complementing it with metadata, descriptors and declarative application definitions.

This decision continues to influence the framework today.

## The Declarative Model

As Tripous evolved, one architectural idea became increasingly important.

Applications contain large amounts of information that describes other information.

Examples include:

- Table definitions
- Field definitions
- Form definitions
- Lookup definitions
- Command definitions
- Module definitions

Instead of scattering such information throughout application code, Tripous gradually adopted a declarative model.

Developers describe the structure of an application using definitions and metadata.

The framework then uses those definitions to construct runtime behavior.

This approach reduces repetitive code while preserving clarity and control.

Over time this idea became one of the defining characteristics of the framework.

## Modern .NET

The transition from the .NET Framework era to modern .NET represented another major milestone.

The framework was gradually updated to take advantage of:

- Modern language features
- Improved runtime performance
- Cross-platform support
- Modern development tools

At the same time, older ideas that had proven successful over many years were preserved.

The objective was not to rewrite the framework from scratch.

The objective was to evolve it without losing the lessons learned from previous generations.

## The Avalonia Era

The most recent chapter in the story of Tripous is the adoption of Avalonia UI.

For many years desktop applications had been closely associated with Windows-specific technologies.

Avalonia changed that equation.

For the first time it became practical to build modern cross-platform desktop applications using a development model that felt familiar to developers with a background in Delphi, WinForms and traditional desktop software.

This led to the creation of Tripous.Desktop.

The current desktop layer of the framework is built on top of Avalonia while preserving the architectural ideas that had evolved over many years.

The result is a framework that combines modern cross-platform technology with concepts that have been refined through decades of practical software development.

## Looking Back

Every software project is shaped by its history.

Tripous is no exception.

The framework contains ideas that originated in the Delphi era, matured through open-source collaboration, survived several technology transitions and continue to evolve today.

Many technologies that once seemed revolutionary have disappeared.

Many frameworks that once appeared indispensable are no longer remembered.

The problems, however, remain remarkably similar.

How do we build software that is understandable?

How do we reduce repetitive work?

How do we create systems that remain maintainable over many years?

How do we help developers remain productive without sacrificing control?

Tripous represents one ongoing attempt to answer those questions.

The story is still being written.

## Further Reading

- [Project JEDI Portal](https://www.delphi-jedi.org/)
- [JEDI Obiwan Object Persistence Framework on SourceForge](https://sourceforge.net/projects/obiwan/)
- [Obiwan Project Website](https://obiwan.sourceforge.net/)
- [Obiwan Documents](https://obiwan.sourceforge.net/documents.html)
- [Undocumented Delphi 7](http://blong.com/Undocumented/Delphi7.htm)
- [What Is Tripous?](what-is-tripous.md)
- [Design Philosophy](design-philosophy.md)
- [Tripous and Data](tripous-and-data.md)
