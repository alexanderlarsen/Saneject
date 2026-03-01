# Key concepts

A quick-reference glossary for the terms used throughout the docs. Each entry links to the article that covers it in full.

---

**[Scope](core-concepts/scopes.md)**
A `MonoBehaviour` that declares where dependencies come from. You create a subclass, override `DeclareBindings()`, and place it on a `GameObject` in the scene or prefab. The Scope is the central configuration point for an injection pass.

---

**[Binding](core-concepts/bindings.md)**
A declaration inside a `Scope` that connects a type to a locator strategy. A binding says: *"when something needs a `T`, here's how to find it."* Each binding is configured with a fluent builder that specifies a locator, optional qualifiers, and optional filters.

---

**[Locator](core-concepts/bindings.md#locator-families)**
The part of a binding that says *where* to search. Locators are either hierarchy-based (searching relative to the Scope, the injection target, or a custom transform) or special (scene-wide search, direct instance, delegate). Asset bindings have their own locators for Resources, folders, and the AssetDatabase.

---

**[Qualifier](core-concepts/bindings.md#qualifiers)**
An optional restriction on a binding that narrows which fields or methods it applies to. Qualifiers let you have multiple bindings of the same type in a Scope by scoping each one to a specific ID (`ToID`), target component type (`ToTarget<T>`), or member name (`ToMember`).

---

**[Filter](core-concepts/bindings.md#filters)**
An optional predicate applied to the candidates found by the locator. Filters let you express conditions like *"only the component whose parent is tagged 'Player'"* or *"only assets whose name starts with 'Enemy'"*.

---

**[Context](core-concepts/contexts.md)**
An isolation boundary for injection. By default, a scene, each prefab instance, and each prefab asset are treated as separate contexts. Dependencies can only be resolved within the same context, which prevents Unity serialization issues and keeps prefabs self-contained. Use `RuntimeProxy<T>` to bridge contexts intentionally.
