# MacStack Build

This repository uses **Cake** as an in-repo build system for the macOS arm64
`mac` binary.

There are **no global tools** and **no cross-platform targets** by design.

## Requirements

- macOS (Apple Silicon)
- .NET SDK (matching `src/mac/mac.csproj`)

## Tooling

Build tooling is defined in:

```
.config/dotnet-tools.json
```

Cake is restored locally via:

```bash
dotnet tool restore
```

## Build

From the repository root:

```bash
./build/build.zsh
```

This performs:

- clean
- publish (`dotnet publish`)
- output to `./.bin/mac`

## Output

```
.bin/mac
```

This is the native AOT binary used during development.

## Design Notes

- Build logic lives in `build/`
- Tool versions are pinned and committed
- No archives, no installers, no matrices
- Packaging and distribution are handled separately

This keeps the build **simple, reproducible, and Unix-like**.
