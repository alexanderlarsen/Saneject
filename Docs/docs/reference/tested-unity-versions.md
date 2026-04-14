---
title: Tested Unity versions
description: Check which Unity editor versions are automatically tested for Saneject in CI, including supported LTS and Unity 6 release lines.
---

# Tested Unity versions

[![Tests](https://img.shields.io/github/actions/workflow/status/alexanderlarsen/Saneject/tests.yml?label=Tests)](https://github.com/alexanderlarsen/Saneject/actions/workflows/tests.yml)

Saneject is automatically tested in [CI](https://github.com/alexanderlarsen/Saneject/actions/workflows/tests.yml) against the following Unity editor versions.

The test matrix is defined using minimum and maximum tested editor versions per supported Unity release line.

| Unity Version | Release Type | Notes                                                         |
|---------------|--------------|---------------------------------------------------------------|
| 2022.3.12f1   | LTS          | Minimum supported version for Roslyn generators and analyzers |
| 2022.3.62f3   | LTS          | Maximum tested 2022 LTS                                       |
| 6000.0.58f2   | LTS          | Minimum tested Unity 6.0 LTS                                  |
| 6000.0.63f1   | LTS          | Maximum tested Unity 6.0 LTS                                  |
| 6000.1.17f1   | Supported    | Minimum/maximum tested Unity 6.1                              |
| 6000.2.6f2    | Supported    | Minimum tested Unity 6.2                                      |
| 6000.2.15f1   | Supported    | Maximum tested Unity 6.2                                      |
| 6000.3.0f1    | LTS          | Minimum tested Unity 6.3                                      |
| 6000.3.12f1   | LTS          | Maximum tested Unity 6.3                                      |
| 6000.4.0f1    | Supported    | Minimum tested Unity 6.4                                      |
| 6000.4.1f1    | Supported    | Maximum tested Unity 6.4                                      |

In-between versions will likely work, but only the above are verified in automated tests.  
Unity 2023 releases are skipped since they are tech stream builds (not long-term supported), and Unity does not recommend them for production.

> Note: Automated tests depend on [GameCI](https://game.ci/). Tested Unity versions are updated periodically as GameCI adds support for newer editor versions.
