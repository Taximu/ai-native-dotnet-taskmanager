# CLAUDE.md — AI Development Agent Operating Guide

This file tells AI coding agents (Claude Code, GitHub Copilot, Cursor and other agents) how to work
on this codebase. Read this before generating any code.

## Project Context

**What this is:** Task Management Service — a .NET 8 Web API demonstrating a
Level 3 AI-native development workflow (agentic code generation, automated
testing, self-healing loops, human review gates).

**Stack:**
- .NET 8, C# 12
- ASP.NET Core Web API
- Entity Framework Core (SQLite for dev/test, no external DB dependency)
- xUnit + Moq for testing
- GitHub Actions for CI

**Architecture pattern:** Clean layering —
`Api` (controllers) → `Services` (business logic) → `Repositories` (data access) → `Data` (EF Core context)

## Agent Responsibilities

When asked to implement a feature, an agent should:

1. Read the relevant spec in `docs/SDD_TASKMANAGEMENT.md` before writing code.
2. Generate implementation code in a **new feature branch** (`feature/<short-name>`).
3. Generate unit tests alongside the implementation — do not treat tests as optional.
4. Run `dotnet build` and `dotnet test`. If either fails, enter the
   self-healing loop (see below) before declaring the task complete.
5. Leave the code in a state ready for human review — do not merge to `main`.

## Self-Healing Loop

```
generate code → dotnet build
  ├─ fails → analyze error → apply fix → dotnet build (retry, max 3 attempts)
  └─ succeeds → dotnet test
       ├─ fails → analyze failing test → apply fix → dotnet test (retry, max 3 attempts)
       └─ succeeds → done, ready for human review
```

If 3 attempts are exhausted without success, stop and flag the specific
failure for a human — do not keep guessing. Document every iteration
(what failed, what was changed) in the PR description.

## Routing Rules (task complexity)

| Complexity | Signal | Agent behavior |
|---|---|---|
| Simple | Single endpoint, no new entities, <2h estimate | Generate code + tests directly from spec |
| Medium | New entity, multiple endpoints, cross-layer changes | Validate spec completeness first, ask clarifying questions if acceptance criteria are ambiguous, then generate |
| Complex | Touches auth, external integration, or >3 files | Require a short design note (2-3 sentences on approach) before generating code, then implement in smaller increments |

## Coding Standards

- Async all the way down — no `.Result` or `.Wait()` on async calls.
- Repositories return domain entities; DTOs are mapped in the Service layer, never in Controllers.
- Use `AsNoTracking()` for read-only EF Core queries.
- No business logic in controllers — controllers only orchestrate.
- Every public method that can fail should have at least one test for the failure path.

## Known Pitfalls (learned the hard way — update this section as you find more)

- EF Core migrations must be generated as a separate explicit step (`dotnet ef migrations add`) — agents should not attempt to auto-generate migrations as part of a feature task.
- Mocking `DbContext` directly is fragile — use an in-memory SQLite provider for repository tests instead.
- Pagination logic is an easy place for off-by-one errors — always include a boundary-value test (page 0, page beyond last, pageSize 0).

## Out of Scope (v1)

- Authentication / authorization
- Real MCP server calls (simulated only — see `docs/MCP_CONNECTIONS.md`)
- Multi-tenancy

## Human Control Points

- **Design validation:** Complex features require a human-approved design note before implementation.
- **Merge gate:** No code reaches `main` without human review of the PR, regardless of test pass status.
