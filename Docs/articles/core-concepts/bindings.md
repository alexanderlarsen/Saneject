# Bindings

A binding connects a type to a locator strategy. Each binding you declare in `DeclareBindings()` says: *"when something needs a `T`, here's how to find it."*

The full shape of a binding is:

```
Scope.Bind<Type>() → .FromLocator() → .Where(filter) → .ToQualifier()
```

Locator and qualifiers can be applied in any order. Filters always come after the locator.

## Binding families

### Component bindings

Used to inject `MonoBehaviour` components (or interfaces implemented by them).

```csharp
// Bind a concrete type
BindComponent<AudioService>()
    .FromAnywhere();

// Bind by interface — resolves an IEnemyObservable, must be implemented by EnemyManager
BindComponent<IEnemyObservable, EnemyManager>()
    .FromScopeDescendants();

// Bind a collection — injects all matching components as a List<T> or T[]
BindComponents<IEnemy, Enemy>()
    .FromScopeDescendants();
```

The single-type overloads (`BindComponent<T>`) accept either a concrete type or an interface directly. The two-type overloads (`BindComponent<TInterface, TConcrete>`) let you enforce which concrete type satisfies an interface.

### Asset bindings

Used to inject `UnityEngine.Object` assets — `ScriptableObject`, `Texture`, `Material`, `GameObject`, etc.

```csharp
// Inject a single asset from Resources
BindAsset<AudioClip>()
    .FromResources("Audio/Theme");

// Inject a prefab by path from the AssetDatabase
BindAsset<GameObject>()
    .ToTarget<EnemyManager>()
    .ToMember("enemyPrefab")
    .FromAssetLoad("Assets/Prefabs/Enemy.prefab");

// Inject all ScriptableObject configs from a folder
BindAssets<EnemyConfig>()
    .FromFolder("Assets/Data/Enemies");
```

Asset bindings support the same qualifiers as component bindings. For a full list of asset locators see the [API reference](#).

### Global bindings

Used to register components into the [GlobalScope](global-scope.md) at runtime. See that article for the full picture.

```csharp
// Promote a component to the global scope so proxies and other scenes can access it
BindGlobal<GameStateManager>()
    .FromScopeDescendants();
```

## Interface vs concrete vs mapped

| Syntax | Resolves | Use when |
|---|---|---|
| `BindComponent<IMyInterface>()` | Any object implementing the interface | You want any implementor |
| `BindComponent<MyConcrete>()` | Only `MyConcrete` exactly | You want a specific concrete type |
| `BindComponent<IMyInterface, MyConcrete>()` | Only `MyConcrete`, injected as `IMyInterface` | You want to enforce which concrete type satisfies an interface |

## Single vs collection

`BindComponent<T>` targets `[Inject] T field` — a single value.
`BindComponents<T>` targets `[Inject] T[] field` or `[Inject] List<T> field` — all matching instances.

Single and collection bindings are independent. Both can exist in the same Scope for the same type.

## Locator families

The locator is the part of a binding that says *where* to search. Component bindings have four families:

### Scope-relative

Search relative to the `Scope`'s own transform. The shorthands (`FromSelf`, `FromDescendants`, etc.) are aliases for the `FromScope*` variants.

```csharp
BindComponent<AudioService>().FromSelf();             // On the Scope's own transform
BindComponent<AudioService>().FromDescendants();      // Any descendant of the Scope
BindComponent<AudioService>().FromParent();           // Direct parent of the Scope
BindComponent<AudioService>().FromAncestors();        // Any ancestor of the Scope
BindComponent<AudioService>().FromSiblings();         // Siblings of the Scope
```

### Root-relative

Search relative to the hierarchy root of the Scope's transform.

```csharp
BindComponent<AudioService>().FromRootSelf();         // The root transform itself
BindComponent<AudioService>().FromRootDescendants();  // Any descendant of the root
```

### Target-relative

Search relative to the transform of the component being injected (the injection target). Useful when each instance needs a dependency from its own immediate surroundings.

```csharp
// Each injected component gets the CharacterController on its own GameObject
BindComponent<CharacterController>().FromTargetSelf();

// Get the Rigidbody from the injection target's parent
BindComponent<Rigidbody>().FromTargetParent();
```

### Custom transform

Search relative to any specific transform reference you provide at binding declaration time.

```csharp
[SerializeField] private Transform sharedParent;

protected override void DeclareBindings()
{
    BindComponent<AudioService>().From(sharedParent);
    BindComponent<Enemy>().FromDescendantsOf(sharedParent);
}
```

### Special locators

```csharp
// Scene-wide search (FindObjectsByType under the hood)
BindComponent<Camera>().FromAnywhere();

// Bind a specific already-known instance
BindComponent<AudioService>().FromInstance(myServiceRef);

// Delegate — resolution deferred to your code
BindComponent<AudioService>().FromMethod(() => FindMyService());
```

## Asset locators

```csharp
.FromResources("path/to/asset")       // Resources.Load
.FromResourcesAll("path/to/folder")   // Resources.LoadAll
.FromAssetLoad("Assets/path/to.ext")  // AssetDatabase.LoadAssetAtPath
.FromAssetLoadAll("Assets/path")      // AssetDatabase.LoadAllAssetsAtPath
.FromFolder("Assets/path/to/folder")  // All assets of matching type in folder
.FromInstance(myAssetRef)             // Direct reference
.FromMethod(() => MyCustomLoad())     // Delegate
```

## Qualifiers

Qualifiers narrow which fields, properties, or methods a binding applies to. They can be combined freely.

### `ToID`

Matches fields and methods decorated with `[Inject("myId")]`:

```csharp
// Binding side
BindComponent<IAudioService, MusicService>().FromAnywhere().ToID("music");
BindComponent<IAudioService, SfxService>().FromAnywhere().ToID("sfx");

// Field side
[Inject("music"), SerializeInterface] private IAudioService musicService;
[Inject("sfx"),   SerializeInterface] private IAudioService sfxService;
```

### `ToTarget<T>`

Restricts the binding to apply only when injecting into a specific component type:

```csharp
// Only inject this binding into Player components
BindComponent<IAudioService, MusicService>().FromAnywhere().ToTarget<Player>();
```

### `ToMember`

Restricts the binding to a specific field, property, or method name:

```csharp
BindAsset<GameObject>()
    .FromAssetLoad("Assets/Prefabs/Enemy.prefab")
    .ToTarget<EnemyManager>()
    .ToMember("enemyPrefab");
```

Qualifiers can be stacked. A binding with both `ToTarget<T>` and `ToMember` only applies when both conditions match.

## Filters

Filters are predicates applied to the candidates found by the locator. They come after the locator call and can be chained.

```csharp
// Only the AudioService whose GameObject is tagged "Music"
BindComponent<IAudioService, AudioService>()
    .FromAnywhere()
    .WhereGameObject(go => go.CompareTag("Music"));

// Only the component whose parent is named "AudioRoot"
BindComponent<AudioService>()
    .FromScopeDescendants()
    .WhereParent(parent => parent.name == "AudioRoot");

// Multiple filters — all conditions must pass
BindComponent<Enemy>()
    .FromScopeDescendants()
    .WhereGameObject(go => go.activeSelf)
    .Where(e => e.IsAlive);
```

Available filter methods on component bindings: `Where`, `WhereComponent`, `WhereTransform`, `WhereGameObject`, `WhereParent`, `WhereAnyAncestor`, `WhereRoot`, `WhereAnyChild`, `WhereChildAt`, `WhereFirstChild`, `WhereLastChild`, `WhereAnyDescendant`, `WhereAnySibling`.

Asset bindings support `Where(Func<TAsset, bool>)`.

## The `[Inject]` attribute

Fields and methods are marked for injection with `[Inject]`:

```csharp
[Inject] private AudioService audioService;         // Basic
[Inject("sfx")] private IAudioService sfxService;  // Shorthand ID match
[Inject(suppressMissingErrors: true)]               // Suppress "no binding found" warnings
private IAudioService optionalService;
```

`suppressMissingErrors` is useful for optional dependencies where a missing binding is expected.

## Binding uniqueness

Each binding in a `Scope` must be unique. Two bindings conflict if they have the same bound type (interface takes precedence over concrete), the same collection flag, the same global flag, and overlapping qualifiers.

Qualifier overlap is checked by intersection, not equality:

- **ID qualifiers** — two bindings conflict if they share at least one ID string.
- **Target qualifiers** — two bindings conflict if any target types are the same or assignable.
- **Member-name qualifiers** — two bindings conflict if they share at least one member name.
- An empty qualifier set is treated as a generic binding and does not conflict with a targeted binding.

Examples:

```csharp
// Conflict — same interface, no differentiating qualifiers
BindComponent<IMyService>();
BindComponent<IMyService, MyServiceConcrete>();

// OK — same interface but different IDs
BindComponent<IMyService>().ToID("primary");
BindComponent<IMyService>().ToID("secondary");

// OK — same interface but different target filters
BindComponent<IMyService>().ToTarget<Player>();
BindComponent<IMyService>().ToTarget<Enemy>();

// OK — one is single, one is collection
BindComponent<IMyService>();
BindComponents<IMyService>();
```

Duplicate bindings are logged and the duplicate is skipped.
