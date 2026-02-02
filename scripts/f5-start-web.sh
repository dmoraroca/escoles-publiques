#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

COMPOSE_FILES="-f docker-compose.yml -f docker-compose.override.yml"
WEB_HEALTH_URL="http://localhost:5042/Auth/Login"


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

echo "Tailing web logs while waiting (Ctrl+C to stop)..."
docker compose $COMPOSE_FILES logs -f --timestamps --tail 50 web >/dev/null 2>&1 &
logs_pid=$!

echo "Waiting for HTTP endpoint $WEB_HEALTH_URL (up to 120s)"
started=false
for i in $(seq 1 120); do
  status=$(curl -s -o /dev/null -w "%{http_code}" --max-time 2 "$WEB_HEALTH_URL" || true)
  if [ "$status" != "000" ]; then
    started=true
    break
  fi
  sleep 1
done

if kill -0 "$logs_pid" >/dev/null 2>&1; then
  kill "$logs_pid" >/dev/null 2>&1 || true
fi

if [ "$started" = true ]; then
  echo "Web responded with HTTP $status"
  echo "Endpoint web: $WEB_HEALTH_URL"
  echo "F5 Start Web: done."
  exit 0
fi
echo "ERROR: web no ha respost. Revisa logs i assegura't que el build és correcte."
docker compose $COMPOSE_FILES logs --timestamps --tail 200 web || true
exit 1
