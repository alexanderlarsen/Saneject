# Docs build notes

## DocFX version

CI pins DocFX to `2.78.4` to match the existing local documentation toolchain. Use the same version locally to keep API generation and site builds consistent:

```powershell
dotnet tool update -g docfx --version 2.78.4
```

## Why metadata is generated locally

API metadata must be generated locally because `docfx metadata` needs the Unity-generated `.csproj` files. Those files are gitignored and are not source controlled, so CI does not have them.

If the API changes, regenerate the API docs locally and commit the updated files in `Docs/api`:

```powershell
docfx metadata
python python/prebuild.py
```

`python/prebuild.py` must run after `docfx metadata` because `docfx metadata` generates `Docs/api`, including the raw `Docs/api/toc.yml` file that the prebuild script rewrites into the structure used by the site.

## Full local build and serve

```powershell
docfx metadata
python python/prebuild.py
docfx build
python python/postbuild.py
docfx serve _site
```

## What CI does

CI is defined in [`.github/workflows/docfx.yml`](E:/Unity/Personal/Saneject/.github/workflows/docfx.yml).

It does not run `docfx metadata`.

It builds the site from the checked-in docs content, including the already-generated files in `Docs/api`, runs the postbuild Python scripts, and deploys `Docs/_site` to GitHub Pages.
