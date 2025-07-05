# Building and Updating Saneject Source-Generator DLLs

The Unity project already ships with the two source-generator DLLs in place and correctly configured.  
Only follow the steps below when you need to rebuild them (for example after code changes).

## Prerequisites
- .NET SDK 7.0 or later on your PATH.
- The repository cloned locally.

## Build + Copy Commands

**SerializeInterface generator build command**
```bash
dotnet build -c Release -p:Generator=SerializeInterface -p:AssemblyName=SerializeInterfaceGenerator
```

**SerializeInterface generator copy command**
```bash
copy bin\Release\netstandard2.0\SerializeInterfaceGenerator.dll ../UnityProject/Saneject/Assets/Plugins/Saneject/SourceGenerators
```

**InterfaceProxyObject generator build command**
```bash
dotnet build -c Release -p:Generator=InterfaceProxy -p:AssemblyName=InterfaceProxyGenerator
```

**InterfaceProxyObject generator copy command**
```bash
copy bin\Release\netstandard2.0\InterfaceProxyGenerator.dll ../UnityProject/Saneject/Assets/Plugins/Saneject/SourceGenerators
```

Unity compiles the new DLLs automatically the next time it gains focus.

## Fresh Setup (only if you start from scratch)

1. Copy both DLLs to `Assets/Plugins/Saneject/SourceGenerators/`.

2. Select each DLL in Unity and set import options:
   - **Platform**: untick all platforms, including *Any Platform*.
   - Leave **Auto Reference** and **Validate References** unchecked.

3. In *Asset Labels*, add `RoslynAnalyzer`.

Unity will now load the generators during IDE analysis and builds.
