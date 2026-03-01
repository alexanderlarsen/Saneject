# Troubleshooting

## `[SerializeInterface]` field is not showing in the Inspector

**Cause:** The class containing the field is not declared `partial`.

**Fix:** Add the `partial` keyword to your class declaration:

```csharp
public partial class MyComponent : MonoBehaviour  // Must be partial
{
    [SerializeInterface]
    private IMyInterface myField;
}
```

The Roslyn generator needs to emit a second partial with the backing field. Without `partial`, compilation will fail with an error.

---

## Injected field is still null after injection

Work through this checklist:

1. **Is a `Scope` present in the hierarchy above the component?** The component must have at least one `Scope` ancestor.
2. **Is a binding declared for the field's type?** Check that `DeclareBindings()` includes a binding that matches the field type (exact type match for concretes, interface type for interfaces).
3. **Does the locator point to the right place?** Verify that the `From*` method matches where the dependency actually lives. For example, `FromTargetSelf()` looks on the injection target's own transform, not the Scope's transform.
4. **Is context isolation filtering it out?** If the dependency is a prefab component and the field is on a scene object (or vice versa), context isolation will reject it. See [Contexts](../core-concepts/contexts.md).
5. **Are qualifiers too narrow?** If the binding uses `ToTarget<T>()` or `ToMember(...)`, verify those match the field's owner type and name exactly.
6. **Check the console.** Saneject logs a message for every missing dependency. The log entry identifies the component, field name, and the scope chain searched.

---

## "Duplicate binding" error in the console

Two bindings in the same `Scope` are considered to conflict if they have the same bound type and overlapping qualifiers. The duplicate is discarded.

Common causes:

- Declaring both `BindComponent<IMyService>()` and `BindComponent<IMyService, MyConcrete>()` in the same Scope — they both resolve `IMyService` with no differentiating qualifier.
- Declaring the same binding twice.

Fix by adding an `ToID`, `ToTarget<T>`, or `ToMember` qualifier to differentiate them, or by removing the duplicate. See [Binding uniqueness](../core-concepts/bindings.md#binding-uniqueness).

---

## Proxy is not resolving its target at runtime

Checklist:

1. **`FromGlobalScope` requires `BindGlobal<T>`** — if the proxy uses `FromGlobalScope`, the target type must be registered globally by a `Scope` using `BindGlobal<T>()`. Verify the scene containing the global registration is loaded before the proxy is first accessed.
2. **`FromAnywhereInLoadedScenes` requires the scene to be loaded** — the scene containing the target must be loaded at the point the proxy is first accessed.
3. **Check Play Mode logging** — enable **Log On Proxy Instance Resolve** in User Settings to see when and whether the proxy is resolving.
4. **Is the proxy asset correctly assigned?** The `[SerializeInterface]` or `[Inject]` field should hold a reference to the generated proxy `ScriptableObject` asset, not `null`.

---

## The Inspector looks wrong after installing Saneject

**Cause:** Another plugin has a `[CustomEditor]` for a base class or type that takes priority over Saneject's fallback inspector. Interface backing fields appear at the bottom, and field ordering is off.

**Fix:** Delegate to `SanejectInspector` from your own editor class. See [Custom inspector](../editor-and-tooling/custom-inspector.md).

---

## Fields on prefab instances get cleared after injecting the scene

This is expected behaviour when context isolation is enabled. Scene injection skips prefab instances entirely — it does not inject or modify them. Fields on prefab instances that were previously injected by a prefab pass remain as they were.

If you see fields being cleared, check whether something else is resetting the prefab instance (e.g. reverting to prefab defaults, reimporting the prefab asset).

---

## `FromAnywhere()` is picking the wrong instance

`FromAnywhere()` uses `FindObjectsByType` and returns the first matching instance (by Unity's default sort order). If multiple instances exist and order matters, use filters or qualifiers to narrow the result:

```csharp
// Only the instance tagged "MainAudio"
BindComponent<AudioService>()
    .FromAnywhere()
    .WhereGameObject(go => go.CompareTag("MainAudio"));

// Only the instance specifically on the Player
BindComponent<AudioService>()
    .FromAnywhere()
    .ToTarget<Player>();
```

---

## New proxy script was generated but injection didn't complete

When `FromRuntimeProxy()` generates a new proxy script for the first time, it triggers a Unity script recompilation. The in-progress injection pass is cancelled because the domain reloads.

**Fix:** Click **Inject** again after recompilation completes. The proxy asset will already exist and no recompilation will be triggered this time.

---

## Injection runs but nothing happens

- Verify you're not in Play Mode — injection is Editor-only and is blocked during Play Mode.
- Verify the correct scene or prefab is open and has `Scope` components in the hierarchy.
- Check that **Clear Logs On Injection** is not hiding relevant output.
