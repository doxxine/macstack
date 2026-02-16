# 1. Introduction

## 1.1 System Purpose

MacStack is a modular CLI toolkit for macOS that provides semantic document processing, workflow automation, and structured command execution. It follows Unix conventions to bring clarity to digital workflows without making assumptions about system configuration or workspace structure.

**Version**: 0.2.0
**License**: MIT (Copyright 2025 MathAndEmotion)
**Target Platform**: macOS (Apple Silicon / ARM64 primary, x64 supported)

## 1.2 Scope

MacStack addresses three core concerns:

| Domain | Capability |
|--------|------------|
| **Document Processing** | Semantic file naming, Markdown/HTML to PDF export via Pandoc and WeasyPrint |
| **System Management** | Orchestrated updates (Homebrew, Chezmoi, Oh-My-Zsh), cleanup, diagnostics |
| **Developer Utilities** | Git shortcuts, SSH helpers, UUID generation, HTTP server, network tools |

MacStack explicitly does **not**:
- Install or manage system-level packages (defers to Homebrew)
- Manage dotfiles (defers to Chezmoi)
- Provide cross-platform support (macOS only by design)

## 1.3 Stakeholders

| Stakeholder | Role | Concern |
|-------------|------|---------|
| End User (Developer) | Primary user of CLI commands | Fast, predictable, composable commands |
| Plugin Author | Extends MacStack with new commands | Clear plugin conventions, stable bootstrap API |
| Maintainer | Maintains core router and shared libraries | Build reproducibility, minimal coupling |
| CI/CD System | Automates builds and releases | Deterministic builds, AOT compilation |

## 1.4 Context Summary

MacStack is a single-user, local-only CLI tool. It has no server component, no network API, and no persistent data store. It integrates with external tools (Pandoc, WeasyPrint, Homebrew, Git, etc.) via subprocess invocation and composes workflows through Unix conventions: environment variables, stdin/stdout, and exit codes.

The system is structured as a thin, native AOT-compiled C# binary that routes commands to discoverable Zsh plugin scripts, following the same pattern used by Git and kubectl.
