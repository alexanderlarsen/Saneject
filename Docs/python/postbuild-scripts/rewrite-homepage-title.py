#!/usr/bin/env python3
"""
rewrite-homepage-title.py

Rewrites the generated home page title using `_appTitle` from docfx.json.
"""

from __future__ import annotations

import json
import re
from pathlib import Path


DOCS_ROOT = Path(__file__).resolve().parent.parent.parent
DOCFX_CONFIG_PATH = DOCS_ROOT / "docfx.json"
HOMEPAGE_PATH = DOCS_ROOT / "_site" / "index.html"
TITLE_PATTERN = re.compile(r"<title>.*?</title>", re.IGNORECASE | re.DOTALL)
META_TITLE_PATTERN = re.compile(
    r'(<meta\s+name=["\']title["\']\s+content=["\']).*?(["\'][^>]*>)',
    re.IGNORECASE | re.DOTALL,
)


def load_app_title() -> str:
    config = json.loads(DOCFX_CONFIG_PATH.read_text(encoding="utf-8"))
    try:
        return config["build"]["globalMetadata"]["_appTitle"]
    except KeyError as exc:
        raise KeyError(f"Missing build.globalMetadata._appTitle in {DOCFX_CONFIG_PATH}") from exc


def main() -> None:
    app_title = load_app_title()
    original = HOMEPAGE_PATH.read_text(encoding="utf-8")
    rewritten, title_replacements = TITLE_PATTERN.subn(f"<title>{app_title}</title>", original, count=1)

    if title_replacements == 0:
        raise RuntimeError(f"No <title> tag found in {HOMEPAGE_PATH}")

    rewritten, meta_replacements = META_TITLE_PATTERN.subn(
        rf"\1{app_title}\2",
        rewritten,
        count=1,
    )

    if meta_replacements == 0:
        raise RuntimeError(f'No <meta name="title"> tag found in {HOMEPAGE_PATH}')

    if rewritten == original:
        print(f"Home page title metadata already matches {DOCFX_CONFIG_PATH.name}")
        return

    HOMEPAGE_PATH.write_text(rewritten, encoding="utf-8", newline="\n")
    print(f"Updated home page title metadata in {HOMEPAGE_PATH}")


if __name__ == "__main__":
    main()
