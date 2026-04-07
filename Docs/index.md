---
layout: _landing
title: Saneject
---

<img class="index-logo index-logo-light-theme" src="images/logo-dark.webp" alt="Saneject logo" width="260">
<img class="index-logo index-logo-dark-theme" src="images/logo-light.webp" alt="Saneject logo" width="260">

![Unity](https://img.shields.io/badge/Unity-2022.3.12+-ff8383)
[![Tests](https://img.shields.io/github/actions/workflow/status/alexanderlarsen/Saneject/tests.yml?label=Tests)](https://github.com/alexanderlarsen/Saneject/actions/workflows/tests.yml)
[![Release](https://img.shields.io/github/v/release/alexanderlarsen/Saneject?include_prereleases&color=blue&label=Release)](https://github.com/alexanderlarsen/Saneject/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow)](https://github.com/alexanderlarsen/Saneject/blob/main/LICENSE)

Resolve dependencies in the Unity Editor, not Play Mode, and write them directly into serialized fields so everything stays visible in the Inspector, including interfaces.

No runtime container, no startup cost, no hidden wiring. Just simple, deterministic DI that works with Unity instead of around it.

## Mental model

1. Mark your fields, properties and methods `[Inject]`.
2. Create a `Scope` that binds dependencies for `[Inject]` members and add it to the scene.
3. Click **Inject** in the Unity Editor.
4. Dependencies are injected into serialized fields.
5. Start the game and enjoy fast startup with Unity’s normal lifecycle.
 
## Main features

| Feature                              | Description                                                                                                                       |
|--------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------|
| Editor-time injection                | Resolve dependencies before Play Mode and store them as normal serialized references.                                             |
| Inspector-visible wiring             | Keep dependencies, including interfaces, visible and editable in the Inspector instead of hiding them behind a runtime container. |
| Fluent binding API                   | Declare bindings in `Scope` components with familiar DI-style syntax.                                                             |
| Serialized interfaces                | Use `[SerializeInterface]` for interface fields, arrays, and lists without wrapper classes.                                       |
| Scene, prefab, and context awareness | Control what gets injected where and how cross-context resolution behaves.                                                        |
| Runtime proxy bridging               | Handle references Unity cannot serialize directly across scene and prefab boundaries.                                             |
| Low runtime overhead                 | No runtime container, no reflection-based startup pass, and no extra lifecycle layer.                                             |
| Built-in tooling                     | Use injection menus, batch injection, logging, validation, settings, analyzers, and more directly in the Unity Editor.            |

For more features, see [Feature overview](docs/getting-started/feature-overview.md).

## Try Saneject now

Add this URL to Unity Package Manager (Unity 2022.3.12 or newer):

```text
https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject
```

Or download the [latest release](https://github.com/alexanderlarsen/Saneject/releases) and import the `Saneject` folder into your Unity project.

Then jump to [Quick start](docs/getting-started/quick-start.md).

## Feedback appreciated

If you try Saneject and something works well, feels unclear, or seems broken, I would love to hear about it.

- If you find a bug, please open an [Issue](https://github.com/alexanderlarsen/Saneject/issues).
- If you want to share feedback, ideas, or first impressions, drop a note in [Discussions](https://github.com/alexanderlarsen/Saneject/discussions).

## Links

- [Docs](docs/getting-started/introduction.md)
- [API](xref:Plugins.Saneject.Editor.Inspectors.SanejectInspector)
- [GitHub repo](https://github.com/alexanderlarsen/Saneject)
- [Releases](https://github.com/alexanderlarsen/Saneject/releases)
- [MIT license](https://github.com/alexanderlarsen/Saneject/blob/main/LICENSE)
 
