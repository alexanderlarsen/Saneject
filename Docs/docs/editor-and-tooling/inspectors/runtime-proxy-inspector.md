---
title: Runtime proxy inspector
---

# Runtime proxy inspector

The [runtime proxy inspector](../../reference/glossary.md#runtime-proxy-inspector) is the inspector for generated [runtime proxy](../../reference/glossary.md#runtime-proxy) assets (`RuntimeProxyBase`). It is intentionally read-only. Use it to verify configuration created from your [binding](../../reference/glossary.md#binding) declarations, not to author configuration directly.

For full runtime behavior, see [Runtime proxy](../../core-concepts/runtime-proxy.md).

## What this inspector is for

Use this inspector to verify:

- Which resolve strategy the [runtime proxy](../../reference/glossary.md#runtime-proxy) will use at runtime.
- What [runtime proxy](../../reference/glossary.md#runtime-proxy) instance is resolved at runtime.
- Whether creation-based proxies are `Transient` or `Singleton`.
- Whether `Dont Destroy On Load` is enabled for created objects.
- Which interfaces the [runtime proxy](../../reference/glossary.md#runtime-proxy) type implements.
- In Play Mode, which concrete object instance was resolved.

## Read-only behavior

All fields in this inspector are disabled.
That means you cannot edit `Resolved Instance`, `Resolve Method`, `Prefab`, `Instance Mode`, or `Dont Destroy On Load` here.

The configuration values come from [runtime proxy bindings](../../reference/glossary.md#runtime-proxy-binding) in `Scope.DeclareBindings()` and are assigned when Saneject creates or reuses [runtime proxy](../../reference/glossary.md#runtime-proxy) assets during injection.

To change values:

1. Update the [binding](../../reference/glossary.md#binding) chain in your [scope](../../reference/glossary.md#scope).
2. Run injection again.
3. Find the new generated [runtime proxy](../../reference/glossary.md#runtime-proxy) asset to confirm the changes.

Example [binding](../../reference/glossary.md#binding):

```csharp
using Plugins.Saneject.Experimental.Runtime.Scopes;
using UnityEngine;

public class AudioScope : Scope
{
    [SerializeField] 
    private GameObject audioPrefab;

    protected override void DeclareBindings()
    {
        BindComponent<IAudioService, AudioService>()
            .FromRuntimeProxy()
            .FromComponentOnPrefab(audioPrefab, dontDestroyOnLoad: true)
            .AsSingleton();
    }
}
```

## Inspector fields

### Resolved Instance (Play Mode only)

Displays the runtime object that the [runtime proxy](../../reference/glossary.md#runtime-proxy) resolved to, if any and if the application is running.

### Resolve Method

Matches `RuntimeProxyResolveMethod` and defines how the [runtime proxy](../../reference/glossary.md#runtime-proxy) finds or creates its target at runtime.

Available values:

- `FromGlobalScope`: Looks up an already registered component in `GlobalScope`.
- `FromAnywhereInLoadedScenes`: Finds the first matching component in loaded scenes, including inactive objects.
- `FromComponentOnPrefab`: Instantiates the configured prefab and resolves the target component on that instance.
- `FromNewComponentOnNewGameObject`: Creates a new `GameObject` and adds the target component.

See [Runtime proxy](../../core-concepts/runtime-proxy.md) and [Global scope](../../core-concepts/global-scope.md) for more details.

### Instance Mode

Shown only for creation-based methods:

- `FromComponentOnPrefab`: This method can create new instances, so `Instance Mode` controls whether each resolve creates a new object or reuses one.
- `FromNewComponentOnNewGameObject`: This method also creates instances, so `Instance Mode` controls reuse vs new creation.

Matches `RuntimeProxyInstanceMode`:

- `Transient`: Creates a new instance each time the [runtime proxy](../../reference/glossary.md#runtime-proxy) resolves (creation-based methods only).
- `Singleton`: Reuses one created instance and registers it in `GlobalScope`.

See [Runtime proxy](../../core-concepts/runtime-proxy.md) and [Global scope](../../core-concepts/global-scope.md) for more details.

### Prefab

Shown only for `FromComponentOnPrefab`. This is the prefab that will be instantiated and searched for the target component.

### Dont Destroy On Load

Shown only for creation-based methods:

- `FromComponentOnPrefab`: Controls whether the instantiated prefab object survives scene changes.
- `FromNewComponentOnNewGameObject`: Controls whether the created runtime `GameObject` survives scene changes.

See [Runtime proxy](../../core-concepts/runtime-proxy.md) for more details.

If disabled for a creation-based method, the inspector shows a warning that the created object will be destroyed on scene load.

### Interfaces

Lists the interface names implemented by the [runtime proxy](../../reference/glossary.md#runtime-proxy) type, mirrored from its target. This helps verify what the generated [runtime proxy](../../reference/glossary.md#runtime-proxy) can stand in for at [injection sites](../../reference/glossary.md#injection-site).

## Related pages

- [Runtime proxy](../../core-concepts/runtime-proxy.md)
- [Global scope](../../core-concepts/global-scope.md)
- [Scope inspector](scope-inspector.md)
- [Scope](../../core-concepts/scope.md)
- [Glossary](../../reference/glossary.md)
