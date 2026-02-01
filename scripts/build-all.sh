#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

STATUS_FILE="$ROOT/.build-status"
COMPOSE_FILES="-f docker-compose.yml -f docker-compose.override.yml"

write_status() {
  local status="$1"
  echo "$status $(date -Iseconds)" > "$STATUS_FILE"
}

echo "Building API..."
if docker compose $COMPOSE_FILES ps --status running -q api 2>/dev/null | grep -q .; then
  echo "API està en marxa — aturant abans del build..."
  docker compose $COMPOSE_FILES stop api >/dev/null 2>&1 || true
fi
docker pull mcr.microsoft.com/dotnet/sdk:8.0 >/dev/null || true
if ! docker run --rm -v "$ROOT":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 \
  dotnet build src/Api/Api.csproj -c Debug /property:GenerateFullPaths=true; then
  echo "ERROR: build API ha fallat."
  write_status "fail"
  exit 1
fi

echo "Building Web..."
if docker compose $COMPOSE_FILES ps --status running -q web 2>/dev/null | grep -q .; then
  echo "Web està en marxa — aturant abans del build..."
  docker compose $COMPOSE_FILES stop web >/dev/null 2>&1 || true
fi
if ! docker run --rm -v "$ROOT":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 \
  dotnet build src/Web/Web.csproj -c Debug /property:GenerateFullPaths=true; then
  echo "ERROR: build Web ha fallat."
  write_status "fail"
  exit 1
fi

write_status "ok"
echo "Build completat correctament."
