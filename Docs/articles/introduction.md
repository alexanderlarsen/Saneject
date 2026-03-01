# Introduction

Saneject is a middle-ground between hand-wiring references and a full runtime DI container. It resolves dependencies in the Unity Editor using familiar DI syntax, writes the results directly into serialized fields — including interfaces — and keeps your classes free of `GetComponent`, static singletons, and manual look-ups. At runtime it's just regular serialized Unity objects. No reflection, no container, no startup hit.

## Why editor-time DI?

| Pain point | How Saneject helps |
|---|---|
| "We want structured dependency management but don't want a runtime container." | Saneject uses DI-style binding syntax without any runtime container. You keep editor-time determinism, Unity's default lifecycle, and full Inspector visibility. |
| "We want to see what's wired where in the Inspector." | All references are regular serialized fields. Nothing is hidden behind a runtime graph. |
| "Interfaces can't be dragged into the Inspector." | The Roslyn source generator adds hidden backing fields for `[SerializeInterface]` members, making interface fields show up and persist like any other serialized reference. |
| "Runtime DI lifecycles fight Unity's own Awake/Start order." | Everything is resolved and serialized in the Editor. Unity's normal lifecycle is untouched. |
| "Large reflection-heavy containers add startup cost." | Saneject resolves once in the Editor. Zero runtime reflection or container initialization. |
| "Can't serialize references between scenes or from a scene into prefabs." | `RuntimeProxy<T>`, a Roslyn-generated `ScriptableObject`, can be referenced anywhere like any project asset and resolves its target at runtime. |
| "Non-programmers struggle with code-only installers." | Bindings live in `Scope` scripts as plain declarative C#. Dependencies are visible serialized fields in the Inspector. |

Saneject is not meant to replace full runtime frameworks like Zenject or VContainer. It's an alternative for projects that value determinism, Inspector transparency, and minimal runtime overhead.

## Runtime DI vs Saneject

| | Runtime DI (Zenject, VContainer) | Saneject |
|---|---|---|
| Injection timing | Runtime (container initialization) | Editor-time (stored as serialized fields) |
| Lifecycle | Adds a second lifecycle on top of Unity's | Regular Unity lifecycle (Awake, Start, etc.) |
| Performance | Some startup cost (reflection, allocations) | Zero runtime reflection or container overhead |
| Inspector visibility | Limited — wiring is handled by the container | All dependencies visible as serialized fields |
| Flexibility | High — bindings can change at runtime | Fixed after injection |
| Testing / mocking | Strong — easy to substitute in unit tests | Less ergonomic — requires modifying serialized data |
| Plain C# classes | Full support — constructor injection, POCOs | No constructor injection; supports nested `[Serializable]` class injection |

## Features

### Injection & binding

- **Editor-time, deterministic injection** — bindings are resolved in the Editor and stored in serialized fields, including nested `[Serializable]` classes.
- **Fluent, scope-aware binding API** — traverse hierarchies or load project assets, filter results, bind by type or ID.
- **Component and asset bindings** — components traverse `Transform` hierarchies; asset bindings load from Resources, project folders, and more.
- **Collection binding support** — inject arrays or lists with full filter, scope, and ID support.
- **Method injection** — inject dependencies as method parameters, runs after field injection.
- **Context isolation** — scenes and prefabs are injected in isolation by default.
- **Flexible filtering** — predicate-based filtering on components, their `Transform`, `GameObject`, parent, ancestors, children, descendants, or siblings.
- **Target qualification** — narrow bindings by ID, target component type, or member name.
- **Unified `Scope` component** — one type handles both scene and prefab contexts with automatic context detection.
- **Batch injection** — inject all or selected scenes and prefabs in one pass from the Batch Injector window.

### Serialization & interfaces

- **Interface serialization with Roslyn** — `[SerializeInterface]` fields show up in the Inspector.
- **Serialized collections of interfaces** — arrays and lists of interfaces are fully supported.
- **Cross-context references** — `RuntimeProxy<T>` bridges scenes and prefabs while preserving Unity's serialization rules.
- **GlobalScope** — scene components can be promoted to runtime-accessible singletons at editor time.

### Performance & runtime

- **No runtime reflection** — everything is injected and serialized in the Editor.
- **Proxy resolution** — proxies resolve their target once and cache it.
- **Zero startup cost** — no container initialization, no additional lifecycles, no allocations on play.

### Editor UX & tooling

- **Comprehensive logging & validation** — all failures logged non-blocking in a single pass, with a summary at the end.
- **Native UI/UX** — polished inspectors, contextual menus, and intuitive behavior.
- **Inspector polish** — injected fields are grayed out, interface fields display their resolved type, nested serialized classes are drawn in order.
- **User Settings panel** — toggle field visibility, logging behavior, context filtering, and more.
- **Scope path display** — shows the parent scope chain in the Scope inspector.
- **Create Scope menu** — generates Scope boilerplate from a menu item.
