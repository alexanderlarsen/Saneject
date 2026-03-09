---
title: Glossary
---

# Glossary

## B

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

### Context

Serialization boundary Saneject uses during injection to decide scope traversal and candidate eligibility.

### Context equality

Rule for determining whether two objects are in the same context. Equality is instance-specific and based on context identity, not just context type.

### Context filtering

Injection-run prefilter that decides which transforms and injection targets are active for a run.

### Context isolation

Project setting (`UseContextIsolation`) that controls whether dependency resolution can cross context boundaries.

### ContextWalkFilter

Filter enum used during graph walk to include specific context sets in a run (`AllContexts`, `SameContextsAsSelection`, `SceneObjects`, `PrefabAssetObjects`, `PrefabInstances`).

## I

### Injection site

Injected field, property or method.

### Injection target

`Component` with injected fields, properties or methods.

## L

### Locator strategy

The `From...` part of a binding that defines where candidates are searched or loaded from.

## P

### Prefab asset

Reusable prefab definition in the Project window.

### Prefab instance

Instantiated prefab placed in a scene or nested inside another prefab.

## R

### Runtime proxy binding

Component binding configured with `FromRuntimeProxy()` that injects a proxy asset at editor time and swaps it for a real runtime instance during scope initialization.

## S

### Scene object

Non-prefab `GameObject` in a scene.

### Scope

`MonoBehaviour` that declares bindings for a part of your hierarchy.
