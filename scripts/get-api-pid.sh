#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

COMPOSE_FILES="-f docker-compose.yml -f docker-compose.override.yml"

pid="$(
  docker compose $COMPOSE_FILES exec -T api sh -lc "ps -axww -o pid=,comm=,args=" \
    | awk '$2=="Api"{print $1; exit} $0 ~ /\/app\/src\/Api\/bin\/Debug\/net8\.0\/Api/ {print $1; exit}'
)"

if [ -z "$pid" ]; then
  echo "No API process found" >&2
  exit 1
fi

echo "$pid"
