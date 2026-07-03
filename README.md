# AI-Native .NET Development Workflow

A working demonstration of a Level 3 agentic SDLC on a real .NET codebase —
spec-driven development, AI code and test generation, self-healing build
loops, and human-in-the-loop review gates.

## What this shows

- **Spec-driven development** — features are specified with acceptance
  criteria and edge cases before any code is written
- **Agentic code + test generation** — AI agents (Claude Code, GitHub
  Copilot) implement features from specs on dedicated feature branches
- **Self-healing loop** — agents build, test, and auto-correct failures
  before handing off for review
- **Human control points** — every feature goes through a human merge gate
- **Simulated MCP integration** — a notification gateway shows how a real
  MCP connection (ADO, Confluence) would plug into the workflow

## Where to look

| File | What it is |
|---|---|
| [`CLAUDE.md`](./CLAUDE.md) | Agent operating manual — standards, routing rules, self-healing loop |
| [`AGENTS.md`](./AGENTS.md) | Sub-agent roles and how they hand off work |
| [`docs/SDD_TASKMANAGEMENT.md`](./docs/SDD_TASKMANAGEMENT.md) | Feature specs, written for agents to implement directly |
| `src/` | The .NET 8 Web API (Core → Infrastructure → Api layers) |
| `tests/` | xUnit tests, generated alongside each feature |

## Stack

.NET 8 · ASP.NET Core Web API · EF Core (SQLite) · xUnit · Moq

## Running it

```bash
dotnet build
dotnet test
dotnet run --project src/TaskManagement.Api
```
