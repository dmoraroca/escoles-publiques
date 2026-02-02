#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

COMPOSE_FILES="-f docker-compose.yml -f docker-compose.override.yml"
API_INDEX_URL="http://localhost:7000/api/index.html"


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
  echo "Endpoint API: $API_INDEX_URL"
  echo "F6 Start API: done."
  exit 0
fi

echo "Waiting for API index $API_INDEX_URL (up to 120s)"
for i in $(seq 1 120); do
  status=$(curl -s -o /dev/null -w "%{http_code}" "$API_INDEX_URL" || echo "000")
  if [ "$status" = "200" ] || [ "$status" = "301" ] || [ "$status" = "302" ]; then
    echo "API index responded with HTTP $status"
    echo "Endpoint API: $API_INDEX_URL"
    echo "F6 Start API: done."
    exit 0
  fi
  sleep 1
done

echo "ERROR: api no ha respost — mostrant els últims 200 logs"
docker compose $COMPOSE_FILES logs --timestamps --tail 200 api || true
exit 1
