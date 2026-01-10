# Building and Updating Saneject Roslyn Generators and Analyzers

The Unity project already ships with the required Roslyn DLLs in place and correctly configured.

Only follow the steps below when you need to modify and rebuild them.

## Prerequisites

- .NET SDK installed (any reasonably recent version works)
- The repository cloned locally
- Open the `Saneject.Roslyn.sln` solution in Rider or Visual Studio

## Build workflow

The Roslyn projects are now split cleanly and build into unified output folders.

### 1. Build the solution

From the IDE:

- Open `Saneject.Roslyn.sln`
- Build the solution in **Release**

Or from the command line (from the `Roslyn` folder):

```bash
dotnet build -c Release
```

### 2. Build output locations

After a successful build, the DLLs are built to flat these folders at the repo root:

**Experimental (current generators/analyzers):**
```
<repo root>/Build/Roslyn.Experimental/
```

Contains:
- `Saneject.SerializeInterface.Generator.dll`
- `Saneject.ProxyObject.Generator.dll`
- `Saneject.Analyzers.dll`

**Legacy (backward-compatibility versions):**
```
<repo root>/Build/Roslyn.Legacy/
```

Contains:
- `Saneject.SerializeInterface.Generator.Legacy.dll`
- `Saneject.ProxyObject.Generator.Legacy.dll`
- `Saneject.Analyzers.Legacy.dll`

### 3. Copy DLLs into the Unity project

Copy the desired DLLs into the Unity plugin folder:

```
UnityProject/Saneject/Assets/Plugins/Saneject/RoslynLibs/
```

(Choose either Experimental or Legacy DLLs depending on what you want Unity to load.)

Unity will reimport the DLLs automatically the next time it gains focus.

## Unity Setup (only needed once or on fresh setup)

1. Ensure all Roslyn DLLs are located in:
   ```
   Assets/Plugins/Saneject/RoslynLibs/
   ```

2. For each DLL in Unity:
    - Select the DLL in the Project view
    - **Untick all platforms**, including *Any Platform*
    - Leave **Auto Reference** and **Validate References** unchecked


3. In **Asset Labels**, add:
   ```
   RoslynAnalyzer
   ```

Unity will now load the source generators and analyzers during IDE analysis and builds.