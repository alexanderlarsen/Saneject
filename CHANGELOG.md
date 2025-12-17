# Changelog

## Version 0.21.0

### Context isolation overhaul

- Replaced the previous context filtering implementation with a clearer, stricter and fully deterministic model.
- Context isolation now applies consistently to **dependency resolution, hierarchy traversal, scope parent resolution and object injection**.
- Scenes, prefab instances and prefab assets are treated as **hard-isolated contexts** when filtering is enabled.
- Each prefab instance in a scene is its own context, even when multiple instances come from the same prefab asset.
- Nested prefabs are isolated from their parents and siblings, regardless of prefab source.
- Scene injection fully ignores prefab instances, prefab components and prefab scopes.
- Prefab injection fully ignores scene objects, scene components and scene scopes.
- Scene objects nested under prefabs are now injected correctly during scene injection.
- Prefab instances no longer partially see scene scopes or bindings.
- Fixed multiple edge cases where injection traversal stopped incorrectly or leaked across boundaries.

### Dependency resolution improvements

- Dependency candidates are now filtered strictly by context before assignment.
- Rejected candidates are collected and reported together in a single, contextual error message.
- ScriptableObjects and other non-GameObject assets remain context-agnostic and can resolve from any context.
- Method injection fully respects the same context filtering rules as field injection.

### Scope resolution changes

- Scope parent resolution now respects context isolation.
- Parent scopes are only assigned if they exist within the same context.
- Scene scopes nested under prefabs correctly resolve to the nearest valid scene scope ancestor.
- Prefab scopes no longer inherit parent scopes across prefab or scene boundaries.

### Scope inspector improvements

- Scope inspector now displays the full scope hierarchy path with a clear visual indication of context boundaries.
- Out-of-context scopes are dimmed and include tooltips explaining why they are ignored during injection.
- Improved multi-selection handling, including mixed scene- and prefab selection states.
- Shows how many hierarchies or prefabs will be affected by the current injection operation. 

### Tests

- Refactored and expanded dependency context filtering tests to cover scenes, prefab instances, prefab assets, scriptable objects and method injection.
- Added various scope context isolation tests covering scene injection, prefab injection, nested prefabs, sibling prefabs and mixed hierarchies.
- Added scope parent resolution tests validating correct behavior across complex alternating scene- and prefab hierarchies.
- Adjusted and fixed existing tests to reflect the new, stricter isolation rules.

### Summary

This release hardens Saneject's context isolation model. Injection behavior is now consistent, predictable and aligned with Unity's serialization constraints, eliminating multiple edge cases and making scene and prefab reuse safer.

## Version 0.20.1

### Fixes

- Fixed missing serialization sync when multiple classes in an inheritance hierarchy use `SerializeInterface`, by generating `virtual` methods for base classes and `new` methods for derived classes so that `OnBeforeSerialize` and `OnAfterDeserialize` chain correctly across the hierarchy.

## Version 0.20.0

### New Inspector (faster, cleaner, more reliable)

- Replaced the old inspector API with a new data-driven system that behaves much closer to Unity's native inspectors.
- All serialized fields are now discovered in a single structured pass, which removes a lot of brittle special-case logic.
- Interface fields and properties now show Unity's default PropertyField UI, so attributes like Header, Tooltip and others finally work as expected.
- Collections (arrays/lists) use Unity's built-in rendering inside a disabled scope instead of a custom drawer.
- Nested serializable classes are drawn more consistently thanks to the unified PropertyData pipeline.
- Result: a cleaner inspector that is easier to extend, easier to debug and more predictable.

### Improved SerializeInterface code generation

- Major refactor of the SerializeInterfaceGenerator to make the generated code cleaner and more robust.
- User attributes on interface fields and auto-properties are now copied correctly to the generated backing fields, enabling full Unity inspector support.
- Generator now uses fully qualified names everywhere to avoid namespace conflicts.
- InterfaceBackingFieldAttribute has been simplified to store only the interface type, since InjectAttribute is now copied directly onto the generated field.
- PropertyInjector now reads real Inject attributes instead of custom metadata, reducing duplication and improving accuracy.

### Full support for auto-property interface fields

- Added proper support for attributes on auto-properties using [field: SerializeInterface].
- The generator now targets the property's compiler-generated backing field (<Name>k__BackingField), ensuring attributes behave the same as on normal fields.
- Auto-properties now support Header, Tooltip, Inject and other attributes with no extra work from the user.

### Bug fixes

- "Show Injected Fields/Properties" user setting no longer toggles non-injected serialized interfaces.
- Suppress missing errors via Inject-attribute now works on serialized interfaces.

### Summary

This release is mostly invisible to end users but significantly improves Saneject's stability and internal consistency. Interface serialization, inspector rendering and attribute handling are now far more reliable, which also makes future features easier to build.

## Version 0.19.0

### New batch injection system

- Introducing the **Batch Injector**, a new editor window for injecting multiple scenes and prefabs across your entire project in one click, which can be quite the time-saver as your project grows. Drag scenes and prefabs into the window, toggle them on or off and choose between injecting scenes, prefabs or everything in a single pass.
- **Toggle items on or off** to decide exactly which assets will be included when you run a batch injection.
- **Inject scenes, inject prefabs or inject everything** with dedicated buttons so you can target only what you need.
- **Status indicators** show whether each asset succeeded, warned or failed during the last injection, helping you spot issues at a glance.
- **Sorting and search** let you quickly find or organize assets when working with large projects.
- **Context menu actions** let you bulk-select, enable, disable, remove entries, clear injection status or inject only the selected assets for fast list management.
- **GUID based tracking** keeps entries stable even if assets move or get renamed in the project.
- **Confirmation dialogs** let you avoid accidental large batch operations, with per-operation control in User Settings.
- **Save prompts** ensure you don't lose unsaved scene changes before injection starts.
- **Log pinging** lets you click a log entry to highlight the scene or prefab it refers to, speeding up debugging.
- **Scope-level injection logs** show which scopes ran and what was injected for each asset, making results transparent.
- **Batch injection summaries** give a final report of everything processed, including counts for scenes, prefabs, scopes, fields, methods, globals and suppressed errors.

### Tests

- Added **BatchInjectionTests** to validate dependency resolution across multiple scenes and prefabs when using the new Batch Injector.

## Version 0.18.0

### Features

- Added support for **optional error suppression**
    - You can now mark `[Inject(suppressMissingErrors: true)]` to silence expected missing bindings or dependencies.
    - Suppressed issues are still counted and clearly noted below the injection summary log for full transparency.
    - Mostly useful for advanced edge cases, e.g., shared components used in multiple contexts where some references are intentionally null.

### Fixes

- Fix incorrect `memberName` parameter for method injection
    - Updated `MethodInjector` to pass `method.Name` instead of parameter name (`p.Name`) as the `memberName` value during method injection.
    - Ensures `.ToMember("...")` correctly targets method names when using method injection.

### Refactor

- The large `DependencyInjector` class has been refactored into smaller, modular classes with clear, isolated responsibilities, reusable methods, which makes the system easier to maintain.

## Version 0.17.0

### Features

- `ToTarget<T>` now correctly supports nested serializable classes. Previously it only evaluated against the root `MonoBehaviour`, but it now recognizes and targets nested class instances as well.
- Added **method injection** support (`[Inject]` on methods). Useful for configuration or injection into third party components.
    - Supports single, array and `List<T>` parameters.
    - Works alongside field injection - methods are invoked after fields are injected.
    - Fully supported in nested `[Serializable]` classes.
    - Parameters participate in full binding resolution (context filtering, qualifiers, proxy resolution).

### Improvements

- Injection logs and stats now include **injected method count**.
- Improved serialization handling for nested method injection targets.

### Tests

- Added unit tests for **`ToTarget` qualification** with nested serialized classes.
- Added unit tests for **method injection**, covering single, array and list parameter injection paths.

## Version 0.16.1

### Security patch

- Upgraded the project to Unity 6000.0.59f2 to address the Unity security vulnerability [CVE-2025-59489](https://unity.com/security/sept-2025-01).
- This update ensures the project uses a patched Unity runtime that resolves the untrusted search path issue.
- This change only affects users who clone and open the Unity project directly using the previous Unity version listed in ProjectSettings. Opening the project with the updated Unity 6000.0.59f2 or later is safe.
- Users who install Saneject via UPM or a .unitypackage file are not affected and don't need to take any action.

## Version 0.16.0

### Features

- Display scope path in Inspector
    - Shows the chain of parent scopes from the topmost scope down to the current one.
    - Added a **Show Scope Path** toggle in User Settings.
    - Includes jump-to-scope buttons for quick navigation.
- Inspector tooltip support for `[SerializeInterface]`
    - `[Tooltip]` text on a `[SerializeInterface]` field is now reflected on its generated backing field.
- Binding identity strings now list the names of their targets and members, making log messages clearer and helping distinguish bindings whose uniqueness comes from those specific targets or members.
- Added global binding context filtering
    - Skip prefab components by default when registering in SceneGlobalContainer (can be turned off in user settings - but not recommended).
    - Injection logic now tracks invalid bindings for clearer debugging.
- Added MonoBehaviour script field to inspectors
    - Both `ScopeEditor` and `SceneGlobalContainerEditor` now show the standard "Script" field with improved spacing and headers.

### Fixes

- Nicified type names in `SceneGlobalContainerEditor` inspector – raw type names are now shown with user-friendly formatting and spaces, matching the default Unity inspector.

### Improvements

- Enhanced error handling and logging for global dependency resolution
    - Error messages include the global binding identity for better debugging clarity.
    - Duplicate bindings and resolution failures now provide more detailed context.

## Version 0.15.0

### Changes

- Allow injecting abstract `UnityEngine.Object` types, aligning with Unity's own `GetComponent` APIs.
- Simplified the filter surface API for component and asset bindings:
    - Removed built-in syntactic sugar methods (`WhereNameIs`, `WhereSiblingIndexIs`, etc.).
    - Added more hierarchy predicate-based base methods (`Where`, `WhereComponent`, `WhereTransform`, `WhereGameObject`, etc.).
    - **Benefits:** smaller API surface while retaining flexibility, less mental load, more powerful composition.
    - **Trade-off:** slightly more boilerplate when writing filters.
    - To mitigate boilerplate:
        - The filter system is now extensible with **user-defined syntactic sugar** methods.
        - Developers can implement only the helpers they need, keeping the API lean while retaining ergonomics.
        - Example sugar helpers are provided as an importable **UPM sample** ("Component Filter Extensions") demonstrating how to extend the filter API (works the same way for both component and asset filters).

### Tests

- Added new tests for `ComponentFilterBuilder` base and relation methods, covering both concrete and interface bindings.
- Added tests for `AssetFilterBuilder` to ensure consistent behavior across asset filters.
- Added tests for the included sample extension methods.
- Removed old tests for deprecated sugar methods.

## Version 0.14.0

### Features

- Added contextual right-click menu item in the Inspector: **Filter Logs By This Component**.  
  Copies the component's full transform path into the Console search bar.
    - For regular components, filters logs by their hierarchy path.
    - For `Scope` components, automatically filters by the scope's type name.  
      This makes it easy to isolate errors and warnings for a single component or scope in large projects.

## Version 0.13.1

### Fixes

- Fixed a bug where fields marked with `[Inject("id")]` could be resolved by bindings without an ID.  
  Now ID-qualified fields only match bindings that explicitly declare the same ID, ensuring deterministic injection.

### Tests

- Added unit tests for both component and asset bindings to assert that non-ID bindings do not resolve into ID-qualified fields.

## Version 0.13.0

### Changes

#### Make binding ID a qualifier with multiple match support

- Converted binding ID from a single string into a qualifier set, aligned with target and member qualifiers.
- IDs can now stack via `.ToId("A", "B")` and the binding will match if **any** of the IDs match, just like the other qualifiers.
- Updated binding equality rules: bindings with overlapping IDs are treated as duplicates to preserve deterministic resolution.
- Extended unit tests to cover equality and HashSet deduplication behavior for bindings with multiple and overlapping IDs.

#### Introduce target qualifier methods (ToTarget, ToMember)

- Moved `WhereTargetIs` and `WhereMemberNameIs` out of filter builders and into binding builders. Filters should constrain dependency *candidates*, while these methods constrain the *injection target*.
- Renamed them to `ToTarget` and `ToMember` for clarity. Added overloads to support both singular and multiple qualifiers.
- Updated all internal references and tests to use the new qualifier methods.
- Updated sample game bindings to use the qualifiers where applicable.

#### Switch global bindings to dedicated GlobalBindingBuilder

- Added `GlobalBindingBuilder<TComponent>` to replace `ComponentBindingBuilder` for global bindings.
- Global bindings do not support qualifiers (`ToId`, `ToTarget`, `ToMember`) or `.FromProxy()`, since globals are promoted into a `SceneGlobalContainer` and proxies already resolve from the global scope.
- Locator methods remain identical to component bindings and still return a `ComponentFilterBuilder<TComponent>` so filters work the same way.
- Updated `Scope` API and tests to use the new builder for `BindGlobal<T>()`.

## Version 0.12.0

### Features

- Injection error logs now include the full path to the injected field, making it easier to pinpoint where a failure occurred.
- Injection stats now track the number of missing dependencies in addition to missing and invalid bindings.
- Added log severity levels (Info, Warning, Error) to injection stats logs.
- Added a new Scope creation tool:
    - Available via `Saneject/Create New Scope` or `Assets/Create/Saneject/Create New Scope`.
    - Generates a new `Scope` class with the correct boilerplate code.
    - Namespace generation can be toggled in **User Settings** with the option **Generate Scope Namespace From Folder** (uses folder path relative to `Assets/`).
- Added new **`WhereMemberNameIs`** filter for component and asset bindings:
    - This allows bindings to target specific injected fields or properties by name for fine-grained control and can in many case replace ID boilerplate.
    - Unit tests added for both component and asset versions.

### Changes

- Binding target filters (`WhereTargetIs<T>`) now evaluate with **`Any`** instead of **`All`**.  
  This allows chaining multiple filters to mean "inject into `T1` **or** `T2`" instead of requiring all filters simultaneously.
- Binding uniqueness & equality updated:
    - `Binding.Equals` now treats **target-type** and **member-name** filters as **overlap-based** (subset/superset considered equal if they can apply to the same injection site).
    - `GetHashCode` was adjusted accordingly (hash depends on presence of these filters rather than their full contents).
    - This tightens duplicate detection: e.g., `WhereTargetIs<MonoA>()` is considered a duplicate of `WhereTargetIs<MonoA>().WhereTargetIs<MonoB>()`; same for `WhereMemberNameIs("monoA")` vs `WhereMemberNameIs("monoA","monoB")`.
    - Added/updated unit tests to cover overlap equality for both target-type and member-name filters, including assignability (base/derived) cases.
- Scope inspector **Inject** button now injects only the selected hierarchy instead of the entire scene. This makes it easier to process smaller hierarchies and view context-relevant logs. Full-scene injection is still available via scene right-click and the **Saneject** menu.
- Fully abort injection when proxy script creation is pending
    - Updated `CreateMissingProxyStubs` to return a flag when proxy generation is required.
    - Injection now aborts early if proxies are pending, since Unity recompilation halts the process anyway.
    - Reduces irrelevant logs during proxy generation by deferring them until the next injection pass.
    - Improved progress display and clarity of messages when proxy creation interrupts injection.
- Updated sample game `EnemyManager`: now spawns enemies from a prefab at runtime instead of injecting scene children, keeping the sample compatible with context filtering enabled and aligned with recommended usage.

### Fixes

- Fixed an issue where missing dependencies were not counted correctly in the injection stats.
- Restored property drawing for `[field: ...]` auto-properties in `SanejectInspector`, so `[SerializeField]` and `[SerializeInterface]` properties (with or without `[Inject]`, including in nested classes) now draw correctly again.
- Fixed a bug in **FilterBySameContext** where prefab asset injection (via Inspector on a prefab selected in the Project window) incorrectly treated child objects as separate contexts. Now all components inside a prefab asset normalize to the prefab asset root, ensuring they are considered part of the same context. This fixes broken injection when pressing **Inject** on a prefab asset without opening the prefab.
- Fixed singular/plural words in injection stats logs.

## Version 0.11.0

### Features

- Added `FromFolder` asset binding locator that loads all assets of a given type from a project folder and its subfolders.

### Tests

- Added unit tests for `FromFolder` covering both concrete and interface bindings.

## Version 0.10.2

### Fixes

- Fixed a bug that prevented interface only binding `BindComponent<IInterface>().FromAnywhereInScene()` from working.

## Version 0.10.1

### Changes

- Added **prefab-to-prefab isolation** to context filtering.
    - Prefab instances can no longer resolve components from other prefab instances.
    - Prefab assets can no longer resolve components from other prefab assets.
    - This ensures prefab-scoped dependencies remain stable and do not leak across unrelated prefabs.

### Tests

- Extended **ContextFilteringTests** to cover:
    - Rejection of cross-prefab references when filtering is enabled.
    - Rejection of prefab asset references when filtering is enabled.
    - Confirmation that cross-context links are allowed when filtering is disabled.
- Verified prefab isolation does not interfere with scene or proxy workflows.

## Version 0.10.0

### Features

- Added new user setting **Filter By Same Context** (default: enabled).
    - When enabled, Saneject filters out prefab components when injecting into scene objects, and filters out scene components when injecting into prefabs. This enforces context isolation and avoids broken references.
    - When disabled, scene ↔ prefab references are allowed as an escape hatch. This is not recommended because such links are not serialized reliably — removing a prefab instance from the scene will silently break the dependency.
    - For proper cross-context references, the recommended workflow is to use **ProxyObjects** (`BindComponent<TInterface, TConcrete>().FromProxy()`).

### Changes

- Improved error logging when a binding fails to resolve:
    - Logs now include candidate types that were rejected due to scene/prefab context mismatch.
    - Error messages provide guidance to either use ProxyObjects or disable context filtering in user settings (with a warning).
- Cleaner dev UX for injection errors: all rejected candidates for a binding are reported together in a single message instead of spamming per-candidate logs.
- Rename `BindingIdentityHelper` to `BindingSignatureHelper` and refactor related methods.
- Add checker material to sample game ground plane to make player movement visible.

### Tests

- Added **ContextFilteringTests** to verify that scene ↔ prefab resolution is blocked when filtering is enabled, and allowed when filtering is disabled.

## Version 0.9.1

### Changes

- Move global binding prefab validation out of `DependencyInjector` and into `BindingValidator`.

### Fixes

- Fix global bindings always flagged as unused.
- Display invalid bindings count in injection stats log.

### Tests

Wrote new tests:

- Test that global binding on prefab scope is correctly invalidated.
- Test that all used bindings are correctly marked `IsUsed = true`.

## Version 0.9.0

### New Features

- Added `BindComponent<TInterface, TConcrete>().FromProxy()` binding locator that greatly simplifies working with proxies. At injection time, the system will create the stub and proxy asset instance for you and inject the asset. This is now the recommended workflow for normal use cases. Much faster and less context-switching than creating manual instances, as multiple proxies can be created declaratively in one injection pass.
- For advanced use cases, you can still create manual proxy ScriptableObject instances with different resolution strategies. This workflow has been improved as well. Previously it was a tedious multi-step process per proxy (generate stub, domain reload, wait, generate asset). Now both stub and proxy asset generate in one click.
- User settings: customizable path for generated proxy output.
- User settings: toggle to clear log before injection, useful for seeing only logs related to the current context.

### Changes

- Renamed `InterfaceProxyObject` to `ProxyObject` for simplicity and conciseness. Required coordinated renames across Roslyn, code generation, and stub generation.
- Refactored binding validation into a separate `BindingValidator` class instead of having it inside the `Binding` itself.
- Wrote a `BindingIdentityHelper` that constructs a consistent binding identity string for logging, making logs more scannable.
- Cleaned up and streamlined log and validation messages.
- Updated demo game to use `FromProxy()`.

### Validation & Testing

- Added new validation checks related to the `FromProxy()` method.
- Added unit test for `FromProxy()`. Stub generation cannot be tested directly, since it triggers a domain reload (which kills the test process), but loading and creating the proxy asset is verified in CI.

## Version 0.8.3

- Fixed bug where `SceneGlobalContainer` failed to unregister from `GlobalScope` on scene unload if the instance was null.

## Version 0.8.2

### Packaging

- Added Unity Package Manager (UPM) support.
- Package can now be installed directly via Git URL:  
  `https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject`
- No changes to runtime or editor functionality.
- Fixed a bug in `SceneGlobalContainer` that made it break in sample imported via UPM.

## Version 0.8.1

### CI/Testing

- Added assembly definition files for runtime and test code to ensure CI discovers and runs tests consistently across Unity versions.
- Moved test `Resources` folder into a folder covered by an assembly definition so `Resources.Load` works in CI.
- Updated test setup to automatically bypass injection confirmation prompts when running in batch mode, fixing mass test failures in CI.
- Confirmed automated tests now pass for all supported Unity versions.

### Editor

- No functional changes to runtime/editor features, only improvements to test reliability and CI integration.

## Version 0.8.0

Saneject is now confirmed to work from 2022.3.12f LTS.

### Backwards Compatibility

- Swap `TextMeshProUGUI` components and references with `UnityEngine.UI.Text`.
- Removed new Unity input system and updated demo scripts to use old input system.
- Added extension `GetComponentIndexCompat` with preprocessor version check, so versions older than Unity 2022 versions don't call `component.GetComponentIndex()` that doesn't exist.

### Editor

- Renamed `MonoBehaviourInspector` to `MonoBehaviourFallbackInspector` and made it a true fallback so it only applies when no more specific custom inspector exists.
- Added `SanejectInspector` public API for integrating Saneject's injection-aware inspector features into custom editors.
- Introduced unified `SanejectInspector.DrawDefault` method to restore the full Saneject inspector UI/UX in a single call from any custom inspector.

### CI/Testing

- Added automated test runs for oldest/newest 2022 LTS, 6.0 LTS, plus latest 6.1 and 6.2.

## Version 0.7.1

- Allow `[Inject]` attribute to be used with public fields without `[SerializeField]`.
- Small refactor in the `Scope` binding API: `BindComponent<TComponent>()` and single generic variations are now `BindComponent<T>()`, to make it clearer that `T` can be both interface and concrete.

## Version 0.7.0

### API Changes

- Refactored binding API to be more type-safe via generics. The API now returns only appropriate binding methods depending on whether you're binding assets or components.
- Added support for collection bindings using `BindMultipleComponents<T>()`, `BindMultipleComponents<TInterface, TConcrete>()`, and shorthand aliases `BindComponents<T>()` and `BindComponents<TInterface, TConcrete>()`.
- Added support for multiple bindings of the same type, allowing interface injection with different concretes based on the injection target using `.WhereTargetIs<T>()`.
- Enforced stricter resolution rules: interface bindings no longer resolve concrete fields.
- Added support for interface-only bindings, e.g. `BindComponent<IInterface>()`, useful for resolving collections of different concrete implementations.
- Removed dependency injector caching due to problems with target filtering and multi-binding resolution.
- `Scope.GetBindingRecursivelyUpwards()` now respects target filters and no longer returns the first match unconditionally.

### Validation & Error Handling

- Improved validation and error handling across the binding system.
- Binding configuration issues (missing, conflicting, unused bindings) are now reported in a single pass without halting the injection flow.
- Added extended validation for binding declarations to detect invalid or contradictory configurations.

### Inspector & UI

- Removed legacy property drawers. All field rendering is now handled by `MonoBehaviourInspector` and `SanejectInspector`.
- Inspector draws injected collections (arrays/lists) as read-only, grayed-out UI with no editing controls.
- Interface field sync logic (`ISerializationCallbackReceiver.OnBeforeSerialize`) is now generated by Roslyn and no longer depends on `PropertyDrawer`, improving robustness.
- Tooltips added to all toggles in `UserSettingsEditorWindow`.
- Serialize interface backing fields and generated serialization methods are now hidden from IntelliSense to prevent accidental misuse.

### Testing

- Added 225 unit tests covering core functionality and edge cases. All tests are passing.

### Bug Fixes

- Fixed a bug where `[SerializeInterface]` did not work correctly in nested `[Serializable]` classes.
- Fixed a bug where interface bindings incorrectly resolved concrete fields.

## Version 0.6.0

### Roslyn Analyzers & Code Fixes

- Added a new Roslyn analyzer (`AttributesAnalyzer.dll`) that validates:
    - `[Inject]` fields must also have `[SerializeField]` or `[SerializeInterface]`.
    - `[SerializeInterface]` can only be applied to fields of interface type.
- Added code fixes that offer context-aware actions to add, remove or correct attribute usage.
- Updated Roslyn tools documentation and reorganized project structure for clarity.

See [Roslyn Tools in Saneject](https://github.com/alexanderlarsen/Saneject?tab=readme-ov-file#roslyn-tools-in-saneject) for more details.
