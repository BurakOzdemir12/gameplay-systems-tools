<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=unity,cs,dotnet,git,github,rider,blender" />
  </a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.3.13f1-black?logo=unity" />
  <img src="https://img.shields.io/badge/Render%20Pipeline-URP%2017-blue" />
  <img src="https://img.shields.io/badge/Architecture-VContainer%20%7C%20EventBus-purple" />
  <img src="https://img.shields.io/badge/Input-New%20Input%20System-green" />
  <img src="https://img.shields.io/badge/Platform-PC%20Windows%20x64-lightgrey" />
  <img src="https://img.shields.io/badge/Status-In%20Development-orange" />
</p>

<div align="center">

**Demo Video:**

[![Gameplay Systems & Tools - Demo](https://img.youtube.com/vi/SjGvMaqIAJg/0.jpg)](https://www.youtube.com/watch?v=Dh1aK_0vWTw)
</div>

## Table of Contents
- **[Introduction](#introduction)**
- **[Core Player Systems & Locomotion](#core-player-systems--locomotion)**
- **[Dynamic Parkour, Climbing & Vaulting](#dynamic-parkour-climbing--vaulting)**
- **[Action RPG Combat & Target Locking](#action-rpg-combat--target-locking)**
- **[Resource Gathering & Interaction](#resource-gathering--interaction)**
- **[Artificial Intelligence & Perception](#artificial-intelligence--perception)**
- **[Environment & World Simulation](#environment--world-simulation)**
- **[Inventory, Items & Health](#inventory-items--health)**
- **[Architecture & Design Patterns](#architecture--design-patterns)**
- **[Unity Features Used](#unity-features-used)**
- **[Tech Stack](#tech-stack)**
- **[Project Structure](#project-structure)**
- **[Roadmap](#roadmap)**

<section id="introduction">

<h1 align="center">Gameplay Systems & Tools — 3D Action RPG Framework</h1>

**Gameplay Systems & Tools** is a modular, production-ready gameplay architecture and sandbox built in **Unity 6 (6000.3.13f1)** with the **Universal Render Pipeline (URP 17)**, engineered specifically for PC (Windows x64 Standalone).

Designed as an extensible gameplay laboratory and core systems foundation, the project delivers interconnected, actor-agnostic mechanics commonly found in modern 3D Action RPGs and adventure titles: **hierarchical state machine locomotion, dynamic parkour and ledge climbing, deep combat with parry/block/combo chains, vision and acoustic AI perception, full 24-hour celestial time-of-day with dynamic weather & seasons, item inventory, and active physics ragdolls.**

The framework strictly adheres to clean architectural principles: **VContainer Dependency Injection**, zero-allocation **UniTask** asynchronous workflows, a type-safe static **Event Bus (`EventBus<T>`)**, and a **Feature + Layer Hybrid Architecture** with 100% data-driven `ScriptableObject` configurations.

</section>

<section id="core-player-systems--locomotion">

<h2>Core Player Systems & Locomotion</h2>

- **Hierarchical Finite State Machine (HFSM)**
  - Top-level root states (`PlayerGroundedState`, `PlayerAirborneState`, `PlayerClimbState`) cleanly isolate major locomotion contexts.
  - Sub-states (`PlayerFreeLookState`, `PlayerTargetingState`, `PlayerJumpingState`, `PlayerFallingState`, `PlayerLandingState`, `PlayerDodgeState`, `PlayerRollState`, `PlayerAttackingState`, `PlayerBlockingState`, `PlayerParryState`, `PlayerImpactState`, `PlayerStunnedState`, `PlayerGatheringState`, `PlayerHangingState`, `PlayerClimbUpState`, `PlayerParkourActionState`, `PlayerDeadState`) handle granular actions with seamless transitions.

- **Responsive Locomotion & Camera Integration**
  - **Free-Look Movement**: Camera-relative 3D movement with smooth acceleration, rotation damping, and velocity blending.
  - **Strafe / Lock-on Movement**: Precision directional movement centered around locked targets with Cinemachine integration.
  - **Dodge & Combat Roll**: Quick directional step-dodges and evasive combat rolls equipped with configurable invulnerability frames (i-frames).
  - **Dynamic Jump & Landing System**: Variable jump curves, air control, and data-driven landing responses (`LandingType`: Soft, Normal, Hard) based on fall velocity and duration.

- **Physics-Driven Handling & Ground Evaluation**
  - **`ForceReceiver`**: Manages external physical impacts, knockbacks, directional launch vectors, and custom gravity curves with smooth velocity decay.
  - **`GroundChecker`**: Multi-raycast ground evaluation system tracking surface normals, slope steepness, ground layers, and step offsets.

</section>

<section id="dynamic-parkour-climbing--vaulting">

<h2>Dynamic Parkour, Climbing & Vaulting</h2>

- **Actor-Agnostic Ledge & Wall Detection**
  - Modular `ClimbController`, `LedgeValidator`, and `ParkourObstacle` pipeline capable of analyzing arbitrary geometry in real-time.
  - Multi-raycast height checking, forward wall clearance evaluation, obstacle thickness calculation, and landing surface validation.

- **Dynamic Traversal Maneuvers (`ParkourDecision`)**
  - **Step-Up Climb**: Rapid low-obstacle mantling without interrupting forward momentum.
  - **Full-Body Ledge Climb**: High-wall climbing transitions with procedural root-motion matching.
  - **Ledge Hanging & Shimmying**: Grabbing ledges from air or drops, holding position, and executing climb-up or drop-down maneuvers.
  - **Normal & Center Vaulting**: Vaulting over fences, low walls, and narrow obstacles with obstacle thickness checks (`CenterVaultSo`, `NormalVaultDataSo`).

</section>

<section id="action-rpg-combat--target-locking">

<h2>Action RPG Combat & Target Locking</h2>

- **Modular Weapon & Combo System**
  - Data-driven attack chains via `AttackDataSO` (One-Handed, Two-Handed Heavy, etc.) defining timing windows, damage multipliers, knockback impulses, and stamina costs.
  - Precise hit detection through weapon hitboxes and dynamic trail visualizers (`TrailSystem`).
  - Seamless animation cancels and combo branch windows.

- **Defensive & Counter Mechanics**
  - **Active Blocking**: Directional shield blocking (`ShieldHandler`, `ShieldLogic`) mitigating incoming damage based on shield attributes (`ShieldDataSo`).
  - **Precision Parry System**: Timed parry window (`PlayerParryState`) deflecting enemy attacks, staggering attackers into a vulnerable stunned state.
  - **Hit Reactions & Stun**: Layered impact states (`PlayerImpactState`, `PlayerStunnedState`) responsive to attack weight and poise break.

- **Target-Locking System**
  - Cinemachine-powered lock-on targeter (`Targeter`, `Target`) with radial target search, obstacle occlusion checks, smooth target switching, and dynamic HUD reticles.

- **Surface-Driven Impact & Combat Feedback**
  - Dynamic surface evaluation (`SurfaceDefinition`, `SurfaceDetection`) detecting material types (Flesh, Stone, Wood, Metal, Dirt).
  - Surface-specific impact VFX (sparks, blood splatters, dust) dispatched via `VfxService`.
  - Directional camera shakes (`CineMachineShake`), hit-stop micro-pauses, and spatial audio feedback profiles.

</section>

<section id="resource-gathering--interaction">

<h2>Resource Gathering & Interaction</h2>

- **Tool & Harvesting Mechanics**
  - Modular tool handling (`ToolHandler`, `ToolLogic`, `ToolDataSo`) supporting pickaxes, axes, and specialized harvesting equipment.
  - Gathering state machine integration (`PlayerGatheringState`, `GatheringController`, `GatheringValidator`).

- **Interactive Resource Nodes (`ResourceNode`, `IGatherable`)**
  - Dynamic resource node durability, multi-hit harvesting loops, and physical hit wobble feedback.
  - Procedural loot spawning and direct routing into the player inventory.

</section>

<section id="artificial-intelligence--perception">

<h2>Artificial Intelligence & Perception</h2>

- **Hierarchical Enemy AI State Machine**
  - Multi-state behavioral graph: `EnemyIdleState`, `EnemyPatrolState`, `EnemySuspiciousState`, `EnemyChaseState`, `EnemyAttackingState`, `EnemyAttackCooldownState`, `EnemyParryState`, `EnemyImpactState`, `EnemyStunnedState`, and `EnemyDeadState`.

- **Multi-Sensory Perception Engine**
  - **Visual Perception (`FieldOfView`)**: Conical vision cones with configurable sight radius, peripheral angles, and line-of-sight raycast occlusion.
  - **Acoustic Perception (Hearing System)**:
    - Global `NoiseService` routing noise events (`NoiseEmittedEvent`).
    - Characters emit acoustic signatures on footstep sprinting, weapon swings, tool impacts, or throwing items (`NoiseEmitter`, `Throwable`).
    - `NoiseSensor` on enemies receives acoustic data, triggering investigation (`EnemySuspiciousState`) at the noise location.

- **Tactical Combat Brain & Defense**
  - Tactical decision making (`EnemyAIBrainDataSo`, `EnemyCombatDataSo`) managing attack cooldowns, strafe distances, combo selection, and defensive parry reactions (`EnemyDefenceBrain`).

- **World-Space Pooled Enemy HUD**
  - Overhead UI indicators (`EnemyHUDPool`, `EnemyHUDView`, `EnemyUIController`) displaying dynamic health bars, stun gauges, and alert status indicators.

</section>

<section id="environment--world-simulation">

<h2>Environment & World Simulation</h2>

- **24-Hour Celestial Time-of-Day System**
  - Continuous day/night simulation (`TimeOfDayController`, `TimeService`, `ITimeOfDayService`) with customizable time scale and division of day markers (Dawn, Morning, Noon, Afternoon, Dusk, Night).
  - Synchronized celestial rotation of the sun and moon, ambient light gradients, and dynamic shadow color shifts.
  - Real-time time broadcast events (`TimeChangedEvent`, `DayChangedEvent`) for world consumers.

- **Procedural Weather System**
  - Dynamic weather states (`WeatherController`, `WeatherFXController`, `WeathersConfigSo`): Clear, Cloudy, Rain, Storm, Snow, Fog.
  - Smooth particle transitions, volumetric fog shifts, and synchronized environmental audio blending (`EnvironmentalAudioProfile`).

- **Dynamic Seasonal Cycle**
  - Four-season progression (`SeasonController`, `SeasonConfigSo`): Spring, Summer, Autumn, Winter.
  - Triggers seasonal environment updates and audio profile swaps via `SeasonChangedEvent`.

- **Synchronized Atmospheric Skybox**
  - Procedural skybox controller (`SkyboxController`) coordinating horizon gradients, atmosphere colors, sun disk intensity, and starfield density with the time and weather pipeline.

</section>

<section id="inventory-items--health">

<h2>Inventory, Items & Health</h2>

- **Grid Inventory & Hotbar System**
  - Data-driven item definitions (`ItemData`, `IPickupable`) with item types, icon assets, stack limits, and equipment data.
  - Slot-based inventory model (`InventoryComponent`, `InventorySlot`) paired with hotbar selection (`HotbarController`) and interactive UI (`InventoryUIController`, `SlotUI`).
  - World item drop, physical throwing (`Throwable`), and proximity pickup detection (`PickupController`).

- **Health, Damage Pipeline & Active Ragdoll**
  - Robust health and damage architecture (`HealthBase`, `HurtboxBase`, `IDamageable`, `IStunnable`).
  - Comprehensive `DamageInfo` payload carrying damage amount, damage type, poise break, critical flags, and hit normal vectors.
  - Seamless, zero-stutter transition into active physics `Ragdoll` upon character defeat.

</section>

<section id="architecture--design-patterns">

<h2>Architecture & Design Patterns</h2>

- **Dependency Injection (VContainer)** — Per-scene composition root (`Core/GameplayLifetimeScope`) cleanly registers scene services (`ITimeOfDayService`, `IAudioService`, `IVfxService`, `INoiseService`, `IEnemyHudPool`, `PlayerInputHandler`) behind clean interfaces and injects scene consumers without static singletons or runtime `FindObjectOfType`.

- **Type-Safe Static Event Bus (`EventBus<T>`)** — Ultra-performant, decoupled cross-feature communication channel. Events (`CharacterCombatActionEvent`, `DayChangedEvent`, `NoiseEmittedEvent`, `WeatherChangedEvent`, `SoundPlayRequestedEvent`, `VfxPlayRequestedEvent`, etc.) utilize lifecycle-safe `EventBinding<T>` to prevent memory leaks across scene lifecycles.

- **Hierarchical Finite State Machine (HFSM)** — Strict separation between actor-owned state graphs (`Features/Player/`, `Features/Enemy/`) and reusable mechanic engines (`Shared/Gameplay/`). Root states dictate overarching movement modes while sub-states manage granular actions.

- **ScriptableObject-Driven Configuration** — 100% of gameplay constants, attack animations, jump physics, AI profiles, weather presets, surface definitions, and audio datasets reside in `ScriptableObject` assets, enabling rapid tuning without recompilation.

- **Surface Detection & Feedback Decoupling** — Surface detection converts physical raycast materials into typed `SurfaceType` data, automatically routing matching particle effects and footstep/impact audio.

- **Object Pooling** — Built-in pooling infrastructure for 3D spatial audio emitters (`SoundEmitter`) and overhead enemy HUD elements (`EnemyHUDPool`).

</section>

<section id="unity-features-used">

<h2>Unity Features Used</h2>

- **Universal Render Pipeline (URP 17)** with optimized forward rendering, custom shader graphs, and atmospheric post-processing.
- **Cinemachine 3** — orbital third-person follow cameras, dynamic combat lock-on tracking, and impulse-based camera shakes.
- **New Unity Input System** — action-based, fully rebindable input handling supporting keyboard, mouse, and gamepads.
- **Unity Physics & Character Controller** — custom slope handling, multi-raycast surface probing, and active physics ragdoll integration.
- **VContainer** — high-performance, lightweight pure C# dependency injection container for Unity.
- **Cysharp UniTask** — zero-allocation asynchronous programming for lifecycle operations and event sequencing.
- **Custom Editor Tooling** — automated namespace stamping (`AutoNamespaceProcessor`), custom inspectors, and asset creation menus.

</section>

<section id="tech-stack">

<h2>Tech Stack</h2>

**Engine & Language**
- Unity 6 (6000.3.13f1)
- C# / .NET Standard 2.1
- Universal Render Pipeline (URP 17)

**Core Packages & Libraries**
- VContainer (Dependency Injection)
- Cysharp UniTask (Async / Await)
- Cinemachine 3 (Camera System)
- Input System (Unity New Input)
- AI Navigation (NavMesh & Pathfinding)
- ProBuilder & UModeler X (In-engine level greyboxing)
- TextMesh Pro & uGUI (User Interface)

**Persistence & Architecture**
- ScriptableObject Data Containers
- Typed Static `EventBus<T>`

</section>

<section id="project-structure">

<h2>Project Structure</h2>

```
Assets/_Project/
├── Core/                              # Application composition root & LifetimeScopes
│   ├── AppRootLifetimeScope.cs
│   └── GameplayLifetimeScope.cs
├── Features/                          # Domain-isolated feature modules
│   ├── Player/                        # Player HFSM, states, components, and configs
│   │   ├── Logic/                     # PlayerStateMachine & root/sub states
│   │   ├── Components/                # Attack signals, health, hurtbox
│   │   └── Data/                      # Jump, landing, and player movement configs
│   ├── Enemy/                         # Enemy AI HFSM, perception, combat brain, HUD
│   │   ├── Logic/                     # EnemyStateMachine & state implementations
│   │   ├── Components/                # Perception controller, defense brain, health
│   │   ├── View/                      # World-space pooled Enemy HUD
│   │   └── Data/                      # AI brain, combat, and movement configs
│   ├── Environment/                   # Grouped environment simulation features
│   │   ├── TimeOfDay/                 # 24h cycle, TimeService, time display UI
│   │   ├── Weather/                   # Weather controllers, configs, and FX
│   │   ├── Season/                    # Seasonal progression system & configs
│   │   └── Skybox/                    # Procedural skybox sync controller
│   └── Inventory/                     # Inventory logic, slots, UI, and pickups
│       ├── Logic/                     # InventorySlot model
│       ├── Components/                # InventoryComponent & PickupController
│       └── View/                      # InventoryUI, Hotbar, and SlotUI
├── Shared/                            # Reusable, actor-agnostic gameplay engines
│   ├── Events/                        # EventBus<T> infrastructure & event types
│   ├── Gameplay/                      # Core gameplay engines
│   │   ├── StateMachine/              # StateBase & StateMachineBase plumbing
│   │   ├── Climbing/                  # LedgeValidator, ClimbController, Parkour
│   │   ├── Combat/                    # Weapons, Shields, Tools, Targeting
│   │   ├── Gathering/                 # ResourceNode, GatheringController
│   │   ├── Health/                    # HealthBase, HurtboxBase, Ragdoll, DamageInfo
│   │   ├── Perception/                # FieldOfView, NoiseService, NoiseEmitter
│   │   ├── Feedback/                  # Surface feedback & impact profiles
│   │   └── Surfaces/                  # SurfaceDefinition & SurfaceDetection
│   ├── Audio/                         # AudioService, SoundEmitter pool, profiles
│   ├── Input/                         # PlayerInputHandler & InputActions
│   └── Visuals/                       # CineMachineShake & VfxService
├── Data/                              # Global ScriptableObject asset instances
└── Prefabs/                           # Assembled gameplay & environment prefabs
```

</section>

<section id="roadmap">

<h2>Roadmap</h2>

This project is under active development. Planned milestones and expansions:

- **Expanded Combat Archetypes** — Ranged weapon handling (Bows, Crossbows) with trajectory prediction, projectile physics, and drawing mechanics.
- **Extended AI Behaviors** — Group coordination, tactical surrounding/flanking algorithms, and patrol route waypoints.
- **Enhanced Parkour System** — Wall-running, corner-turning while hanging, and dynamic sliding under low obstacles.
- **Save & Persistence System** — Async file-based slot persistence for inventory, player position, world time, and environment state.
- **Quest & Dialogue Framework** — Modular node-based dialogue runner and event-driven quest tracking integration.

</section>

