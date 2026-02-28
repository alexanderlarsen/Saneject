# Introduction

> 🚀 **Quick start**  
> Install, learn the basics, and inject your first scene in 5 minutes → [Jump to quick start](quick-start.html)

## What is this?

Saneject is a middle-ground between hand-wiring references and a full runtime DI container. It resolves dependencies in the Unity Editor using familiar DI syntax and workflows, writes them straight into serialized fields (including interfaces), and keeps your classes free of `GetComponent`, static singletons, manual look-ups, etc. At runtime it's just regular, serialized Unity objects. No reflection, no container, no startup hit.

## What is dependency injection?

Dependency injection (DI) is a design pattern where objects receive their dependencies from an external source, rather than creating or locating them themselves. Instead of calling `new()`, or searching the scene themselves (`GetComponent`, `FindFirstObjectOfType`, etc.), an external system supplies the needed objects.

## Why another DI tool?

| Pain point                                                                                         | How Saneject helps                                                                                                                                                                              |
|----------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| "We want structured dependency management but don't want to commit to a full runtime DI workflow." | Saneject offers DI-style binding syntax and organisation without a runtime container. You keep editor-time determinism, default Unity lifecycle and Inspector visibility.                       |
| "We want to see what's wired where in the Inspector."                                              | All references are regular serialized fields. Nothing is hidden behind a runtime graph.                                                                                                         |
| "Interfaces can't be dragged into the Inspector."                                                  | Saneject's Roslyn generator adds safe interface-backing fields with Inspector support. `[SerializeInterface] IMyInterface myInterface` shows up as a proper serialized field.                   |
| "Runtime DI lifecycles can feel opaque or fight Unity's own Awake/Start order."                    | Everything is set and serialized in the editor. Unity's normal lifecycle stays untouched.                                                                                                       |
| "Large reflection-heavy containers add startup cost."                                              | Saneject resolves once in the editor. Zero runtime reflection or allocation at runtime.                                                                                                         |
| "Can't serialize references between scenes or from a scene into prefabs."                          | `ProxyObject`, a Roslyn generated `ScriptableObject`, can be referenced anywhere like any asset. At runtime, it resolves and forwards all calls to a real scene instance with minimal overhead. |
| "Non-dev team members struggle with code-only installers and like visible dependencies."           | Bindings live in `Scope` scripts as simple, declarative C#. Fields are regular serialized fields marked with `[Inject]`, and field visibility can be toggled from settings.                     |

Saneject isn't meant to replace full runtime frameworks like Zenject or VContainer. It's an alternative workflow for projects that value determinism, Inspector visibility, and minimal runtime overhead.

## How runtime DI typically works

In most DI frameworks like Zenject or VContainer, you declare bindings in code or installers. At runtime, a container resolves and injects all dependencies before or during startup. This provides flexibility, supports unit testing, and allows highly dynamic setups. However, it introduces complexity, runtime overhead (reflection, allocations, container resolving), a second lifecycle on top of Unity's (`Awake`, `Start`, `OnEnable`, etc.) and the wiring can feel less transparent to non-programmers,
since it's not always visible in the Inspector.

## How Saneject DI works

Saneject flips the model: you still declare bindings in code (via a `Scope`), but all bindings are resolved in the Unity Editor. The results are written directly to serialized fields. There's no container or injection step at runtime. At startup, Unity loads the scene with references already assigned. This has both benefits and trade-offs compared to runtime DI, outlined below.

## Runtime DI vs Saneject

| Approach             | Runtime DI (Zenject, etc.)                               | Saneject                                                                                         |
|----------------------|----------------------------------------------------------|--------------------------------------------------------------------------------------------------|
| Injection timing     | Runtime (container initialization)                       | Editor-time (stored as serialized fields)                                                        |
| Lifecycle            | More complex - adds a second lifecycle on top of Unity's | Regular Unity lifecycle (Awake, Start, etc.)                                                     |
| Performance          | Some startup cost (reflection, allocations)              | Zero reflection or container at runtime (small runtime lookup if using global dependencies)      |
| Inspector visibility | Limited - container handles wiring                       | All dependencies are visible in the Inspector                                                    |
| Flexibility          | High - bindings can change at runtime                    | Lower - wiring is fixed after injection                                                          |
| Testing / mocking    | Strong - easy to substitute in unit tests                | Less ergonomic - requires setting serialized data                                                |
| Visual debuggability | Can be opaque - dynamic graph                            | Fully deterministic - just look at the fields for missing dependencies                           |
| Plain C# classes     | Full support - constructor injection, POCO creation      | No constructor injection and POCO creation. Does support injection into serialized nested POCOs. |