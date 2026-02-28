# Batch injection

## Batch injector editor window

The Batch Injector lets you inject multiple scenes and prefabs across your entire project in one click, which can be quite the time-saver as your project grows.

Open it via `Saneject/Batch Injector`

- **Add scenes and prefabs** by dragging them into the window or using buttons, making it easy to build and manage large injection lists.
- **Toggle items on or off** to decide exactly which assets will be included when you run a batch injection.
- **Inject scenes, inject prefabs, or inject everything** with dedicated buttons so you can target only what you need.
- **Status indicators** show whether each asset succeeded, warned, or failed during the last injection, helping you spot issues at a glance.
- **Sorting and search** let you quickly find or organize assets when working with large projects.
- **Context menu actions** let you bulk-select, enable, disable, remove entries, clear injection status, or inject only the selected assets for fast list management.
- **GUID based tracking** keeps entries stable even if assets move or get renamed in the project.
- **Confirmation dialogs** let you avoid accidental large batch operations, with per-operation control in User Settings.
- **Save prompts** ensure you don't lose unsaved scene changes before injection starts.
- **Log pinging** lets you click a log entry to highlight the scene or prefab it refers to, speeding up debugging.
- **Scope-level injection logs** show which scopes ran and what was injected for each asset, making results transparent.
- **Batch injection summaries** give a final report of everything processed, including counts for scenes, prefabs, scopes, fields, methods, globals, and suppressed errors.

![Batch Injector editor window](Docs/batch-injector-window.webp)
![Batch Injector logs](Docs/batch-injector-logs.webp)