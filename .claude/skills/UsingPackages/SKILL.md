---
name: UsingPackages
description: Specific package usage in this project. ie python requirements, npm/node packages, Laravel Composer etc.
---

# What I do

PACKAGE MANAGEMENT:
When adding new dependencies, always check for the latest stable version using webfetch:

- **Python (PyPI)**: `https://pypi.org/pypi/{package_name}/json`
- **npm/Node.js**: `https://registry.npmjs.org/{package_name}/latest`
- **NuGet (.NET)**: `https://api.nuget.org/v3/registration5-semver1/{package_name}/index.json`
- **Maven (Java)**: `https://search.maven.org/solrsearch/select?q=g:{group_id}+AND+a:{artifact_id}&rows=1&wt=json`
- **Cargo (Rust)**: `https://crates.io/api/v1/crates/{crate_name}`
- **Go modules**: `https://proxy.golang.org/{module_path}/@latest`
- **RubyGems**: `https://rubygems.org/api/v1/versions/{gem_name}/latest.json`

Example workflow when adding a Python package:
1. Use webfetch to check `https://pypi.org/pypi/chromadb/json`
2. Extract the latest version from the response
3. Add to requirements.txt with pinned version: `chromadb==0.x.x`
4. Document why this version was chosen if relevant

This ensures reproducible builds and awareness of the dependency landscape.