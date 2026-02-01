#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

if ! docker ps --format '{{.Names}}' | grep -qx 'escoles_api' && ! docker ps --format '{{.Names}}' | grep -qx 'escoles_web'; then
  echo "Api and web are not running."
  exit 0
fi

echo "Stopping api + web services..."
docker compose stop api web
echo "Api + web services stopped."
