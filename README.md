<p align="center">
  <code>▰▰▰ MACSTACK ▰▰▰</code><br/>
  <strong>v0.2.0 · macOS · ARM64 · Native AOT</strong>
</p>

<p align="center">
  <em>A modular CLI toolkit for semantic workflows, clean document processing, and predictable automation on macOS.</em>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/platform-macOS-black?style=flat-square&logo=apple" />
  <img src="https://img.shields.io/badge/arch-ARM64-blueviolet?style=flat-square" />
  <img src="https://img.shields.io/badge/runtime-.NET_10_AOT-512bd4?style=flat-square&logo=dotnet" />
  <img src="https://img.shields.io/badge/plugins-zsh-4EAA25?style=flat-square&logo=gnubash" />
  <img src="https://img.shields.io/badge/license-MIT-green?style=flat-square" />
</p>

---

## ⚡ TL;DR

```
mac <command> [args]        # that's it. unix-style. no magic.
```

MacStack is a **native AOT binary** that routes to **discoverable Zsh plugins** — following the same model as `git` and `kubectl`. No framework. No runtime. No daemon. Just a fast, composable toolchain.

---

## 🧬 Architecture

```
┌─────────────────────────────────────────────────┐
│                   mac (binary)                   │
│          Native AOT · C# · .NET 10              │
│         ┌──────────────────────────┐             │
│         │  parse → discover → exec │             │
│         └──────────┬───────────────┘             │
│                    │                             │
│    ┌───────────────┼───────────────┐             │
│    ▼               ▼               ▼             │
│  built-ins    plugins/*      $PATH/mac-*         │
│  (help,ver,   (mac-export,   (3rd party)         │
│   list)        mac-doctor…)                      │
└─────────────────────────────────────────────────┘
         │               │
         ▼               ▼
   bootstrap.zsh    shared.zsh
         │
         ▼
   macstack.conf  (XDG-compliant)
```

### 🔩 Resolution Order

```
mac <command> [args]
  │
  ├─➊─ built-in?          → execute directly
  ├─➋─ plugins/mac-<cmd>? → exec plugin
  └─➌─ $PATH/mac-<cmd>?   → exec from PATH
       └─ ✗ → exit 127
```

---

## 🧰 Commands

### ⌂ Built-ins

| Command | Description |
|:--------|:------------|
| `mac help [cmd]` | Styled help for any command |
| `mac version` | Print version string |
| `mac list` | Enumerate all available commands |

### 🔌 Plugins

| Plugin | Description | Deps |
|:-------|:------------|:-----|
| `mac export` | 📄 Document export — MD/HTML → PDF | `pandoc` `weasyprint` |
| `mac doctor` | 🩺 System health checks & prerequisites | — |
| `mac system` | 🔄 System updates & utilities | `brew` |
| `mac git` | 🔀 Git shortcuts — wip, undo, reset | `git` |
| `mac ssh` | 🔑 SSH helpers — pubkey, upload, sync | `ssh` `rsync` |
| `mac net` | 🌐 Network helpers — ip, dns, routes | `curl` |
| `mac tools` | 🛠 Utilities — uuid, serve, trim, convert | `python3` |
| `mac coffee` | ☕ Caffeinate helpers — stay awake | — |
| `mac completion` | 🧩 Generate shell completions | — |
| `mac config` | ⚙️ Config diagnostics & resolution | — |
| `mac init` | 📦 Initialize XDG config | — |
| `mac emo` | 🎭 ASCII emotions → clipboard | — |
| `mac finder` | 🗂 Open Finder at cwd | — |
| `mac help` | 📖 Styled help router | — |
| `mac license` | ⚖️ Display MIT license | — |

---

## 📄 Export Pipeline

```
                  ┌──────────┐
  report.md ────▶ │  pandoc  │ ────▶  20260216_company_report.pdf
                  └──────────┘
                  ┌───────────┐
  page.html ───▶ │ weasyprint │ ───▶  20260216_company_page.pdf
                  └───────────┘

  ✦ semantic filenames    ✦ configurable geometry
  ✦ finder tagging        ✦ custom fonts & layouts
```

```bash
mac export markdown report.md --title "Q4 Report" --company acme
mac export html    page.html  --title "Dashboard"
```

---

## ⚙️ Configuration

```
$MACSTACK_CONFIG                          ➊ env override
$XDG_CONFIG_HOME/macstack/macstack.conf   ➋ user config (~/.config/…)
<repo>/macstack.conf                      ➌ default
```

First match wins. Config is a plain Zsh-sourceable file — no YAML, no JSON, no surprises.

```bash
mac init          # bootstrap ~/.config/macstack/
mac config path   # show resolved config path
mac config print  # dump active config
```

---

## 🏗 Build

```bash
./build/build.zsh                  # → .bin/mac (native AOT)
```

```
build.zsh → dotnet tool restore → Cake → dotnet publish -r osx-arm64
                                           │
                                           ▼
                                     .bin/mac  ← single binary, no runtime
```

| Target | What it does |
|:-------|:-------------|
| `clean` | Wipe `.bin/` and `dist/` |
| `build` | Compile native AOT binary |
| `release` | Stage + tar.gz + SHA256 |

---

## 🚀 Install

```bash
# from release archive
tar -xzf macstack-0.2.0-darwin-arm64.tar.gz -C ~/.local/bin/
mac init
mac doctor    # verify everything works
```

### Requirements

| | |
|:--|:--|
| **OS** | macOS 12+ (Monterey) |
| **Arch** | Apple Silicon (ARM64) or x64 |
| **Shell** | Zsh 5.x+ (ships with macOS) |
| **Runtime** | None — Native AOT = zero dependencies |

---

## 🧠 Design Philosophy

```
  ┌─────────────────────────────────────────────────┐
  │  composition  >  inheritance                     │
  │  executables  >  registries                      │
  │  conventions  >  configuration                   │
  │  unix pipes   >  framework abstractions          │
  │  fail fast    >  fail silently                   │
  └─────────────────────────────────────────────────┘
```

MacStack is **not**:

- ✗ a framework
- ✗ a package manager
- ✗ a bootstrapper
- ✗ a dotfile manager
- ✗ a cross-platform tool

MacStack **is**:

- ✓ a precision CLI instrument
- ✓ a semantic workflow engine
- ✓ a composable Unix toolchain
- ✓ fast, predictable, transparent

---

## 🧩 Integrations

Works with (but never requires):

| Tool | How MacStack uses it |
|:-----|:---------------------|
| 🍺 **Homebrew** | `mac system update` orchestrates `brew upgrade` |
| 📝 **Pandoc** | Markdown → PDF export engine |
| 🎨 **WeasyPrint** | HTML → PDF rendering |
| 🏠 **Chezmoi** | Optional dotfile pull during system update |
| 🏷 **Finder Tags** | Auto-tagging exported files via `tag` |
| 🔍 **ripgrep / fd / fzf** | Checked by `mac doctor`, used in workflows |

---

## 📊 Exit Codes

```
0   ✓  success
1   ✗  generic failure
2   ?  unknown subcommand
126 ⊘  found but not executable
127 ∅  command not found
```

---

## 🔌 Plugin Contract

Want to extend MacStack? Create an executable named `mac-<command>`:

```bash
#!/usr/bin/env zsh
# @describe: What your plugin does
source "$MACROOT/lib/bootstrap.zsh"
macstack_load_config || { print_error "Config load failed"; exit 1; }

case "${1:-}" in
  --help)        print "Usage: mac yourcmd [args]" ;;
  --description) print "What your plugin does" ;;
  *)             # your logic here ;;
esac
```

Drop it in `plugins/` — done. No registration, no manifest, no ceremony.

---

## 📜 License

MIT — Copyright 2025 [MathAndEmotion](https://github.com/doxxine)

---

<p align="center">
  <sub>built with obsessive minimalism · no telemetry · no runtime · no nonsense</sub>
</p>
