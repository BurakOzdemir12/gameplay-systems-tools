# CLAUDE.md

**Read [`AGENTS.md`](AGENTS.md) at session start. Follow its rules.**

`AGENTS.md` is the single source of truth for AI-tool rules in this repo. This file
exists only so Claude Code auto-loads the redirect.

This project is a **PC game sandbox (Windows x64)** used to build and battle-test
gameplay systems and tools (movement, combat, climbing, gathering, inventory,
perception, environment simulation).

Mobile-only concerns (ads, IAP, haptics, battery) are **out of scope** for this project.

For Unity engineering rules (VContainer, UniTask, dev perf) also read
[`TEAM_UNITY_DEV_GUIDELINES.md`](TEAM_UNITY_DEV_GUIDELINES.md).

For folder structure, namespace rules, asmdef strategy, and scene organization read
[`Docs/PROJECT_ARCHITECTURE.md`](Docs/PROJECT_ARCHITECTURE.md).

For all naming rules (files, classes, assets, prefabs, scenes) read
[`Docs/NAMING_CONVENTIONS.md`](Docs/NAMING_CONVENTIONS.md).

## Learned Behaviors
- **Rule Language**: Always write and edit rule files (like AGENTS.md, CLAUDE.md, etc.) in English, regardless of the language the user speaks in the chat.
- **Turkish-Only Chat, English-Only Deliverables**: Write all conversational responses, implementation plans, and explanations in Turkish — never mix English and Turkish within the same message. Everything that becomes a project artifact — code, comments, file/asset names, PR titles/descriptions, and commit messages — must be entirely in English regardless of chat language. Never produce bilingual (EN + TR) output in either direction. **Exceptions**: (a) a standalone human-facing learning/reference `.md` doc may be bilingual when the user explicitly asks for it; (b) feature design/implementation-plan `.md` docs are ALWAYS bilingual per the Design Doc Format rule below. Bilingual layout is always: Turkish translation section on top, English original section below, in that same file. Code blocks and inline code comments inside such a doc always stay English-only; the file name always stays English-only; Turkish prose uses no Turkish characters (ASCII-fied).
- **Design Doc Format (Tasks First, Reference Below, Bilingual)**: Every feature design/implementation-plan `.md` written for the user to hand-build MUST have two language sections — full Turkish translation on top, English original below. Inside each language section, split the content into: **Part A — Task List**: imperative orders naming the exact files, members, and required behavior, but withholding finished code bodies; where a design choice has a non-obvious reason, prompt the user to reason it out instead of stating the answer. **Part B — Reference Implementation**: the complete worked solution (full code + "why" notes) that the user checks their own attempt against after doing Part A. Reason: the user is a junior developer practicing implementation from specs; they build from Part A first and self-verify with Part B.
- **Code Diff Markers**: When showing a full file or large code block with changes in chat, mark each changed line at the start: `=>` for new additions, `?==` for modifications, `~~` for deletions. Skip markers for small focused snippets where the change is already obvious.
- **Hybrid Architecture**: Follow the Feature + Layer hybrid architecture. Isolate game-specific mechanics into `Assets/_Project/Scripts/Features/<FeatureName>/` (containing their own UI, Logic, Data). Place globally reusable code in `Assets/_Project/Scripts/Shared/`. See `Docs/PROJECT_ARCHITECTURE.md` for details.
- **Feature Folder Roles**: `View/` is reserved for UI-facing MonoBehaviours/Presenters only (HUD, popups). Non-UI MonoBehaviours (physics probes, sensors, gameplay components attached to a GameObject) go in `Components/` instead — a sibling folder to `Logic/`, `View/`, `Data/`.
- **FSM Ownership**: A state machine's full graph (root states + sub-states) stays inside its owning actor's own feature (`Features/Player/`, `Features/Enemy/`) — never split across features by mechanic. Shared, actor-agnostic mechanic engines (climbing, gathering, force/knockback) go in `Shared/Gameplay/`. See `Docs/PROJECT_ARCHITECTURE.md`.
- **SOLID & Design Patterns**: Apply SOLID principles and design patterns pragmatically in every design and implementation — favor composition over inheritance, small single-purpose interfaces, and abstractions at feature boundaries; never pattern-for-pattern's-sake. Full rule in `AGENTS.md` → Engineering rules.
