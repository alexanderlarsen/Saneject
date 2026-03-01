# Settings

## User settings

Found under **Saneject → User Settings**. These settings are per-user and not committed to source control.

| Setting | Description |
|---|---|
| **Ask Before Scene Injection** | Show a confirmation dialog before injecting the full scene (excluding prefab instances). |
| **Ask Before Hierarchy Injection** | Show a confirmation dialog before injecting a single selected hierarchy (excluding prefab instances inside it). |
| **Ask Before Prefab Injection** | Show a confirmation dialog before injecting a prefab. |
| **Ask Before Batch Inject** | Show a confirmation dialog before running a batch injection from the Batch Injector window. |
| **Use Context Isolation** | When enabled, scenes and prefab instances are treated as separate contexts with hard boundaries. When disabled, they form a single unified hierarchy. **Keeping this enabled is strongly recommended.** See [Contexts](../core-concepts/contexts.md). |
| **Show Injected Fields/Properties** | Show `[Inject]` fields and `[field: Inject]` auto-properties in the Inspector. When disabled, injected fields are hidden. |
| **Show Help Boxes** | Show help boxes in the Inspector on Saneject components. |
| **Show Scope Path** | Display the scope chain (from ancestor scopes down to the selected scope) in the Scope inspector. |
| **Log On Proxy Instance Resolve** | Log when a `RuntimeProxy` resolves its target instance during Play Mode. |
| **Log Global Scope Register/Unregister** | Log when components are registered or unregistered in the `GlobalScope` during Play Mode. |
| **Log Injection Stats** | Log a summary after each injection: scopes processed, fields injected, methods called, globals added, unused bindings, missing dependencies. |
| **Log Prefab Skipped During Scene Injection** | Log when a prefab instance is skipped during a scene injection pass. |
| **Log Unused Bindings** | Log when a binding is declared in a `Scope` but no matching `[Inject]` field or method was found. |
| **Log Unused Runtime Proxies On Domain Reload** | Log `RuntimeProxy` assets that exist in the project but are not referenced by any binding, checked on domain reload. |
| **Clear Logs On Injection** | Clear the console before injection starts. Useful for seeing only the output of the current injection pass. |
| **Generated Proxy Asset Folder** | The folder where proxy `ScriptableObject` assets are created by `FromRuntimeProxy()`. Defaults to `Assets/Generated`. |
| **Generate Scope Namespace From Folder** | When enabled, new Scopes created via the editor menu get a namespace matching their folder path relative to `Assets/`. |

## Project settings

Found under **Edit → Project Settings → Saneject**. These settings are shared across the team and should be committed to source control.

| Setting | Description |
|---|---|
| **Use Context Isolation** | Project-level default for context isolation. |
| **Generate Scope Namespace From Folder** | Project-level default for namespace generation on new Scopes. |
| **Generate Proxy Scripts On Domain Reload** | When enabled, Saneject checks for and generates any missing proxy scripts on every domain reload. |
| **Proxy Asset Generation Folder** | Project-level default folder for generated proxy `ScriptableObject` assets. |
