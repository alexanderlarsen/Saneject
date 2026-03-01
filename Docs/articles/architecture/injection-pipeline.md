# Injection pipeline

When you trigger injection — from a Scope inspector, the hierarchy menu, or the Batch Injector — Saneject runs a fixed sequence of steps. Each step is editor-only. Nothing runs at runtime.

## Phases

### 1. Graph build

Saneject constructs an `InjectionGraph` from the starting transforms. The graph represents the full hierarchy as a tree of `TransformNode`s. Each node holds references to the `Component`s on that transform, and those components are inspected for `[Inject]` fields, `[Inject]` methods, and `Scope` components.

### 2. Context filtering

A walk filter (`ContextWalkFilter`) is applied to the graph based on the injection mode:

- **Scene pass** — only scene transforms are active. Prefab instance roots and everything below them are excluded.
- **Prefab pass** — only the targeted prefab's hierarchy is active. Scene transforms are excluded.

This produces the set of `TransformNode`s that will actually be processed.

### 3. Binding collection and validation

`BindingValidator` calls `CollectBindings()` on each `Scope` node in the active graph, which internally calls `DeclareBindings()` on the user's `Scope` subclass. The resulting `Binding` objects are validated for uniqueness. Duplicate or conflicting bindings are logged and the duplicate is discarded.

### 4. Resolution

`Resolver` walks every `FieldNode` and `MethodParameterNode` in the active graph and finds a matching binding for each. For each `[Inject]` field:

1. The resolver finds the nearest `Scope` above the injection target.
2. It looks for a binding whose type and qualifiers (ID, target type, member name) match the field.
3. If no match is found, it bubbles up through parent `Scope`s.
4. The matching binding's locator is executed to find the actual `Component` or asset.
5. Any filters are applied to the candidate set.
6. The result is stored in the `InjectionContext`.

All resolution happens in a single pass. Errors are collected non-blocking — the entire graph is processed before any logging occurs.

### 5. Injection

`Injector.InjectDependencies` writes the resolved values from the `InjectionContext` into the serialized fields on each `Component`, using the Unity `SerializedObject` / `SerializedProperty` API so changes are properly recorded and dirty-marked.

Method injection runs after all fields have been assigned.

Global bindings (`BindGlobal<T>`) are also written at this stage — the resolved components are stored in the `SceneGlobalContainer` on the Scope's `GameObject`.

### 6. Proxy swap target collection

`ProxySwapTargetCollector` registers any components that implement `IRuntimeProxySwapTarget` with the nearest `Scope`. At runtime on `Awake`, the Scope calls `SwapProxiesWithRealInstances()` on each of them.

### 7. Logging

`Logger.LogResults` emits one message per error or warning collected during resolution. `Logger.LogSummary` then emits a single summary line with counts for scopes processed, fields injected, methods called, globals added, unused bindings, and suppressed errors.

## Scene vs prefab pass

A full "inject scene" operation runs two separate passes:

1. **Scene pass** — injects all scene objects, skipping prefab instances entirely.
2. **Prefab pass** — for each prefab instance found in the scene, runs a separate isolated injection against that prefab's hierarchy.

This is why `Scope` components on prefab instances are never processed during the scene pass — they get their own dedicated pass.

## Non-blocking error model

All errors are collected into the `InjectionContext` during resolution and emitted at the end of the pass. The pipeline never aborts early on a single missing binding or unresolved field. You always get a complete picture of everything that failed in a single run.
