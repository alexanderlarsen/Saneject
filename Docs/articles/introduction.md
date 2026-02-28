# Introduction



```csharp
using Plugins.Saneject.Experimental.Runtime.Attributes;
using UnityEngine;

namespace Dev.Experimental.Scenarios.Demo
{
    public partial class GameManager : MonoBehaviour
    {
        [Inject, SerializeInterface]
        private IPlayer player;
    }
}
```


## What is this?

Saneject is a middle-ground between hand-wiring references and a full runtime DI container. It resolves dependencies in the Unity Editor using familiar DI syntax and workflows, writes them straight into serialized fields (including interfaces), and keeps your classes free of `GetComponent`, static singletons, manual look-ups, etc. At runtime it's just regular, serialized Unity objects. No reflection, no container, no startup hit.

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

## Features

### Injection & binding

- **Editor-time, deterministic injection:** Bindings are resolved in the editor, stored directly in serialized fields, including nested serialized classes.
- **Fluent, scope-aware binding API:** Search hierarchy or project, filter by tag/layer/name, bind by type or ID.
- **Bind Components and Assets:** Use familiar APIs with context-aware methods. Component methods traverse GameObject hierarchies; asset bindings can load from Resources, folders, and more.
- **Collection binding support:** Inject arrays or lists with full support for filters, scoping, and binding IDs.
- **Method injection:** Inject dependencies as method parameters. Supports single values, arrays, and lists, and works alongside field injection, including inside nested serialized classes.
- **Context isolation:** Prefabs and scenes are injected in isolation. A prefab inside a scene will not be used to resolve scene dependencies and vice versa.
- **Flexible filtering:** Predicate-based filtering system for both components and assets. Compact, composable, and extensible with user-defined helper methods.
- **Target qualification:** Inject by ID, target type, or member name. Target bindings can specify which classes and members to apply to, working on fields, properties, and methods.
- **Unified Scope component:** One `Scope` type handles both scenes and prefabs, with automatic context detection.
- **Batch injection:** Editor window to inject all or selected scenes and prefabs in the project with one click, with detailed logs per item and cumulative summary stats at the end.
