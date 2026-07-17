# AGENTS.md

Single source of truth for AI-agent rules in this repository. Read this at the
start of every session and follow it.

## Platform: PC (Windows x64 Standalone)
This is a **PC game sandbox** targeting Windows x64 standalone builds. Mobile
platforms (Android/iOS) are out of scope. Do not introduce mobile-only patterns:
no touch-input assumptions, no battery-saving logic, no mobile ads or IAP, no
haptics (unless explicitly scoped for gamepad rumble). Always think in terms of
keyboard + mouse as the primary input and frame rate targets of 60–120+ fps.

## Project purpose
This repository is a gameplay-systems laboratory: player movement/combat/climbing/
gathering state machines, enemy AI, perception, inventory, and environment
simulation (time of day, weather, seasons). Systems built here are meant to be
extracted and reused in production projects — keep them modular and actor-agnostic
where the architecture says so.

## Engineering rules
- **Guiding principle (most important): simple, proven, whole-system, stoic.** Prefer the
  simplest, most effective, optimized solution. No temporary patches — before each change,
  step back and see from a wide angle how it interacts with the whole system. Never work by
  trial-and-error; act only on proven, verified truths. Work calmly and deliberately (stoic):
  measured, evidence-led, no guessing.
- **Always current:** work from the project's ACTUAL state — re-read the real files before
  answering or editing; never rely on a stale or remembered version of the code.
- **No external links/attributions (unless explicitly approved):** never put an external URL,
  link, citation, attribution, or reference to an outside source/repo into the project, plans,
  memory, code comments, or any reply. Default to none; ask first if one seems needed.
- DI: **VContainer** with constructor injection. App-wide singletons →
  `AppRootLifetimeScope`; per-scene → scene `LifetimeScope`. No service-locator,
  no static singletons for core dependencies.
- Decoupled one-to-many notifications: the typed static `EventBus<T>` in
  `Shared/Events/` is the project's sanctioned event channel. Prefer it over direct
  cross-feature calls; prefer VContainer injection over it for direct service calls.
- Async: **UniTask**, propagate `CancellationToken`.
- Dev perf: avoid hot-path allocations, LINQ/reflection/string churn in loops;
  prefer events over polling; pool frequently created objects.
- Style: minimal and modular; one public type per file; comments only for
  non-obvious intent. No over-engineering.
- **SOLID & design patterns (pragmatic):** Apply SOLID principles and classic design
  patterns (Strategy, State, Adapter, Observer, composition over inheritance) wherever
  they genuinely make sense — as tools for extensibility, not as ceremony. Prefer small
  single-purpose interfaces, depend on abstractions across feature boundaries, keep
  behavior in composable strategies and data in ScriptableObject configs. Never force a
  pattern where a simpler construct does the job (see guiding principle above).
- **English only, no Turkish characters:** No Turkish characters (ş ı ğ ü ö ç and their
  uppercase variants) anywhere in the project — code, comments, file names, folder names,
  asset names, or scene names. All code and comments must be in plain English.

## Architecture enforcement (read `Docs/PROJECT_ARCHITECTURE.md` for full detail)
- **Namespace = folder path.** `Core/` → `GameplaySystemsAndTools.Core`, `Shared/` →
  `GameplaySystemsAndTools.Shared`, `Features/X/` → `GameplaySystemsAndTools.Features.X`.
  Never place a type in a namespace that does not match its physical folder.
- **Feature isolation.** Never write `using GameplaySystemsAndTools.Features.X` inside another
  feature's namespace. Features communicate only through `Shared/` interfaces, the
  `EventBus<T>` channel, or VContainer-registered services.
- **Asmdef split trigger.** Do not create a new `.asmdef` speculatively. Split only when a feature is
  large, stable, has no circular deps, and is a package extraction candidate. Current state:
  `Game.Runtime` + `Game.Editor` only.
- **Move files inside Unity Editor only** (Project window drag). Moving via OS explorer orphans `.meta`
  files and corrupts GUID references. (AI agents moving files from the shell MUST move the paired
  `.meta` file together with each asset in the same operation.)
- **No magic values.** No hardcoded numeric or string literals in logic. Use constants classes
  (`SceneNames`, `LayerMasks`, `AnimatorParams`) or `ScriptableObject` fields.
- **No runtime state in ScriptableObjects.** They are static data containers only.
- **No `Debug.Log` in production paths.** Guard with `#if UNITY_EDITOR || DEVELOPMENT_BUILD`.
- **`Core/` is the composition root.** `AppRootLifetimeScope`, `GameplayLifetimeScope` and
  bootstrappers live there, not in `Shared/`.

See `Docs/NAMING_CONVENTIONS.md` for all naming rules.

## PC-Specific rules

### Input
- Use **Unity Input System** (new) exclusively. No `Input.GetKey` / legacy API.
- All player-facing bindings must be **rebindable at runtime** and persisted via the
  save/settings service. Do not hardcode key constants in gameplay logic.
- Support at minimum: keyboard + mouse, and XInput gamepad.
- Manage cursor state explicitly (`Cursor.lockState`, `Cursor.visible`) — define
  clear rules per gameplay context (e.g. locked during play, visible in menus).

### Graphics & Display
- Expose **quality presets** (Low / Medium / High / Ultra) through the settings
  service; never hardcode quality levels in gameplay code.
- Support **resolution selection** and **windowed / borderless / fullscreen** toggle
  via the settings service; use `Screen.SetResolution` + `FullScreenMode`.
- **VSync** and **target frame rate** must be configurable settings, not hardcoded.
  Default: VSync on (60 fps cap); power users may disable for uncapped rates.
- Shadow quality, AA, draw distance, and LOD bias must be part of quality presets,
  not scattered magic numbers.

### Save & Persistence
- Use **file-based save slots** (JSON or binary) under `Application.persistentDataPath`.
  `PlayerPrefs` is allowed only for window/display preferences (resolution, fullscreen).
- Support **multiple save slots**; never overwrite the only save without confirmation.
- All save/load operations must be async (UniTask) and cancellation-safe.

### Build
- Target: **Windows x64**, scripting backend **IL2CPP**, .NET Standard 2.1.
- Keep build size reasonable: strip unused engine modules, use asset bundles /
  Addressables for large content if the build grows beyond ~2 GB.

See also `TEAM_UNITY_DEV_GUIDELINES.md`, `Docs/PROJECT_ARCHITECTURE.md`, and `Docs/NAMING_CONVENTIONS.md`.

## Learned Behaviors
- **Rule Language**: Always write and edit rule files (like AGENTS.md, CLAUDE.md, etc.) in English, regardless of the language the user speaks in the chat.
- **Turkish-Only Chat, English-Only Deliverables**: Write all conversational responses, implementation plans, and explanations in Turkish — never mix English and Turkish within the same message. Everything that becomes a project artifact — code, comments, file/asset names, PR titles/descriptions, and commit messages — must be entirely in English regardless of chat language. Never produce bilingual (EN + TR) output in either direction. **Exceptions**: (a) a standalone human-facing learning/reference `.md` doc may be bilingual when the user explicitly asks for it; (b) feature design/implementation-plan `.md` docs are ALWAYS bilingual per the Design Doc Format rule below. Bilingual layout is always: Turkish translation section on top, English original section below, in that same file. Code blocks and inline code comments inside such a doc always stay English-only; the file name always stays English-only; Turkish prose uses no Turkish characters (ASCII-fied).
- **Design Doc Format (Tasks First, Reference Below, Bilingual)**: Every feature design/implementation-plan `.md` written for the user to hand-build MUST have two language sections — full Turkish translation on top, English original below. Inside each language section, split the content into: **Part A — Task List**: imperative orders naming the exact files, members, and required behavior, but withholding finished code bodies; where a design choice has a non-obvious reason, prompt the user to reason it out instead of stating the answer. **Part B — Reference Implementation**: the complete worked solution (full code + "why" notes) that the user checks their own attempt against after doing Part A. Reason: the user is a junior developer practicing implementation from specs; they build from Part A first and self-verify with Part B.
- **Code Diff Markers**: When showing a full file or large code block with changes in chat, mark each changed line at the start: `=>` for new additions, `?==` for modifications, `~~` for deletions. Skip markers for small focused snippets where the change is already obvious.
- **Hybrid Architecture**: Follow the Feature + Layer hybrid architecture. Isolate game-specific mechanics into `Assets/_Project/Scripts/Features/<FeatureName>/` (containing their own UI, Logic, Data). Place globally reusable code in `Assets/_Project/Scripts/Shared/`. See `Docs/PROJECT_ARCHITECTURE.md` for details.
- **Feature Folder Roles**: `View/` is reserved for UI-facing MonoBehaviours/Presenters only (HUD, popups). Non-UI MonoBehaviours (physics probes, sensors, gameplay components attached to a GameObject) go in `Components/` instead — a sibling folder to `Logic/`, `View/`, `Data/`.
- **FSM Ownership**: A state machine's full graph (root states + sub-states) stays inside its owning actor's own feature (`Features/Player/`, `Features/Enemy/`) — never split across features by mechanic. Shared, actor-agnostic mechanic engines (climbing, gathering, force/knockback) go in `Shared/Gameplay/`. See `Docs/PROJECT_ARCHITECTURE.md`.
