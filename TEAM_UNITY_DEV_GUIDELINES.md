# Unity Dev Coding Standard

Owner: Lead Unity Team
Scope: All gameplay/client code
Platform: **PC — Windows x64 Standalone**

---

## Goals
- Keep code simple, modular, and maintainable.
- Optimize for CPU, GPU, GC, and memory — PC hardware tiers range widely; profile on the lowest target spec.
- Use `VContainer` for DI and modular wiring.
- Use `UniTask` for async flow and cancellation safety.

---

## Architecture Rules
- Build by feature modules, not giant managers.
- One responsibility per class.
- Feature internals are private; expose only clear interfaces.
- No hidden dependencies; no static coupling in runtime logic (the typed `EventBus<T>` in `Shared/Events/` is the one sanctioned static channel).
- See `Docs/PROJECT_ARCHITECTURE.md` for folder rules, namespace rules, and feature isolation.
- See `Docs/NAMING_CONVENTIONS.md` for all naming rules.

---

## Assembly Definition Rules
- Start with the fewest assemblies possible. The default state is everything in `Game.Runtime`.
- **Do not create a feature `.asmdef` preemptively.** Create one only when the feature is large, stable, and either (a) needs its own test assembly or (b) is a package extraction candidate.
- Keep references minimal and intentional — only reference what you directly use.
- Separate runtime and test assemblies always.
- No circular references. A circular reference means the design boundary is wrong.
- Never move files via OS file explorer or terminal — always use the **Unity Editor Project window**. Unity must regenerate `.meta` GUIDs on moves. (AI agents working from the shell must always move the `.meta` file together with the asset.)
- Do not leave production scripts in `Assembly-CSharp`.

---

## VContainer Rules
- Use constructor injection for all required dependencies.
- Register the abstraction (`IService`) and resolve the concrete when practical.
- `AppRootLifetimeScope` registers app-wide singletons (live for the full session).
- Scene `LifetimeScope` (e.g. `GameplayLifetimeScope`) registers scene-local dependencies (destroyed when the scene unloads).
- No `LifetimeScope.Find`, no `Container.Resolve` in gameplay code — that is service-locator pattern.
- `IStartable` / `IInitializable` / `ITickable` are the VContainer lifecycle hooks — prefer these over MonoBehaviour callbacks in pure logic classes.
- Scene-placed MonoBehaviours that need dependencies get them via `[Inject]` method injection, wired by the scene scope (`RegisterComponentInHierarchy` or an explicit build callback in the composition root).

---

## Feature Communication Rules
- Features must not reference each other's namespaces directly.
- Communication paths (choose the simplest that fits):
  1. Inject a shared `Shared/` interface via VContainer (preferred for direct calls).
  2. `EventBus<T>` events declared in `Shared/Events/` (one-to-many notifications, spawn-safe).
  3. C# events or delegates on a shared service.
- If two features keep needing to talk, ask whether they should be one feature or whether the shared concept belongs in `Shared/`.

---

## UniTask Rules
- Prefer `UniTask` over `Task` in all Unity runtime code.
- Always pass `CancellationToken` through async chains.
- Bind cancellation to object lifetime (`destroyCancellationToken` on MonoBehaviours).
- No fire-and-forget unless the failure path is explicitly safe and logged.
- Use `UniTaskVoid` only for event-handler entry points.

---

## Input System Rules (PC)
- **Unity Input System (new package) only.** No `Input.GetKey`, no `UnityEngine.Input`.
- Define all actions in `.inputactions` assets; reference via generated C# wrappers.
- All player-facing bindings must support runtime rebinding; persist overrides through the settings service.
- Support at minimum: keyboard + mouse and XInput gamepad.
- Debug-only shortcut keys are allowed exclusively inside `#if UNITY_EDITOR || DEVELOPMENT_BUILD` guards.

---

## ScriptableObject Rules
- ScriptableObjects are **static data only** — configs, definitions, tunable parameters.
- Never store runtime state in a ScriptableObject (corrupts play-mode/edit-mode boundary; breaks save systems).
- Always add `[CreateAssetMenu(menuName = "GameplaySystems/Category/Name")]`.
- Name classes with `Config`, `Data`, or `Profile` suffix: `WeaponData`, `PlayerConfigSo`.
- Location: feature-specific → `Features/X/Data/`. Global → `Assets/_Project/Data/`.

---

## Logging Rules
- No `Debug.Log` / `Debug.LogWarning` / `Debug.LogError` in production code paths.
- Wrap all logs in `#if UNITY_EDITOR || DEVELOPMENT_BUILD` guards, or use a logging abstraction that can be disabled.
- Exception: `Debug.LogException` is always allowed — exceptions must surface.
- Log levels: errors for broken invariants, warnings for recoverable unexpected states, verbose for dev-only traces.

---

## No Magic Values Rule
- No hardcoded numeric or string literals in logic code.
- Use: named constants in a static class (`SceneNames`, `LayerMasks`, `AnimatorParams`), `ScriptableObject` fields, or serialized `[SerializeField]` fields.
- Scene names go in `SceneNames` constants. Layer indices go in `LayerMasks`. Animator string hashes go in `AnimatorParams`.

---

## Graphics & Display Rules (PC)
- Quality presets (Low / Medium / High / Ultra) are the settings service's responsibility. Game code reads; never sets raw quality values.
- Resolution and window mode configurable via settings UI using `Screen.SetResolution` + `FullScreenMode`.
- VSync and target frame rate are user settings, not hardcoded. Default: VSync on.
- Per-preset visual effect cost must be documented when adding a new effect.

---

## Save & Persistence Rules (PC)
- File-based save slots (JSON or binary) under `Application.persistentDataPath`. One file per slot.
- `PlayerPrefs` only for display preferences that must survive before the save service initializes.
- Multiple save slots; never silently overwrite — require explicit user confirmation.
- All save/load I/O is `async` (UniTask) and cancellation-safe. No synchronous file I/O on the main thread.

---

## Testing Rules
- Test location: `Assets/_Project/Tests/Runtime/` and `Assets/_Project/Tests/Editor/`.
- Test assembly references only the assembly under test — no broad `Game.Runtime` reference from a feature test.
- Unit tests: pure C# logic, no MonoBehaviour, no scene required.
- Play-mode tests: for systems that require Unity lifecycle (physics, coroutines, scene loading).
- Naming: `[ClassUnderTest]Tests.cs`, method: `[MethodName]_[Scenario]_[ExpectedResult]`.
- Add tests for every non-trivial public method and every async edge case.

---

## Dev Performance Rules
- No avoidable allocations in hot paths.
- No LINQ or string-heavy operations in per-frame or frequent loops.
- Cache component references and repeated lookups.
- Prefer event-driven updates over constant polling.
- Pool frequently spawned/despawned objects.
- Profile on the **lowest target hardware spec**, not the dev machine.
- Use Unity Profiler and Frame Debugger before and after significant rendering changes.

---

## Code Quality Rules
- One public type per file.
- Short, predictable methods.
- Explicit names: `XService`, `XSystem`, `XPresenter`, `XView`. No `Manager`, `Helper`, `Util`.
- Comments explain *why*, not *what*. No commented-out code committed.
- No `TODO` in committed code — open a ticket instead.

---

## AI Prompt Template (Team Standard)

```text
Generate minimal production-ready Unity C# code for PC (Windows x64).
Requirements:
- Use VContainer constructor injection. No service-locator, no static singletons.
- Use UniTask for async methods and propagate CancellationToken.
- Use Unity Input System (new); no legacy Input API. All bindings via InputAction assets.
- Namespace must match folder path: GameplaySystemsAndTools.{Core|Shared|Features.X}.
- Respect feature isolation: no cross-feature namespace references.
- No magic numbers or strings — use named constants or ScriptableObject fields.
- No Debug.Log in production paths — guard with UNITY_EDITOR || DEVELOPMENT_BUILD.
- No ScriptableObjects holding runtime state.
- Avoid per-frame allocations and LINQ/string churn in hot paths.
- Do not add unnecessary abstractions, wrappers, or boilerplate.
- Output only required files/classes plus a short rationale.
```

---

## Required Final Validation (Before PR / AI Final Output)
- [ ] Namespace matches folder path exactly.
- [ ] Feature does not reference another feature's namespace.
- [ ] Architecture boundary check passed (asmdef references, no circular dependency).
- [ ] VContainer check passed (constructor injection, no service locator, correct LifetimeScope).
- [ ] Async safety check passed (UniTask, cancellation propagation, no unsafe fire-and-forget).
- [ ] Input System check passed (new Input System only, debug keys guarded).
- [ ] No magic values (constants class or ScriptableObject fields used).
- [ ] No Debug.Log in production paths.
- [ ] No runtime state in ScriptableObjects.
- [ ] Performance check passed (no avoidable allocations in hot paths).
- [ ] Simplicity check passed (no overengineered abstractions, small APIs).

---

## PR Checklist
- [ ] Feature is modular; assembly boundaries respected.
- [ ] Namespace matches folder path.
- [ ] No cross-feature namespace imports.
- [ ] VContainer wiring is correct (right scope, constructor injection).
- [ ] UniTask + cancellation-safe async flow.
- [ ] Input uses new Input System.
- [ ] No magic numbers/strings.
- [ ] Logging guards in place.
- [ ] Tests added/updated.
- [ ] Performance reviewed (profiled on low-spec if rendering changed).
