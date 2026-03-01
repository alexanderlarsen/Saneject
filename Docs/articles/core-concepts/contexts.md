# Contexts

Saneject enforces *context isolation* by default to prevent serialization issues and keep prefabs deterministic and reusable.

Context isolation affects two things: which candidates are eligible to resolve a dependency, and which transforms are traversed during an injection pass.

## What counts as a context

- A **scene**
- A **prefab instance** in a scene (each instance is its own context, even when multiple instances share the same source prefab)
- A **prefab asset** opened or injected directly from the Project window

`ScriptableObject`s and other project assets are not context-restricted and can be resolved from any context.

## Rules when context isolation is enabled

### Dependency resolution

An injected dependency is only eligible if the injection target and the candidate belong to the same context:

- Scene objects only resolve dependencies from the same scene.
- A prefab instance only resolves dependencies from within that same prefab instance.
- A prefab asset only resolves dependencies from within its own asset root.
- Prefab instances cannot resolve from other prefab instances, even if they share the same source prefab.
- Scene objects cannot resolve from prefab instances or prefab assets.
- `ScriptableObject`s and other project assets can be resolved from any context.

Candidates that fail the context check are filtered out before assignment and reported in a single error message.

### Hierarchy traversal

Context isolation also limits which transforms are traversed during injection:

- **Scene injection pass** — traverses only scene objects. Prefab instances, prefab scopes, and prefab components are completely ignored.
- **Prefab injection pass** — traverses only the targeted prefab instance or prefab asset hierarchy. Scene objects and scene scopes are ignored. Nested prefabs are treated as separate contexts.

This applies to field injection, method injection, scope parent resolution, and scope hierarchy traversal.

## Disabling context isolation

Context isolation is toggled via **Saneject → User Settings → Use Context Isolation**.

When disabled:

- Scenes and prefab instances form a single unified hierarchy.
- Injection traversal crosses scene and prefab boundaries.
- Dependencies can resolve freely across contexts.

This mode exists as an escape hatch. Unity does not reliably serialize cross-context references, so removing a prefab instance or unloading a scene will break those dependencies. Keeping context isolation enabled is strongly recommended.

## Cross-context references

If a prefab needs to reference a scene object, or if multiple scenes need to share an instance, use a [RuntimeProxy](proxies.md). Proxies are serializable project assets that resolve their target at runtime and are the supported way to bridge contexts without violating isolation.

## Global bindings and context filtering

The same isolation rules apply to `BindGlobal<T>()` bindings.

When **Filter By Same Context** is enabled (the default), prefab components are filtered out when resolving global bindings. Only scene objects in the matching context are eligible for promotion to the global scope. This prevents accidentally registering a prefab component that may not be present when the game runs.

Disabling the setting removes that safeguard and allows prefab components to be registered globally.
