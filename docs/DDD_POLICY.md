# DDD Policy

## Objective
Keep the solution aligned with Domain-Driven Design and Clean Architecture boundaries.

## Layer Responsibilities
- `Domain`: business model and rules (entities, value objects, aggregates, domain services, domain exceptions).
- `Application`: use cases, orchestration, ports/interfaces, DTOs.
- `Infrastructure`: technical adapters (EF Core, repositories, external services, persistence).
- `Api` / `Web`: presentation and composition root (HTTP/UI, DI wiring, auth, transport concerns).

## Dependency Rules
Allowed project references:

- `Domain` -> (none)
- `Application` -> `Domain`
- `Infrastructure` -> `Application`, `Domain`
- `Api` -> `Application`, `Domain`, `Infrastructure`
- `Web` -> `Application`, `Domain`, `Infrastructure`

Additional rule:
- Domain code must never depend on EF Core, ASP.NET Core, or other infrastructure/presentation frameworks.

## Practical Conventions
- Put invariants in `Domain` constructors/methods, not in controllers.
- Keep use-case logic in `Application/UseCases`.
- Keep repository interfaces in `Application` (or `Domain` if purely domain concept), implementations in `Infrastructure`.
- Use `Api`/`Web` only as transport/composition layers.

## Enforcement
Architecture tests in `src/UnitTest/Architecture/DDDProjectDependenciesTests.cs` validate project dependency boundaries.
