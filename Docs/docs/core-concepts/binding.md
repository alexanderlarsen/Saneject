---
title: Binding
description: Learn how Saneject bindings define what to resolve, where dependencies come from, and which Unity injection sites may consume them.
---

# Binding

A binding is a rule declared in a `Scope` that tells Saneject:

- What dependency type should be resolved
- Where candidates come from
- Which injection sites may use the binding
- Optional filters that reject candidates

Bindings are collected from `Scope.DeclareBindings()` during editor injection, validated, then used to resolve
`[Inject]` fields, properties and method parameters.

## Binding rules

1. Every binding must specify a locator strategy with a `From...` call.
2. Type matching is strict:
    - Specifying an interface is optional; concrete-only bindings are valid.
    - `BindComponent<TConcrete>()` and `BindAsset<TConcrete>()` only match `TConcrete` injection sites.
    - `BindComponent<TInterface>()` only matches `TInterface` injection sites.
    - `BindComponent<TInterface, TConcrete>()` and `BindAsset<TInterface, TConcrete>` only match `TInterface` injection
      sites, with a `TConcrete` object that implements `TInterface`.
3. Single and collection bindings do not mix:
    - Single bindings (`BindComponent`, `BindAsset`) resolve single fields/properties, parameters.
    - Collection bindings (`BindComponents`/`BindAssets`/`BindMultiple...`) resolve arrays and `List<>`.
4. Bindings are local to the declaring scope. If the nearest scope to an injection site has no matching binding,
   Saneject walks up parent scopes until it finds one.
5. Invalid bindings are excluded from valid resolution and logged.

See [Scope](scope.md) for more information

## Fluent binding flow

Most bindings follow this general pattern or a combination of these:

```csharp
BindComponent<IAudioService, AudioManager>()
    .ToID("hud") // Optional qualifier that matches [Inject("hud")]
    .ToTarget<CombatHud>() // Optional qualifier that matches injection target objects of type CombatHud
    .ToMember("audioService") // Optional qualifier that matches members named "audioService"
    .FromTargetSelf() // Required locator strategy that looks for the AudioManager on the transform of the CombatHud
    .WhereComponent(c => c.isActiveAndEnabled); // Optional filter that only includes enabled components
```

This resolves candidates in three phases:

1. Match binding by binding qualifiers (if any).
2. Locate candidates with a `From...` method.
3. Apply `Where...` filters (if any).

## Binding families

### Component bindings

Component bindings resolve `UnityEngine.Component` dependencies from transforms, hierarchy traversal, scene-wide search,
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

- [Scope binding entry points](xref:Plugins.Saneject.Runtime.Scopes.Scope)
- [Component locator methods](xref:Plugins.Saneject.Runtime.Bindings.Component.BaseComponentBindingBuilder`1)
- [Component qualifiers](xref:Plugins.Saneject.Runtime.Bindings.Component.ComponentBindingBuilder`1)
- [Component filters](xref:Plugins.Saneject.Runtime.Bindings.Component.ComponentFilterBuilder`1)

### Global component bindings

Global bindings are component bindings declared with `BindGlobal<TComponent>()`. The resolved component is serialized
into the declaring scope at editor time and registered in `GlobalScope` during that scope's `Awake()`.

```csharp
protected override void DeclareBindings()
{
    BindGlobal<AudioManager>()
        .FromScopeSelf()
        .WhereGameObject(go => go.activeInHierarchy);
}
```

Global bindings support locator and filter methods, but not binding qualifiers and not runtime proxy methods.

Full global binding API:

- [Scope binding entry points](xref:Plugins.Saneject.Runtime.Scopes.Scope)
- [Global component binding builder](xref:Plugins.Saneject.Runtime.Bindings.Component.GlobalComponentBindingBuilder`1)

### Asset bindings

Asset bindings resolve `UnityEngine.Object` assets from `Resources`, `AssetDatabase` paths/folders, or explicit
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

- [Scope binding entry points](xref:Plugins.Saneject.Runtime.Scopes.Scope)
- [Asset binding builder](xref:Plugins.Saneject.Runtime.Bindings.Asset.AssetBindingBuilder`1)
- [Asset filters](xref:Plugins.Saneject.Runtime.Bindings.Asset.AssetFilterBuilder`1)

### Runtime proxy bindings

Runtime proxy bindings are configured from component bindings via `FromRuntimeProxy()`. They inject a proxy asset at
editor time, then swap to a real runtime instance in the scope's `Awake()`.

Rules:

- Must be `BindComponent<TInterface, TConcrete>()`.
- Must be single-value (not collection).
- Binding qualifiers are supported.
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

Full runtime proxy API:

- [Scope binding entry points](xref:Plugins.Saneject.Runtime.Scopes.Scope)
- [Runtime proxy binding builder](xref:Plugins.Saneject.Runtime.Bindings.RuntimeProxy.RuntimeProxyBindingBuilder)
- [Runtime proxy instance mode builder](xref:Plugins.Saneject.Runtime.Bindings.RuntimeProxy.RuntimeProxyInstanceModeBuilder)

## Binding qualifiers

Binding qualifiers restrict which injection sites (fields, properties, methods) can be resolved from a binding.

| Qualifier                    | Injection site match                                                            |
|------------------------------|---------------------------------------------------------------------------------|
| `ToID("someId")`             | Fields, properties, methods marked with `[Inject("someId")]`                    |
| `ToTarget<TTarget>()`        | Fields, properties, methods owned by `TTarget` objects and derived types        |
| `ToMember("someMemberName")` | Fields, properties, methods with name `"someMemberName"`                        |

Important behavior:

- Binding qualifiers are additive, so all specified qualifiers must match.
- Injection sites without an ID match bindings without `ToID`. Injection sites with an ID only match bindings with the same `ToID`.
- If `ToTarget` or `ToMember` is not set on the binding, that qualifier does not restrict where the binding applies.
- `ToTarget<TTarget>()` matches the actual object that owns the injected member. This can be a component or a nested serializable object, and a binding targeted to a base type also matches derived types.
- Binding qualifiers apply to component, asset, and runtime proxy bindings.
- Binding qualifiers do not apply to global bindings.

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

Binding filters run after locator search and before final assignment. A candidate (potentially injected dependency) must pass
all filters on the binding.

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

| Binding Family         | Filter Support                               |
|------------------------|----------------------------------------------|
| Component bindings     | Yes                                          |
| Asset bindings         | Yes                                          |
| Global bindings        | Yes (same filter API as component bindings). |
| Runtime proxy bindings | No                                           |

If a binding filter throws an exception, Saneject logs a binding filter error for that binding.

## Binding uniqueness

Saneject enforces unambiguous bindings within each `Scope`. When two bindings are considered duplicate or ambiguous, Saneject logs an error and excludes the conflicting binding from the injection run.

Duplicate and ambiguity checks use these criteria:

1. Same scope.
2. Same binding family (`ComponentBindingNode`, `AssetBindingNode`, or `GlobalComponentBindingNode`).
3. Same primary type:
    - `TInterface` when present.
    - Otherwise `TConcrete`.
4. Same single/collection shape.
5. Qualifier ambiguity:
   - If the criteria above do not separate two bindings, they conflict unless at least one qualifier rule below separates them.
   - `ToID` separates bindings by ID. A binding without `ToID` does not overlap a binding with `ToID`, and two bindings with `ToID` overlap only when their IDs overlap.
    - `ToTarget` qualifiers overlap when their target type hierarchies overlap.
   - `ToMember` qualifiers separate bindings only when both bindings specify non-overlapping values.
   - Empty `ToTarget` and `ToMember` qualifier sets are unrestricted and do not separate bindings.

Examples:

```csharp
// Duplicate or ambiguous: same scope, same family, same type, same shape, no qualifiers.
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

```csharp
// Distinct: one binding matches ID "menu" and the other matches injection sites without an ID.
BindAsset<IGameConfig, GameConfigAsset>()
    .ToID("menu")
    .FromResources("Configs/Menu");

BindAsset<IGameConfig, GameConfigAsset>()
    .ToTarget<MainMenuController>()
    .FromResources("Configs/Menu");
```

```csharp
// Ambiguous: both bindings can resolve the config member on MainMenuController.
BindAsset<IGameConfig, GameConfigAsset>()
    .ToMember("config")
    .FromResources("Configs/Menu");

BindAsset<IGameConfig, GameConfigAsset>()
    .ToTarget<MainMenuController>()
    .FromResources("Configs/Menu");
```

Global bindings have an extra rule: only one global binding per concrete component type is allowed across active
bindings across all scopes. A second `BindGlobal<AudioManager>()` is invalid even if declared in another scope.

## Binding validation

While the fluent API prevents most invalid bindings, it's still possible to create invalid bindings that will compile. However, the injection run has a validation step that catches invalid bindings, excludes them from the run and logs them as errors.

Current validation checks include:

- Duplicate binding in the same scope.
- Duplicate global binding by concrete type.
- Runtime proxy binding constraints:
    - Interface type required.
    - Concrete type required.
    - Collection mode not allowed.
- Component binding concrete type must derive from `UnityEngine.Component`.
- Asset binding concrete type must not derive from `UnityEngine.Component`.
- Interface type must actually be an interface.
- Concrete type must implement the declared interface.
- Locator strategy must be set.

Examples of invalid but compilable bindings:

```csharp
// Invalid: no locator strategy specified.
BindAsset<GameConfigAsset>();
```

```csharp
// Invalid: runtime proxy bindings cannot be collection bindings.
BindComponents<ICombatService, CombatService>()
    .FromRuntimeProxy();
```

Validation runs before dependency resolution, so invalid bindings are never used to satisfy `[Inject]` members.

## Related pages

- [Scope](scope.md)
- [Context](context.md)
- [Field & property injection](field-property-and-method-injection.md)
- [Runtime proxy](runtime-proxy.md)
- [Global scope](global-scope.md)
- [Logging & validation](../editor-and-tooling/logging-and-validation.md)
- [Glossary](../reference/glossary.md)
