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

## I

### Injection site

Injected field, property or method.

### Injection target

`Component` with injected fields, properties or methods.

## L

### Locator strategy

The `From...` part of a binding that defines where candidates are searched or loaded from.

## R

### Runtime proxy binding

Component binding configured with `FromRuntimeProxy()` that injects a proxy asset at editor time and swaps it for a real runtime instance during scope initialization.

## S

### Scope

`MonoBehaviour` that declares bindings for a part of your hierarchy.
