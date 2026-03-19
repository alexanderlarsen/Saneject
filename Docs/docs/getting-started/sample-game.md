---
title: Sample game
---

# Sample game

The sample game is a compact reference project that shows how Saneject is used in a real Unity setup.

You control a green player character. Red enemies wander around the map and flee when you get close. Catch them all to end the round, then restart from the game-over UI.

![Screenshot of Saneject sample game](../../images/sample-game-screenshot.webp)

> Some older Unity versions (for example `2022.3.12f1`) can lose script references when importing samples from Package Manager.
> If that happens, right-click the imported `Samples` folder and choose **Reimport**.
> Reference discussion: https://discussions.unity.com/t/broken-script-references-on-updating-custom-package-through-package-manager-and-committing-it-to-git/910632/7

The sample intentionally keeps gameplay simple so you can focus on dependency structure:

- **Multiple [scope](../reference/glossary.md#scope) levels:** bootstrap, scene-wide, object-local, and prefab-local [bindings](../reference/glossary.md#binding).
- **Interface-first wiring:** most systems communicate through interfaces instead of hard references.
- **Global gameplay registrations:** the player, camera controller, enemy manager, score manager, and game-state manager are registered through `BindGlobal<T>()`.
- **Runtime proxy bridges:** UI and prefab systems consume scene-owned services through [runtime proxy bindings](../reference/glossary.md#runtime-proxy-binding).
- **Runtime-created service:** the sample's `ISceneManager` is created from a runtime proxy binding with `FromNewComponentOnNewGameObject().AsSingleton()`.
- **UI integration:** HUD, game-over UI, and enemy markers react to gameplay state through interfaces.

## Where to find it

| Install method                  | Location                                     |
|---------------------------------|----------------------------------------------|
| Unity package                   | `Assets/Plugins/Saneject/Samples~/DemoGame`  |
| Imported Package Manager sample | `Assets/Samples/Saneject/<version>/DemoGame` |

## How to run the sample

1. Add scenes to Build Settings in this order:
    - `StartScene`
    - `GameScene`
    - `UIScene`
2. Open `StartScene`.
3. Enter Play Mode.

## Scene flow

The sample uses a small bootstrap scene plus two additive runtime scenes:

- `StartScene`: bootstrap scene that contains `BootstrapScope` and `Bootstrapper`.
- `GameScene`: gameplay systems (player, enemies, score, game state, camera).
- `UIScene`: HUD and game-over UI.

At runtime:

1. `Bootstrapper` resolves `ISceneManager`.
2. The `ISceneManager` implementation loads `GameScene` and `UIScene` additively.
3. `StartScene` is unloaded.
4. Restart loads `StartScene` again with `LoadSceneMode.Single`.

## Scope layout and responsibilities

The sample demonstrates [scope](../reference/glossary.md#scope) composition at multiple levels:

- `BootstrapScope`: declares the bootstrap binding that creates the runtime `ISceneManager`.
- `GameSceneScope`: declares scene-level gameplay [bindings](../reference/glossary.md#binding), [global registrations](../reference/glossary.md#global-registration), and the enemy prefab asset binding.
- `PlayerScope`: declares player-local [bindings](../reference/glossary.md#binding) such as movement dependencies.
- `EnemyScope`: declares per-enemy prefab [bindings](../reference/glossary.md#binding) and runtime proxy access to the player and camera.
- `UISceneScope`: declares UI-side [bindings](../reference/glossary.md#binding), including runtime proxies to gameplay systems and the restart scene manager.

This layout shows the core Saneject rule in practice: each `Scope` owns [bindings](../reference/glossary.md#binding) for its local part of the hierarchy, while parent [scopes](../reference/glossary.md#scope) provide fallback when local [bindings](../reference/glossary.md#binding) do not match.

See [Scope](../core-concepts/scope.md) and [Binding](../core-concepts/binding.md).

## Binding patterns shown by the sample

### 1. Bootstrap-created runtime service

`BootstrapScope` binds `ISceneManager` through a runtime proxy that creates the concrete component on demand:

```csharp
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.SceneManagement;

public class BootstrapScope : Scope
{
    protected override void DeclareBindings()
    {
        BindComponent<ISceneManager, SceneManager>()
            .FromRuntimeProxy()
            .FromNewComponentOnNewGameObject()
            .AsSingleton();
    }
}
```

Why this matters:

- The sample is not limited to resolving pre-existing scene objects.
- `SceneManager` is created at runtime, cached as a singleton, and then reused through the same interface from other [contexts](../reference/glossary.md#context).
- This shows the creation-based side of [runtime proxy](../reference/glossary.md#runtime-proxy) bindings, not just lookup-based proxies.

See [Runtime proxy](../core-concepts/runtime-proxy.md).

### 2. Scene-level composition and globals

`GameSceneScope` declares both normal [bindings](../reference/glossary.md#binding) and [global registrations](../reference/glossary.md#global-registration):

```csharp
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems;
using UnityEngine;

public class GameSceneScope : Scope
{
    protected override void DeclareBindings()
    {
        BindGlobal<Player>()
            .FromScopeDescendants();

        BindGlobal<EnemyManager>()
            .FromScopeDescendants();

        BindGlobal<ScoreManager>()
            .FromScopeDescendants();

        BindGlobal<CameraController>()
            .FromScopeDescendants();

        BindGlobal<GameStateManager>()
            .FromScopeDescendants();

        BindComponent<ICameraFollowTarget, Player>()
            .FromScopeDescendants();

        BindComponent<IScoreUpdater, ScoreManager>()
            .FromScopeDescendants();

        BindComponent<IEnemyObservable, EnemyManager>()
            .FromScopeDescendants();

        BindComponent<Camera>()
            .FromAnywhere();

        BindAsset<GameObject>()
            .ToTarget<EnemyManager>()
            .ToMember("enemyPrefab")
            .FromAssetLoad("Assets/Plugins/Saneject/Samples/DemoGame/Prefabs/Enemy.prefab");
    }
}
```

Why this matters:

- Gameplay systems in `GameScene` resolve dependencies directly with component and [asset bindings](../reference/glossary.md#asset-binding).
- UI and prefab [contexts](../reference/glossary.md#context) can resolve the player, camera, score manager, enemy manager, and game-state manager at runtime through `GlobalScope`.
- The sample shows both local scene wiring and the global-registration pattern in the same scope.

See [Global scope](../core-concepts/global-scope.md).

### 3. Runtime proxy bridges for UI and enemy prefabs

UI systems and enemy prefab systems consume gameplay interfaces through [runtime proxies](../reference/glossary.md#runtime-proxy):

```csharp
using Plugins.Saneject.Runtime.Scopes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Camera;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.PlayerSystems;
using Plugins.Saneject.Samples.DemoGame.Scripts.UI.Enemy;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScope : Scope
{
    protected override void DeclareBindings()
    {
        BindComponent<IEnemyEvadeTarget, Player>()
            .FromRuntimeProxy()
            .FromGlobalScope();

        BindComponent<IMainCamera, CameraController>()
            .FromRuntimeProxy();
    }
}
```

The UI scene uses the same pattern for `IGameStateObservable`, `IScoreObservable`, `IEnemyObservable`, and `ISceneManager`.

Why this matters:

- Enemy prefabs and UI components stay decoupled from direct scene references.
- The sample demonstrates both explicit `FromGlobalScope()` and the default behavior when `FromRuntimeProxy()` is used without a follow-up resolve method.
- The shipped sample also includes pre-generated proxy assets under `DemoGame/Proxies`, so you can inspect the concrete proxy setup directly.

See [Runtime proxy](../core-concepts/runtime-proxy.md) and [Context](../core-concepts/context.md).

### 4. Interface injection in gameplay and UI

Most sample systems depend on interfaces, not concrete classes. That is why interface fields use `[SerializeInterface]`.

```csharp
using Plugins.Saneject.Runtime.Attributes;
using Plugins.Saneject.Samples.DemoGame.Scripts.Enemies;
using Plugins.Saneject.Samples.DemoGame.Scripts.GameState;
using Plugins.Saneject.Samples.DemoGame.Scripts.Highscore;
using Plugins.Saneject.Samples.DemoGame.Scripts.UI.MVC;

public partial class HUDController : ControllerBase<HUDView>
{
    [Inject, SerializeInterface]
    private IGameStateObservable gameStateObservable;

    [Inject, SerializeInterface]
    private IScoreObservable scoreObservable;

    [Inject, SerializeInterface]
    private IEnemyObservable enemyObservable;
}
```

This gives you:

- Decoupled systems that are easier to replace and test.
- [Serialized interface](../reference/glossary.md#serialized-interface) references that persist in scenes and prefabs.
- Automatic proxy swap support for single interface members when [runtime proxy bindings](../reference/glossary.md#runtime-proxy-binding) are used.
- The same pattern is used in `GameOverController`, which also injects `ISceneManager` for restart.

See [Field, property & method injection](../core-concepts/field-property-and-method-injection.md) and [Serialized interface](../core-concepts/serialized-interface.md).

## Game loop wiring shown by the sample

The game loop is connected through interface events and injected collaborators:

1. `Bootstrapper` resolves `ISceneManager` and starts the additive scene flow.
2. `EnemyManager` spawns enemies and tracks the active set.
3. Each `Enemy` raises `OnEnemyCaught` when the player collides with it.
4. `ScoreManager` adds points for each caught enemy.
5. `GameStateManager` monitors the remaining enemy count and emits game over when the count reaches zero.
6. `HUDController` updates the live HUD, while `GameOverController` shows the final summary and restart button.
7. Restart uses the same injected `ISceneManager`, which reloads `StartScene`.

Separately, `EnemyMarker` uses runtime proxy access to the player and camera so it can clamp off-screen enemy indicators without direct cross-context references.

The important part is not the gameplay logic itself. The important part is that each step is wired through scoped [bindings](../reference/glossary.md#binding), interface contracts, and runtime proxy boundaries instead of direct [scene object](../reference/glossary.md#scene-object) references.

## What to study first in the sample

If you are new to Saneject, inspect these in order:

1. Bootstrap flow: `BootstrapScope`, `Bootstrapper`, `SceneManager`.
2. Scene and prefab [scopes](../reference/glossary.md#scope): `GameSceneScope`, `UISceneScope`, `PlayerScope`, `EnemyScope`.
3. Interface contracts: `IEnemyObservable`, `IScoreObservable`, `IGameStateObservable`, `ISceneManager`, `IMainCamera`.
4. UI systems: `HUDController`, `GameOverController`, `EnemyMarker`.
5. Gameplay services and entities: `EnemyManager`, `ScoreManager`, `GameStateManager`, `Enemy`, `Player`.

This path gives you the fastest overview of how [bindings](../reference/glossary.md#binding), [scopes](../reference/glossary.md#scope), and [runtime proxy](../reference/glossary.md#runtime-proxy) features fit together.

## Related pages

- [Quick start](quick-start.md)
- [Scope](../core-concepts/scope.md)
- [Binding](../core-concepts/binding.md)
- [Context](../core-concepts/context.md)
- [Field, property & method injection](../core-concepts/field-property-and-method-injection.md)
- [Serialized interface](../core-concepts/serialized-interface.md)
- [Runtime proxy](../core-concepts/runtime-proxy.md)
- [Global scope](../core-concepts/global-scope.md)
- [Glossary](../reference/glossary.md)
