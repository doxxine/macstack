# MACSTACK PLUGIN BOOTSTRAP
# Source this from any ZSH plugin after setting MACROOT.
# Handles shared lib loading with a clear error on failure.

if [[ -z "${MACROOT:-}" ]]; then
  echo "mac: MACROOT is not set. Cannot bootstrap plugin." >&2
  exit 1
fi

if [[ ! -f "$MACROOT/lib/shared.zsh" ]]; then
  echo "mac: missing shared lib: $MACROOT/lib/shared.zsh" >&2
  exit 1
fi

source "$MACROOT/lib/shared.zsh"
