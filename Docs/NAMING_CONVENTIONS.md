# Naming Conventions

Consistency removes cognitive load. Every name in this project follows these rules.

---

## C# Files and Types

| Thing | Pattern | Example |
|---|---|---|
| Class file | `PascalCase`, matches class name | `TimeService.cs` |
| Interface | `I` prefix + PascalCase | `IAudioService` |
| MonoBehaviour (view/presenter) | Suffix = responsibility | `EnemyHUDView`, `SlotUI` |
| Pure service / system | Suffix `Service` or `System` | `TimeService`, `NoiseService` |
| ScriptableObject | Suffix `Config`, `Data`, `So`, or `Profile` | `WeaponData`, `PlayerConfigSo` |
| Abstract base class | Prefix `Base` or suffix the concept | `StateBase`, `StateMachineBase`, `HealthBase` |
| Enum | PascalCase type, PascalCase values | `SeasonType { Spring, Summer, Autumn, Winter }` |
| Constants class | Static class, PascalCase members | `SceneNames.Boot`, `AnimatorParams.Mirror` |
| Private field | `camelCase` (project style; keep consistent per file) | `timeService` |
| Property | PascalCase | `CurrentTime` |
| Event | PascalCase, past-tense or noun phrase | `OnPlayerDied`, `HealthChanged` |
| Async method | Suffix `Async` | `LoadSaveSlotAsync` |

---

## Assets

| Asset type | Pattern | Example |
|---|---|---|
| Prefab | `PascalCase`, category prefix optional | `Player_Main.prefab`, `UI_HUD.prefab` |
| Scene | PascalCase, category prefix | `World_Sandbox.unity`, `Menu_Main.unity` |
| ScriptableObject instance | PascalCase, descriptive | `IronSword_Data.asset`, `DefaultPlayerConfig.asset` |
| Material | PascalCase | `SnowGround_Mat.mat` |
| Texture | PascalCase, suffix with type | `SnowGround_Albedo.png`, `SnowGround_Normal.png` |
| Shader | PascalCase | `SnowBlend.shader` |
| Animation Clip | PascalCase, verb-based | `Player_Walk.anim`, `Player_Jump.anim` |
| Animator Controller | PascalCase | `Player_AC.controller` |
| Input Actions asset | PascalCase, `Actions` suffix | `PlayerInputActions.inputactions` |
| Audio Clip | PascalCase, category prefix | `SFX_FootstepSnow.wav`, `Music_MainTheme.mp3` |
| Asmdef | Matches logical module name | `Game.Runtime.asmdef`, `Game.Editor.asmdef` |

---

## Folders

- Always PascalCase: `Features/`, `Player/`, `Logic/`, `Components/`, `View/`, `Data/`
- Feature sub-folders: `Logic/`, `Components/`, `View/`, `Data/`, `Editor/` (consistent across all features)
- `View/` is UI-only (HUD, popups, presenters). Non-UI MonoBehaviours go in `Components/`.
- Do not use spaces, underscores, `&`, or lowercase in folder names

---

## Scenes (expanded)

Use a category prefix that matches the scope:

| Prefix | Scope | Examples |
|---|---|---|
| *(none)* | Bootstrap | `Boot.unity` |
| `Menu_` | Menu screens | `Menu_Main.unity`, `Menu_Settings.unity` |
| `World_` | Gameplay worlds | `World_Sandbox.unity` |
| `Test_` | Dev/QA scenes, excluded from build | `Test_Shaders.unity` |

Existing scenes (`PlayerSystems`, `MainScene`, `ShaderTestScene`) predate this rule;
rename them from inside the Unity Editor when convenient.

---

## VContainer / DI Naming

- Register the **interface**, not the concrete type, whenever an interface exists.
- `LifetimeScope` subclasses: `AppRootLifetimeScope`, `GameplayLifetimeScope`.
- `IStartable` entry-point classes: suffix `Bootstrapper` or `Initializer` — `GameBootstrapper`, `AudioInitializer`.

---

## What to Avoid

- No abbreviations unless universally known (`UI`, `HUD`, `FPS`, `NPC`).
- No generic names: `Manager`, `Helper`, `Util`, `Data` as a standalone class name.
- No numbered suffixes: `PlayerController2`, `NewSaveSystem`.
- No `Temp`, `Test`, `Old`, `New` prefixes in committed code.
