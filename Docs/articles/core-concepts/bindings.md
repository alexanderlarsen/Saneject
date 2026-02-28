# Bindings

To start binding dependencies, create a custom `Scope` and use one of the following `Bind` methods to start a fluent builder.

A few rules:

- **Field types must match bindings**: Interface bindings won't resolve concrete fields, and vice versa.
- **Single vs. collections bindings must match**: Single-value bindings won't resolve collection (list/array) fields, and collection bindings won't resolve single fields. The system validates everything automatically and reports missing/invalid bindings during injection.

You can bind dependencies in three ways, depending on how you want injection to work:

- **Bind by interface**: Matches any object that implements the interface.
- **Bind by concrete type**: Matches only objects of that exact type.
- **Bind an interface to a specific concrete type**: Ensures that only objects of a specific type are used to fulfill an interface.

## Binding API

**TODO – Refactor Binding API docs**

- Stop duplicating API method tables (DocFX handles full reference).
- Convert `Binding API` page into a conceptual guide:
  - Explain binding model (Scope → type → locator → qualifiers → filters).
  - Clarify interface vs concrete vs mapped bindings.
  - Explain single vs collection rules.
  - Describe locator *families* (scope/root/target/explicit/special), not every method.
  - Describe qualifier behavior conceptually.
  - Show a few canonical usage examples.
- Replace long method tables with links to generated API reference.
- Keep this section focused on “how to think about it” and workflow.
- Reduce maintenance surface and avoid drift between guide and API docs.
 
## Binding uniqueness

Each binding you declare in a `Scope` must be unique. If two bindings in the same `Scope` conflict, Saneject will log an error and ignore the duplicate.

A binding is considered unique within a scope based on the following:

- **Bound type**: If an interface type is provided, uniqueness is based only on the interface type (ignoring concrete). If no interface type is provided, uniqueness is based on the concrete type.
- **ID qualifiers**: If set via `.ToId("...")` and matched with `[Inject(Id = "...")]`.
- **Single vs collection**: Whether it's a single-value binding or a collection (`List<T>` or `T[]`).
- **Global flag**: Whether the binding is marked as global.
- **Target qualifiers**: If the binding uses `ToTarget<T>()`.
- **Member-name qualifiers**: If the binding uses `ToMember(...)`.

**Overlap rule (important):**

When determining duplicates, target, member-name, and ID qualifiers are compared by overlap, not by full-set equality.

- **Target qualifiers:** two bindings overlap if any target types are the same or assignable (base/derived).
- **Member-name qualifiers:** two bindings overlap if they share at least one member name.
- **ID qualifiers:** two bindings overlap if they share at least one ID string.
- **Empty qualifier set**: is treated as a generic binding and does not overlap a filtered binding for uniqueness (so a general binding can coexist with a targeted one).

For example, the following two bindings are considered duplicates and will conflict:

```csharp
BindComponent<IMyService>();
BindComponent<IMyService, MyServiceConcrete>();
```

or:

```csharp
BindComponent<MyServiceConcrete>();
BindComponent<MyServiceConcrete>();
```

But the following combinations are allowed:

```csharp
// Interface and direct concrete binding
BindComponent<IMyService>();
BindComponent<MyServiceConcrete>();

// Same interface, but with a unique ID
BindComponent<IMyService>();
BindComponent<IMyService, MyServiceConcrete>().ToID("Secondary");

// Same interface, one single and one collection binding
BindComponent<IMyService>();
BindComponents<IMyService, MyServiceConcrete>();

// Same interface but different target filters
BindComponent<IMyService>().ToTarget<Player>();
BindComponent<IMyService>().ToTarget<Enemy>();

// Same interface but different member name filters
BindComponent<IMyService>().ToMember("serviceA");
BindComponent<IMyService>().ToMember("serviceB");
```

This uniqueness model ensures deterministic resolution and early conflict detection. Duplicate bindings are logged and skipped automatically.