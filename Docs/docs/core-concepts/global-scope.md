---
title: Global scope
description: See how GlobalScope provides runtime registration and lookup for globally accessible Unity components that cannot be serialized directly.
---

# Global scope

`GlobalScope` is Saneject's static runtime registry for globally accessible `Component` instances.

Saneject is mostly editor-time DI, but some runtime scenarios still need lookup by type. `GlobalScope` and `RuntimeProxy` covers that gap:

- `BindGlobal<TComponent>()` stores resolved components in a `Scope` at editor-time and registers them at runtime.
- `RuntimeProxy` can resolve through `FromGlobalScope()` for fast lookup.
- You can also call `GlobalScope` directly as a service locator when needed.

> ℹ️ `GlobalScope` is automatically cleared when returning to Edit Mode after Play Mode ends. This prevents stale global registrations from leaking into the next Play session when domain reload is disabled.

## What problem it solves

Unity serialization cannot serialize direct references between certain boundaries:

- Scene ↔ other scene
- Scene ↔ prefab asset
- Prefab asset ↔ other prefab asset

`GlobalScope` gives you a shared runtime lookup point for registered components. You declare those components in a normal `Scope`, then Saneject registers them `GlobalScope` before regular `Awake()` methods run.

This keeps the common workflow explicit:

1. Resolve dependency candidates in the editor through bindings.
2. Persist global candidates on the declaring `Scope`.
3. Register those components into `GlobalScope` at runtime startup.

## Declaring global components with bindings

Use `BindGlobal<TConcrete>()` inside `DeclareBindings()`:

```csharp
using Plugins.Saneject.Runtime.Scopes;

public class BootstrapScope : Scope
{
    protected override void DeclareBindings()
    {
        BindGlobal<AudioManager>()
            .FromScopeSelf();
    }
}
```

`BindGlobal<T>()`:

- Only accepts `Component` types.
- Supports normal `From...` locator methods and `Where...` filters.
- Does not support qualifiers (`ToID`, `ToTarget`, `ToMember`).
- Does not support runtime proxy binding methods.

See [Binding](binding.md) and [Scope](scope.md) for more information.

## Global lifecycle

When you declare `BindGlobal<T>()`, Saneject handles it in two phases.

Editor phase:

1. During injection, Saneject resolves the binding like any other component locator.
2. If multiple candidates are found, the first candidate is selected.
3. The selected component is serialized into the declaring `Scope`'s global components list.

Runtime phase:

1. `Scope.Awake()` runs at execution order `-10000`.
2. The scope registers serialized global components into `GlobalScope`.
3. The same `Awake()` then swaps runtime proxies to real instances.
4. `Scope.OnDestroy()` unregisters the components that this scope owns.

That order matters: global registration runs before proxy swapping, so runtime proxies configured to resolve from `GlobalScope` can do that during startup.

For full proxy behavior, see [Runtime proxy](runtime-proxy.md).

## Using global scope with runtime proxy

A common pattern is:

- In one scope, declare a global component with `BindGlobal<TConcrete>()`.
- In another scope or context, bind an interface through `FromRuntimeProxy().FromGlobalScope()`.

```csharp
using Plugins.Saneject.Runtime.Scopes;

public class BootstrapScope : Scope
{
    protected override void DeclareBindings()
    {
        BindGlobal<GameManager>()
            .FromScopeSelf();
    }
}
```

```csharp
using Plugins.Saneject.Runtime.Scopes;

public class HudScope : Scope
{
    protected override void DeclareBindings()
    {
        BindComponent<IGameManager, GameManager>()
            .FromRuntimeProxy()
            .FromGlobalScope();
    }
}
```

This is usually the intended use of `GlobalScope`: fast runtime lookup for proxy resolution, not a replacement for normal editor-time injection.

For full proxy behavior, see [Runtime proxy](runtime-proxy.md).

## Using global scope directly as a service locator

If you want to avoid runtime proxies, direct usage is available when you need fast runtime lookup outside the injection pipeline:

```csharp
using Plugins.Saneject.Runtime.Scopes;
using UnityEngine;

public class AudioBootstrap : MonoBehaviour
{
    private void Start()
    {
        if (GlobalScope.TryGetComponent<AudioManager>(out AudioManager audio))
            audio.InitializeMixer();
    }
}
```

API overview:

- `GlobalScope.GetComponent<T>()`
- `GlobalScope.TryGetComponent<T>(out T component)`
- `GlobalScope.IsRegistered<T>()`
- `GlobalScope.RegisterComponent(Component instance, Object caller)`
- `GlobalScope.UnregisterComponent(Component instance, Object caller)`
- `GlobalScope.Clear()`

[Full API here](xref:Plugins.Saneject.Runtime.Scopes.GlobalScope)

Use manual `RegisterComponent` and `UnregisterComponent` carefully. Most projects should let `Scope` manage lifecycle through `BindGlobal<T>()`.

## Rules and constraints

- `GlobalScope` stores `Component` instances only.
- Only one registration per concrete component type is allowed at a time.
- Lookup is exact-type by key. If `AudioManager` is registered, `GetComponent<MonoBehaviour>()` does not return it.
- Generic lookup methods require `T : Component`, so interface lookups are not supported.
- Global bindings are validated for unique type ownership. Duplicate `BindGlobal<SameType>()` bindings are invalid.
- Global binding candidate selection still obeys context isolation rules during editor resolution.
- Global bindings are not used to resolve `[Inject]` fields or methods directly. They feed runtime registration, not normal binding resolution.
- Modifying `GlobalScope` (`Register`, `Unregister`, `Clear`) is runtime-only. Calls outside Play Mode are rejected.
- Unregister requires ownership: only the same caller object that registered a type can unregister that type.

## Related pages

- [Scope](scope.md)
- [Binding](binding.md)
- [Runtime proxy](runtime-proxy.md)
- [Context](context.md)
- [Glossary](../reference/glossary.md)

