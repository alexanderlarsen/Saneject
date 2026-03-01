# Roslyn tools

Saneject ships three Roslyn DLLs in `Saneject/RoslynLibs`:

| DLL | Type | Purpose |
|---|---|---|
| `SerializeInterfaceGenerator.dll` | Source generator | For every `[SerializeInterface]` field, emits a hidden `Object` backing field and `ISerializationCallbackReceiver` implementation in a matching partial class. This is what makes interface fields appear in the Inspector and survive serialization. |
| `ProxyObjectGenerator.dll` | Source generator | For every partial class marked `[GenerateRuntimeProxy]`, emits a second partial that implements the same interfaces as the bound type and forwards all method calls, property accesses, and event subscriptions to the resolved runtime instance. |
| `AttributesAnalyzer.dll` | Analyzer + code fix | Validates correct usage of `[Inject]`, `[SerializeField]`, and `[SerializeInterface]`. Reports errors for patterns like using `[SerializeInterface]` without `partial`, or combining incompatible attribute combinations. Provides quick-fix actions in IDEs that support the Roslyn code fix protocol (e.g. Rider). |

## How Roslyn tools work in Unity

- A **source generator** runs during compilation and adds new C# source to the project. The generated code is not written to disk — it lives in memory and is compiled alongside your code.
- An **analyzer** inspects your code as you type and reports diagnostics in supported IDEs and in the Unity console.
- A **code fix** provides an automated repair action when an analyzer reports an error.

For setup details and Unity version requirements, see the [Unity Roslyn Analyzers documentation](https://docs.unity3d.com/Manual/roslyn-analyzers.html).

The Roslyn source code is in the [RoslynTools](../../../Roslyn) folder of the repository.
