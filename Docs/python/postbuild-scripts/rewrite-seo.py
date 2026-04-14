#!/usr/bin/env python3
"""
rewrite-seo.py

Adds social SEO tags to generated HTML using existing DocFX title and meta description output.
"""

from __future__ import annotations

import html
import json
import re
from pathlib import Path
from urllib.parse import urljoin


DOCS_ROOT = Path(__file__).resolve().parent.parent.parent
DOCFX_CONFIG_PATH = DOCS_ROOT / "docfx.json"
SITE_DIR = DOCS_ROOT / "_site"
SOCIAL_IMAGE_URL = "https://saneject.dev/images/social-preview.webp"
SOCIAL_IMAGE_ALT = "Saneject: Dependency injection the Unity way"
TWITTER_SITE = "@alexanderlarsen"
TITLE_PATTERN = re.compile(r"<title>(?P<value>.*?)</title>", re.IGNORECASE | re.DOTALL)
DESCRIPTION_PATTERN = re.compile(
    r'<meta\s+name=["\']description["\']\s+content=["\'](?P<value>.*?)["\'][^>]*>',
    re.IGNORECASE | re.DOTALL,
)
HEAD_CLOSE_PATTERN = re.compile(r"</head>", re.IGNORECASE)
META_TAG_PATTERN = re.compile(
    r'<meta\s+(?P<attr_name>property|name)=["\'](?P<attr_value>[^"\']+)["\']\s+content=["\'](?P<content>.*?)["\'][^>]*>\s*',
    re.IGNORECASE | re.DOTALL,
)


def load_site_config() -> tuple[str, str]:
    config = json.loads(DOCFX_CONFIG_PATH.read_text(encoding="utf-8"))
    try:
        base_url = config["build"]["sitemap"]["baseUrl"].rstrip("/") + "/"
        app_name = config["build"]["globalMetadata"]["_appName"]
        return base_url, app_name
    except KeyError as exc:
        raise KeyError(
            f"Missing build.sitemap.baseUrl or build.globalMetadata._appName in {DOCFX_CONFIG_PATH}"
        ) from exc


def iter_html_files():
    for path in SITE_DIR.rglob("*.html"):
        if "public" in path.parts:
            continue
        yield path


def extract_title(content: str, path: Path) -> str:
    title_match = TITLE_PATTERN.search(content)
    if title_match is None:
        raise RuntimeError(f"No <title> tag found in {path}")
    return html.unescape(title_match.group("value")).strip()


def extract_description(content: str) -> str | None:
    description_match = DESCRIPTION_PATTERN.search(content)
    if description_match is None:
        return None
    return html.unescape(description_match.group("value")).strip()


def build_page_url(base_url: str, path: Path) -> str:
    relative_path = path.relative_to(SITE_DIR).as_posix()
    if relative_path == "index.html":
        return base_url
    if relative_path.endswith("/index.html"):
        relative_path = relative_path[: -len("index.html")]
        return urljoin(base_url, relative_path)
    if relative_path.endswith(".html"):
        relative_path = relative_path[: -len(".html")]
    return urljoin(base_url, relative_path)


def upsert_meta_tag(content: str, key: str, value: str, *, use_property: bool) -> str:
    escaped_value = html.escape(value, quote=True)
    attr_name = "property" if use_property else "name"
    tag = f'<meta {attr_name}="{key}" content="{escaped_value}">'

    def replace(match: re.Match[str]) -> str:
        if match.group("attr_value").lower() != key.lower():
            return match.group(0)
        replace.found = True
        return f"{tag}\n      "

    replace.found = False  # type: ignore[attr-defined]
    rewritten = META_TAG_PATTERN.sub(replace, content)
    if replace.found:  # type: ignore[attr-defined]
        return rewritten

    head_close_match = HEAD_CLOSE_PATTERN.search(content)
    if head_close_match is None:
        raise RuntimeError("No </head> tag found while updating SEO tags")

    insertion = f"      {tag}\n"
    return content[:head_close_match.start()] + insertion + content[head_close_match.start():]


def rewrite_content(content: str, path: Path, base_url: str, app_name: str) -> str:
    if HEAD_CLOSE_PATTERN.search(content) is None:
        return content

    title_match = TITLE_PATTERN.search(content)
    if title_match is None:
        return content

    title = html.unescape(title_match.group("value")).strip()
    description = extract_description(content)
    page_url = build_page_url(base_url, path)
    og_type = "website" if path.name == "index.html" else "article"

    rewritten = content
    rewritten = upsert_meta_tag(rewritten, "og:site_name", app_name, use_property=True)
    rewritten = upsert_meta_tag(rewritten, "og:title", title, use_property=True)
    rewritten = upsert_meta_tag(rewritten, "og:type", og_type, use_property=True)
    rewritten = upsert_meta_tag(rewritten, "og:url", page_url, use_property=True)
    rewritten = upsert_meta_tag(rewritten, "og:image", SOCIAL_IMAGE_URL, use_property=True)
    rewritten = upsert_meta_tag(rewritten, "og:image:alt", SOCIAL_IMAGE_ALT, use_property=True)
    rewritten = upsert_meta_tag(rewritten, "twitter:image", SOCIAL_IMAGE_URL, use_property=False)
    rewritten = upsert_meta_tag(rewritten, "twitter:card", "summary_large_image", use_property=False)
    rewritten = upsert_meta_tag(rewritten, "twitter:site", TWITTER_SITE, use_property=False)
    rewritten = upsert_meta_tag(rewritten, "twitter:title", title, use_property=False)
    if description is not None:
        rewritten = upsert_meta_tag(rewritten, "og:description", description, use_property=True)
        rewritten = upsert_meta_tag(rewritten, "twitter:description", description, use_property=False)
    return rewritten


def main() -> None:
    base_url, app_name = load_site_config()
    files_changed = 0

    for path in iter_html_files():
        original = path.read_text(encoding="utf-8")
        rewritten = rewrite_content(original, path, base_url, app_name)
        if rewritten == original:
            continue

        path.write_text(rewritten, encoding="utf-8", newline="\n")
        files_changed += 1

    print(f"Updated SEO tags in {files_changed} HTML files")


if __name__ == "__main__":
    main()
