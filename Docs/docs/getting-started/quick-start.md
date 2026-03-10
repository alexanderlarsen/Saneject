---
title: Quick start
---

# Quick start

This page walks through a minimal Saneject setup in one scene and shows how to run your first injection. It should take around 5 minutes.

## Requirements

| Requirement       | Description                                                                                                                                         |
|-------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------|
| Unity version     | Unity `2022.3.12f1` LTS or newer. Saneject's Roslyn source generators and analyzers do not work in earlier versions                                 |
| Scripting backend | Mono or IL2CPP.                                                                                                                                     |
| Platform          | Editor-only tooling; runtime code is plain C#, so it should run on any platform Unity supports but only tested on Windows + Android (Mono & IL2CPP) |

## Installation

| Install method         | Instruction                                                                                                                                                                              |
|------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Unity package          | 1. Download the latest `.unitypackage` from [Releases](https://github.com/alexanderlarsen/Saneject/releases).<br>2. Import it in Unity.                                                  |
| UPM (latest)           | 1. Open Package Manager.<br>2. Select **Add package from git URL**.<br>3. Paste:<br>`https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject` |
| UPM (specific version) | Same as above, but add `#<version>`.<br>Example:<br>`https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject#0.21.1`                          |

## Hello Saneject

### 1. Create the scene hierarchy

1. Create a `GameObject` named `Root`.
2. Create a child `GameObject` named `Player` under `Root`.
3. Add `CharacterController` to `Player`.

### 2. Create a component to inject

Create `GameManager.cs` and place it on any `GameObject` in the scene.

```csharp
using UnityEngine;

public interface IGameStateObservable
{
}

public class GameManager : MonoBehaviour, IGameStateObservable
{
}
```

### 3. Create an injection target

Create `Player.cs` and attach it to `Player`.

```csharp
using Plugins.Saneject.Experimental.Runtime.Attributes;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    [Inject, SerializeInterface]
    private IGameStateObservable gameStateObservable;

    [Inject, SerializeField]
    private CharacterController controller;
}
```

`Player` is `partial` because `[SerializeInterface]` uses generated code to serialize interface references.

### 4. Create a scope and declare bindings

Create `GameScope.cs`, attach it to `Root`, and declare bindings:

You can create a scope manually generate a scope script from:

- Main menu: `Saneject/Create New Scope`
- Project window context menu: `Assets/Saneject/Create New Scope`

```csharp
using Plugins.Saneject.Experimental.Runtime.Scopes;

public class GameScope : Scope
{
    protected override void DeclareBindings()
    {
        BindComponent<IGameStateObservable, GameManager>()
            .FromAnywhere();

        BindComponent<CharacterController>()
            .FromTargetSelf();
    }
}
```

`Scope` is where bindings are declared. During injection, Saneject resolves each `[Inject]` site from the nearest `Scope`, with fallback to parent scopes.

### 5. Run injection

Run dependency injection with either method:

- Scope inspector: Find the `GameScope` component, then `Injection Controls` → `Inject Scene By Context` → `All`.
- Main menu: `Saneject/Inject/Current Scene (All Contexts)`.
- GameObject context menu: `GameObject/Saneject/Inject/Current Scene (All Contexts)`.

After injection, `Player` has serialized values for `gameStateObservable` and `controller`. Enter Play Mode and the scene runs without a runtime container.

## Inspector integration note

Saneject includes a custom MonoBehaviour inspector that keeps injected and serialized-interface fields ordered and preserves Saneject's intended inspector UX.
If the inspector looks wrong or incomplete, another custom inspector or plugin is likely overriding Saneject.
In that case, either disable the conflicting inspector or integrate Saneject's inspector API in your custom inspector to restore the Saneject inspector UX.

See:

- [MonoBehaviour inspector](../editor-and-tooling/inspectors/monobehaviour-inspector.md)
- [Saneject inspector API](xref:Plugins.Saneject.Experimental.Editor.Inspectors.SanejectInspector)

## Related pages

- [Introduction](introduction.md)
- [Feature overview](feature-overview.md)
- [Sample game](sample-game.md)
- [Scope](../core-concepts/scope.md)
- [Binding](../core-concepts/binding.md)
- [Field, property & method injection](../core-concepts/field-property-and-method-injection.md)
- [Serialized interface](../core-concepts/serialized-interface.md)
- [Glossary](../reference/glossary.md)

