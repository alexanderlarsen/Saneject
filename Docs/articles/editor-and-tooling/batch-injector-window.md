# Batch injector window

The Batch Injector lets you inject multiple scenes and prefabs across your project in one pass. Open it via **Saneject → Batch Injector**.

## Managing the asset list

- **Add assets** by dragging scenes and prefabs into the window, or using the add buttons.
- **Toggle items on or off** to control which assets are included when you run a batch.
- **GUID-based tracking** keeps entries stable when assets are moved or renamed.

## Running injection

Three buttons let you target exactly what you need:

- **Inject Scenes** — injects only the scene assets in the list.
- **Inject Prefabs** — injects only the prefab assets in the list.
- **Inject All** — injects everything that's enabled in the list.

Before injection starts, Saneject prompts you to save any unsaved scene changes.

## Results and logging

- **Status indicators** on each entry show whether the last injection succeeded, warned, or failed.
- **Scope-level logs** show which scopes ran and what was injected for each asset.
- **Log pinging** — click a log entry to highlight the corresponding scene or prefab in the Project window.
- **Batch summary** reports total counts for scenes processed, prefabs processed, scopes run, fields injected, methods called, globals registered, and suppressed errors.

## List management

Right-clicking the list opens a context menu with bulk actions: select all, enable/disable selected, remove selected, clear injection status, and inject only selected entries.

The search bar and sort menu let you find and organize entries when working with large projects.

## Confirmation dialogs

Each injection operation (scenes, prefabs, all) has an independent confirmation dialog that can be turned on or off from **Saneject → User Settings → Ask Before Batch Inject**.
