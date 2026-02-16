# 2. Constraints

## 2.1 Technical Constraints

| Constraint | Description |
|------------|-------------|
| **macOS Only** | The system targets macOS exclusively. The AOT binary is compiled for `osx-arm64` (primary) and `osx-x64` (CI support). No Linux or Windows targets. |
| **Native AOT Compilation** | The CLI router is compiled using .NET Native AOT, eliminating the need for a .NET runtime at execution time but restricting the use of reflection and dynamic loading. |
| **.NET 10.0** | The C# router requires .NET 10.0 SDK for compilation. |
| **Zsh Dependency** | All plugins are Zsh scripts. Bash or other shells are not supported as plugin runtimes. |
| **No Runtime Dependencies** | The compiled binary has zero runtime dependencies. Plugins depend only on Zsh and explicitly documented external tools. |
| **XDG Base Directory Compliance** | Configuration must follow XDG conventions, defaulting to `~/.config/macstack/`. |

## 2.2 Organizational Constraints

| Constraint | Description |
|------------|-------------|
| **Single Maintainer** | The project is maintained by a single developer, favoring simplicity over enterprise features. |
| **MIT License** | All contributions and dependencies must be compatible with the MIT license. |
| **No Telemetry** | The tool collects no usage data, analytics, or telemetry. |

## 2.3 Convention Constraints

| Constraint | Description |
|------------|-------------|
| **Plugin Naming** | Plugins must be named `mac-<command>` and placed in the `plugins/` directory or on `$PATH`. |
| **Plugin Interface** | Plugins must support `--help` and `--description` flags. They must source `bootstrap.zsh` for initialization. |
| **Unix Exit Codes** | Plugins must use standard exit codes: 0 (success), 1 (failure), 2 (usage error). |
| **No System Modification** | MacStack must not install software, modify system files, or alter shell profiles. |
