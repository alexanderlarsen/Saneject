# Quick start

## Requirements

| Requirement       | Description                                                                                                                                         |
|-------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------|
| Unity version     | Unity 2022.3.12f1 LTS or newer. Saneject's Roslyn source generators and analyzers do not work in earlier versions                                   |
| Scripting backend | Mono or IL2CPP                                                                                                                                      |
| Platforms         | Editor-only tooling; runtime code is plain C#, so it should run on any platform Unity supports but only tested on Windows + Android (Mono & IL2CPP) |

## Installation

| Install method         | Instruction                                                                                                                                                                                                                          |
|------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Unity package          | 1. Grab the latest `unitypackage` from the [Releases page](https://github.com/alexanderlarsen/Saneject/releases)<br>2. Double-click → import.                                                                                        |
| UPM (latest version)   | 1. Open Unity Package Manager.<br>2. Click the + button and "Add package from git URL".<br>3. Copy-paste:<br>`https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject`<br>4. Press Enter. |
| UPM (specific version) | Same steps as above, but include version after `#` to lock to that version.<br>Example:<br>`https://github.com/alexanderlarsen/Saneject.git?path=UnityProject/Saneject/Assets/Plugins/Saneject#0.8.2`                                |

## Hello Saneject (basic example)

1. Create a `GameObject` named `Root` in the scene.
2. Add a `GameObject` named `Player` under the `Root` and attach `Player.cs` and a `CharacterController` to it:

```csharp
// Classes with [SerializeInterface] must be declared 'partial'. The Roslyn generator injects hidden interface serialization code into a matching partial class behind the scenes.
public partial class Player : MonoBehaviour
{
    // Interface field, marked for injection, shows up in the Inspector
    [Inject, SerializeInterface]
    private IGameStateObservable gameStateObservable;

    // Concrete component field, marked for injection, living on the same GameObject
    [Inject, SerializeField]
    private CharacterController controller;
}
```

3. Add `GameManager.cs` somewhere in the scene:

```csharp
// Will satisfy the IGameStateObservable binding
public class GameManager : MonoBehaviour, IGameStateObservable
{ }

public interface IGameStateObservable 
{ }
```

4. Add `GameScope.cs` to the `Root` `GameObject`:

```csharp
// One place to declare where things come from
public class GameScope : Scope
{
    public override void Configure()
    {
        // Look anywhere in the loaded scene for a GameManager and bind by interface.
        // Resolves via FindObjectsByType<GameManager>(FindObjectsInactive.Include, FindObjectsSortMode.None)) under the hood.
        BindComponent<IGameStateObservable, GameManager>().FromAnywhereInScene();

        // Grab CharacterController on the injection target (Player) transform.
        // Resolves via player.transform.GetComponent<CharacterController>() under the hood.
        BindComponent<CharacterController>().FromTargetSelf();
    }
}
```

> 💡 Scope boilerplate classes can be created via `Saneject/Create New Scope`
> or `Assets/Create/Saneject/Create New Scope`.
>
> Namespace generation can be toggled in `User Settings/Generate Scope Namespace From Folder`.

5. Run dependency injection using either method:

- **Scope inspector:** `Inject Hierarchy Dependencies` button
- **Hierarchy context menu:** Right-click hierarchy panel → `Inject Scene Dependencies`
- **Unity main menu bar:** `Saneject/Inject Scene Dependencies`

Saneject fills in the serialized fields. Press Play, no runtime container required.

> ⚠️️ **Potential inspector conflicts**  
> Saneject includes a `MonoBehaviourFallbackInspector` that ensures injected fields, `[SerializeInterface]` fields, and nested types are drawn with the intended UI/UX by default.
>
> If your inspector looks wrong or incomplete, it's likely another custom inspector or plugin overriding Saneject's inspector. In that case, you can restore the full Saneject layout inside your own inspector by calling:
> ```csharp
> SanejectInspector.DrawDefault(serializedObject, targets, target);
> ```
> For partial integration, you can call individual `SanejectInspector` methods to draw only what you need.
> See [MonoBehaviour fallback inspector](#monobehaviour-fallback-inspector) and [Saneject inspector API](#saneject-inspector-api) for details.