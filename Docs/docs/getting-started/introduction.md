---
title: Introduction
---

# Introduction

Saneject is an [editor-time injection](../reference/glossary.md#editor-time-injection) framework for Unity. You declare [bindings](../reference/glossary.md#binding) in [Scope](../reference/glossary.md#scope) components, run an [injection run](../reference/glossary.md#injection-run) in the editor, and Saneject writes resolved dependencies directly into serialized members in scenes and prefabs. This keeps dependency wiring close to Unity's native serialization model: dependencies remain visible in the Inspector, runtime startup stays lightweight, and gameplay code can still be structured around interfaces and explicit dependencies.

## What Saneject is optimized for

Saneject is designed for teams that want DI structure without giving up Unity's serialized workflows.

- Keep dependencies explicit in serialized data and in the Inspector.
- Use interfaces in gameplay code while still working with [serialized interfaces](../reference/glossary.md#serialized-interface).
- Use structured, deterministic [injection runs](../reference/glossary.md#injection-run) across scenes and prefabs.
- Control large injection passes from editor tooling instead of runtime bootstrap code.
- Keep runtime overhead low for dependency wiring.

## How it works

During an [injection run](../reference/glossary.md#injection-run), Saneject resolves dependencies in the editor and writes values directly into serialized members, including `[Inject]` fields, `[field: Inject]` [auto-property backing fields](../reference/glossary.md#auto-property-backing-field), and methods marked with `[Inject]`.

1. You mark some fields, properties, or methods with `[Inject]`.
2. You declare [bindings](../reference/glossary.md#binding) in `Scope.DeclareBindings()` to describe where dependencies should come from.
3. You run injection in the editor for the scene, prefab, or selection you want to process.
4. Saneject builds an [injection graph](../reference/glossary.md#injection-graph), validates [bindings](../reference/glossary.md#binding), resolves dependencies, writes the results into serialized members, and logs a summary for the run.

## When Saneject is a good fit

Saneject is a good fit when you want deterministic, editor-driven dependency wiring that behaves like normal Unity serialization at runtime. It is especially useful when your project needs interface-heavy gameplay code, clear Inspector-visible dependencies, or repeatable validation across scenes and prefabs.

The tradeoff is that dependency wiring becomes part of the authoring workflow. Instead of relying on a runtime container to compose everything dynamically at startup, you run injection ahead of time and treat serialized references as the runtime result.

## Related pages

- [Feature overview](feature-overview.md)
- [Installation & requirements](installation-and-requirements.md)
- [Quick start](quick-start.md)
- [Scope](../core-concepts/scope.md)
- [Binding](../core-concepts/binding.md)
- [Context](../core-concepts/context.md)
- [Field, property & method injection](../core-concepts/field-property-and-method-injection.md)
- [Serialized interface](../core-concepts/serialized-interface.md)
- [Glossary](../reference/glossary.md)
