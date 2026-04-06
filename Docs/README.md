# Docs build notes

## Why metadata is generated locally

API metadata must be generated locally because `docfx metadata` needs the Unity-generated `.csproj` files. Those files are gitignored and are not source controlled, so CI does not have them.

If the API changes, regenerate the API docs locally and commit the updated files in `Docs/api`:

```powershell
docfx metadata
python fix-api-toc.py
```

`fix-api-toc.py` must run after `docfx metadata` because `docfx metadata` generates `Docs/api`, including the raw `Docs/api/toc.yml` file that the script rewrites into the structure used by the site.

`build-glossary-tooltips.py` rewrites glossary links in generated HTML into focusable inline tooltip markup. Tooltip positioning and viewport clamping are handled by `templates/custom/public/main.css` and `templates/custom/public/main.js`.

## Full local build and serve

```powershell
docfx metadata
python fix-api-toc.py
docfx build
python build-glossary-tooltips.py
python rewrite-logo-link.py
docfx serve _site
```

## What CI does

CI is defined in [`.github/workflows/docfx.yml`](E:/Unity/Personal/Saneject/.github/workflows/docfx.yml).

It does not run `docfx metadata`.

It builds the site from the checked-in docs content, including the already-generated files in `Docs/api`, then runs the glossary tooltip post-processing script, rewrites the DocFX navbar brand link to `/`, and deploys `Docs/_site` to GitHub Pages.
