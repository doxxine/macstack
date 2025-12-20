
# MacStack — Usage

## Invocation

```
mac <command> [args]
mac help <command>
mac <command> --help
```

## Built‑in Commands

- `mac help` — show global help
- `mac version` — print version
- `mac list` — list available commands/plugins

## Plugins

Any executable named `mac-<command>` is a valid plugin.

Resolution order:
1. `<repo>/plugins/mac-<command>`
2. `$PATH/mac-<command>`

## Exit Codes

- `0` — success
- `1` — generic failure
- `126` — found but not executable
- `127` — command not found

## Examples

```
mac list
mac doctor
mac help doctor
```
