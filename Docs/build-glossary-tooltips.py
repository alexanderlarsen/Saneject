#!/usr/bin/env python3
"""
build-glossary-tooltips.py

Rewrites generated glossary links in DocFX HTML into inline tooltip terms.

Authoring convention:
    Write normal markdown links to glossary anchors, for example:
        [Binding](../reference/glossary.md#binding)

Build flow:
    1. Run `docfx build`
    2. Run `python build-glossary-tooltips.py`

The script reads the generated glossary page at `_site/docs/reference/glossary.html`,
extracts glossary entries, then replaces matching glossary links across generated HTML
with focusable tooltip markup. Styling lives in `templates/custom/public/main.css`
and runtime viewport clamping lives in `templates/custom/public/main.js`.
"""

from __future__ import annotations

import html
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parent
SITE_DIR = ROOT / "_site"
GLOSSARY_HTML = SITE_DIR / "docs" / "reference" / "glossary.html"

ENTRY_PATTERN = re.compile(
    r'<h3 id="(?P<slug>[^"]+)">(?P<title>.*?)</h3>\s*<p>(?P<body>.*?)</p>',
    re.IGNORECASE | re.DOTALL,
)
ANCHOR_PATTERN = re.compile(
    r"<a\b(?P<attrs>[^>]*)>(?P<label>.*?)</a>",
    re.IGNORECASE | re.DOTALL,
)
HREF_PATTERN = re.compile(
    r'\bhref\s*=\s*(["\'])(?P<href>.*?)\1',
    re.IGNORECASE | re.DOTALL,
)
GLOSSARY_HREF_PATTERN = re.compile(
    r"(?:^|[\\/])glossary\.(?:html|md)#(?P<slug>[-a-z0-9_]+)$",
    re.IGNORECASE,
)
TAG_PATTERN = re.compile(r"<[^>]+>")
CODE_ONLY_LABEL_PATTERN = re.compile(
    r"^\s*<code\b[^>]*>.*?</code>\s*$",
    re.IGNORECASE | re.DOTALL,
)


def normalize_whitespace(value: str) -> str:
    return re.sub(r"\s+", " ", value).strip()


def strip_tags(value: str) -> str:
    return normalize_whitespace(html.unescape(TAG_PATTERN.sub("", value)))


def load_glossary_entries() -> dict[str, dict[str, str]]:
    if not GLOSSARY_HTML.exists():
        raise FileNotFoundError(
            f"Missing generated glossary page: {GLOSSARY_HTML}. Run `docfx build` first."
        )

    content = GLOSSARY_HTML.read_text(encoding="utf-8")
    entries: dict[str, dict[str, str]] = {}

    for match in ENTRY_PATTERN.finditer(content):
        slug = match.group("slug").lower()
        title = strip_tags(match.group("title"))
        body_html = match.group("body").strip()
        body_text = strip_tags(body_html)
        entries[slug] = {
            "title": title,
            "body_html": body_html,
            "body_text": body_text,
        }

    if not entries:
        raise RuntimeError(
            f"No glossary entries found in generated glossary page: {GLOSSARY_HTML}"
        )

    return entries


def iter_html_files():
    for path in SITE_DIR.rglob("*.html"):
        if path == GLOSSARY_HTML:
            continue
        if "public" in path.parts:
            continue
        yield path


def rewrite_glossary_links(content: str, glossary: dict[str, dict[str, str]]):
    tooltip_index = 0
    replaced = 0
    missing_slugs: set[str] = set()

    def replace_anchor(match: re.Match[str]) -> str:
        nonlocal tooltip_index, replaced

        attrs = match.group("attrs")
        href_match = HREF_PATTERN.search(attrs)
        if href_match is None:
            return match.group(0)

        href = html.unescape(href_match.group("href"))
        glossary_match = GLOSSARY_HREF_PATTERN.search(href)
        if glossary_match is None:
            return match.group(0)

        slug = glossary_match.group("slug").lower()
        entry = glossary.get(slug)
        if entry is None:
            missing_slugs.add(slug)
            return match.group(0)

        label_html = match.group("label")
        if CODE_ONLY_LABEL_PATTERN.match(label_html):
            return match.group(0)

        tooltip_index += 1
        replaced += 1
        tooltip_id = f"glossary-tooltip-{tooltip_index}"
        term_title = html.escape(entry["title"])

        return (
            f'<span class="glossary-term" tabindex="0" '
            f'aria-describedby="{tooltip_id}">'
            f'<span class="glossary-term__label">{label_html}</span>'
            f'<span class="glossary-term__tooltip" id="{tooltip_id}" role="tooltip">'
            f'<span class="glossary-term__title">{term_title}</span>'
            f'<span class="glossary-term__body">{entry["body_html"]}</span>'
            f"</span>"
            f"</span>"
        )

    rewritten = ANCHOR_PATTERN.sub(replace_anchor, content)
    return rewritten, replaced, sorted(missing_slugs)


def main():
    glossary = load_glossary_entries()
    files_changed = 0
    replacements = 0
    missing_by_file: dict[Path, list[str]] = {}

    for path in iter_html_files():
        original = path.read_text(encoding="utf-8")
        rewritten, replaced_count, missing_slugs = rewrite_glossary_links(original, glossary)

        if missing_slugs:
            missing_by_file[path] = missing_slugs

        if rewritten == original:
            continue

        path.write_text(rewritten, encoding="utf-8", newline="\n")
        files_changed += 1
        replacements += replaced_count

    print(f"Loaded {len(glossary)} glossary entries from {GLOSSARY_HTML}")
    print(f"Updated {files_changed} HTML files")
    print(f"Replaced {replacements} glossary links with tooltip terms")

    if missing_by_file:
        print("Skipped links with missing glossary entries:")
        for path, slugs in sorted(missing_by_file.items()):
            print(f"  {path}: {', '.join(slugs)}")


if __name__ == "__main__":
    main()
