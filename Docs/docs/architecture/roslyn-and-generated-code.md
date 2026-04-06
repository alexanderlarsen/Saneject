---
title: Roslyn & generated code
---

# Roslyn & generated code

This page describes the Roslyn/code-generation layer in Saneject. It is a cross-cutting architecture layer that supports both edit-time injection and runtime startup behavior.

Saneject uses this layer to close three gaps:

- Unity cannot serialize interface-typed members directly.
- Runtime proxy types and stubs must be generated from binding usage.
- Some injection misuse should be rejected during compile/IDE analysis, before injection runs.

## Layer boundaries

The Roslyn/code-generation layer is not the injection pipeline itself and not a runtime container. It provides generated types and generated members that the editor pipeline and runtime startup consume.

At a high level:

1. Roslyn analyzes source and emits generated outputs.
2. Unity imports and compiles those outputs.
3. Editor pipeline and runtime startup operate on the generated artifacts.

## Assemblies and loading model

Saneject ships Roslyn assemblies in `Assets/Plugins/Saneject/Roslyn`:

- `Saneject.SerializeInterface.Generator.dll`
- `Saneject.RuntimeProxy.Generator.dll`
- `Saneject.Analyzers.dll`

These assemblies are imported as Roslyn analyzers, so they run in the C# compilation/analyzer pipeline instead of gameplay runtime code paths.

## SerializeInterface generation

The `SerializeInterface` generator enables interface members to participate in Unity serialization by generating partial class members.

Generated output includes:

- Hidden serialized backing fields (`Object`, `Object[]`, or `List<Object>`)
- Synchronization methods (`OnBeforeSerialize()` and `OnAfterDeserialize()`)
- `SwapProxiesWithRealInstances()` implementation for single interface members

Architectural effect:

- Edit-time injection can write stable serialized data for interface members.
- Runtime startup can swap proxy placeholders to real instances without reflection-heavy graph traversal logic.

See [Serialized interface](../core-concepts/serialized-interface.md) for details.

## Runtime proxy generation

Runtime proxy generation is a multi-stage pipeline across Roslyn and Unity Editor:

1. Roslyn inspects bindings and emits an assembly-level manifest of concrete proxy target types.
2. On domain reload, editor utilities read that manifest and generate missing proxy script stubs.
3. Roslyn generates partial implementations so each proxy type implements the target component's public non-generic interfaces.

At injection time, the pipeline uses those generated proxy types to resolve/reuse/create proxy assets. At runtime, generated swap code and proxy code finalize references during startup.

See [Runtime proxy](../core-concepts/runtime-proxy.md) for details.

## Roslyn analyzers

Analyzers run in IDE/compile analysis and report invalid patterns before injection starts.

Current analyzer scope focuses on injection/serialization correctness, including:

- `[Inject]` fields that Unity will not serialize correctly
- Invalid `[SerializeInterface]` usage on non-interface shapes

Architectural effect:

- Some validation shifts from injection-time logs to compile-time diagnostics.
- Developers get earlier feedback, reducing failed injection passes caused by static code issues.

See [Code analyzers](../editor-and-tooling/code-analyzers.md) for details.

## Edit-time and runtime touchpoints

Edit-time architecture depends on generated outputs for:

- Serializing interface dependencies
- Creating and resolving proxy assets
- Reducing invalid input through analyzer diagnostics

Runtime architecture depends on generated outputs for:

- Proxy swap target methods (`SwapProxiesWithRealInstances()`)
- Generated proxy interface implementations used during startup resolution
 
## Related pages

- [Architecture overview](architecture-overview.md)
- [Edit-time architecture](edit-time-architecture.md)
- [Runtime architecture](runtime-architecture.md)
- [Serialized interface](../core-concepts/serialized-interface.md)
- [Runtime proxy](../core-concepts/runtime-proxy.md)
- [Code analyzers](../editor-and-tooling/code-analyzers.md)
- [Glossary](../reference/glossary.md)
