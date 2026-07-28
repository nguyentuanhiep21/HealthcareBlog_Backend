<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **HealthcareBlog_Backend** (1981 symbols, 5269 relationships, 167 execution flows). Use the GitNexus MCP tools to understand code, assess impact, and navigate safely.

> If any GitNexus tool warns the index is stale, run `npx gitnexus analyze` in terminal first.

## Always Do

- **MUST run impact analysis before editing any symbol.** Before modifying a function, class, or method, run `gitnexus_impact({target: "symbolName", direction: "upstream"})` and report the blast radius (direct callers, affected processes, risk level) to the user.
- **MUST run `gitnexus_detect_changes()` before committing** to verify your changes only affect expected symbols and execution flows.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- When exploring unfamiliar code, use `gitnexus_query({query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `gitnexus_context({name: "symbolName"})`.

## Never Do

- NEVER edit a function, class, or method without first running `gitnexus_impact` on it.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis.
- NEVER rename symbols with find-and-replace — use `gitnexus_rename` which understands the call graph.
- NEVER commit changes without running `gitnexus_detect_changes()` to check affected scope.

## Resources

| Resource | Use for |
|----------|---------|
| `gitnexus://repo/HealthcareBlog_Backend/context` | Codebase overview, check index freshness |
| `gitnexus://repo/HealthcareBlog_Backend/clusters` | All functional areas |
| `gitnexus://repo/HealthcareBlog_Backend/processes` | All execution flows |
| `gitnexus://repo/HealthcareBlog_Backend/process/{name}` | Step-by-step execution trace |

## CLI

| Task | Read this skill file |
|------|---------------------|
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus/gitnexus-cli/SKILL.md` |

<!-- gitnexus:end -->

<!-- begin:backend-rules -->

# C# .NET Clean Architecture Rules & Guidelines

## 1. Project Structure & Dependency Flow
Enforce Clean Architecture layer separation. Dependencies MUST pointing INWARD only:
`Web.API` -> `Infrastructure` -> `Application` -> `Domain`

- **Domain Layer (`Core/Domain`)**: 
  - Contains Entities, Value Objects, Domain Events, Enums, and Repository Interfaces.
  - Zero external dependencies! (NO EF Core, NO ASP.NET, NO third-party packages except C# primitives/guards).
- **Application Layer (`Core/Application`)**:
  - Contains Use Cases (CQRS Commands/Queries), DTOs, Mapping, and Service Interfaces.
  - Depends ONLY on the `Domain` layer.
- **Infrastructure Layer (`Infrastructure`)**:
  - Contains EF Core DbContext, External Services integration (Email, Supabase, Payment), Repositories implementations.
- **Web.API Layer (`Presentation/Web.API`)**:
  - Contains Controllers / Minimal APIs, Middlewares, Program.cs DI configurations.

---

## 2. C# Language & Modern Code Quality
- **Language Level**: Use modern C# features (C# 12+ / .NET 8+).
- **Immutability**: Prefer `record` for DTOs, Commands, Queries, and Value Objects.
- **Nullable Context**: `<Nullable>enable</Nullable>` is mandatory. Avoid using `!` (null-forgiving) unless strictly necessary.
- **Async/Await**: 
  - All I/O operations MUST be async (`async Task<T>`).
  - Always pass `CancellationToken` down to the DB/I/O level.
- **Primary Constructors**: Use primary constructors for Dependency Injection in classes and record definitions.

---

## 3. Application Layer & CQRS Pattern
- Implement CQRS using **MediatR** (or explicit Command/Query handlers).
- Organize code by **Feature/Vertical Slices** within Application:
  `Features/[FeatureName]/Commands/Create[Entity]/`
  - `Create[Entity]Command.cs` (MediatR `IRequest<Result<T>>`)
  - `Create[Entity]CommandHandler.cs`
  - `Create[Entity]CommandValidator.cs` (FluentValidation)
- **DTO Isolation**: NEVER return Domain Entities directly from API or Application Layer. ALWAYS map to DTOs.
- **Validation**: Use **FluentValidation** for automatic command/query validation via MediatR pipeline behaviors.

---

## 4. Entity Framework Core & Data Access
- **Encapsulation**: Use private setters or `init` properties for Entities. Enforce business logic through Entity methods.
- **Queries Optimization**:
  - Use `.AsNoTracking()` for ALL read-only queries.
  - Avoid N+1 queries using explicit `.Include()` / `.ThenInclude()` or Projection (`.Select()`).
- **Configuration**: Use `IEntityTypeConfiguration<T>` in Infrastructure layer instead of attributes on Domain Entities.

---

## 5. API Design & Global Error Handling
- **Controllers / Endpoints**: Keep them thin! Delegates business execution to MediatR (`IMediator.Send()`).
- **RESTful Naming**: Plural nouns for routes (e.g., `api/v1/users`). Explicit status codes (200, 201, 400, 404, 500).
- **Error Handling**: 
  - Do NOT use exceptions for control flow. Use a **Result Pattern** (`Result<T>` or `OneOf`).
  - Handle unexpected exceptions globally via `IExceptionHandler` middleware producing **RFC 7807 ProblemDetails** JSON responses.

---

## 6. Code Generation Checklist for AI
When generating new features, always generate code in this order:
1. **Domain Entity / Value Object** (if new)
2. **Repository Interface** (in Domain)
3. **Command/Query & DTOs** (in Application)
4. **Validator** (in Application)
5. **CommandHandler / QueryHandler** (in Application)
6. **EF Core Configuration & Repository Implementation** (in Infrastructure)
7. **Controller / Minimal API Endpoint** (in Web.API)

<!-- end:backend-rules -->