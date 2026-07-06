# AGENTS.md — Sub-Agent Architecture

This describes the distinct agent roles used on this project and how they hand
off work to each other. Each role can be a separate Claude Code session, a
separate tool (Copilot vs Cursor), or a distinct prompt within one session —
what matters is that each has a clearly bounded job and reviews the previous
agent's output rather than blindly trusting it.

## Roles

### 1. Solution/Code Generator Agent
- **Input:** A spec section from `docs/SDD_TASKMANAGEMENT.md`
- **Output:** Implementation code in a feature branch, following `CLAUDE.md` standards
- **Does not:** Write its own tests (handed off to the Unit Test Agent) or merge anything

### 2. Unit Test Agent
- **Input:** The implementation code produced above + the acceptance criteria from the spec
- **Output:** xUnit tests covering the happy path, documented edge cases, and at least one failure path
- **Runs in a fresh session** from the Code Generator — this matters, a fresh
  context is less likely to only test what the generator already assumed would work
- **Target:** >80% coverage on new code

### 3. Build & Self-Healing Agent
- **Input:** Code + tests from the above two agents
- **Output:** A build that compiles and a test suite that passes, or a clearly
  documented failure after 3 correction attempts
- **Behavior:** See the self-healing loop in `CLAUDE.md`

### 4. Code Review Agent
- **Input:** The finished feature branch (code + tests)
- **Output:** A review comment list — security, performance, architecture, and
  adherence to `CLAUDE.md` standards
- **Runs in a different tool or fresh session** than the one that generated the
  code — the point is to catch what the generating agent was blind to, not to
  re-confirm its own work
- **Feeds into:** the human merge gate. This agent recommends; it does not approve.

### 5. Discovery Agent (used when onboarding to unfamiliar code — see `docs/DISCOVERY.md`)
- **Input:** An existing codebase with no prior context
- **Output:** A map of architecture, module boundaries, and coding conventions
- **Used once per new codebase**, not per feature

## Inter-Agent Handoff

Handoffs happen via Git artifacts, not memory — this keeps the workflow
reproducible and reviewable by a human at any point:

- Feature branches (`feature/<name>`) carry the code between generator → tester → reviewer
- PR descriptions carry the self-healing log (what failed, what was changed, how many iterations)
- `docs/` markdown files carry the spec and discovery context that agents read before starting

## Task Classifier (which agents run, and in what order)

```
New feature request
  │
  ├─ Is there an existing spec? ──No──▶ Stop. Write spec first (human + AI collaboration).
  │
  Yes
  │
  ▼
Complexity from CLAUDE.md routing table
  │
  ├─ Simple  → Code Generator → Unit Test Agent → Build/Self-Heal → Human merge
  ├─ Medium  → Code Generator (spec check first) → Unit Test Agent → Build/Self-Heal → Code Review Agent → Human merge
  └─ Complex → Design note (human approval) → Code Generator → Unit Test Agent → Build/Self-Heal → Code Review Agent → Human merge
```

## Pitfalls in Agent Design (update as you learn more)

- Letting the same session both generate code and review it produces reviews
  that rubber-stamp the generator's own blind spots — always use a fresh
  session or different tool for review.
- Skipping the spec step and going straight to code generation produces
  plausible-looking code that doesn't match actual acceptance criteria —
  the spec check is not optional busywork, it's what makes the rest of the
  loop reliable.
