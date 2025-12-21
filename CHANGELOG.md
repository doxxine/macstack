# Changelog

All notable changes to this project will be documented in this file.

The format is based on *Keep a Changelog* and follows semantic intent rather than strict versioning.

## Unreleased

### Added

- Native AOT `mac` binary as command router
- Git‑style plugin execution (`mac-<command>`)
- `mac list` for command discovery
- Initial `mac doctor` plugin

### Changed

- Execution model redesigned from shell dispatcher to native binary

### Removed

- Implicit shell sourcing for command execution
