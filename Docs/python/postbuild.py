#!/usr/bin/env python3
"""
postbuild.py

Runs DocFX postbuild scripts in sequence.
"""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
POSTBUILD_SCRIPTS = [
    "postbuild-scripts/rewrite-logo-link.py",
    "postbuild-scripts/rewrite-homepage-title.py",
    "postbuild-scripts/rewrite-seo.py",
]


def main() -> None:
    for script_name in POSTBUILD_SCRIPTS:
        script_path = SCRIPT_DIR / script_name
        print(f"Running {script_path.relative_to(SCRIPT_DIR)}")
        subprocess.run([sys.executable, str(script_path)], check=True)


if __name__ == "__main__":
    main()
