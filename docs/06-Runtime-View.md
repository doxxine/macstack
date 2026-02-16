# 6. Runtime View

## 6.1 Scenario 1 — Command Startup and Plugin Execution

This scenario shows the full lifecycle of a user running `mac export markdown report.md --title "Q4 Report"`.

```mermaid
sequenceDiagram
    participant User
    participant Router as CLI Router (mac binary)
    participant FS as Filesystem
    participant Bootstrap as bootstrap.zsh
    participant Shared as shared.zsh
    participant Config as macstack.conf
    participant Plugin as mac-export
    participant Pandoc as Pandoc (external)

    User->>Router: mac export markdown report.md --title "Q4 Report"
    Router->>Router: Parse args: command="export"
    Router->>Router: Check built-ins (help, version, list) - no match
    Router->>FS: Check plugins/mac-export exists
    FS-->>Router: Found, executable
    Router->>Router: Set MACROOT environment variable
    Router->>Plugin: exec mac-export markdown report.md --title "Q4 Report"

    Plugin->>Bootstrap: source bootstrap.zsh
    Bootstrap->>Bootstrap: Validate MACROOT is set
    Bootstrap->>Shared: source shared.zsh
    Shared-->>Bootstrap: Helpers loaded
    Bootstrap->>Config: macstack_load_config
    Config-->>Bootstrap: Configuration sourced

    Plugin->>Plugin: Parse subcommand "markdown"
    Plugin->>Plugin: Validate --title is provided
    Plugin->>Plugin: Generate semantic filename (YYYYMMDD_company_slug.pdf)
    Plugin->>Pandoc: pandoc report.md -o output.pdf (with geometry, font options)
    Pandoc-->>Plugin: PDF generated
    Plugin->>FS: Move PDF to EXPORT_OUTPUT_DIR
    Plugin->>User: print_success "Exported: 20260216_private_q4-report.pdf"
```

## 6.2 Scenario 2 — System Update Workflow

This scenario shows `mac system update`, which orchestrates multiple external tool updates.

```mermaid
sequenceDiagram
    participant User
    participant Router as CLI Router
    participant Plugin as mac-system
    participant Brew as Homebrew
    participant Chezmoi as Chezmoi
    participant Git as Git
    participant OMZ as Oh-My-Zsh

    User->>Router: mac system update
    Router->>Plugin: exec mac-system update

    Plugin->>Plugin: source bootstrap.zsh, load config

    Plugin->>Brew: brew update
    Brew-->>Plugin: Updated
    Plugin->>Brew: brew upgrade
    Brew-->>Plugin: Upgraded
    Plugin->>User: print_success "Homebrew updated"

    alt USE_CHEZMOI_PULL == true
        Plugin->>Chezmoi: chezmoi git pull
        Chezmoi-->>Plugin: Pulled
        Plugin->>User: print_success "Chezmoi updated"
    end

    Plugin->>Git: git -C omz-custom-dir pull (quiet)
    Git-->>Plugin: Pulled
    Plugin->>User: print_success "Oh-My-Zsh plugins updated"

    Plugin->>Git: git -C workspace-tools pull (quiet)
    Git-->>Plugin: Pulled
    Plugin->>User: print_success "Workspace tools updated"

    alt SYSTEM_UPDATE_RUN_CLEANUP == true
        Plugin->>Brew: brew cleanup --prune=all
        Brew-->>Plugin: Cleaned
    end

    Plugin->>User: print_success "System update complete"
```

## 6.3 Scenario 3 — Error Handling: Command Not Found

This scenario demonstrates the error path when a user invokes a non-existent command.

```mermaid
sequenceDiagram
    participant User
    participant Router as CLI Router
    participant FS as Filesystem
    participant PATH as System PATH

    User->>Router: mac frobnicate --verbose
    Router->>Router: Parse args: command="frobnicate"
    Router->>Router: Check built-ins - no match

    Router->>FS: Check plugins/mac-frobnicate exists
    FS-->>Router: Not found

    Router->>PATH: Search PATH for mac-frobnicate
    PATH-->>Router: Not found

    Router->>User: Error: unknown command "frobnicate"
    Router->>User: Run "mac list" to see available commands
    Router->>Router: Exit with code 127
```

## 6.4 Scenario 4 — Plugin Validation Failure

This scenario shows error handling within a plugin when required arguments are missing.

```mermaid
sequenceDiagram
    participant User
    participant Router as CLI Router
    participant Plugin as mac-export
    participant Bootstrap as bootstrap.zsh

    User->>Router: mac export markdown report.md
    Router->>Plugin: exec mac-export markdown report.md

    Plugin->>Bootstrap: source bootstrap.zsh
    Bootstrap-->>Plugin: Initialized

    Plugin->>Plugin: Parse subcommand "markdown"
    Plugin->>Plugin: Check --title flag
    Plugin->>Plugin: require_value: title is empty
    Plugin->>User: print_error "Missing required option: --title"
    Plugin->>Router: Exit with code 1
```
