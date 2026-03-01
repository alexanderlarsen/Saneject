# Sample game

Saneject ships with a demo game that demonstrates common patterns in a real (small) project. It uses two scenes, proxies, GlobalScope, scoped hierarchies, interface injection, and method injection.

## What it demonstrates

- **Multi-scope hierarchy** — a root scene scope with nested player and enemy scopes, each with their own bindings.
- **Interface injection** — `IGameStateObservable`, `IEnemyObservable`, `ICameraFollowTarget`, and others are injected via `[SerializeInterface]` fields.
- **GlobalScope** — `Player`, `EnemyManager`, `ScoreManager`, `CameraController`, and `GameStateManager` are promoted to the global scope so they can be resolved by proxies from the UI scene.
- **RuntimeProxy** — `GameStateManagerProxy`, `EnemyManagerProxy`, `ScoreManagerProxy`, `CameraControllerProxy`, and `PlayerProxy` are used by the UI scene to reference game scene objects without hard scene dependencies.
- **Cross-scene wiring** — the game runs across two additive scenes (`GameScene` and `UIScene`), wired together via proxies.
- **Asset binding** — the enemy prefab is injected into `EnemyManager` using `BindAsset<GameObject>().FromAssetLoad(...)`.

## How to find it

| Install method | Location |
|---|---|
| Unity package | `Assets/Plugins/Saneject/Samples/DemoGame` |
| UPM | Import via **Package Manager → Saneject → Samples → Demo Game** |

## How to run it

1. Open `Samples/DemoGame/Scenes/GameScene.unity`.
2. Make sure `UIScene.unity` is added to your Build Settings or opened additively.
3. Press Play.

The demo runs without any manual injection step — scenes are pre-injected and saved in the repository.

To experiment with the bindings, open a scene, modify a `Scope`, and re-inject using **Saneject → Inject Scene Dependencies**.
