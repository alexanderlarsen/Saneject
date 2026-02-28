# Feature overview

## Injection & binding

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

## Serialization & interfaces

- **Interface serialization with Roslyn:** `[SerializeInterface]` fields show up in the Inspector.
- **Serialized collections of interfaces:** Arrays and lists of interfaces are fully supported, injectable, and visible in the Inspector.
- **No more interface class wrappers:** Write plain `[SerializeInterface] IMyInterface foo` instead of wrapping or unwrapping values.
- **Cross-scene and prefab references:** Proxies allow serialized references between scenes and prefabs while preserving Unity's isolation rules.
- **Global Scope:** Scene dependencies can be promoted to global singletons at editor time and resolved statically at runtime.

## Performance & runtime

- **No runtime reflection:** Everything is injected and serialized in the editor. At runtime, it's just Unity's own data.
- **Proxy resolution:** Proxies resolve their targets once, then cache them for minimal overhead.
- **Zero startup cost:** No container initialization, no additional lifecycles, no reflection, or allocations during play.

## Editor UX & tooling

- **Comprehensive logging & validation:** Every point of failure is logged non-blocking in one pass, including missing, conflicting, or invalid bindings and missing dependencies. Logs both individual messages and total summary stats after each injection.
- **Native UI/UX:** Designed to feel at home in Unity with polished inspectors, contextual menus, and intuitive behavior.
- **User-friendly tooling:** One-click injection for scenes or prefabs, automatic proxy generation, and scope-aware inspector actions.
- **Inspector polish:** Injected fields are clearly marked or grayed out, interfaces display implemented types, and nested serialized classes are drawn in order.
- **User Settings panel:** Toggle injected field visibility, logging behavior, context filtering, and more, directly from the editor.
- **Show Scope path:** Displays the parent scope chain in the Scope inspector for quick navigation and debugging.
- **Create Scope menu:** Menu item that generates Scope boilerplate code.