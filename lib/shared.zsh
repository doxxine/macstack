# MACSTACK SHARED LIBRARY (ZSH)
# PURPOSE: COMMON HELPERS FOR ZSH-BASED PLUGINS.
# STYLE: UNIX / SNAKE_CASE IS CANONICAL.
# THIS FILE IS A LIBRARY. IT IS NOT MEANT TO BE EXECUTED DIRECTLY.

# DO NOT SET 'set -euo pipefail' HERE.

# ── IDEMPOTENT LOAD GUARD ───────────────────────────────────────────────────
if [[ -n "${MACSTACK_SHARED_LOADED:-}" ]]; then
  return 0
fi
MACSTACK_SHARED_LOADED=1

# ── XDG BASE DIRECTORIES ────────────────────────────────────────────────────
: "${XDG_CONFIG_HOME:=${HOME}/.config}"
: "${MACSTACK_CONFIG_DIR:=${XDG_CONFIG_HOME}/macstack}"
: "${MACSTACK_CONFIG_FILE:=${MACSTACK_CONFIG_DIR}/macstack.conf}"

# ── ANSI COLORS (UNIX STYLE) ────────────────────────────────────────────────
COLOR_BOLD=$'\e[1m'
COLOR_INFO=$'\e[1;36m'
COLOR_SUCCESS=$'\e[1;32m'
COLOR_WARN=$'\e[1;33m'
COLOR_ERR=$'\e[1;31m'
COLOR_RESET=$'\e[0m'

print_info()    { print -r -- "${COLOR_INFO}$*${COLOR_RESET}"; }
print_success() { print -r -- "${COLOR_SUCCESS}$*${COLOR_RESET}"; }
print_warn()    { print -u2 -r -- "${COLOR_WARN}$*${COLOR_RESET}"; }
print_error()   { print -u2 -r -- "${COLOR_ERR}$*${COLOR_RESET}"; }

# ── CONFIG RESOLUTION (DEV + XDG) ───────────────────────────────────────────
macstack_config_path() {
  local repo_cfg="${MACROOT:-}/macstack.conf"

  if [[ -n "${MACSTACK_CONFIG:-}" && -f "$MACSTACK_CONFIG" ]]; then
    print -r -- "$MACSTACK_CONFIG"; return 0
  fi
  if [[ -f "$MACSTACK_CONFIG_FILE" ]]; then
    print -r -- "$MACSTACK_CONFIG_FILE"; return 0
  fi
  if [[ -n "${MACROOT:-}" && -f "$repo_cfg" ]]; then
    print -r -- "$repo_cfg"; return 0
  fi
  return 1
}

macstack_load_config() {
  local cfg
  cfg="$(macstack_config_path)" || return 1
  source "$cfg"
}

macstack_ensure_xdg_config() {
  mkdir -p "$MACSTACK_CONFIG_DIR"
}

# ── STRING / FILENAME HELPERS ───────────────────────────────────────────────
slugify() {
  local value="${1:-}"
  print -r -- "$value"     | tr '[:upper:]' '[:lower:]'     | sed 's/[^a-z0-9]/_/g'     | sed 's/__*/_/g'     | sed 's/^_//; s/_$//'
}

semantic_filename() {
  local title="$1"
  local company="${2:-private}"
  local date_prefix="${3:-$(date +%Y%m%d)}"

  local slug
  slug="$(slugify "$title")"
  print -r -- "${date_prefix}_${company}_${slug}.pdf"
}

export_target_directory() {
  local base="${EXPORT_DEST_BASE:-${HOME}/workspace/documents/business}"
  mkdir -p "$base"
  print -r -- "$base"
}

tag_file() {
  local file="$1"
  local tags_csv="${2:-}"
  [[ -z "$tags_csv" ]] && return 0

  command -v tag >/dev/null 2>&1 || return 0

  # NORMALIZE: REMOVE SPACES AROUND COMMAS
  local tags
  tags="$(print -r -- "$tags_csv" | tr -d '[:space:]')"

  # ONE MODE FLAG ONLY
  tag -a "$tags" -- "$file"
}

# ── DIAGNOSTICS ─────────────────────────────────────────────────────────────
macstack_print_config_source() {
  local cfg
  if cfg="$(macstack_config_path)"; then
    print_info "CONFIG: $cfg"
    return 0
  fi
  print_warn "CONFIG: NOT FOUND"
  return 1
}
