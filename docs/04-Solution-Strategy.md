# 4. Solution Strategy

## 4.1 Core Architectural Decisions

| Decision | Rationale |
|----------|-----------|
| **Unix-style command routing** | Follow proven patterns from Git and kubectl: a thin binary discovers and delegates to subcommand executables. This maximizes composability and minimizes coupling. |
| **Native AOT compilation** | Eliminates .NET runtime dependency at execution time. Provides fast startup (critical for CLI tools) and a single self-contained binary. |
| **Zsh plugin scripts** | Plugins are plain Zsh scripts, enabling rapid development, easy debugging, and zero compilation overhead. macOS ships with Zsh as the default shell. |
| **Plugin discovery by convention** | Plugins named `mac-<command>` are automatically discovered in the `plugins/` directory or on `$PATH`. No registration, no manifest files. |
| **XDG-compliant configuration** | Follows the XDG Base Directory Specification for predictable, standard configuration paths. |
| **No framework dependencies in plugins** | Plugins depend only on a small shared library (`shared.zsh`) and the bootstrap loader. No package managers or dependency resolution for plugins. |

## 4.2 Technology Choices

| Concern | Technology | Why |
|---------|-----------|-----|
| CLI Router | C# / .NET 10 / Native AOT | Type-safe, fast, single binary output |
| Plugins | Zsh | Native to macOS, excellent process control, rapid iteration |
| Build | Cake (C# build tool) | Reproducible, programmable, same ecosystem as router |
| CI/CD | GitHub Actions | Integrated with GitHub repository hosting |
| Distribution | tar.gz + SHA256 | Minimal, verifiable, no installer complexity |
| Document Export | Pandoc + WeasyPrint | Industry-standard converters, no custom rendering |

## 4.3 Quality Goals Mapping

| Quality Goal | Approach |
|-------------|----------|
| **Fast startup** | Native AOT eliminates JIT warmup. Thin router with minimal logic. |
| **Predictability** | No hidden state, no background processes, deterministic config resolution. |
| **Extensibility** | Plugin system allows adding commands without modifying the core. |
| **Simplicity** | Each plugin is a standalone script. No dependency injection, no ORM, no abstractions. |
| **Reliability** | Defensive validation at plugin entry. Explicit error messages. Unix exit codes. |

## 4.4 Decomposition Strategy

The system is decomposed along **command boundaries**:

- The **core** (C# binary) handles only routing and discovery
- Each **plugin** is a self-contained Zsh script handling one command domain
- **Shared infrastructure** (bootstrap, shared library) provides common utilities without enforcing structure
- **Configuration** is resolved once at startup and sourced into the plugin environment
