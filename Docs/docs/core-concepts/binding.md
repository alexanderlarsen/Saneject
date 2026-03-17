---
title: Binding
---

# Binding

A [binding](../reference/glossary.md#binding) is a rule declared in a `Scope` that tells Saneject:

- What dependency type should be resolved
- Where candidates come from
- Which [injection sites](../reference/glossary.md#injection-site) may use the [binding](../reference/glossary.md#binding)
- Optional filters that reject candidates

[Bindings](../reference/glossary.md#binding) are collected from `Scope.DeclareBindings()` during editor injection, validated, then used to resolve
`[Inject]` fields, properties and method parameters.

## Binding rules

1. Every [binding](../reference/glossary.md#binding) must specify a [locator strategy](../reference/glossary.md#locator-strategy) with a `From...` call.
2. Type matching is strict:
    - Specifying an interface is optional; concrete-only [bindings](../reference/glossary.md#binding) are valid.
    - `BindComponent<TConcrete>()` and `BindAsset<TConcrete>()` only match `TConcrete` [injection sites](../reference/glossary.md#injection-site).
    - `BindComponent<TInterface>()` only matches `TInterface` [injection sites](../reference/glossary.md#injection-site).
    - `BindComponent<TInterface, TConcrete>()` and `BindAsset<TInterface, TConcrete>` only match `TInterface` injection
      sites, with a `TConcrete` object that implements `TInterface`.
3. Single and [collection bindings](../reference/glossary.md#collection-binding) do not mix:
    - Single [bindings](../reference/glossary.md#binding) (`BindComponent`, `BindAsset`) resolve single fields/properties, parameters.
    - [Collection bindings](../reference/glossary.md#collection-binding) (`BindComponents`/`BindAssets`/`BindMultiple...`) resolve arrays and `List<>`.
4. [Bindings](../reference/glossary.md#binding) are local to the declaring [scope](../reference/glossary.md#scope). If the nearest [scope](../reference/glossary.md#scope) to an [injection site](../reference/glossary.md#injection-site) has no matching [binding](../reference/glossary.md#binding),
   Saneject walks up parent [scopes](../reference/glossary.md#scope) until it finds one.
5. Invalid [bindings](../reference/glossary.md#binding) are excluded from valid resolution and logged.

See [Scope](scope.md) for more information

## Fluent binding flow

Most [bindings](../reference/glossary.md#binding) follow this general pattern or a combination of these:

```csharp
BindComponent<IAudioService, AudioManager>()
    .ToID("hud") // Optional qualifier that matches [Inject("hud")]
    .ToTarget<CombatHud>() // Optional qualifier that matches injection targets of type CombatHud
    .ToMember("audioService") // Optional qualifier that matches and methods named "audioService"
    .FromTargetSelf() // Required locator strategy that looks for the AudioManager on the transform of the CombatHud
    .WhereComponent(c => c.isActiveAndEnabled); // Optional filter that only includes enabled components
```

This resolves candidates in three phases:

1. Match [binding](../reference/glossary.md#binding) by [binding qualifiers](../reference/glossary.md#binding-qualifier) (if any).
2. Locate candidates with a `From...` method.
3. Apply `Where...` filters (if any).

## Binding families

### Component bindings

[Component bindings](../reference/glossary.md#component-binding) resolve `UnityEngine.Component` dependencies from transforms, hierarchy traversal, scene-wide search,
or explicit instances.

```csharp
protected override void DeclareBindings()
{
    // Interface + concrete mapping.
    BindComponent<IAudioService, AudioManager>()
        .FromScopeSelf();

    // Collection binding.
    BindComponents<EnemyController>()
        .FromScopeDescendants(includeSelf: true)
        .WhereComponent(c => c.gameObject.activeInHierarchy);
}
```

Full component API:

- [Scope binding entry points](xref:Plugins.Saneject.Experimental.Runtime.Scopes.Scope)
- [Component locator methods](xref:Plugins.Saneject.Experimental.Runtime.Bindings.Component.BaseComponentBindingBuilder`1)
- [Component qualifiers](xref:Plugins.Saneject.Experimental.Runtime.Bindings.Component.ComponentBindingBuilder`1)
- [Component filters](xref:Plugins.Saneject.Experimental.Runtime.Bindings.Component.ComponentFilterBuilder`1)

### Global component bindings

Global [bindings](../reference/glossary.md#binding) are [component bindings](../reference/glossary.md#component-binding) declared with `BindGlobal<TComponent>()`. The resolved component is serialized
into the declaring [scope](../reference/glossary.md#scope) at editor time and registered in `GlobalScope` during that [scope](../reference/glossary.md#scope)'s `Awake()`.

```csharp
protected override void DeclareBindings()
{
    BindGlobal<AudioManager>()
        .FromScopeSelf()
        .WhereGameObject(go => go.activeInHierarchy);
}
```

Global [bindings](../reference/glossary.md#binding) support locator and filter methods, but not [binding qualifiers](../reference/glossary.md#binding-qualifier) and not [runtime proxy](../reference/glossary.md#runtime-proxy) methods.

Full global [binding](../reference/glossary.md#binding) API:

- [Scope binding entry points](xref:Plugins.Saneject.Experimental.Runtime.Scopes.Scope)
- [Global component binding builder](xref:Plugins.Saneject.Experimental.Runtime.Bindings.Component.GlobalComponentBindingBuilder`1)

### Asset bindings

[Asset bindings](../reference/glossary.md#asset-binding) resolve `UnityEngine.Object` assets from `Resources`, `AssetDatabase` paths/folders, or explicit
instances.

```csharp
protected override void DeclareBindings()
{
    BindAsset<IGameConfig, GameConfigAsset>()
        .ToID("default")
        .FromResources("Configs/GameConfig");

    BindAssets<AudioClip>()
        .FromFolder("Assets/Game/Audio/Sfx")
        .Where(clip => clip.name.StartsWith("Enemy_"));
}
```

Full asset API:

- [Scope binding entry points](xref:Plugins.Saneject.Experimental.Runtime.Scopes.Scope)
- [Asset binding builder](xref:Plugins.Saneject.Experimental.Runtime.Bindings.Asset.AssetBindingBuilder`1)
- [Asset filters](xref:Plugins.Saneject.Experimental.Runtime.Bindings.Asset.AssetFilterBuilder`1)

### Runtime proxy bindings

[Runtime proxy bindings](../reference/glossary.md#runtime-proxy-binding) are configured from [component bindings](../reference/glossary.md#component-binding) via `FromRuntimeProxy()`. They inject a proxy asset at
editor time, then swap to a real runtime instance in the [scope](../reference/glossary.md#scope)'s `Awake()`.

Rules:

- Must be `BindComponent<TInterface, TConcrete>()`.
- Must be single-value (not collection).
- [Binding qualifiers](../reference/glossary.md#binding-qualifier) are supported.
- Filters are not supported.

```csharp
protected override void DeclareBindings()
{
    BindComponent<ICombatService, CombatService>()
        .ToID("combatService")
        .FromRuntimeProxy()
        .FromGlobalScope();
}
```

```csharp
protected override void DeclareBindings()
{
    BindComponent<ICombatService, CombatService>()
        .FromRuntimeProxy()
        .FromComponentOnPrefab(combatServicePrefab, dontDestroyOnLoad: true)
        .AsSingleton();
}
```

Full [runtime proxy](../reference/glossary.md#runtime-proxy) API:

- [Scope binding entry points](xref:Plugins.Saneject.Experimental.Runtime.Scopes.Scope)
- [Runtime proxy binding builder](xref:Plugins.Saneject.Experimental.Runtime.Bindings.RuntimeProxy.RuntimeProxyBindingBuilder)
- [Runtime proxy instance mode builder](xref:Plugins.Saneject.Experimental.Runtime.Bindings.RuntimeProxy.RuntimeProxyInstanceModeBuilder)

## Binding qualifiers

[Binding qualifiers](../reference/glossary.md#binding-qualifier) restrict which [injection sites](../reference/glossary.md#injection-site) (fields, properties, methods) can be resolved from a [binding](../reference/glossary.md#binding).

| Qualifier                    | Injection site match                                          |
|------------------------------|---------------------------------------------------------------|
| `ToID("someId")`             | Fields, properties, methods marked with `[Inject("someId")]`  |
| `ToTarget<TTarget>()`        | Fields, properties, methods declared in a `TTarget` component |
| `ToMember("someMemberName")` | Fields, properties, methods with name `"someMemberName"`      |

Important behavior:

- [Binding qualifiers](../reference/glossary.md#binding-qualifier) are additive, so all specified qualifiers must match.
- If a [binding qualifier](../reference/glossary.md#binding-qualifier) is not set on the [binding](../reference/glossary.md#binding), the [binding](../reference/glossary.md#binding) matches by `TInterface` or `TConcrete` only.
- [Binding qualifiers](../reference/glossary.md#binding-qualifier) apply to component, asset, and [runtime proxy bindings](../reference/glossary.md#runtime-proxy-binding).
- [Binding qualifiers](../reference/glossary.md#binding-qualifier) do not apply to global [bindings](../reference/glossary.md#binding).

Example:

```csharp
protected override void DeclareBindings()
{
    BindAsset<IGameConfig, GameConfigAsset>()
        .ToID("menu")
        .ToTarget<MainMenuController>()
        .ToMember("config")
        .FromResources("Configs/Menu");
}
```

## Binding filters

[Binding filters](../reference/glossary.md#binding-filter) run after locator search and before final assignment. A candidate (potentially injected dependency) must pass
all filters on the [binding](../reference/glossary.md#binding).

Component filter example:

```csharp
protected override void DeclareBindings()
{
    // First component anywhere in the scene, that is active/enabled and is a descendant of a Transform tagged "GameplayRoot".
    BindComponent<EnemyController>()
        .FromAnywhere()
        .WhereComponent(c => c.isActiveAndEnabled)
        .WhereAnyAncestor(t => t.CompareTag("GameplayRoot"));
}
```

Asset filter example:

```csharp
protected override void DeclareBindings()
{
    // All assets in the "Assets/Game/Audio/Music" folder with names starting with "Boss_".
    BindAssets<AudioClip>()
        .FromFolder("Assets/Game/Audio/Music")
        .Where(clip => clip.name.StartsWith("Boss_"));
}
```

| [Binding Family](../reference/glossary.md#binding-family)                | Filter Support                                                                             |
|--------------------------------------------------------------------------|--------------------------------------------------------------------------------------------|
| [Component bindings](../reference/glossary.md#component-binding)         | Yes                                                                                        |
| [Asset bindings](../reference/glossary.md#asset-binding)                 | Yes                                                                                        |
| Global [bindings](../reference/glossary.md#binding)                      | Yes (same filter API as [component bindings](../reference/glossary.md#component-binding)). |
| [Runtime proxy bindings](../reference/glossary.md#runtime-proxy-binding) | No                                                                                         |

If a [binding filter](../reference/glossary.md#binding-filter) throws an exception, Saneject logs a [binding filter](../reference/glossary.md#binding-filter) error for that [binding](../reference/glossary.md#binding).

## Binding uniqueness

Saneject enforces [binding](../reference/glossary.md#binding) unique within each `Scope`. When a [binding](../reference/glossary.md#binding) is considered duplicate, Saneject logs an error and excludes the duplicate from the [injection run](../reference/glossary.md#injection-run).

Duplicate checks use these criteria:

1. Same [scope](../reference/glossary.md#scope).
2. Same [binding family](../reference/glossary.md#binding-family) (`ComponentBindingNode`, `AssetBindingNode`, or `GlobalComponentBindingNode`).
3. Same primary type:
    - `TInterface` when present.
    - Otherwise `TConcrete`.
4. Same single/collection shape.
5. Qualifier overlap:
    - If both [bindings](../reference/glossary.md#binding) have no [binding qualifiers](../reference/glossary.md#binding-qualifier) at all, they conflict.
    - Otherwise, conflict requires full overlap in `ToTarget`, `ToMember`, and `ToID` simultaneously.

Examples:

```csharp
// Duplicate: same scope, same family, same type, same shape, no qualifiers.
BindComponent<AudioManager>()
    .FromScopeSelf();

BindComponent<AudioManager>()
    .FromScopeParent(); // Invalid duplicate.
```

```csharp
// Distinct: different qualifier sets.
BindAsset<IGameConfig, GameConfigAsset>()
    .ToTarget<MainMenuController>()
    .ToMember("config")
    .ToID("menu")
    .FromResources("Configs/Menu");

BindAsset<IGameConfig, GameConfigAsset>()
    .ToTarget<GameplayController>()
    .ToMember("config")
    .ToID("gameplay")
    .FromResources("Configs/Gameplay");
```

Global [bindings](../reference/glossary.md#binding) have an extra rule: only one global [binding](../reference/glossary.md#binding) per concrete component type is allowed across active
[bindings](../reference/glossary.md#binding) across all [scopes](../reference/glossary.md#scope). A second `BindGlobal<AudioManager>()` is invalid even if declared in another [scope](../reference/glossary.md#scope).

## Binding validation

While the fluent API prevents most invalid [bindings](../reference/glossary.md#binding), it's still possible to create invalid [bindings](../reference/glossary.md#binding) that will compile. However, the [injection run](../reference/glossary.md#injection-run) has a validation step that catches invalid [bindings](../reference/glossary.md#binding), excludes them from the run and logs them as errors.

Current validation checks include:

- Duplicate [binding](../reference/glossary.md#binding) in the same [scope](../reference/glossary.md#scope).
- Duplicate global [binding](../reference/glossary.md#binding) by concrete type.
- [Runtime proxy binding](../reference/glossary.md#runtime-proxy-binding) constraints:
    - Interface type required.
    - Concrete type required.
    - Collection mode not allowed.
- [Component binding](../reference/glossary.md#component-binding) concrete type must derive from `UnityEngine.Component`.
- [Asset binding](../reference/glossary.md#asset-binding) concrete type must not derive from `UnityEngine.Component`.
- Interface type must actually be an interface.
- Concrete type must implement the declared interface.
- [Locator strategy](../reference/glossary.md#locator-strategy) must be set.

Examples of invalid but compilable [bindings](../reference/glossary.md#binding):

```csharp
// Invalid: no locator strategy specified.
BindAsset<GameConfigAsset>();
```

```csharp
// Invalid: runtime proxy bindings cannot be collection bindings.
BindComponents<ICombatService, CombatService>()
    .FromRuntimeProxy();
```

Validation runs before dependency resolution, so invalid [bindings](../reference/glossary.md#binding) are never used to satisfy `[Inject]` members.

## Related pages

- [Scope](scope.md)
- [Context](context.md)
- [Field & property injection](field-property-and-method-injection.md)
- [Runtime proxy](runtime-proxy.md)
- [Global scope](global-scope.md)
- [Logging & validation](../editor-and-tooling/logging-and-validation.md)
- [Glossary](../reference/glossary.md)