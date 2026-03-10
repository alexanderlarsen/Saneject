---
title: Injection menus
---

# Injection menus

Saneject injection menus are editor commands that run dependency injection on scenes, prefab assets, or batches of assets.

The menu label tells you two things:

- What area you are injecting, for example current scene, selected scene hierarchies, or current prefab asset.
- Which `ContextWalkFilter` is applied to that run, for example `AllContexts` or `SceneObjects`.

If you are new to contexts and filters, read [Context](../core-concepts/context.md) first.

## Menu locations

Saneject exposes the same injection commands in multiple Unity menus:

- Main menu: `Saneject/Inject/...`
- Hierarchy context menu: `GameObject/Saneject/Inject/...`
- Project window context menu: `Assets/Saneject/Batch Inject/Selected Assets`
- Main menu batch shortcut: `Saneject/Batch Inject/Selected Assets`

There is also a dedicated batch injector window:

- `Saneject/Batch Inject/Open Batch Injector Window`

That window is covered in [Batch injection](batch-injection.md).

## Scene and prefab injection commands

All commands in `Saneject/Inject/...` and `GameObject/Saneject/Inject/...` use the same injection pipeline. The only differences are:

- Start objects for the run.
- `ContextWalkFilter` selected by that menu item.

### Current Scene group

Menu paths:

- `Saneject/Inject/Current Scene (All Contexts)`
- `Saneject/Inject/Current Scene (Scene Objects)`
- `Saneject/Inject/Current Scene (Prefab Instances)`

Availability:

- Enabled only while editing a scene.
- Disabled in Prefab Mode.

Behavior:

- Uses the active scene's root `GameObject` objects as start objects.
- Runs injection for that scene using the selected filter.

### Selected Scene Hierarchies group

Menu paths:

- `Saneject/Inject/Selected Scene Hierarchies (All Contexts)`
- `Saneject/Inject/Selected Scene Hierarchies (Scene Objects)`
- `Saneject/Inject/Selected Scene Hierarchies (Prefab Instances)`
- `Saneject/Inject/Selected Scene Hierarchies (Same Contexts As Selection)`

Availability:

- Enabled only while editing a scene.
- Requires at least one selected `GameObject`.

Behavior:

- Uses `Selection.gameObjects` as start objects.
- The graph is built from each selected object's root transform, then filtered by the selected `ContextWalkFilter`.
- This means a selected child can still cause its full root hierarchy to be part of the run.

### Current Prefab Asset group

Menu paths:

- `Saneject/Inject/Current Prefab Asset (All Contexts)`
- `Saneject/Inject/Current Prefab Asset (Prefab Asset Objects)`
- `Saneject/Inject/Current Prefab Asset (Prefab Instances)`
- `Saneject/Inject/Current Prefab Asset (Same Contexts As Selection)`

Availability:

- Enabled only in Prefab Mode.
- `Same Contexts As Selection` also requires a selection.

Behavior:

- For `All Contexts`, `Prefab Asset Objects`, and `Prefab Instances`, Saneject starts from the current prefab asset root.
- For `Same Contexts As Selection`, Saneject starts from the current selection.

## Filter labels used in menu names

Menu labels map directly to `ContextWalkFilter` values:

| Menu label suffix            | Context Walk Filter       | Included contexts                                     |
|------------------------------|---------------------------|-------------------------------------------------------|
| `All Contexts`               | `AllContexts`             | Scene objects, prefab instances, prefab asset objects |
| `Scene Objects`              | `SceneObjects`            | Scene object contexts only                            |
| `Prefab Instances`           | `PrefabInstances`         | Prefab instance contexts only                         |
| `Prefab Asset Objects`       | `PrefabAssetObjects`      | Prefab asset contexts only                            |
| `Same Contexts As Selection` | `SameContextsAsSelection` | Contexts matching selected start object contexts      |

`ContextWalkFilter` controls what enters the run. It does not override context isolation.
Context isolation is configured in project settings and still applies during resolution.
See [Context](../core-concepts/context.md).

## Batch inject from selected assets

Command paths:

- `Assets/Saneject/Batch Inject/Selected Assets (All Contexts)`
- `Saneject/Batch Inject/Selected Assets (All Contexts)`

Availability:

- Enabled only when the current Project selection includes at least one scene asset or prefab asset.
- Folder selection works because Saneject scans deep selected assets.

Behavior:

1. Collect selected scene and prefab assets.
2. Assign `AllContexts` filter to each collected asset.
3. Show batch confirmation dialog with scene and prefab counts.
4. Ask to save currently modified open scenes before running.
5. Inject each selected scene and prefab asset.
6. Save scene and asset changes.
7. Log per-asset sections and final batch summary.

If an injected asset has no `Scope` components, the run logs that nothing was injected for that asset or run.

## Confirmation dialogs and Play Mode behavior

All injection menu commands are editor-only:

- If triggered in Play Mode, Saneject shows a dialog telling you to exit Play Mode.

By default, menu commands show confirmation dialogs before injecting. You can toggle these prompts in:

- `Saneject/Settings` → `User Settings` → `Ask Before Injection`

Relevant toggles:

- `Scene`
- `Prefab Asset`
- `Selected Scene Hierarchies`
- `Batch Injection`

## Example: running a focused scene injection

Component:

```csharp
using Plugins.Saneject.Experimental.Runtime.Attributes;
using UnityEngine;

public partial class PlayerHud : MonoBehaviour
{
    [Inject, SerializeField]
    private HealthService healthService;
}
```

Run steps:

1. Select one or more scene objects in the hierarchy.
2. Run `Saneject/Inject/Selected Scene Hierarchies (Scene Objects)`.
3. Check the Console for Saneject summary logs and any missing binding or dependency errors.

This workflow is useful when you want to validate scene-object wiring without also processing prefab instances in the same run.

## Related pages

- [Context](../core-concepts/context.md)
- [Scope](../core-concepts/scope.md)
- [Field, property & method injection](../core-concepts/field-property-and-method-injection.md)
- [Batch injection](batch-injection.md)
- [Settings](settings.md)
- [Logging](logging.md)
- [Glossary](../reference/glossary.md)
