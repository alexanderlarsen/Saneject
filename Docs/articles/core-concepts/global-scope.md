# GlobalScope

`GlobalScope` is a static runtime service locator. It holds one registered instance per type and makes those instances accessible from anywhere in code without a direct object reference.

It's primarily used as the resolution target for `RuntimeProxy<T>` bindings configured with `.FromGlobalScope()`, but can also be accessed directly.

## How it gets populated

`BindGlobal<T>()` in a `Scope` registers a component for global promotion. At injection time, Saneject writes the resolved component into a `SceneGlobalContainer` — a hidden component created automatically on the Scope's `GameObject`. On `Awake` (with `DefaultExecutionOrder(-10000)`), `SceneGlobalContainer` calls `GlobalScope.RegisterComponent` for each stored instance.

```csharp
public class GameSceneScope : Scope
{
    protected override void DeclareBindings()
    {
        // Finds GameStateManager in the hierarchy and registers it globally at runtime
        BindGlobal<GameStateManager>()
            .FromScopeDescendants();
    }
}
```

Only one `SceneGlobalContainer` is allowed per scene — it's created automatically and manual creation is blocked. If two components of the same type are registered, the second registration fails and an error is logged. The original remains registered.

On scene unload (or `OnDestroy`), the Scope unregisters all its components from the `GlobalScope` automatically.

## Runtime API

`GlobalScope` is Play Mode only. Calling it in Edit Mode logs a warning (unless overridden for testing).

| Method | Description |
|---|---|
| `GlobalScope.GetComponent<T>()` | Returns the registered instance of `T`, or `null` if none is registered. |
| `GlobalScope.TryGetComponent<T>(out T component)` | Safe get; returns `true` if found. |
| `GlobalScope.IsRegistered<T>()` | Returns `true` if an instance of `T` is currently registered. |
| `GlobalScope.RegisterComponent(instance, caller)` | Registers an instance. `caller` is used for error reporting. |
| `GlobalScope.UnregisterComponent(instance, caller)` | Unregisters a specific instance. |
| `GlobalScope.Clear()` | Clears all registrations. Useful in test teardown. |

## Relationship to proxies

`RuntimeProxy<T>` configured with `.FromRuntimeProxy().FromGlobalScope()` resolves its target by calling `GlobalScope.GetComponent<T>()` at the moment it's first accessed. This is the recommended approach when you need cross-scene references — it's a simple dictionary lookup with no reflection.

```csharp
// In the game scene's Scope — register GameStateManager globally
BindGlobal<GameStateManager>()
    .FromScopeDescendants();

// In the UI scene's Scope — inject a proxy that resolves from GlobalScope
BindComponent<IGameStateObservable, GameStateManager>()
    .FromRuntimeProxy()
    .FromGlobalScope();
```

The proxy in the UI scene holds a reference to `GameStateManagerProxy`. At runtime, when the proxy is first accessed, it calls `GlobalScope.GetComponent<GameStateManager>()` and caches the result.

## Logging

Registration and unregistration events can be logged during Play Mode via **Saneject → User Settings → Log Global Scope Register/Unregister**.
