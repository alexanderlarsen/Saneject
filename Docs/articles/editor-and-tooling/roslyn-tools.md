# Roslyn tools

Roslyn tools enhance your compile-time experience using C#'s powerful compiler APIs.

- A **Roslyn analyzer** inspects code and reports diagnostics (like errors or suggestions).
- A **Roslyn code fix** provides a quick-fix action for an analyzer error.
- A **Roslyn source generator** adds new C# code to your project during compilation.

Saneject ships with three Roslyn tool DLLs (in `Saneject/RoslynLibs`):

| DLL                                 | Type                | Purpose                                                                                                                                                                                            |
|-------------------------------------|---------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **SerializeInterfaceGenerator.dll** | Source generator    | Generates hidden backing fields and serialization hooks for `[SerializeInterface]` members.                                                                                                        |
| **ProxyObjectGenerator.dll**        | Source generator    | Emits proxy classes for types marked with `[GenerateProxyObject]`. Each proxy is a `ScriptableObject` that implements the same interfaces and forwards all calls to the resolved runtime instance. |
| **AttributesAnalyzer.dll**          | Analyzer + code fix | Validates field decoration rules for `[Inject]`, `[SerializeField]`, and `[SerializeInterface]`. Includes context-aware quick fixes in supported IDEs (like Rider).                                |

Unity's official Roslyn analyzer documentation (including setup instructions):  
<https://docs.unity3d.com/Manual/roslyn-analyzers.html>

Use that guide if you want to plug in custom Roslyn tooling or integrate Saneject's tools in other project structures.

Roslyn source code is in the [RoslynTools](RoslynTools) folder.