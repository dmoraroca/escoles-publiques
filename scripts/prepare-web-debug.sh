#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

open_url() {
  local url="$1"
  if [ -n "${BROWSER:-}" ] && command -v "$BROWSER" >/dev/null 2>&1; then
    "$BROWSER" "$url" && return 0
  fi
  if command -v brave-browser-stable >/dev/null 2>&1; then
    nohup brave-browser-stable --new-window --user-data-dir=/tmp/brave-codex --no-first-run "$url" >/dev/null 2>&1 &
    return 0
  fi
  if command -v brave-browser >/dev/null 2>&1; then
    nohup brave-browser --new-window --user-data-dir=/tmp/brave-codex --no-first-run "$url" >/dev/null 2>&1 &
    return 0
  fi
  if command -v brave >/dev/null 2>&1; then
    nohup brave --new-window --user-data-dir=/tmp/brave-codex --no-first-run "$url" >/dev/null 2>&1 &
    return 0
  fi
  if command -v xdg-open >/dev/null 2>&1; then
    xdg-open "$url" && return 0
  fi
  if command -v gio >/dev/null 2>&1; then
    gio open "$url" && return 0
  fi
  if command -v sensible-browser >/dev/null 2>&1; then
    sensible-browser "$url" && return 0
  fi
  echo "No he pogut obrir el navegador automàticament. Obre manualment: $url"
  return 1
}

echo "Building solution inside Docker SDK image..."
# Build using the official .NET SDK container so host dotnet isn't required
docker run --rm -v "$ROOT":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 \
  dotnet build EscolesPubliques.sln -c Debug /property:GenerateFullPaths=true

echo "Starting web container (docker compose)..."
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d --no-build web

echo "Waiting for web container to be ready..."
# wait for container to exist
cnt=0
until docker compose ps web | grep Up >/dev/null 2>&1 || [ $cnt -gt 60 ]; do
  sleep 1; cnt=$((cnt+1))
done

echo "Installing vsdbg in web container (if missing)..."
docker compose exec -T web bash -lc 'if [ ! -x /vsdbg/vsdbg ]; then curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg; fi'

echo "Waiting for HTTP endpoint http://localhost:5042 ..."
./scripts/wait-for-url.sh http://localhost:5042 120

echo "Opening browser http://localhost:5042"
open_url http://localhost:5042 || true

echo "Prepare Web Debug: done. You can now attach the debugger (F5)."
