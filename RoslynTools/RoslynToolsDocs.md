# Building and Updating Saneject Source-Generator and Analyzer DLLs

The Unity project already ships with the required Roslyn DLLs in place and correctly configured.

Only follow the steps below when you need to modify and rebuild them.

## Prerequisites

- .NET SDK 7.0 or later on your PATH
- The repository cloned locally

## Build + Copy Commands

### Ensure you're in the `RoslynTools` directory:

```bash
cd path/to/RoslynTools
```

### SerializeInterface generator build command

```bash
dotnet build -c Release -p:Module=SerializeInterface -p:AssemblyName=SerializeInterfaceGenerator
```

### SerializeInterface generator copy command

```bash
copy bin\Release\netstandard2.0\SerializeInterfaceGenerator.dll ../UnityProject/Saneject/Assets/Plugins/Saneject/RoslynLibs
```

### ProxyObject generator build command

```bash
dotnet build -c Release -p:Module=InterfaceProxy -p:AssemblyName=ProxyObjectGenerator
```

### ProxyObject generator copy command

```bash
copy bin\Release\netstandard2.0\ProxyObjectGenerator.dll ../UnityProject/Saneject/Assets/Plugins/Saneject/RoslynLibs
```

### AttributesAnalyzer build command

```bash
dotnet build -c Release -p:Module=AttributesAnalyzer -p:AssemblyName=AttributesAnalyzer
```

### AttributesAnalyzer copy command

```bash
copy bin\Release\netstandard2.0\AttributesAnalyzer.dll ../UnityProject/Saneject/Assets/Plugins/Saneject/RoslynLibs
```

Unity compiles the new DLLs automatically the next time it gains focus.

## Fresh Setup (only if you start from scratch)

1. Copy all DLLs to `Assets/Plugins/Saneject/RoslynLibs/`.

2. Select each DLL in Unity and set import options:
    - **Platform**: untick all platforms, including *Any Platform*.
    - Leave **Auto Reference** and **Validate References** unchecked.

3. In *Asset Labels*, add `RoslynAnalyzer`.

Unity will now load the generators during IDE analysis and builds.
