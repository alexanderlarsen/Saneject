---
title: Feature overview
---

# Feature overview

## Injection & binding

- **Editor-time, deterministic injection:** [Bindings](../reference/glossary.md#binding) are resolved in the editor, stored directly in serialized fields, and everything is wired before runtime.
- **Familiar, fluent [binding](../reference/glossary.md#binding) API:** Saneject's familiar, fluent [binding](../reference/glossary.md#binding) API gives precise control over where dependencies come from and where they may be injected. If you have worked with DI frameworks before, you'll feel right at home.
- **Component and [asset bindings](../reference/glossary.md#asset-binding):** [Component bindings](../reference/glossary.md#component-binding) and [asset bindings](../reference/glossary.md#asset-binding) are both supported, so dependencies can come from hierarchies, scenes, `Resources`, and project folders.
- **Single-value and [collection bindings](../reference/glossary.md#collection-binding):** Single-value and [collection bindings](../reference/glossary.md#collection-binding) (`T[]`, `List<T>`) use the same overall model with a consistent API.
- **Qualifier-based targeting:** Qualifiers with `ToID`, `ToTarget`, and `ToMember` let you narrow a [binding](../reference/glossary.md#binding) to specific [injection sites](../reference/glossary.md#injection-site).
- **Powerful filtering:** `.Where(x => ...)` predicates let you filter dependencies candidates in hierarchies and assets by custom criteria.
- **Recursive [scope](../reference/glossary.md#scope) fallback:** Recursive upwards parent-scope fallback makes it possible to keep local overrides while still sharing common [bindings](../reference/glossary.md#binding) higher in the hierarchy.

## Scenes, prefabs & contexts

- **Context-aware injection:** Saneject treats [scene objects](../reference/glossary.md#scene-object), [prefab instances](../reference/glossary.md#prefab-instance), and [prefab assets](../reference/glossary.md#prefab-asset) as separate [contexts](../reference/glossary.md#context) during injection.
- **Control over what gets processed:** You can choose which [contexts](../reference/glossary.md#context) are processed in an [injection run](../reference/glossary.md#injection-run), which makes focused validation and iteration easier.
- **Control over cross-context resolution:** You can decide whether dependency resolution is allowed to cross [context](../reference/glossary.md#context) boundaries or not.
- **Safer prefab reuse:** These [context](../reference/glossary.md#context) rules help prevent accidental cross-context coupling and make prefab reuse safer across scenes.

## Interfaces & serialization

- **Serializable interface fields and auto-properties:** Unity does not serialize interface members by default, so Saneject adds `[SerializeInterface]` support through Roslyn-generated backing members.
- **Interfaces in the Inspector:** Interface fields remain visible in the Inspector, including arrays and lists of interfaces.
- **Type-safe Inspector assignment:** `[SerializeInterface] IGameObservable gameObservable` will produce a serialized `UnityEngine.Object` field in the Inspector that can only be populated with an `IGameObservable` object.
- **No wrapper classes needed:** Your code stays clean, with no wrapper classes needed just to get interface references into Unity serialization.

## Runtime bridging

- **RuntimeProxy for placeholder injection:** For dependencies Unity cannot serialize (scene ↔ other scene; [prefab asset](../reference/glossary.md#prefab-asset) ↔ other [prefab asset](../reference/glossary.md#prefab-asset); scene ↔ [prefab asset](../reference/glossary.md#prefab-asset)), `RuntimeProxy` can inject a serialized placeholder asset at editor time and swap it to a real instance during early startup, without reflection.
- **[GlobalScope](../reference/glossary.md#global-scope) for fast runtime lookups:** `GlobalScope` is a static runtime registry used for fast type-based lookups, often as a proxy resolve source.
- **Lazy-loaded runtime bridging:** You can think of proxies as a lazy loaded dependency that resolves to a real instance at runtime.
- **Editor-time wiring stays the default:** This keeps your dependency wiring editor-time, while still supporting cross-context runtime cases when you need them.

## Low runtime overhead

- **Fast startup from serialized references:** Dependencies are already resolved and serialized before Play Mode starts.
- **No runtime container initialization:** Saneject does not build or bootstrap a runtime DI container.
- **No reflection-based startup pass:** Dependency wiring does not rely on a reflection-driven initialization step.
- **No second lifecycle on top of Unity's:** The runtime is just normal `Awake`, `Start`, `OnEnable`, and the rest of Unity's lifecycle.
- **Runtime work is limited and optional:** Runtime work is mainly limited to features that intentionally, and optionally, run at startup, such as [global scope](../reference/glossary.md#global-scope) registration and [runtime proxy](../reference/glossary.md#runtime-proxy) swapping to real instances.

## Tooling & validation

- **Contextual toolbar buttons and [injection context menus](../reference/glossary.md#injection-context-menu):** The main toolbar exposes `Inject Scene`, `Inject Selected Scene Hierarchies`, `Inject Prefab Asset`, and `Batch Inject Selected Assets` when they apply, while [injection context menus](../reference/glossary.md#injection-context-menu) let you run more focused scene, hierarchy, and prefab passes with specific [context walk filter](../reference/glossary.md#context-walk-filter) values.
- **[Batch injection](../reference/glossary.md#batch-injection):** [Batch Injector](../reference/glossary.md#batch-injector) can inject all (or selected) scenes and prefabs in a project in one click.
- **Structured logging and validation:** Structured logging and validation with per-run and batch summaries help make dependency validation repeatable and easier to audit.
- **Configurable settings:** Settings cover injection prompts, logging, [context](../reference/glossary.md#context) behavior, and proxy generation.
- **Native Unity UI/UX**: Designed to feel at home in Unity with polished inspectors, contextual menus, and intuitive behavior.

## Code analysis

- **[Roslyn analyzers](../reference/glossary.md#roslyn-analyzer) and code fixes:** Saneject ships [Roslyn analyzers](../reference/glossary.md#roslyn-analyzer) and code fixes for common attribute mistakes, such as invalid `[Inject]` field serialization setup and incorrect `[SerializeInterface]` usage.
- **Issues caught during code analysis:** This catches issues during code analysis instead of waiting for injection-time failures.

## Related pages

- [Introduction](introduction.md)
- [Quick start](quick-start.md)
- [Scope](../core-concepts/scope.md)
- [Binding](../core-concepts/binding.md)
- [Context](../core-concepts/context.md)
- [Field, property & method injection](../core-concepts/field-property-and-method-injection.md)
- [Serialized interface](../core-concepts/serialized-interface.md)
- [Runtime proxy](../core-concepts/runtime-proxy.md)
- [Global scope](../core-concepts/global-scope.md)
- [Injection toolbar & context menus](../editor-and-tooling/injection-toolbar-and-context-menus.md)
- [Batch injection](../editor-and-tooling/batch-injection.md)
- [Logging & validation](../editor-and-tooling/logging-and-validation.md)
- [Settings](../editor-and-tooling/settings.md)
- [Code analyzers](../editor-and-tooling/code-analyzers.md)
- [Glossary](../reference/glossary.md)
