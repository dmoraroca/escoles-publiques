#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

COMPOSE_FILES="-f docker-compose.yml -f docker-compose.override.yml"
SWAGGER_URL="http://localhost:7000/swagger"

open_url() {
  local url="$1"
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
    if xdg-open "$url" >/dev/null 2>&1; then
      return 0
    fi
  fi
  if command -v gio >/dev/null 2>&1; then
    if gio open "$url" >/dev/null 2>&1; then
      return 0
    fi
  fi
  if command -v sensible-browser >/dev/null 2>&1; then
    if sensible-browser "$url" >/dev/null 2>&1; then
      return 0
    fi
  fi
  echo "No he pogut obrir el navegador automàticament. Obre manualment: $url"
  return 1
}


if docker compose $COMPOSE_FILES ps --status running -q api 2>/dev/null | grep -q .; then
  api_running=true
else
  api_running=false
fi

if [ "$api_running" = false ]; then
  echo "API no està aixecada — iniciant..."
  docker compose $COMPOSE_FILES up -d --no-deps --no-build api >/dev/null 2>&1 || true
else
  echo "API ja està aixecada."
  echo "Obrint Swagger: $SWAGGER_URL"
  open_url "$SWAGGER_URL" || true
  echo "F6 Start API: done."
  exit 0
fi

echo "Waiting for Swagger UI $SWAGGER_URL (up to 120s)"
for i in $(seq 1 120); do
  status=$(curl -s -o /dev/null -w "%{http_code}" "$SWAGGER_URL" || echo "000")
  if [ "$status" = "200" ] || [ "$status" = "301" ] || [ "$status" = "302" ]; then
    echo "API Swagger UI responded with HTTP $status"
    echo "Obrint Swagger: $SWAGGER_URL"
    open_url "$SWAGGER_URL" || true
    echo "F6 Start API: done."
    exit 0
  fi
  sleep 1
done

echo "ERROR: api no ha respost — mostrant els últims 200 logs"
docker compose $COMPOSE_FILES logs --timestamps --tail 200 api || true
exit 1
