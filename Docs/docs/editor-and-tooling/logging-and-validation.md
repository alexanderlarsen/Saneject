---
title: Logging & validation
---

# Logging & validation

Saneject injection is designed to finish a full run and then report everything it found.
Instead of stopping at the first failure, one run gives you a complete list of issues to fix: invalid bindings, missing bindings, missing dependencies, unused bindings, and summary totals.

## Log examples by type

### ❌ Error examples

```text
❌ Saneject: Invalid binding [Binding: BindAsset<PlayerController>() | Scope: GameplayScope] Asset binding type 'PlayerController' derives from Component.
❌ Saneject: Missing binding. Expected something like [Binding: BindComponent<IHealthService>() | Nearest scope: GameplayScope] [Field: Root/Player/PlayerHud/PlayerHud.healthService]
❌ Saneject: Method invocation exception (exception details in next log) [Method: Root/Player/PlayerBootstrap/PlayerBootstrap.Initialize]
```

### ⚠️ Warning examples

```text
⚠️ Saneject: Unused binding [Binding: BindComponent<AudioSource>().FromDescendants() | Scope: GameplayScope]. If you don't plan to use this binding, you can safely remove it.
⚠️ 2 missing binding/dependency errors were suppressed due to [Inject(suppressMissingErrors: true)]. Remove the flag to view detailed logs.
```

### ℹ️ Info examples

```text
ℹ️ Saneject: Created proxy asset at 'Assets/SanejectGenerated/RuntimeProxies/EnemyServiceProxy.asset'.
ℹ️ Saneject: Injection complete | 3 scopes processed | 6 globals registered | 1 proxy swap target registered | 24 fields injected | 3 properties injected | 2 methods injected | 0 missing dependencies | 0 missing bindings | 0 invalid bindings | 1 unused binding | Completed in 18 ms
```

## Where validation and logging happen in the pipeline

For each run, Saneject executes these stages in order:

1. Build the injection graph and apply `ContextWalkFilter`.
2. Build an `InjectionContext` (active transforms, components, scopes, and bindings).
3. Validate bindings.
4. Resolve globals, fields/properties, and methods.
5. Inject resolved values.
6. Collect runtime proxy swap targets.
7. Log errors, warnings, and summary.

Validation runs before resolution. Invalid bindings are reported and then excluded from resolution for that run.

## Binding validation

Validation checks each active binding and records all failures without aborting the run.

Saneject validates:

- Duplicate binding declarations inside the same scope.
- Duplicate global bindings for the same concrete type across scopes.
- Runtime proxy binding requirements (`interface` and `concrete` type required, and single-value only).
- Component binding concrete type must derive from `UnityEngine.Component`.
- Asset binding concrete type must not derive from `UnityEngine.Component`.
- `InterfaceType` must be an interface.
- Concrete type must implement the interface when both are specified.
- A locator strategy must be specified (`From...`).

If a binding fails validation:

- It is logged as an `Invalid binding` error.
- It is excluded from `ValidBindingNodes`.
- The rest of injection still continues.

## What gets logged in a single run

A single run can emit:

- Error logs: invalid binding, missing binding, missing global object, missing dependency, binding filter exception, method invocation exception.
- Warning logs: unused bindings.
- Info logs: created runtime proxy assets and summary logs (if enabled).

Summary behavior:

- Controlled by `Saneject/Settings -> User Settings -> Log Injection Summary`.
- Severity is computed from unsuppressed counts:
  - `Error` if there are missing binding/dependency/invalid binding errors.
  - `Warning` if there are no such errors but there are unused bindings.
  - `Info` otherwise.
- If `[Inject(suppressMissingErrors: true)]` suppresses missing binding/dependency errors, the summary appends a warning line with the suppressed count.

No-scope behavior:

- If zero scopes are processed, Saneject logs that no scopes were found and nothing was injected.

## What gets logged in batch runs

Batch injection adds structure around normal per-asset logs:

1. Batch header for scenes and/or prefabs.
2. Per-asset start/end markers.
3. Normal injection logs inside each asset section, including per-asset summary.
4. Final batch summary section with aggregated scene and prefab summaries.

If the save-scenes prompt is cancelled before batch starts, Saneject logs `Injection cancelled by user.` and exits without injecting.

## Exceptions and flow continuity

Exceptions from user code inside `[Inject]` methods are caught and logged as `Method invocation exception`, followed by the exception details in a separate log entry.
This keeps the injection run moving so later targets can still be processed.

Exceptions thrown by binding dependency filters are also caught and logged as `Binding filter exception`, so one faulty filter does not terminate the run.

## Filter logs from scopes and components

Saneject adds context menu actions that set the Unity Console search query:

- On `Scope`: `Saneject/Filter Logs By Scope Type`
  - Sets search to `Scope: <ScopeTypeName>`.
- On `Scope` and `MonoBehaviour`: `Saneject/Filter Logs By Component Path`
  - Sets search to the full component path, for example `Root/Player/Hud/PlayerHud`.

These filters work well because Saneject log signatures include scope names and full member/component paths.

## Settings that affect logging

In `Saneject/Settings -> User Settings`:

- `Clear Logs On Injection`: clears the console before injection starts.
- `Log Injection Summary`: enables/disables summary logging.
- `Log Unused Bindings`: enables/disables unused binding warnings.

In `Saneject/Settings -> Project Settings`:

- `Use Context Isolation`: when enabled, cross-context candidates are rejected during resolution. This can surface missing dependency errors that include rejected candidate type details.

## Related pages

- [Injection menus](injection-menus.md)
- [Batch injection](batch-injection.md)
- [Context](../core-concepts/context.md)
- [Field, property & method injection](../core-concepts/field-property-and-method-injection.md)
- [Settings](settings.md)
- [Glossary](../reference/glossary.md)
