#!/usr/bin/env python3
"""
prebuild.py

Runs DocFX prebuild scripts in sequence.
"""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path


SCRIPT_DIR = Path(__file__).resolve().parent
PREBUILD_SCRIPTS = [
    "prebuild-scripts/fix-api-toc.py",
]


def main() -> None:
    for script_name in PREBUILD_SCRIPTS:
        script_path = SCRIPT_DIR / script_name
        print(f"Running {script_path.relative_to(SCRIPT_DIR)}")
        subprocess.run([sys.executable, str(script_path)], check=True)


if __name__ == "__main__":
    main()
