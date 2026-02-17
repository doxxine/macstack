# 9. Architecture Decisions

## ADR-0001: Native AOT Compilation for CLI Router

**Status**: Accepted
**Context**: CLI tools must start quickly. The .NET runtime adds significant startup latency (100-500ms) due to JIT compilation. Users expect sub-100ms response times for simple commands like `mac version`.
**Decision**: Compile the CLI router using .NET Native AOT (`PublishAot=true`), producing a self-contained native binary.
**Consequences**:
- Startup time is near-instantaneous (comparable to C/Go binaries)
- No .NET runtime installation required on target machines
- Reflection and dynamic assembly loading are unavailable
- Binary size is larger than a managed assembly but acceptable (~10 MB)
- Build requires .NET 10 SDK; runtime does not

## ADR-0002: Zsh as Plugin Runtime

**Status**: Accepted
**Context**: Plugins need to integrate deeply with macOS system tools (Finder, caffeinate, networksetup) and compose Unix commands. A compiled language would add friction for rapid iteration.
**Decision**: Implement all plugins as Zsh scripts. macOS ships with Zsh as the default shell since Catalina (10.15).
**Consequences**:
- Zero compilation overhead for plugin development
- Direct access to all system utilities without FFI or subprocess wrappers
- Plugins are human-readable and easily debuggable
- No cross-platform portability (acceptable given macOS-only scope)
- Type safety is limited to runtime validation

## ADR-0003: Plugin Discovery by Naming Convention

**Status**: Accepted
**Context**: Plugin registration via manifest files or configuration adds complexity and creates coupling between the core and plugins.
**Decision**: Discover plugins by naming convention (`mac-<command>`), searching the `plugins/` directory first, then `$PATH`.
**Consequences**:
- Adding a plugin requires only placing an executable file with the correct name
- No registration step, no manifest files, no plugin configuration
- Third-party plugins can be installed anywhere on `$PATH`
- Risk of naming collisions with other `mac-*` executables on `$PATH` (mitigated by checking `plugins/` first)

## ADR-0004: XDG Base Directory Compliance

**Status**: Accepted
**Context**: macOS applications traditionally use `~/Library/` for configuration. However, developer tools increasingly follow XDG conventions for consistency with Linux tooling.
**Decision**: Store user configuration at `$XDG_CONFIG_HOME/macstack/macstack.conf` (defaulting to `~/.config/macstack/macstack.conf`).
**Consequences**:
- Configuration location is predictable and follows developer conventions
- Compatible with dotfile managers (Chezmoi, stow) that expect XDG paths
- Diverges from macOS platform conventions (`~/Library/Preferences/`)
- Users can override via `$MACSTACK_CONFIG` environment variable

## ADR-0005: Cake as Build System

**Status**: Accepted
**Context**: The project needs a build system that can handle .NET AOT compilation, file staging, and archive creation. Shell scripts alone lack structure for multi-step builds.
**Decision**: Use Cake (C# Make), a C#-based build automation tool, invoked via `dotnet cake`.
**Consequences**:
- Build logic is written in C#, the same language as the router
- Supports task dependencies, error handling, and cross-step state
- Pinned locally via `.config/dotnet-tools.json` for reproducibility
- Adds a .NET SDK dependency to the build environment (not the runtime)

## ADR-0006: No Persistent State

**Status**: Accepted
**Context**: CLI tools that maintain databases, caches, or state files introduce complexity in backup, migration, and debugging.
**Decision**: MacStack maintains no persistent state. All behavior is determined by configuration files and command arguments.
**Consequences**:
- No migration concerns between versions
- No data loss risk beyond configuration files
- No caching benefits (every invocation starts fresh)
- Stateful features (e.g., command history, usage analytics) are not possible

## ADR-0007: Single Binary Distribution

**Status**: Accepted
**Context**: The system needs to be easily installable without package managers or complex setup procedures.
**Decision**: Distribute as a tar.gz archive containing the native binary, plugins, libraries, and default configuration. Include SHA256 checksums for verification.
**Consequences**:
- Installation is a simple extract-and-run operation
- No dependency on Homebrew tap, npm, or other package registries
- Updates require manual download (no auto-update mechanism)
- Assumption: Users can add the binary to their `$PATH` manually

## ADR-0008: Separation of Core Router and Plugin Logic

**Status**: Accepted
**Context**: Mixing routing logic with business logic in a single binary would create tight coupling and prevent independent plugin development.
**Decision**: The C# router handles only command parsing, plugin discovery, and process execution. All business logic lives in Zsh plugins.
**Consequences**:
- Router changes never affect plugin behavior and vice versa
- Plugins can be developed and tested independently
- Cross-language boundary (C# to Zsh) adds a process execution overhead per command
- Router cannot validate plugin arguments (delegates fully to the plugin)
