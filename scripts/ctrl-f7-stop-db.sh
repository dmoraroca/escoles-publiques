#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

DB_COMPOSE="-f docker/docker-compose.yml"

if ! docker ps --format '{{.Names}}' | grep -qx 'escoles-db'; then
  echo "Postgres is not running."
  exit 0
fi

echo "Stopping postgres (escoles-db)..."
docker compose $DB_COMPOSE stop db
echo "Postgres stopped."
