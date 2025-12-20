# MACSTACK SHARED LIBRARY (ZSH)
# PURPOSE: SHARED HELPERS FOR ZSH-BASED PLUGINS. NOT EXECUTABLE BY ITSELF.
# NOTE: DO NOT SET "set -euo pipefail" HERE; PLUGINS DECIDE THEIR OWN STRICTNESS.

if [[ -n "${MACSTACK_SHARED_LOADED:-}" ]]; then
  return 0
fi
MACSTACK_SHARED_LOADED=1

# ── XDG FALLBACK (BOOTSTRAP MIGRATION) ─────────────────────────────────────
: "${XdgConfigHome:=${XDG_CONFIG_HOME:-$HOME/.config}}"
: "${MacStackHome:=${XdgConfigHome}/macstack}"

# ── COLOR PALETTE (ANSI) ──────────────────────────────────────────────────
COLOR_BOLD=$'\e[1m'
COLOR_INFO=$'\e[1;36m'
COLOR_SUCCESS=$'\e[1;32m'
COLOR_WARN=$'\e[1;33m'
COLOR_ERROR=$'\e[1;31m'
COLOR_RESET=$'\e[0m'

PrintInfo()    { print -r -- "${COLOR_INFO}$*${ColorReset}"; }
PrintSuccess() { print -r -- "${COLOR_SUCCESS}$*${ColorReset}"; }
PrintWarn()    { print -u2 -r -- "${COLOR_WARN}$*${ColorReset}"; }
PrintError()   { print -u2 -r -- "${COLOR_ERROR}$*${ColorReset}"; }

# ── STRING HELPERS ────────────────────────────────────────────────────────
# SLUGIFY: LOWERCASE + [A-Z0-9] ONLY, OTHERS TO UNDERSCORE
Slugify() {
  local Value="$1"
  print -r -- "$Value"       | tr '[:upper:]' '[:lower:]'       | sed 's/[^a-z0-9]/_/g'       | sed 's/__*/_/g'       | sed 's/^_//; s/_$//'
}

# ── EXPORT HELPERS (LEGACY) ───────────────────────────────────────────────
# KEEP THESE FOR EXISTING EXPORT PLUGINS UNTIL THEIR LOGIC MOVES INTO .NET.
BuildExportFilename() {
  local Title="$1"
  local Company="${2:-private}"
  local DatePrefix="${3:-$(date +%Y%m%d)}"

  local SlugifiedTitle
  SlugifiedTitle="$(Slugify "$Title")"

  print -r -- "${DatePrefix}_${Company}_${SlugifiedTitle}.pdf"
}

TagFile() {
  local FilePath="$1"
  local ContextTagsCsv="${2:-}"

  if [[ -z "$ContextTagsCsv" ]]; then
    return 0
  fi

  if command -v tag >/dev/null 2>&1; then
    # FINDER-COMPATIBLE TAGS VIA "tag" CLI; CSV -> ARRAY
    tag -a "${(s:,:)ContextTagsCsv}" "$FilePath"
  fi
}

ExportTargetDirectory() {
  local Base="${EXPORT_DEST_BASE:-$HOME/workspace/documents/business}"
  mkdir -p "$Base"
  print -r -- "$Base"
}
