#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

COMPOSE_FILES="-f docker-compose.yml -f docker-compose.override.yml"
WEB_URL="http://localhost:5042"

open_url() {
  local url="$1"
  if [ -n "${BROWSER:-}" ] && command -v "$BROWSER" >/dev/null 2>&1; then
    "$BROWSER" "$url" && return 0
  fi
  if command -v brave-browser-stable >/dev/null 2>&1; then
    nohup brave-browser-stable --new-window --user-data-dir=/tmp/brave-codex --no-first-run "$url" >/dev/null 2>&1 &
    return 0
  fi
  if command -v brave >/dev/null 2>&1; then
    nohup brave --new-window --user-data-dir=/tmp/brave-codex --no-first-run "$url" >/dev/null 2>&1 &
    return 0
  fi
  if command -v brave-browser >/dev/null 2>&1; then
    nohup brave-browser --new-window --user-data-dir=/tmp/brave-codex --no-first-run "$url" >/dev/null 2>&1 &
    return 0
  fi
  if [ -n "${VSCODE_PID:-}" ] && command -v code >/dev/null 2>&1; then
    code --open-url "$url" && return 0
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


echo "Checking existing web container..."
container=$(docker compose $COMPOSE_FILES ps -q web 2>/dev/null || true)
if [ -n "$container" ]; then
  echo "Stopping existing web container..."
  docker compose $COMPOSE_FILES stop web || true
  echo "Removing existing web container..."
  docker compose $COMPOSE_FILES rm -f web || true
fi

echo "Attempting to start web using override (no-build)..."
docker compose $COMPOSE_FILES up -d --no-deps --no-build web >/dev/null 2>&1 || true

echo "Waiting for HTTP endpoint $WEB_URL (up to 120s)"
started=false
for i in $(seq 1 120); do
  status=$(curl -s -o /dev/null -w "%{http_code}" --max-time 2 "$WEB_URL/" || true)
  if [ "$status" != "000" ]; then
    started=true
    break
  fi
  sleep 1
done

if [ "$started" = true ]; then
  echo "Web responded with HTTP $status"
  open_url "$WEB_URL" || true
  echo "F5 Start Web: done."
  exit 0
fi
echo "ERROR: web no ha respost. Revisa logs i assegura't que el build és correcte."
docker compose $COMPOSE_FILES logs --timestamps --tail 200 web || true
exit 1
