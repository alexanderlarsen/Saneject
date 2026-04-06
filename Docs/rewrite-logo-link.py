#!/usr/bin/env python3
"""
rewrite-logo-link.py

Rewrites the DocFX navbar brand link to the site root.

DocFX emits relative links like:
    index.html
    ../index.html

This post-build step rewrites navbar brand anchors to:
    /
"""

from __future__ import annotations

import html
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parent
SITE_DIR = ROOT / "_site"
ANCHOR_START_TAG_PATTERN = re.compile(
    r"<a\b(?P<attrs>[^>]*)>",
    re.IGNORECASE | re.DOTALL,
)
CLASS_PATTERN = re.compile(
    r'\bclass\s*=\s*(["\'])(?P<value>.*?)\1',
    re.IGNORECASE | re.DOTALL,
)
HREF_PATTERN = re.compile(
    r'(?P<name>\bhref\b)\s*=\s*(?P<quote>["\'])(?P<value>.*?)(?P=quote)',
    re.IGNORECASE | re.DOTALL,
)


def iter_html_files():
    for path in SITE_DIR.rglob("*.html"):
        if "public" in path.parts:
            continue
        yield path


def rewrite_content(content: str) -> tuple[str, int]:
    replacements = 0

    def replace_anchor_start_tag(match: re.Match[str]) -> str:
        nonlocal replacements

        attrs = match.group("attrs")
        class_match = CLASS_PATTERN.search(attrs)
        if class_match is None:
            return match.group(0)

        classes = html.unescape(class_match.group("value")).split()
        if "navbar-brand" not in classes:
            return match.group(0)

        def replace_href(href_match: re.Match[str]) -> str:
            nonlocal replacements
            if href_match.group("value") == "/":
                return href_match.group(0)

            replacements += 1
            return f'{href_match.group("name")}={href_match.group("quote")}/{href_match.group("quote")}'

        rewritten_attrs = HREF_PATTERN.sub(replace_href, attrs, count=1)
        return f"<a{rewritten_attrs}>"

    rewritten = ANCHOR_START_TAG_PATTERN.sub(replace_anchor_start_tag, content)
    return rewritten, replacements


def main():
    files_changed = 0
    replacements = 0

    for path in iter_html_files():
        original = path.read_text(encoding="utf-8")
        rewritten, replaced_count = rewrite_content(original)
        if rewritten == original:
            continue

        path.write_text(rewritten, encoding="utf-8", newline="\n")
        files_changed += 1
        replacements += replaced_count

    print(f"Updated {files_changed} HTML files")
    print(f"Rewrote {replacements} navbar brand links to /")


if __name__ == "__main__":
    main()
