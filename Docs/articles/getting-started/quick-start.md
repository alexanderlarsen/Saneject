# Quick start

## Requirements

| Requirement | Details |
|---|---|
| Unity version | 2022.3.12f1 LTS or newer. Saneject's Roslyn source generators and analyzers do not work in earlier versions. |
| Scripting backend | Mono or IL2CPP |
| Platforms | Editor-only tooling. Runtime code is plain C#, so it runs on any platform Unity supports, but only Windows and Android (Mono & IL2CPP) are tested. |

## Installation

| Method | Steps |
|---|---|
| Unity package | 1. Grab the latest `.unitypackage` from the [Releases page](https://github.com/alexanderlarsen/Saneject/releases). <br>2. Double-click to import. |
| UPM (latest) | 1. Open Package Manager. <br>2. Click **+** → **Add package from git URL**. <br>3. Paste: `https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject` <br>4. Press Enter. |
| UPM (pinned version) | Same as above, but append `#version` to the URL. Example: `…/Saneject#0.9.0` |

## Hello Saneject

This walkthrough injects a `CharacterController` and an `IGameStateObservable` interface into a `Player` component.

### 1. Set up the scene

Create a `GameObject` named `Root` in the scene. Add a child `GameObject` named `Player`.

### 2. Write the Player component

Attach `Player.cs` to the `Player` GameObject:

```csharp
// Classes with [SerializeInterface] fields must be declared partial.
// The Roslyn generator emits a matching partial class with the backing field.
public partial class Player : MonoBehaviour
{
    // Concrete component on the same GameObject, managed by Saneject
    [Inject, SerializeField]
    private CharacterController controller;

    // Interface field — shows up in the Inspector thanks to [SerializeInterface]
    [Inject, SerializeInterface]
    private IGameStateObservable gameStateObservable;
}
```

Add a `CharacterController` component to the `Player` GameObject.

### 3. Write the GameManager

Add `GameManager.cs` somewhere in the scene:

```csharp
public class GameManager : MonoBehaviour, IGameStateObservable { }

public interface IGameStateObservable { }
```

### 4. Write the Scope

Add `GameScope.cs` to the `Root` GameObject:

```csharp
public class GameScope : Scope
{
    protected override void DeclareBindings()
    {
        // Find the CharacterController on the same GameObject as the injection target (Player)
        BindComponent<CharacterController>()
            .FromTargetSelf();

        // Find any GameManager anywhere in the scene, bind it as IGameStateObservable
        BindComponent<IGameStateObservable, GameManager>()
            .FromAnywhere();
    }
}
```

> **Tip:** Scope boilerplate can be generated via **Saneject → Create New Scope** or **Assets → Create → Saneject → Create New Scope**. Namespace generation from the folder path can be toggled in User Settings.

### 5. Run injection

Trigger injection using any of these:

- **Scope inspector** → **Inject Hierarchy Dependencies**
- **Hierarchy right-click** → **Inject Scene Dependencies**
- **Menu bar** → **Saneject → Inject Scene Dependencies**

Saneject fills in the serialized fields. Press Play — no runtime container required.

## Inspector conflicts

Saneject includes a `MonoBehaviourInspector` that draws injected fields, `[SerializeInterface]` fields, and nested types in the correct order. If another plugin's custom inspector overrides it, your inspector may look incomplete or out of order.

To restore Saneject's layout inside your own `Editor` class:

```csharp
[CustomEditor(typeof(MyComponent))]
public class MyComponentEditor : UnityEditor.Editor
{
    private ComponentModel componentModel;

    private void OnEnable()
    {
        componentModel = new ComponentModel(target, serializedObject);
    }

    public override void OnInspectorGUI()
    {
        SanejectInspector.OnInspectorGUI(componentModel);
    }
}
```

See [Custom inspector](../editor-and-tooling/custom-inspector.md) for details.
