#!/usr/bin/env bash
set -euo pipefail

url="${1:-}"
if [ -z "$url" ]; then
  echo "Usage: $0 <url>"
  exit 2
fi

profile_dir="/tmp/brave-codex"

find_brave() {
  for bin in brave-browser-stable brave-browser brave; do
    if command -v "$bin" >/dev/null 2>&1; then
      echo "$bin"
      return 0
    fi
  done
  return 1
}

open_with_brave() {
  local bin="$1"
  local pid=""

  if pgrep -f "$bin" >/dev/null 2>&1; then
    "$bin" --new-tab "$url" >/dev/null 2>&1 & pid=$!
  else
    rm -f "$profile_dir"/Singleton* 2>/dev/null || true
    "$bin" --new-window --user-data-dir="$profile_dir" --no-first-run "$url" >/dev/null 2>&1 & pid=$!
  fi

  sleep 1
  if [ -n "$pid" ] && kill -0 "$pid" >/dev/null 2>&1; then
    return 0
  fi
  return 1
}

if bin="$(find_brave)"; then
  if open_with_brave "$bin"; then
    exit 0
  fi
fi

if command -v xdg-open >/dev/null 2>&1; then
  xdg-open "$url" && exit 0
fi

if command -v gio >/dev/null 2>&1; then
  gio open "$url" && exit 0
fi

if command -v sensible-browser >/dev/null 2>&1; then
  sensible-browser "$url" && exit 0
fi

echo "No he pogut obrir el navegador automàticament. Obre manualment: $url"
exit 1
