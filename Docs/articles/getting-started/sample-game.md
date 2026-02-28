# Sample game

> ⚠️ **Missing sample refs in older Unity versions**
>
> Some older Unity versions, such as 2022.3.12f1, have a bug that can break script references in sample prefabs or scenes when importing samples via the Package Manager. If this happens, right-click the **Samples** folder and then **Reimport** to refresh all references.  
> More details: https://discussions.unity.com/t/broken-script-references-on-updating-custom-package-through-package-manager-and-committing-it-to-git/910632/7

The sample is provided as a reference project and can be studied to see how bindings, scopes, and proxies work together in practice.

It contains a small three-scene game where the player (green bean) chases enemies (red beans) while they try to evade. When all enemies are caught, the game is over and restart UI appears. It's intentionally kept minimal so you can study the setup and learn Saneject's core concepts without distractions.

![Screenshot of Saneject sample game](Docs/sample-game-screenshot.webp)

It shows how to use Saneject in a real (but simple) game setup:

- **Multiple levels of binding**: Shows how you can declare bindings at different granularities (scene-wide, prefab-local, or object-local) so each piece only knows about what it needs.
- **Cross-scene and prefab references**: Demonstrates how to connect systems that live in different scenes or inside prefabs using `SerializeInterface` and proxies, without breaking Unity's prefab isolation.
- **Global scope usage**: Core systems like the game manager or score tracker are promoted to globals, making them easily accessible across the whole project without resorting to singletons.
- **UI integration**: The sample UI is wired entirely through interfaces in an MVC-like pattern, so buttons and text elements update automatically from gameplay state, cleanly separated from game logic.
- **Game loop orchestration**: Player, enemies, and UI are stitched together through DI so that chasing, scoring, and restarting the game all happen through clear, testable contracts instead of hard references.

## Location by install method

| Install method | Location                                                                                                       |
|----------------|----------------------------------------------------------------------------------------------------------------|
| Unity package  | `Assets/Plugins/Saneject/Samples/DemoGame`                                                                     |
| UPM / Git URL  | First import from the Package Manager Samples tab, then locate at `Assets/Samples/Saneject/<version>/DemoGame` |

### To run it

1. Add the following scenes to Build Settings (in this order):

- `StartScene` (bootstrap)
- `GameScene`
- `UIScene`

2. Open `StartScene`
3. Press Play.