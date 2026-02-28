# Context isolation

Saneject enforces *context isolation* by default during injection to prevent references that Unity cannot serialize safely and to keep prefabs reusable and deterministic.

Context isolation affects both dependency resolution and hierarchy traversal.

When enabled, scenes, prefab instances, and prefab assets are treated as separate, hard-isolated contexts.

## What counts as a context

A context is one of the following:

- A **scene**
- A **single prefab instance** in a scene. Each prefab instance is its own context, even if multiple instances come from the same prefab asset
- A **prefab asset** opened or injected directly from the Project window

ScriptableObjects and other project assets are not context-restricted.

## Rules when context isolation is enabled

### Dependency resolution

An injected dependency is only allowed if the injection target and the candidate belong to the same context.

- Scene objects only resolve dependencies from the same scene
- A prefab instance only resolves dependencies from within that same prefab instance
- A prefab asset only resolves dependencies from within its own asset root
- Prefab instances cannot resolve from other prefab instances, even if they share the same prefab source
- Scene objects cannot resolve from prefab instances or prefab assets
- ScriptableObjects and other assets can be resolved from any context

Rejected candidates are filtered before assignment and reported in a single, contextual error message.

### Hierarchy traversal during injection

Injection traversal is also context-aware.

- **Scene injection pass**
    - Traverses only scene objects
    - Prefab instances, prefab scopes, and prefab components are completely ignored
- **Prefab injection pass**
    - Traverses only the targeted prefab instance or prefab asset hierarchy
    - Scene objects and scene scopes are ignored
- Nested prefabs are treated as separate contexts
- Injecting one prefab instance never injects on its parents, siblings, or children, even if they are the same prefab

This applies equally to:

- Field injection
- Method injection
- Scope parent resolution
- Scope hierarchy traversal

## Turning it off

Context isolation is controlled in **User Settings** via **Use Context Isolation**.

When disabled:

- Scenes and prefab instances form a single unified hierarchy
- Injection traversal crosses scene and prefab boundaries
- Dependencies can resolve freely across contexts

This mode exists as an escape hatch but is unsafe for most workflows. Unity does not reliably serialize these links, and removing a prefab instance or scene will break dependencies.

Keeping context isolation enabled and structuring your dependencies around that is strongly recommended.

## Cross-context references

If a prefab needs to reference a scene object, or multiple scenes need to reference shared instances, use **ProxyObjects**.

Proxies are serializable assets that resolve their real target at runtime and are the supported way to bridge contexts without breaking isolation.

## Global bindings and context filtering

The same context rules also apply to global bindings.

When **Filter By Same Context** is enabled:

- Prefab components are automatically filtered out when resolving global bindings, so only scene objects in the same context can be promoted to the global scope
- This prevents accidentally registering prefab components that might be missing at runtime

Disabling the setting removes that safeguard and allows prefab components to be registered globally, which can break if the prefab is not present when the game runs.