# 11. Risks and Technical Debt

## 11.1 Risks

| ID | Risk | Probability | Impact | Mitigation |
|----|------|-------------|--------|------------|
| R-1 | **External tool breakage**: Pandoc, WeasyPrint, or Homebrew CLI changes break plugin assumptions | Medium | Medium | Version-pin external tools via Homebrew; `mac doctor` detects missing tools early |
| R-2 | **macOS API changes**: Future macOS versions may deprecate or change system commands (e.g., `networksetup`, `caffeinate`) | Low | High | macOS has strong backward compatibility; affected plugins can be updated independently |
| R-3 | **Zsh deprecation on macOS**: Apple could change the default shell again | Very Low | High | Zsh is deeply embedded in macOS; even if default changes, Zsh would remain available |
| R-4 | **.NET AOT limitations**: Future router features may require reflection or dynamic loading incompatible with AOT | Low | Medium | Router is intentionally minimal; complex logic belongs in plugins |
| R-5 | **Single maintainer risk**: Bus factor of 1 | Medium | High | MIT license allows community forking; architecture is simple enough for new maintainers to understand quickly |
| R-6 | **No automated tests**: Regressions may go undetected | High | Medium | Introduce shell-based integration tests; `mac doctor` provides partial coverage |
| R-7 | **PATH collision**: Other tools named `mac-*` could conflict with plugin discovery | Low | Low | Local `plugins/` directory is checked before `$PATH` |

## 11.2 Technical Debt

| ID | Debt | Description | Recommendation |
|----|------|-------------|----------------|
| TD-1 | **No automated test suite** | Reliance on manual testing increases regression risk as plugin count grows | Add shell-based integration tests using a framework like bats-core |
| TD-2 | **No plugin versioning** | Plugins have no version metadata; incompatible changes are not detectable | Add a version header or `--version` flag convention to plugins |
| TD-3 | **Configuration validation** | Invalid config values (e.g., non-existent paths) are not validated at load time | Add a `macstack_validate_config` function to `shared.zsh` |
| TD-4 | **No auto-update mechanism** | Users must manually download new releases | Consider adding a `mac self-update` command or Homebrew tap |
| TD-5 | **Hardcoded paths in config template** | Default config references `$HOME/workspace/...` which may not exist | Use more generic defaults or validate paths before use |
