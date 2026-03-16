---
title: Glossary
---

# Glossary

## A

### Assembly proxy manifest

Roslyn-generated, per-assembly list of concrete component types that require runtime proxy script stubs.

### Auto-property backing field

Compiler-generated field that stores an auto-property value. Saneject can inject these fields with `[field: Inject]`.

### Asset binding

Binding declared with `BindAsset...` or `BindAssets...` that resolves `UnityEngine.Object` assets from project content instead of scene or hierarchy components.

## B

### Batch injection

Running injection across multiple selected scene and prefab assets in one operation.

### Batch Injector

Editor window for preparing and running batch injection with per-asset enable state, status, and `ContextWalkFilter`.

### Binding

Instruction declared in a `Scope` that tells Saneject what to resolve, how to inject it, and where to search.

### Binding family

Binding category with its own resolution behavior and API surface: component, global component, asset, or runtime proxy.

### Binding filter

Predicate declared with `Where...` methods that removes candidates after location and before injection.

### Binding qualifier

Optional binding restriction declared with `ToID`, `ToTarget`, or `ToMember`.

## C

### Collection binding

Binding declared as multiple (`BindComponents`/`BindAssets`/`BindMultiple...`) that resolves arrays or `List<>` injection sites.

### Component path

Full hierarchy path plus component type name, for example `Root/Player/HUD/HudController`. Used by Saneject log filtering and context menu actions.

### Component binding

Binding declared with `BindComponent...` or `BindComponents...` that resolves `Component` instances from transforms, hierarchies, scenes, or explicit instances.

### Context

Serialization boundary Saneject uses during injection to decide scope traversal and candidate eligibility.

### Context equality

Rule for determining whether two objects are in the same context. Equality is instance-specific and based on context identity, not just context type.

### Context filtering

Injection-run prefilter that decides which transforms and injection targets are active for a run.

### Context isolation

Project setting (`UseContextIsolation`) that controls whether dependency resolution can cross context boundaries.

### Context walk filter

Filter enum used during graph walk to include specific context sets in a run (`AllContexts`, `SameContextsAsSelection`, `SceneObjects`, `PrefabAssetObjects`, `PrefabInstances`).
 
## D

### Dependency candidate

Object considered during resolution before qualifiers, filters, and assignment determine whether it becomes the final injected dependency.
 
## E

### Editor-time injection

Saneject workflow that resolves dependencies and writes them into serialized objects in the Unity Editor, before entering Play Mode.

## G

### Global component binding

Binding declared with `BindGlobal<TComponent>()` that resolves a component in the editor and stores it on the declaring `Scope` for runtime registration.

### Global registration

Entry added to `GlobalScope` at runtime, keyed by the component's concrete type and owned by the caller that registered it.

### Global scope

Static runtime registry and service locator in Saneject for globally accessible `Component` instances.

## I

### Injection context

Per-run working set built after graph filtering. It contains the active transforms, injection targets, scopes, bindings, resolution maps, and accumulated results for one injection run.

### Injection controls

Foldout section in the `Scope` inspector that runs scene, hierarchy, or prefab injection with specific `ContextWalkFilter` options.

### Injection graph

Hierarchy model built from selected root hierarchies. It contains transform, component, scope, binding, and injection-member information used by the edit-time pipeline.

### Injection menu

Editor command under `Saneject/Inject/...` or `GameObject/Saneject/Inject/...` that starts an injection run with a selected target and `ContextWalkFilter`.

### Injection pipeline

Ordered stages Saneject runs during injection, from graph build and context filtering through validation, resolution, assignment, and summary logging.

### Injection run

One execution of the injection pipeline for a chosen selection, context walk filter, and active set of targets.

### Injection site

Injected field, property or method.

### Injection status

Per-run result classification used by batch injection rows: Unknown, Success, Warning, or Error.

### Injection summary

End-of-run log line that reports processed scope count, injection counts, error/warning counts, and elapsed time.

### Injection target

`Component` with injected fields, properties or methods.

## L

### Linting

Automated static analysis of C# source code in the editor and compiler pipeline. In Saneject docs, this refers to Roslyn analyzer diagnostics.

### Log context

Unity object passed to a log call so clicking the log can highlight or ping the relevant object in the editor.

### Locator strategy

The `From...` part of a binding that defines where candidates are searched or loaded from.

## P

### Prefab asset

Reusable prefab definition in the Project window.

### Prefab instance

Instantiated prefab placed in a scene or nested inside another prefab.

### Project settings

Saneject settings shared at project scope, stored in `ProjectSettings/Saneject/ProjectSettings.json` and available from `Saneject/Settings -> Project Settings`.

### Proxy swap target

Component that implements `IRuntimeProxySwapTarget` and is asked at runtime startup to replace proxy references with resolved real instances.

## R

### Roslyn analyzer

Compile-time code analysis component that inspects C# syntax and symbols and reports diagnostics in the IDE and during builds.

### Roslyn code fix

Quick action linked to a Roslyn analyzer diagnostic that can automatically apply a source-code fix.

### Runtime proxy

`ScriptableObject` placeholder asset (`RuntimeProxy<TComponent>`) injected into interface members at editor time and swapped to the real instance during scope startup.

### Runtime proxy binding

Component binding configured with `FromRuntimeProxy()` that injects a proxy asset at editor time and swaps it for a real runtime instance during scope initialization.

### Runtime proxy inspector

Read-only custom inspector for runtime proxy assets (`RuntimeProxyBase`) that shows resolve configuration, implemented interfaces, and the resolved runtime instance during Play Mode.

### Runtime proxy instance mode

Lifetime policy for creation-based runtime proxy resolution: `Transient` creates per resolve, `Singleton` reuses one instance through `GlobalScope`.

### Runtime proxy resolve method

Strategy used by a runtime proxy to find or create its target instance at runtime, such as `FromGlobalScope` or `FromComponentOnPrefab`.

### Runtime proxy script stub

Auto-generated partial class that inherits `RuntimeProxy<TComponent>`, is marked with `[GenerateRuntimeProxy]`, and is extended by Roslyn-generated interface member stubs.

### Runtime startup

Phase after entering Play Mode when scopes initialize, global registrations are populated, and runtime proxy references can be swapped to real instances.

## S

### Saneject inspector API

Shared static inspector surface (`SanejectInspector`) that draws and validates serialized fields for Saneject-aware inspectors.

### Scene object

Non-prefab `GameObject` in a scene.

### Serialization boundary

Boundary that determines which objects belong to the same serialized context for resolution and injection.

### Scope

`MonoBehaviour` that declares bindings for a part of your hierarchy.

### Scope hierarchy

Tree view in the `Scope` inspector that shows reachable scopes from the current hierarchy root and allows navigation to each scope `GameObject`.

### Service locator

Object or API that lets runtime code request dependencies on demand instead of receiving them through injection. In Saneject docs, this usually refers to `GlobalScope`.

### Serialized interface

`SerializeInterface` member (`IService`, `IService[]`, or `List<IService>`) that Saneject persists through a generated hidden `Object` backing member.

### Suppress missing errors

`InjectAttribute` option that suppresses missing binding and missing dependency error logs for a specific injection site.

### Suppressed error

Missing binding or missing dependency issue hidden from detailed logs by `suppressMissingErrors`, but still counted in summary output.

## U

### User settings

Saneject settings scoped to one Unity user on one machine, stored in `EditorPrefs` and available from `Saneject/Settings -> User Settings`.
