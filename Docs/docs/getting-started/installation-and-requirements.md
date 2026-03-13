---
title: Installation & requirements
---

# Installation & requirements

Use this page for Saneject prerequisites and package installation options.

## Installation

| Install method         | Instruction                                                                                                                                                                              |
|------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Unity package          | 1. Download the latest `.unitypackage` from [Releases](https://github.com/alexanderlarsen/Saneject/releases).<br>2. Import it in Unity.                                                  |
| UPM (latest)           | 1. Open Package Manager.<br>2. Select **Add package from git URL**.<br>3. Paste:<br>`https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject` |
| UPM (specific version) | Same as above, but add `#<version>`.<br>Example:<br>`https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject#0.21.1`                          |

## Requirements

| Requirement       | Description                                                                                                                                            |
|-------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------|
| Unity version     | Unity `2022.3.12f1` LTS or newer. Saneject's Roslyn source generators and analyzers do not work in earlier versions                                    |
| Scripting backend | Mono or IL2CPP.                                                                                                                                        |
| Platform          | Editor-only tooling; runtime code is plain C#, so it should run on any platform Unity supports but is only tested on Windows + Android (Mono & IL2CPP) |

## Related pages

- [Quick start](quick-start.md)
- [Tested Unity versions](../reference/tested-unity-versions.md)
- [Glossary](../reference/glossary.md)
