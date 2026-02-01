#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

DB_COMPOSE="-f docker/docker-compose.yml"

echo "Starting postgres (escoles-db)..."
docker compose $DB_COMPOSE up -d db

echo "Waiting for postgres to be ready (up to 60s)..."
ready=false
for i in $(seq 1 60); do
  if docker exec escoles-db pg_isready -U app -d escoles >/dev/null 2>&1; then
    ready=true
    break
  fi
  sleep 1
done

if [ "$ready" = true ]; then
  echo "Postgres is ready on localhost:5432"
  exit 0
fi

echo "Postgres did not become ready — showing last 200 log lines"
docker logs --tail 200 escoles-db || true
exit 1
