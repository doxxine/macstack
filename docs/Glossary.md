# Glossary

| Term | Definition |
|------|------------|
| **AOT (Ahead-Of-Time)** | Compilation strategy that produces native machine code at build time, eliminating the need for a runtime or JIT compiler |
| **Bootstrap** | The initialization sequence every plugin executes to validate the environment, load shared libraries, and source configuration |
| **Cake** | A C#-based build automation tool used as MacStack's build system |
| **Chezmoi** | A dotfile management tool that MacStack can optionally integrate with for system updates |
| **CLI Router** | The native AOT binary (`mac`) that parses commands and delegates to plugins |
| **Finder Tags** | macOS metadata tags applied to files via the `tag` command-line utility |
| **MACROOT** | Environment variable pointing to the MacStack installation root directory; set by the router and used by all plugins |
| **Native AOT** | .NET's Native Ahead-Of-Time compilation mode that produces self-contained native executables |
| **Pandoc** | A universal document converter used by `mac-export` for Markdown to PDF conversion |
| **Plugin** | A standalone executable (Zsh script) named `mac-<command>` that implements a specific command domain |
| **Semantic Filename** | A deterministic filename pattern (`YYYYMMDD_company_slug.pdf`) used for exported documents |
| **Shared Library** | The `shared.zsh` file providing common utilities (output formatting, validation, config loading) to all plugins |
| **WeasyPrint** | An HTML/CSS to PDF converter used by `mac-export` for HTML to PDF conversion |
| **XDG Base Directory** | A specification defining standard locations for user configuration, data, and cache files (e.g., `~/.config/`) |
