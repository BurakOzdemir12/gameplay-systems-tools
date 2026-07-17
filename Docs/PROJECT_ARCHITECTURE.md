# Project Architecture

**Platform:** PC — Windows x64 Standalone
**Pattern:** Hybrid (Feature + Layer)

---

## Folder Structure

All game scripts live under `Assets/_Project/Scripts/`. The four top-level folders have strict, non-overlapping definitions.

### `Core/`
Application bootstrap layer. Contains things that exist exactly once and start the app.
- `AppRootLifetimeScope` and any other root VContainer scopes
- `GameplayLifetimeScope` (per-scene scope for gameplay scenes)
- Application bootstrappers and entry points (`IStartable` implementors wired at root)
- Global state machines that govern the entire app lifecycle
- Scene loader/flow controller

**Rule:** Nothing in `Core/` is "reusable" — it is the composition root. Do not put generic utilities here.

### `Features/` (Feature-Based Encapsulation)
One sub-folder per major game mechanic. Everything belonging to a feature stays inside it.

```
Features/Player/
├── Logic/       ← pure C# systems and the actor's FSM (state machine + states)
├── Components/  ← non-UI MonoBehaviours: physics probes, sensors, gameplay components on GameObjects
├── View/        ← UI-facing MonoBehaviours, Presenters (HUD, popups, UI-bound components)
├── Data/        ← ScriptableObjects, config structs, feature enums
└── Editor/      ← custom inspectors for this feature only
```

**Rule:** `View/` is UI-only. A MonoBehaviour that isn't UI (ground checks, colliders, sensors, anything
that just needs Unity's GameObject lifecycle) belongs in `Components/`, not `View/`.

**Rule:** A feature folder must never `using` another feature's namespace directly.
Features communicate only through `Shared/` interfaces, `EventBus<T>` events in `Shared/Events/`,
or VContainer-registered services.

**Current features:**

| Feature | Contents |
|---|---|
| `Features/Player/` | `PlayerStateMachine`, `PlayerBaseState`, all player root/sub states (movement, combat, climbing, gathering), player components (health, hurtbox, attack signal), player configs |
| `Features/Enemy/` | `EnemyStateMachine`, `EnemyBaseState`, all enemy states, enemy brains/perception controller, enemy HUD (View), enemy configs |
| `Features/Inventory/` | Inventory model (`InventorySlot`), `InventoryComponent`, pickup/drop, item data, inventory + hotbar UI |
| `Features/Environment/` | Feature **group**: `TimeOfDay/`, `Weather/`, `Season/`, `Skybox/` — each with the standard layout |

**Feature groups.** When one domain contains several sibling variants that share contracts,
the feature may be a group: `Features/Environment/<Domain>/` where each domain
(`TimeOfDay/`, `Weather/`, `Season/`, `Skybox/`) carries the standard `Logic/`, `Components/`,
`View/`, `Data/` layout. The group counts as ONE feature for the isolation rule.
Namespaces still mirror the path: `GameplaySystemsAndTools.Features.Environment.Weather`.

### `Shared/` (Layer-Based Reusables)
Generic code used by **two or more** features. If only one feature uses it, it stays inside that feature.

```
Shared/
├── Events/     ← EventBus<T> infrastructure + every cross-feature event type
├── Gameplay/   ← actor-agnostic mechanic engines and base classes:
│   ├── StateMachine/  (StateMachineBase, StateBase — pure FSM plumbing)
│   ├── Climbing/      (ledge/parkour detection engine, vault configs)
│   ├── Combat/        (weapons, tools, shields, targeting, attack configs)
│   ├── Gathering/     (resource nodes, gathering validator/controller)
│   ├── Health/        (HealthBase, HurtboxBase, Ragdoll, IDamageable, IStunnable)
│   ├── Perception/    (FieldOfView, noise emit/sense, NoiseService)
│   ├── Feedback/      (surface-driven SFX/VFX feedback profiles)
│   ├── Surfaces/      (SurfaceDefinition, SurfaceDetection)
│   └── ForceReceiver, GroundChecker, Throwable (root-level primitives)
├── Input/      ← input action wrappers used across features (PlayerInputHandler)
├── Audio/      ← AudioService, SoundEmitter pool, audio profiles
├── Data/       ← global configs, universal enums (WeaponType, SurfaceType, SeasonType, ...)
└── Visuals/    ← camera shake, VfxService, generic VFX helpers
```

**Rule:** `Shared/` must not depend on any `Features/` namespace.

### `Editor/`
Project-wide custom editor tools and windows (e.g. `AutoNamespaceProcessor`).
Feature-specific inspectors go in `Features/X/Editor/` instead.

---

## Namespace Convention

Root namespace: `GameplaySystemsAndTools`.

Namespaces mirror folder paths **down to the feature root** (or `Shared/` mechanic root).
Structural folders that only organize files inside that unit — `Logic/`, `Components/`,
`View/`, `Data/`, `Input/`, `States/` (and organizational subfolders like `Interfaces/`,
`Structs/`, `Enums/`, `RootStates/`) — do **not** extend the namespace. Reason: these folders
separate Unity lifecycle concerns, not domain boundaries; extending the namespace with them
forces a `using` churn every time a file moves between sibling subfolders of the same feature.
`Editor/` is the one exception: it is always appended (asmdef requirement).

| Folder | Namespace |
|---|---|
| `Core/` | `GameplaySystemsAndTools.Core` |
| `Shared/Events/` | `GameplaySystemsAndTools.Shared.Events` |
| `Shared/Gameplay/` (root files) | `GameplaySystemsAndTools.Shared.Gameplay` |
| `Shared/Gameplay/Combat/Targeting/` | `GameplaySystemsAndTools.Shared.Gameplay.Combat` |
| `Features/Player/` | `GameplaySystemsAndTools.Features.Player` |
| `Features/Player/Logic/States/RootStates/` | `GameplaySystemsAndTools.Features.Player` |
| `Features/Environment/Weather/View/` | `GameplaySystemsAndTools.Features.Environment.Weather` |
| `Editor/` | `GameplaySystemsAndTools.Editor` |
| `Features/Player/Editor/` | `GameplaySystemsAndTools.Features.Player.Editor` |

New scripts created inside the Unity Editor get their namespace stamped automatically by
`Editor/AutoNamespaceProcessor.cs`, which implements exactly this rule.

**Rule:** Never put a `Core` type in `GameplaySystemsAndTools.Shared` or vice versa. The namespace is the contract.

**Note on Combat subfolders:** `Weapons/`, `Tools/`, `Shields/`, `Targeting/` under
`Shared/Gameplay/Combat/` are organizational — everything is `...Shared.Gameplay.Combat`.

---

## Assembly Definition Strategy

Start with the minimum number of assemblies and split only when justified.

**Current (correct for project scale):**
```
Game.Runtime.asmdef    ← covers Core/, Shared/, and all Features/
Game.Editor.asmdef     ← covers Editor/ and Features/*/Editor/
```

**Split a feature into its own `.asmdef` only when ALL of these are true:**
1. The feature is large (10+ files) and edited in isolation from other features.
2. It has no circular dependencies with other features.
3. It is a candidate for extraction into a standalone package later.

**Rules:**
- Never create an asmdef for a feature that is still actively changing its API — it will force recompiles of every dependent assembly on each change.
- Always move files using the **Unity Editor** (Project window drag), never via OS file explorer or terminal. Unity must regenerate `.meta` files on move.
- No circular asmdef references. If you find yourself needing one, the design boundary is wrong — extract the shared concept into `Shared/`.
- Test assemblies are always separate from runtime assemblies.

---

## Scene Organization

```
Assets/Scenes/
├── PlayerSystems.unity   ← primary gameplay sandbox (player, enemies, environment sim)
├── MainScene.unity       ← environment/world sandbox
└── ShaderTestScene.unity ← dev/QA scene, excluded from build
```

**Rules:**
- Each gameplay scene has its own `GameplayLifetimeScope` GameObject that declares the scene's local dependencies.
- New scenes follow the naming table in `Docs/NAMING_CONVENTIONS.md` (`World_`, `Menu_`, `Test_` prefixes).
- Never use `SceneManager.LoadScene` (non-additive) for gameplay scenes once a persistent root scope exists — use additive loading so the root scope persists.

---

## ScriptableObject Rules

- ScriptableObjects are **static data containers** (configs, definitions, tunable parameters). Never store runtime state in them.
- Place global data at `Assets/_Project/Data/`. Place feature-specific data at `Features/X/Data/`.
- Always use `[CreateAssetMenu(menuName = "GameplaySystems/X/YConfig")]` with an explicit, descriptive menu path.
- Name ScriptableObject classes with a `Config`, `Data`, or `Profile` suffix: `WeaponData`, `PlayerConfigSo`.

---

## Feature-to-Feature Communication

Features must be isolated. When two features need to talk:

| Method | When to use |
|---|---|
| Inject a `Shared/` interface via VContainer | Feature A needs a service Feature B provides → extract the interface to `Shared/` |
| `EventBus<T>` events declared in `Shared/Events/` | One-to-many notifications; also the spawn-safe path for runtime-instantiated objects |
| C# events on a shared service | Simple observer wiring between registered services |

**Never:** `using GameplaySystemsAndTools.Features.Inventory` inside `GameplaySystemsAndTools.Features.Player`. This creates hidden coupling and will eventually produce circular asmdef references.

---

## Dependency Injection (VContainer)

- `Core/AppRootLifetimeScope` — app-wide services (none yet; add here when a service must survive scene loads).
- `Core/GameplayLifetimeScope` — per-scene composition root for gameplay scenes. It:
  1. Registers scene-placed engine components (`RegisterComponentInHierarchy`) behind their interfaces (`ITimeOfDayService`, `IAudioService`, `IVfxService`, `INoiseService`, `IEnemyHudPool`, `PlayerInputHandler`).
  2. Injects scene-placed consumers via an explicit build callback (composition-root-only use of the resolver).
- Runtime-spawned objects (thrown rocks, dropped items) must NOT rely on injection — they communicate through `EventBus<T>` events instead.
- No `Instance` singletons. No `FindObjectOfType` in gameplay code.

---

## State Machines (Hierarchical FSM)

Base FSM engine lives in `Shared/Gameplay/StateMachine/`:
- `StateBase` / `StateMachineBase` — pure FSM plumbing (Enter/Tick/Exit, nested sub-states via `SwitchSubState`). Framework-agnostic: no movement or physics knowledge.

**Rule: an FSM graph belongs to the actor, not the mechanic.** A `PlayerStateMachine`/`EnemyStateMachine` and every root/sub state it switches between live together in that actor's own `Features/<Actor>/` folder. A state graph is one cohesive unit — splitting it across feature boundaries by mechanic (Movement/Climbing/Gathering as separate features) forces circular cross-feature references the moment one root state switches into another mechanic's state.

**Rule: a mechanic's reusable engine belongs in `Shared/Gameplay/`, not in a feature.** Anything the underlying mechanic needs that is actor-agnostic (ledge detection for climbing, resource nodes for gathering, `ForceReceiver` for gravity/knockback) goes in `Shared/Gameplay/<Mechanic>/` as plain code with zero knowledge of Player/Enemy. Each actor's state classes consume it.

**Per-actor state folder convention:**
```
Features/<Actor>/Logic/
├── <Actor>StateMachine.cs
├── <Actor>BaseState.cs
└── States/
    ├── RootStates/               ← mutually exclusive top-level states
    │   ├── PlayerGroundedState.cs
    │   ├── PlayerAirborneState.cs
    │   └── PlayerClimbState.cs
    └── PlayerFallingState.cs     ← a sub-state, reached via SwitchSubState() from a root state
```

---

## PC Platform Notes

- Input: all features read from Unity Input System `.inputactions` assets via `Shared/Input/PlayerInputHandler`.
- Saves: file-based slots under `Application.persistentDataPath`. Async only. No `PlayerPrefs` for game state.
- Graphics: quality/resolution/VSync are data-driven through the settings service. Feature code is read-only.
