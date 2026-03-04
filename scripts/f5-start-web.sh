#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

COMPOSE_FILES="-f docker-compose.yml -f docker-compose.override.yml"
WEB_HEALTH_URL="http://localhost:5042/Auth/Login"


echo "Checking existing web container..."
if docker compose $COMPOSE_FILES ps --status running -q web 2>/dev/null | grep -q .; then
  status=$(curl -s -o /dev/null -w "%{http_code}" --max-time 2 "$WEB_HEALTH_URL" || true)
  if [ "$status" != "000" ]; then
    echo "Web ja està aixecada i respon (HTTP $status)."
    echo "Endpoint web: $WEB_HEALTH_URL"
    echo "F5 Start Web: done."
    exit 0
  fi
  echo "Web container running but not healthy yet, keeping it and waiting..."
else
  echo "Starting web container..."
  # Intentionally allow build if needed, to avoid stale/no-image failures.
  docker compose $COMPOSE_FILES up -d --no-deps web >/dev/null 2>&1 || true
fi

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
