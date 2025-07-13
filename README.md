# Saneject

Editor-time resolved serialized field dependency injection for Unity - keep your references visible, classes clean, ditch the runtime container.

> ⚠️ **Beta Notice**  
> Saneject is currently in beta. While the core functionality is stable, I’m looking for testers to help catch edge cases, bugs and polish the tooling. If you find issues or have feedback, please open an issue or reach out.

## Table of Contents

- [What Is This?](#what-is-this)
- [Why Another DI Tool?](#why-another-di-tool)
- [Features](#features)
- [Quick Start](#quick-start)
    - [Requirements](#requirements)
    - [Installation](#installation)
    - [Hello Saneject (Basic Example)](#hello-saneject-basic-example)
- [Demo Game](#demo-game)
- [Deep Dive](#deep-dive)
    - [What Is Dependency Injection?](#what-is-dependency-injection)
    - [How Runtime DI Typically Works](#how-runtime-di-typically-works)
    - [How Saneject DI Works](#how-saneject-di-works)
    - [Runtime DI vs Saneject Comparison](#runtime-di-vs-saneject-comparison)
    - [Scopes & Resolution Order](#scopes--resolution-order)
    - [Binding Methods](#binding-methods)
    - [Component Locator Methods](#component-locator-methods)
    - [Object Locator Methods](#object-locator-methods)
    - [Component Filter Methods](#component-filter-methods)
    - [Object Filter Methods](#object-filter-methods)
    - [SerializeInterface](#serializeinterface)
    - [Interface Proxy Object](#interface-proxy-object)
    - [Global Scope](#global-scope)
    - [Roslyn Tools in Saneject](#roslyn-tools-in-saneject)
    - [UX](#ux)
    - [User Settings](#user-settings)
- [Limitations / Known Issues](#limitations--known-issues)
- [Credits / Contribution](#credits--contribution)
- [License](#license)

## What Is This?

Saneject is a lightweight middle-ground between hand-wiring references and a full runtime DI container. It resolves dependencies in the Unity Editor using familiar DI syntax and workflows, writes them straight into serialized fields (including interfaces), and keeps your classes free of `GetComponent`, singletons, manual look-ups, etc. At runtime it’s just regular, serialized Unity objects - no reflection, no container, no startup hit.

## Why Another DI Tool?

| Pain Point                                                                                       | How Saneject Helps                                                                                                                                                                             |
|--------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| We want structured dependency management but don’t want to commit to a full runtime DI workflow. | Saneject offers DI-style binding syntax and organisation without a runtime container - you keep editor-time determinism, default Unity lifecycle and Inspector visibility.                     |
| “We want to see what’s wired where in the Inspector.”                                            | All references are regular serialized fields. Nothing is hidden behind a runtime graph.                                                                                                        |
| Interfaces can’t be dragged into the Inspector.                                                  | Saneject’s Roslyn generator adds safe interface-backing fields with Inspector support. `[SerializeInterface] IMyInterface myInterface` shows up as a proper serialized field.                  |
| Runtime DI lifecycles can feel opaque or fight Unity’s own Awake/Start order.                    | Everything is set and serialized in the editor. Unity’s normal lifecycle stays untouched.                                                                                                      |
| Large reflection-heavy containers add startup cost.                                              | Saneject resolves once in the editor - zero reflection or allocation at runtime.                                                                                                               |
| Can’t serialize references between scenes or from a scene into prefabs.                          | `InterfaceProxyObject`, a Roslyn generated `ScriptableObject`, can be referenced anywhere like any asset. At runtime, it resolves and forwards to a real scene instance with minimal overhead. |
| Mixed teams (artists/designers) struggle with code-only installers.                              | Bindings live in Scope scripts as simple, declarative C#. Fields are regular serialized fields marked with `[Inject]`, and field visibility can be toggled from settings.                      |

Saneject isn’t meant to replace full runtime frameworks like Zenject or VContainer. It’s an alternative workflow for projects that value determinism, Inspector visibility, and minimal runtime overhead.

## Features

### Injection & Binding

- **Editor-time, deterministic injection:** Bindings are resolved in the editor, stored directly in serialized fields, including nested serialized classes.
- **Fluent, scope-aware binding API:** Search hierarchy or project, filter by tag/layer/name, bind by type or ID.
- **Fail-fast validation:** Flags missing/conflicting bindings and invalid setups (e.g., global inside prefabs) during editor injection pass.
- **Unified Scope component:** One Scope type handles both scenes and prefabs, with automatic context detection.
- **Built-in caching & safety nets:** Redundant injections are cached, prefab scopes are skipped in scene injection, and unused bindings are reported.

### Serialization & Interfaces

- **Interface serialization with Roslyn:** `[SerializeInterface] IMyInterface` fields show up in the Inspector.
- **Cross-scene / prefab references:** `InterfaceProxyObject` `ScriptableObjects` allow serialized references to objects Unity normally can’t link.
- **Global Scope container:** Scene dependencies can be promoted to global singletons and resolved statically by proxies.

### Performance & Runtime

- **No runtime reflection:** Everything is injected and serialized in the editor. At runtime, it's just data Unity already serialized.
- **Proxy resolution:** Proxies resolve their targets once, then cache them. Minimal overhead (dictionary lookup or simple search).

### Editor UX & Tooling

- **Native UI/UX:** Designed to feel like it belongs in Unity - polished inspectors, minimal ceremony, and contextual behavior that matches Unity workflows.
- **User-friendly tooling:** One-click scene resolve, right-click proxy generation, correct inspector interface ordering, automatic Scope context handling.
- **Inspector polish:** `[Inject]` fields grayed out (or hidden), interface proxies show implemented types, help boxes on components.
- **User Settings panel:** Toggle injected field visibility, logging, and more.

## Quick Start

### Requirements

| Requirement       | Description                                                                                                                  |
|-------------------|------------------------------------------------------------------------------------------------------------------------------|
| Unity Version     | Unity 6000.0.23f1 LTS or newer. Relies on C# Roslyn source generators. Earlier versions may work but are currently untested. |
| Scripting Backend | Mono or IL2CPP                                                                                                               |
| Platforms         | Editor-only tooling; runtime code is plain C#, so it runs on any platform Unity 6 supports                                   |

> ⚠️ **Platform notice**  
> Saneject’s runtime is just plain C# (no reflection, no dynamic code).  
> It's tested on Windows + Android (Mono & IL2CPP) builds without issues, but other IL2CPP targets (iOS, WebGL, consoles) are not yet verified.
>
> The only non-standard Unity moving parts are:
>
> - The Roslyn-generated partial classes compiled into your assemblies.
> - `ISerializationCallbackReceiver` setting interface fields after deserialization.
>
> Both *should* work everywhere Unity does, but if you run into stripping/AOT quirks, please open an issue.

### Installation

| Install Method              | Instruction                                                                                                                                                                                                                                                                            |
|-----------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Unity Package               | Grab the latest `unitypackage` from the [Releases page](https://github.com/alexanderlarsen/Saneject/releases) → double-click → import.                                                                                                                                                 |
| Clone + Copy                | 1. Clone the repo somewhere outside your project<br> `git clone https://github.com/alexanderlarsen/Saneject.git`<br><br> 2. Copy Saneject plugin folder into your own project<br> `cp -r Saneject/UnityProject/Saneject/Assets/Plugins/Saneject <YourUnityProjectRoot>/Assets/Plugins` |
| Unity Package Manager (UPM) | Currently unsupported but I'll get it up and running later.                                                                                                                                                                                                                            |

### Hello Saneject (Basic Example)

1. Create a `GameObject` named `Root` in the scene.
2. Add a `GameObject` named `Player` under the `Root` and attach `Player.cs` and a `CharacterController` to it:

```csharp
public class Player : MonoBehaviour
{
    // Interface field, marked for injection, shows up in the Inspector
    [Inject, SerializeInterface]
    private IGameStateObservable gameStateObservable;

    // Concrete component field, marked for injection, living on the same GameObject
    [Inject, SerializeField]
    private CharacterController controller;
}
```

3. Add `GameManager.cs` somewhere in the scene:

```csharp
// Will satisfy the IGameStateObservable binding
public class GameManager : MonoBehaviour, IGameStateObservable
{ }

public interface IGameStateObservable 
{ }
```

4. Add `GameScope.cs` to the `Root` `GameObject`:

```csharp
// One place to declare where things come from
public class GameScope : Scope
{
    public override void Configure()
    {
        // Look anywhere in the loaded scene for a GameManager and bind by interface.
        // Resolves via FindObjectsByType<GameManager>(FindObjectsInactive.Include, FindObjectsSortMode.None)) under the hood.
        RegisterComponent<IGameStateObservable, GameManager>().FromAnywhereInScene();

        // Grab CharacterController on the injection target (Player) transform.
        // Resolves via player.transform.GetComponent<CharacterController>() under the hood.
        RegisterComponent<CharacterController>().FromTargetSelf();
    }
}
```

5. Run dependency injection using either method:

- **Scope Inspector:** Inject Scene Dependencies button
- **Hierarchy Context Menu:** Right-click hierarchy panel → Inject Scene Dependencies
- **Unity Main Menu Bar:** Saneject → Inject Scene Dependencies

Saneject fills in the serialized fields. Press Play - no runtime container required.

## Demo Game

A small three-scene demo game lives in `Assets/Plugins/Saneject/Demo`. It shows how to use Saneject in a real (but simple) game setup:

- Cross-scene and prefab references using `SerializeInterface` and interface proxies
- Scene and prefab `Scopes` with different bindings
- Global scope usage
- Basic UI and game loop

Gameplay: chase enemies as they evade you. Catch them all to win and restart.

**To run it:**

1. Add the following scenes to Build Settings (in this order):

- `StartScene` (bootstrap)
- `GameScene`
- `UIScene`

2. Open `StartScene`
3. Press Play.

Requires **TextMesh Pro essentials** for the UI to work.

The demo should be fairly self-explanatory and can be used as a code/setup study to understand how to structure bindings, scopes, and proxies.

## Deep Dive

### What Is Dependency Injection?

Dependency Injection (DI) is a design pattern where objects receive their dependencies from an external source, rather than creating or locating them themselves. Instead of calling `new` or searching the scene themselves, an external system supplies the needed objects.

### How Runtime DI Typically Works

In most DI frameworks like Zenject or VContainer, you declare bindings in code or installers. At runtime, a container resolves and injects all dependencies before or during startup. This provides flexibility, supports unit testing, and allows dynamic setups. However, it introduces complexity and runtime overhead (reflection, allocations), and the wiring can feel less transparent to non-programmers, since it's not always visible in the Inspector.

### How Saneject DI Works

Saneject flips the model: you still declare bindings in code (via a `Scope`), but all bindings are resolved in the Unity Editor. The results are written directly to serialized fields. There's no container or injection step at runtime - Unity loads the scene with references already assigned. This has both benefits and trade-offs compared to runtime DI, outlined below.

### Runtime DI vs Saneject Comparison

| Approach             | Runtime DI (Zenject, etc.)                               | Saneject                                                                                         |
|----------------------|----------------------------------------------------------|--------------------------------------------------------------------------------------------------|
| Injection timing     | Runtime (container init)                                 | Editor-time (stored as serialized fields)                                                        |
| Lifecycle            | More complex - adds a second lifecycle on top of Unity's | Regular Unity lifecycle (Awake, Start, etc.)                                                     |
| Performance          | Some startup cost (reflection, allocations)              | Zero reflection or container at runtime (small runtime lookup if using global dependencies)      |
| Inspector visibility | Limited - container handles wiring                       | All dependencies are visible in the Inspector                                                    |
| Flexibility          | High - bindings can change at runtime                    | Lower - wiring is fixed after injection                                                          |
| Testing / mocking    | Strong - easy to substitute in unit tests                | Less ergonomic - requires setting serialized data                                                |
| Visual Debuggability | Can be opaque - dynamic graph                            | Fully deterministic - just look at the fields for missing dependencies                           |
| Plain C# Classes     | Full support - constructor injection, POCO creation      | No constructor injection and POCO creation. Does support injection into serialized nested POCOs. |

### Scopes & Resolution Order

| **Concept**               | **What it Means in Saneject**                                                                                                                                                                                                                                                                                                         |
|---------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Scope component**       | A `MonoBehaviour` that declares bindings for how to resolve dependencies in `Components` **below** its `Transform`.                                                                                                                                                                                                                   |
| **Root-scope scan**       | No matter which `Scope` you start the injection on, Saneject walks up to the top-most Scope first, then injects downward once.                                                                                                                                                                                                        |
| **Resolution fallback**   | When a binding isn’t found in the current `Scope`, the injector climbs upward through parent `Scopes` until it finds one (or fails).                                                                                                                                                                                                  |
| **Scene Scope**           | Lives on a (non-prefab) scene `GameObject`. Can bind to any `Component` or `Object` in the scene or project folder, including prefabs.                                                                                                                                                                                                |
| **Prefab Scope**          | Lives on a prefab. Can bind to any `Component` or `Object` in the prefab itself or project folder. Prefab Scopes present in the scene are skipped during scene injection, to keep the prefab self-contained.<br><br>Need a scene reference inside a prefab? Use an `InterfaceProxyObject` `ScriptableObject` and inject that instead. |
| **Scene vs Prefab Scope** | Same `Component` but the DI system treats them as different contexts.                                                                                                                                                                                                                                                                 |

An example of how scoped resolution works (code below):

```mermaid
flowchart TD
  subgraph Editor
    PrefabDI["Prefab injection pass"]
    DI["Scene injection pass"]
  end

  subgraph Scene
    PrefabDI -->|Inject| UIScope["UIScope (Prefab)"]
    DI -->|Skip injection| UIScope
    DI -->|Inject| EnemyAudioService["Enemy.audioService (IAudioService)"]
    DI -->|Inject| EnemyAIController["Enemy.aiController (AIController)"]
    EnemyAudioService -->|"Try resolve IAudioService → Binding not found"| EnemyScope
    EnemyAIController -->|"Try resolve AIController → Binding found"| EnemyScope
    EnemyScope -->|"Try resolve IAudioService → Binding found"| RootScope
  end
```

> ⚠️ Last time I checked, Mermaid diagrams don’t render in the GitHub mobile app. Use a browser to view them properly.

`DependencyInjector` (injection passes) first queries `EnemyScope` for `IAudioService` but a binding isn't defined there, so the request bubbles up to `RootScope` which has the binding and provides the `Object`.

`AIController` is resolved directly from `EnemyScope`, so no fallback is needed.

Any scopes that live on prefabs (like `UIScope` above) are skipped during a scene-wide injection pass - they get their own dependencies when the prefab is injected in isolation, or you can inject scene objects into a prefab via an `InterfaceProxyObject`.

```csharp
public class RootScope : Scope
{
    public override void Configure()
    {
        RegisterObject<IAudioService>().FromResources("Audio/Service");
    }
}
```

```csharp
public class EnemyScope : Scope
{
    public override void Configure()
    {
        // Enemy-local AIController only; no IAudioService here.
        RegisterComponent<AIController>().FromScopeSelf();
    }
}
```

After injection:

```csharp
public class Enemy : MonoBehaviour
{
    [Inject, SerializeInterface] 
    IAudioService audioService; // Resolved from RootScope
    
    [Inject]
    AIController aiController; // Resolved from EnemyScope        
}
```

> ℹ️ `Scope` uses `HideFlags.DontSaveInBuild` to strip it from builds, to prevent accidental usage at runtime.

### Binding Methods

To start binding dependencies, create a new custom `Scope` and use one of the following `Register` methods to start a fluent builder.

#### Component Bindings

Bind Components from scene/prefab hierarchy. Methods return a `ComponentBindingBuilder<T>` to define a locate strategy.

| Method                                       | Description                                                     |
|----------------------------------------------|-----------------------------------------------------------------|
| `RegisterComponent<T>()`                     | Bind a `Component` in the scene or prefab by its concrete type. |
| `RegisterComponent<TInterface, TConcrete>()` | Bind by interface and resolve with `TConcrete`.                 |

#### Object Bindings

Bind Project folder assets, e.g., prefabs, `ScriptableObjects`. Methods return an `ObjectBindingBuilder<T>` to define a locate strategy.

| Method                                    | Description                                                               |
|-------------------------------------------|---------------------------------------------------------------------------|
| `RegisterObject<T>()`                     | Bind a `UnityEngine.Object` asset in the Project folder by concrete type. |
| `RegisterObject<TInterface, TConcrete>()` | Bind by interface and resolve with `TConcrete` asset.                     |

#### Global Bindings

Bind cross-scene singletons. Methods return a `ComponentBindingBuilder<T>` to define a locate strategy.

| Method                         | Description                                                                                                                                                     |
|--------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `RegisterGlobalComponent<T>()` | Promote a scene `Component` into `SceneGlobalContainer`.<br/>Registered in the global scope at startup for global resolution (e.g., via `InterfaceProxyObject`. |
| `RegisterGlobalObject<T>()`    | Promote a scene `Object` into `SceneGlobalContainer`.<br/>Registered in the global scope at startup for global resolution (e.g., via `InterfaceProxyObject`.    |

### Component Locator Methods

Methods in `ComponentBindingBuilder<T> where T : UnityEngine.Component`. All methods return a `FilterableComponentBindingBuilder<T>` to filter found `Components`.

#### Scope-relative

Looks for the `Component` from the `Scope` transform and its hierarchy.

| Method                                                                                         | Description                                                   |
|------------------------------------------------------------------------------------------------|---------------------------------------------------------------|
| `FromScopeSelf()`<br/>`FromSelf()`                                                             | Component on the Scope’s own `Transform`.                     |
| `FromScopeParent()`<br/>`FromParent()`                                                         | Component on the Scope’s direct parent.                       |
| `FromScopeAncestors()`<br/>`FromAncestors()`                                                   | First match on any ancestor of the Scope (recursive upward).  |
| `FromScopeFirstChild()`<br/>`FromFirstChild()`                                                 | Component on the Scope’s first direct child.                  |
| `FromScopeLastChild()`<br/>`FromLastChild()`                                                   | Component on the Scope’s last direct child.                   |
| `FromScopeChildWithIndex(int index)`<br/>`FromChildWithIndex(int index)`                       | Component on the child at the given index.                    |
| `FromScopeDescendants(bool includeSelf = true)`<br/>`FromDescendants(bool includeSelf = true)` | Any descendant of the Scope.                                  |
| `FromScopeSiblings()`<br/>`FromSiblings()`                                                     | Any sibling of the Scope (other children of the same parent). |

#### Root-relative

Looks for the `Component` from the `Scope.transform.root` transform and its hierarchy.

| Method                                         | Description                                       |
|------------------------------------------------|---------------------------------------------------|
| `FromRootSelf()`                               | Component on the scene root object itself.        |
| `FromRootFirstChild()`                         | Component on the root’s first direct child.       |
| `FromRootLastChild()`                          | Component on the root’s last direct child.        |
| `FromRootChildWithIndex(int index)`            | Component on the root’s child at the given index. |
| `FromRootDescendants(bool includeSelf = true)` | Any descendant of the root object.                |

#### Injection-target-relative.

Looks for the `Component` from the injection target transform and its hierarchy. Injection target is the `Component` of a field/property marked with `[Inject]`, i.e., the `Component` requesting injection.

| Method                                           | Description                                |
|--------------------------------------------------|--------------------------------------------|
| `FromTargetSelf()`                               | Component on the target’s own `Transform`. |
| `FromTargetParent()`                             | Component on the target’s parent.          |
| `FromTargetAncestors()`                          | First match on any ancestor of the target. |
| `FromTargetFirstChild()`                         | Component on the target’s first child.     |
| `FromTargetLastChild()`                          | Component on the target’s last child.      |
| `FromTargetChildWithIndex(int index)`            | Component on the target’s child at index.  |
| `FromTargetDescendants(bool includeSelf = true)` | Any descendant of the target.              |
| `FromTargetSiblings()`                           | Any sibling of the target.                 |

#### Arbitrary Transform target

Looks for the `Component` from the specified `Transform` and its hierarchy.

| Method                                                         | Description                                |
|----------------------------------------------------------------|--------------------------------------------|
| `From(Transform target)`                                       | Component on a specific `Transform`.       |
| `FromParentOf(Transform target)`                               | Component on the target’s parent.          |
| `FromAncestorsOf(Transform target)`                            | First match on any ancestor of the target. |
| `FromFirstChildOf(Transform target)`                           | Component on the target’s first child.     |
| `FromLastChildOf(Transform target)`                            | Component on the target’s last child.      |
| `FromChildWithIndexOf(Transform target, int index)`            | Component on the target’s child at index.  |
| `FromDescendantsOf(Transform target, bool includeSelf = true)` | Any descendant of the target.              |
| `FromSiblingsOf(Transform target)`                             | Any sibling of the target.                 |

#### Scene/Misc

| Method                       | Description                                                            |
|------------------------------|------------------------------------------------------------------------|
| `FromAnywhereInScene()`      | Finds the first matching component anywhere in the loaded scene.       |
| `FromInstance(T instance)`   | Binds to an explicit instance.                                         |
| `FromMethod(Func<T> method)` | Uses a custom factory method to supply the instance.                   |
| `WithId(string id)`          | Assigns a custom ID for fields marked with `[Inject(id: "StringID")]`. |

### Object Locator Methods

Methods in `ObjectBindingBuilder<T> where T : UnityEngine.Object`. All methods return a `FilterableObjectBindingBuilder<T>` to filter found `Components`.

| Method                            | Description                                                                             |
|-----------------------------------|-----------------------------------------------------------------------------------------|
| `FromInstance(T instance)`        | Bind to an explicit `Object` instance.                                                  |
| `From(Func<T> factory)`           | Use a factory delegate to produce the instance at injection time.                       |
| `FromResources(string path)`      | Load a single asset from a `Resources/` path via `Resources.Load`.                      |
| `FromResourcesAll(string path)`   | Load all assets of type `T` at that path via `Resources.LoadAll`.                       |
| `FromAnywhereInScene()`           | Find the first matching object of type `T` anywhere in the loaded scene.                |
| `FromAssetLoad(string assetPath)` | Load an asset at the given path via `AssetDatabase.LoadAssetAtPath<T>()` (Editor only). |
| `WithId(string id)`               | Assign a custom binding ID to match `[Inject(Id = "...")]` fields.                      |

### Component Filter Methods

Methods in `FilterableComponentBindingBuilder<T> where T : UnityEngine.Component`. Returns itself to allow chaining multiple filters.

| Method                                | Description                                                        |
|---------------------------------------|--------------------------------------------------------------------|
| `WhereTargetIs<TTarget>()`            | Apply binding only when the injection target is of type `TTarget`. |
| `WhereTagIs(string tag)`              | Include components whose `GameObject.tag` equals `tag`.            |
| `WhereNameIs(string name)`            | Include components whose `GameObject.name` equals `name`.          |
| `WhereNameContains(string substring)` | Include components whose `GameObject.name` contains `substring`.   |
| `WhereLayerIs(int layer)`             | Include components on the specified `layer`.                       |
| `WhereActiveInHierarchy()`            | Include components with active `GameObject`s.                      |
| `WhereInactiveInHierarchy()`          | Include components with inactive `GameObject`s.                    |
| `WhereSiblingIndexIs(int index)`      | Include components whose transform has the given sibling index.    |
| `WhereIsFirstSibling()`               | Include components that are the first sibling.                     |
| `WhereIsLastSibling()`                | Include components that are the last sibling.                      |
| `Where(Func<T,bool> predicate)`       | Include components satisfying a custom predicate.                  |

### Object Filter Methods

Methods in `FilterableObjectBindingBuilder<T> where T : UnityEngine.Object`. Returns itself to allow chaining multiple filters.

| Method                                | Description                                        |
|---------------------------------------|----------------------------------------------------|
| `WhereNameIs(string name)`            | Include objects whose `name` equals `name`.        |
| `WhereNameContains(string substring)` | Include objects whose `name` contains `substring`. |
| `Where(Func<T,bool> predicate)`       | Include objects satisfying a custom predicate.     |
| `WhereIsOfType<TO>()`                 | Include only objects of subtype `TO`.              |

### SerializeInterface

#### Why Unity can’t “serialize an interface”

Serializing an interface in the literal sense doesn't make much sense, because an interface is just a type contract, not a tangible object. Unity’s serializer only stores concrete `UnityEngine.Object` references, so a field typed as an interface has nothing for the serializer to write and gets skipped completely.

#### What the Saneject Roslyn generator adds

For every `[SerializeInterface]` field the generator emits a hidden, serializable backing `Object` in a partial class that implements `ISerializationCallbackReceiver`. The partial class copies the reference into the real interface field after deserialization:

```csharp
// User written class. 
// Needs to be partial for source generator to extend it with another partial.
public partial class SomeClass : MonoBehaviour
{
    [SerializeInterface]
    ISomeInterface someInterface;
}
```

```csharp
// Auto-generated partial.
// Slightly simplified code for demonstration purposes.
public partial class SomeClass : ISerializationCallbackReceiver
{
    [SerializeField, InterfaceBackingField(typeof(someInterface))]
    private Object __someInterface; // Real serialized field

    public void OnAfterDeserialize()
    {
        // When Unity deserialization occurs, the Object is assigned to the actual interface field.
        someInterface = __someInterface as ISomeInterface;        
    }
}
```

In the Inspector this appears as a standard object picker labelled “Some Interface (ISomeInterface)” that be handled manually - or used for dependency injection if you add `[Inject]` alongside `[SerializeInterface]`. If you plug in an `Object` that doesn’t implement the interface, the property drawer rejects it and clears the field.

![Interface field visible in the Inspector](Docs/SerializeInterfaceInspector.webp)

#### InterfaceBackingFieldPropertyDrawer

Formats that label, disables injected fields, and validates the selection.

It also syncs the interface field with the `Object` backing field - which isn’t ideal to do in a `PropertyDrawer`. But the deserialization assignment only goes one way, so if you assign the interface field directly in code, the backing field would not update and the change wouldn't be reflected in the Inspector. So, having the `PropertyDrawer` sync in the other direction was the most practical solution I could come up with that didn't require user boilerplate.

Now, if you assign the interface reference in code, the backing field and Inspector field will reflect the change automatically.

#### SerializeInterfaceInspector

Unity lists fields from generated partials last, which would push every backing field to the bottom of the Inspector.

A custom Inspector `SerializeInterfaceInspector` reorders them so each backing field appears directly beneath its corresponding interface field, preserving the original/intended layout.

> ⚠️ `SerializeInterfaceInspector` draws all `MonoBehaviour` inspectors, which you may find too intrusive and it may override your own custom inspectors. If that’s the case, you can safely delete it and call `SanejectInspectorUtils.DrawAllSerializedFieldsWithCorrectInterfaceOrder(SerializedObject, Object)`  inside your own custom inspector instead.

### Interface Proxy Object

`InterfaceProxyObject<T>` is a Roslyn-generated `ScriptableObject` that:

- Implements every interface on `T` at compile-time.
- Forwards calls, property gets/sets, and event subscriptions to a concrete `T` instance resolved at runtime.

Why? Unity can’t serialize a scene object reference between scenes (or from a scene into a prefab). The proxy asset is serializable, so you assign it in the Editor and at runtime it locates the real instance the first time it’s used.

```mermaid
flowchart TD
  Proxy["GameManagerProxy : IGameManager (ScriptableObject)"]
  PauseMenuUI["PauseMenuUI (Prefab)"]

  subgraph "Scene A"
    GameManager["GameManager : IGameManager (Scene instance)"]
  end

  subgraph "Scene B"
    EnemySpawner["EnemySpawner (Scene instance)"]
  end


  EnemySpawner -->|References IGameManager| Proxy
  PauseMenuUI  -->|References IGameManager| Proxy
  Proxy -->|Forwards calls| GameManager
```

> ⚠️ Last time I checked, Mermaid diagrams don’t render in the GitHub mobile app. Use a browser to view them properly.

#### Creating a proxy

1. Right-click any `MonoScript` that implements one or more interfaces (or code the stub manually as shown below).
2. Choose **Generate Interface Proxy**.
3. Click **Yes** on both dialogs (creates the partial class and an asset instance).

Example:

```csharp
public interface IGameManager 
{
    bool IsGameOver { get; }
    void RestartGame();
}
```

```csharp
public class GameManager : MonoBehaviour, IGameManager
{
    public bool IsGameOver { get; private set; }
    public void RestartGame() { }
}
```

Generated stub (generated once by editor script):

```csharp
[GenerateInterfaceProxy]
public partial class GameManagerProxy : InterfaceProxyObject<GameManager> { }
```

Roslyn-generated proxy forwarding:

```csharp
public partial class GameManagerProxy : IGameManager
{
    public bool IsGameOver
    {
        get
        {
            if (!instance) instance = ResolveInstance();
            return instance.IsGameOver;
        }
    }
    
    public void RestartGame()
    {
        if (!instance) instance = ResolveInstance();
        instance.RestartGame();
    }
}
```

Now drag the `GameManagerProxy` asset into any `[SerializeInterface] IGameManager` field, in any scene or prefab or inject it.

#### Resolve strategies

| Resolve method                    | What it does                                                                                                                                                     |
|-----------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `FromGlobalScope`                 | Pulls the instance from `GlobalScope`. Register it via `RegisterGlobalComponent` / `RegisterGlobalObject` in a `Scope`. No reflection, just a dictionary lookup. |
| `FindInLoadedScenes`              | Uses `FindFirstObjectByType<T>(FindObjectsInactive.Include)` across all loaded scenes.                                                                           |
| `FromComponentOnPrefab`           | Instantiates the given prefab and returns the component.                                                                                                         |
| `FromNewComponentOnNewGameObject` | Creates a new `GameObject` and adds the component.                                                                                                               |
| `ManualRegistration`              | You call `proxy.RegisterInstance(instance)` at runtime before the proxy is used.                                                                                 |

#### Performance note

The proxy resolves on first access and caches the instance. If the instance becomes null on scene load or otherwise, the proxy will try resolving it again on next access.

Each forwarded call includes a null-check, making it roughly 8x slower than a direct call - but we’re talking nanoseconds per call.

In testing, one million proxy calls in one frame to a trivial method cost ~5 ms on a desktop PC.  
If you’re in a **very** tight loop, extract the real instance via `proxy.GetInstanceAs<TConcrete>()` and call it directly.

### Global Scope

The `GlobalScope` is a static service locator that `InterfaceProxyObject` can fetch from at near-zero cost (dictionary lookup).
Use it to register scene objects or assets as cross-scene singletons. The `GlobalScope` can only hold one instance per unique type.

Bindings are added via `RegisterGlobalComponent<T>()` or `RegisterGlobalObject<T>()` inside a `Scope`. This stores the binding into a `SceneGlobalContainer` component.

At runtime, on `Awake()` (with `[DefaultExecutionOrder(-10000)]`), the `SceneGlobalContainer` adds all its references to the `GlobalScope`.

Only one `SceneGlobalContainer` is allowed per scene - it's created automatically during scene injection and manual creation is not allowed.

#### Global Binding Methods

Register global singletons in the `Scope` using the following methods.

| Method                         | Description                                                              |
|--------------------------------|--------------------------------------------------------------------------|
| `RegisterGlobalComponent<T>()` | Adds a scene `Component` to the global scope via `SceneGlobalContainer`. |
| `RegisterGlobalObject<T>()`    | Adds a scene `Object` (e.g., `ScriptableObject`) to the global scope.    |

#### GlobalScope API

| Method                          | Description                                                       |
|---------------------------------|-------------------------------------------------------------------|
| `GlobalScope.Register<T>(T)`    | Registers an instance globally. Only one per type.                |
| `GlobalScope.Get<T>()`          | Returns the registered instance (or `null`).                      |
| `GlobalScope.Unregister<T>()`   | Removes a registered instance of type `T`.                        |
| `GlobalScope.IsRegistered<T>()` | Returns `true` if an instance of type `T` is in the global scope. |
| `GlobalScope.Clear()`           | Clears all global registrations (Play Mode only).                 |

If you're using `InterfaceProxyObject`, global registration is one of the ways to resolve its target instance.

💡 You can toggle logging for global registration under **Saneject → User Settings → Logging**.

### Roslyn Tools in Saneject

Roslyn tools enhance your compile-time experience using C#'s powerful compiler APIs.

- A **Roslyn analyzer** inspects code and reports diagnostics (like errors or suggestions).
- A **Roslyn code fix** provides a quick-fix action for an analyzer error.
- A **Roslyn source generator** adds new C# code to your project during compilation.

Saneject ships with three Roslyn tool DLLs (in `Saneject/RoslynLibs`):

| DLL                                 | Type               | Purpose                                                                                                                                                             |
|-------------------------------------|--------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **SerializeInterfaceGenerator.dll** | Source Generator   | Generates hidden backing fields and serialization hooks for `[SerializeInterface]` members.                                                                         |
| **InterfaceProxyGenerator.dll**     | Source Generator   | Emits proxy classes that forward interface calls to backing fields (via `InterfaceProxyObject<T>`).                                                                 |
| **AttributesAnalyzer.dll**          | Analyzer + CodeFix | Validates field decoration rules for `[Inject]`, `[SerializeField]`, and `[SerializeInterface]`. Includes context-aware quick fixes in supported IDEs (like Rider). |

Unity’s official Roslyn analyzer documentation (including setup instructions):  
<https://docs.unity3d.com/Manual/roslyn-analyzers.html>

Use that guide if you want to plug in custom Roslyn tooling or integrate Saneject’s tools in other project structures.

Roslyn source code is in the [RoslynTools](RoslynTools) folder.

### UX

A few helpful touches built into the inspector experience:

- Injected fields are grayed out to show they’re auto-filled at runtime.
- A single `Scope` component handles both scene and prefab injection. Context is detected automatically.
- The `Scope` type (Scene, Prefab) is shown in its inspector for clarity.
- Interface proxies list which interfaces they implement.
- Help boxes on all Saneject components explain what they do.
- Right-click any `MonoScript` to generate an `InterfaceProxyObject`.
- `Scope` uses `HideFlags.DontSaveInBuild` so it won’t end up in builds by mistake.
- Internal `Scope` methods are hidden from IntelliSense using `[EditorBrowsable(EditorBrowsableState.Never)]`.
- Provides a `[ReadOnly]` attribute (unrelated to DI) you can use to gray out non-DI fields in the Inspector.

### User Settings

Found under **Saneject → User Settings**, these let you customize editor and logging behavior:

- **Injection Prompts**: Ask before scene or prefab injection runs.
- **Inspector Tweaks**: Toggle visibility of injected fields and help boxes.
- **Play Mode Logging**: Log when a proxy resolves or global scope changes.
- **Editor Logging**: Show injection stats, warn about unused bindings, or skipped prefab injection.

## Limitations / Known Issues

- Unity version support: Confirmed working on Unity 6000.0.23f1 LTS and newer. Older versions aren’t currently supported.
    - Uses newer APIs like `FindObjectsByType<T>(FindObjectsInactive, FindObjectsSortMode)`, which don’t exist in older versions.
    - Roslyn source generators didn’t run properly in 2022.2 when tested, despite Unity 2021.1+ claiming support. No deeper investigation has been done yet.
    - Preprocessor fallback paths haven’t been added yet.
- Platform coverage: so far tested on Windows (Mono + IL2CPP) and Android IL2CPP builds only.
- Proxy-creation menu can be flaky. It relies on `SessionState` keys to survive a domain reload, and occasionally Unity clears them before the follow-up dialog appears. If that happens, the `.cs` proxy file is generated but no `.asset` is created, just run **Generate Interface Proxy** again on the script to finish the flow.
- Circular dependency detection is not yet implemented.
- No warnings if a field is marked `[Inject]` but lacks `[SerializeField]` or `[SerializeInterface]`.
- Unity's object picker cannot filter by interface types in the Inspector.
- Collection bindings (e.g. injecting `IEnumerable<T>`) is not supported yet.
- Unity Package Manager (UPM) support is not yet available. Use `.unitypackage` or clone + copy for now. Planned for post-beta once the API and structure stabilize.

## Credits / Contribution

Inspired by:

- [Zenject](https://github.com/modesttree/Zenject)
- [VContainer](https://github.com/hadashiA/VContainer)
- [Serialize Interface Generator](https://github.com/SimonNordon4/serialize-interface-generator)

Contributions are welcome. If you find bugs or have suggestions, feel free to open an issue. PRs are appreciated too. There’s no formal roadmap right now, but I’ll do my best to review what comes in.

## License

MIT License

Copyright (c) 2025 Alexander Larsen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
