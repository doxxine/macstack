# ⚙️ MacStack  
*A modular CLI toolkit for semantic workflows, clean document processing, and predictable automation on macOS.*

## Overview

MacStack is a lightweight, extensible command-line toolkit designed to bring clarity and structure to digital workflows.  
It focuses on **semantic document handling**, **export pipelines**, **template-driven creation**, and **automation patterns** that help users work more efficiently and consistently — without enforcing a specific workspace or folder philosophy.

MacStack does not manage your system, install tools, or define your workspace.  
Instead, it acts as a **precision instrument** within whatever environment you choose — a clean, predictable layer for file processing and workflow automation.

---

## ✨ Core Principles

### **1. Predictable, Semantic Outputs**  
Files should be *meaningful* the moment they are created.  
MacStack generates consistent semantic filenames, applies metadata, and helps maintain clarity across complex workflows.

### **2. Template-Driven Workflows**  
Documents, reports, notes, and structured artefacts often follow the same patterns.  
MacStack enables reproducible, frontmatter-aware template generation — flexible enough for personal notes, professional documentation, and everything in between.

### **3. Clean Separation of Concerns**  
MacStack avoids assumptions about your system and your workspace.  
It operates as a **standalone CLI**, not as a bootstrap mechanism or a configuration manager.

Those responsibilities belong to other layers such as Workspace-Bootstrap or your personal dotfiles system.

### **4. Extensibility Through Plugins**  
MacStack is built around a modular, plugin-based architecture.  
Every command (`export`, `new`, `doctor`, …) is isolated, composable, and easy to extend.

---

## 🛠️ What MacStack Provides

MacStack focuses on structured, automation-friendly workflows:

### **✔ Semantic File Processing**  
- Semantic filename generation  
- Automatic tagging  
- Hash-based deduplication patterns  
- Frontmatter awareness  

### **✔ Export Pipelines**  
- Markdown → PDF (Pandoc)  
- HTML → PDF (WeasyPrint)  
- Layout, orientation, metadata, and styling controls  
- Temporary or final export destinations  

### **✔ Template Engine**  
- Create new documents based on user-defined templates  
- YAML frontmatter injection  
- Arc42, meeting notes, reports — or any structure you define  
- Fully user overrideable (`~/.config/macstack/templates/`)  

### **✔ Diagnostic & Utility Commands**  
- `mac doctor` checks system state and prerequisites  
- `mac backup` (optional) for config backups  
- Modular plugin architecture for future workflows  

---

## 🧩 What MacStack Is *Not*

MacStack deliberately avoids:

- modifying your macOS system  
- installing dependencies  
- defining your workspace structure  
- managing dotfiles  
- acting as a bootstrapper  

These responsibilities belong to other tools or layers such as Workspace-Bootstrap, Homebrew, or chezmoi.

MacStack focuses on being an **automation and workflow engine**, nothing more, nothing less.

---

## 🤝 Integrations

MacStack works elegantly with:

- **Obsidian** (frontmatter-driven workflows, template generation)  
- **Workspace-Bootstrap** (structured environments)  
- **Pandoc / WeasyPrint** (export pipelines)  
- **macOS Finder tags**  
- **XDG-based config environments (`~/.config/macstack`)**  

But none of these integrations are required.  
MacStack remains fully self-contained.

---

## 🚦 Status

MacStack is under active development.  
The initial goals include:

- finalizing the export engine  
- stabilizing template workflows  
- building a robust plugin architecture  
- providing a clean developer onboarding experience  
- documenting extension points  

Community feedback, ideas, and contributions are welcome.

---

## 📄 License

MacStack is open-source under the MIT License.

---

## 🙌 Contributing

Clear, well-structured, reproducible workflows matter.  
If you believe the same — contributions, concepts, and discussions are always welcome.
