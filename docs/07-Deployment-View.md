# 7. Deployment View

## 7.1 Deployment Model

MacStack is a local-only CLI tool. There are no servers, containers, or cloud infrastructure. The entire system runs on a single macOS workstation.

## 7.2 Deployment Diagram

```mermaid
graph TB
    subgraph "Developer Workstation (macOS)"
        subgraph "MacStack Installation"
            Binary[".bin/mac<br>Native AOT Binary"]
            PluginsDir["plugins/<br>16 Zsh scripts"]
            Lib["lib/<br>bootstrap.zsh + shared.zsh"]
            DefaultConf["macstack.conf<br>Default configuration"]
        end

        subgraph "User Configuration (XDG)"
            UserConf["~/.config/macstack/macstack.conf<br>User overrides"]
        end

        subgraph "External Tools (optional)"
            Pandoc["Pandoc"]
            WeasyPrint["WeasyPrint"]
            Homebrew["Homebrew"]
            GitTool["Git"]
            GH["GitHub CLI"]
            FFmpeg["ffmpeg"]
            ImageMagick["ImageMagick"]
        end

        subgraph "Shell Environment"
            Zsh["Zsh 5.x+"]
            PATH["$PATH"]
        end
    end

    Binary --> PluginsDir
    Binary --> PATH
    PluginsDir --> Lib
    Lib --> DefaultConf
    Lib --> UserConf
    PluginsDir --> Zsh
    PluginsDir -.-> Pandoc
    PluginsDir -.-> WeasyPrint
    PluginsDir -.-> Homebrew
    PluginsDir -.-> GitTool
    PluginsDir -.-> GH
    PluginsDir -.-> FFmpeg
    PluginsDir -.-> ImageMagick
```

## 7.3 Build and Release Pipeline

```mermaid
graph LR
    subgraph "Development"
        Source["Source Code"]
        BuildScript["build/build.zsh"]
    end

    subgraph "Build (Cake)"
        Clean["Clean .bin/ and dist/"]
        Compile["dotnet publish<br>-r osx-arm64<br>-c Release<br>PublishAot=true"]
        Stage["Stage: mac + plugins + lib + conf"]
        Package["tar.gz + SHA256"]
    end

    subgraph "CI/CD (GitHub Actions)"
        TagPush["Tag push v*"]
        ARM64["macOS 14 ARM64 build"]
        X64["macOS 13 x64 build"]
        Release["GitHub Release"]
    end

    Source --> BuildScript
    BuildScript --> Clean --> Compile --> Stage --> Package

    TagPush --> ARM64
    TagPush --> X64
    ARM64 --> Release
    X64 --> Release
```

## 7.4 Installation Layout

After extraction from the distribution archive:

```
<install-root>/
  mac                    # Native AOT binary (add to $PATH)
  plugins/
    mac-export           # Document processing
    mac-doctor           # System diagnostics
    mac-system           # System management
    mac-git              # Git shortcuts
    mac-ssh              # SSH helpers
    mac-net              # Network utilities
    mac-tools            # General utilities
    mac-coffee           # Caffeinate toggle
    mac-help             # Help router
    mac-init             # Config initialization
    mac-completion       # Shell completions
    mac-config           # Config diagnostics
    mac-emo              # ASCII emotions
    mac-finder           # Finder integration
    mac-license          # License display
  lib/
    bootstrap.zsh        # Plugin bootstrap
    shared.zsh           # Shared utilities
  macstack.conf          # Default configuration
```

## 7.5 Runtime Requirements

| Requirement | Detail |
|-------------|--------|
| **OS** | macOS 12+ (Monterey or later) |
| **Architecture** | ARM64 (Apple Silicon) primary, x64 supported |
| **Shell** | Zsh 5.x+ (ships with macOS) |
| **.NET Runtime** | Not required (Native AOT) |
| **Disk Space** | ~10 MB (binary + plugins + libs) |
| **Permissions** | Standard user (no root required) |
