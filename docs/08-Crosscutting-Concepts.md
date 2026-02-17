# 8. Crosscutting Concepts

## 8.1 Plugin Bootstrap Pattern

Every plugin follows the same initialization sequence:

```zsh
#!/usr/bin/env zsh
source "$MACROOT/lib/bootstrap.zsh"
macstack_load_config || { print_error "Config failed"; exit 1; }
```

This pattern ensures:
- `MACROOT` is validated
- The shared library is loaded (colors, helpers, validation)
- Configuration is sourced into the environment
- Failure at any stage aborts with a clear error

## 8.2 Configuration Resolution

Configuration follows a deterministic resolution order:

1. `$MACSTACK_CONFIG` (environment variable override)
2. `$XDG_CONFIG_HOME/macstack/macstack.conf` (user config)
3. `<MACROOT>/macstack.conf` (repository default)

First match wins. Configuration files are Zsh-sourceable, making all values immediately available as shell variables.

## 8.3 Error Handling

### Exit Code Conventions

| Code | Meaning | Source |
|------|---------|--------|
| 0 | Success | All components |
| 1 | General failure / validation error | Plugins |
| 2 | Unknown subcommand / usage error | Plugins |
| 126 | Plugin found but not executable | Router |
| 127 | Command not found | Router |

### Error Output Pattern

- Errors are written to stderr via `print -u2`
- Colored output uses ANSI escape codes: red for errors, yellow for warnings
- Helper functions (`print_error`, `print_warn`, `print_info`, `print_success`) standardize formatting

### Validation Pattern

```zsh
require_value "$title" "--title is required"
```

Validation happens early (fail-fast). Missing arguments, unset variables, and missing tools are checked before any work begins.

## 8.4 Colored Output

All plugins use a shared color palette defined in `shared.zsh`:

| Function | Color | Purpose |
|----------|-------|---------|
| `print_success` | Green | Completed operations |
| `print_warn` | Yellow | Non-fatal warnings |
| `print_error` | Red | Fatal errors |
| `print_info` | Cyan | Informational messages |

## 8.5 Semantic Filename Generation

The export plugin generates filenames using a deterministic pattern:

```
YYYYMMDD_company_slugified-title.pdf
```

- Date: current date at time of export
- Company: from `--company` flag or `EXPORT_COMPANY` config variable
- Title: slugified (lowercased, spaces replaced with hyphens, special characters removed)

## 8.6 Tool Availability Checking

Plugins that depend on external tools check for their presence before attempting to use them:

```zsh
if ! command -v pandoc &>/dev/null; then
    print_error "pandoc is not installed"
    exit 1
fi
```

The `mac-doctor` plugin provides a centralized health check that validates all required and optional tool dependencies.

## 8.7 Plugin Discovery Convention

Plugins are discovered by naming convention, not registration:

1. The router looks for `mac-<command>` in `<MACROOT>/plugins/`
2. Falls back to searching `$PATH` for `mac-<command>`
3. Plugins must be executable files
4. Plugins must support `--help` and `--description` flags

This follows the same pattern used by Git (`git-<command>`) and kubectl plugins.

## 8.8 Shell Completion

The `mac-completion` plugin generates Zsh completion definitions by:

1. Iterating over all discovered plugins
2. Calling each plugin with `--description` to get a one-line summary
3. Emitting a Zsh `_describe` completion function

This allows tab-completion of all commands in the user's shell.
