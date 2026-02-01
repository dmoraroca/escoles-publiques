#!/usr/bin/env bash
set -euo pipefail

ROOT=$(cd "$(dirname "$0")/.." && pwd)
cd "$ROOT"

echo "Starting api + web services..."
docker compose up -d api web
echo "Api + web services are up."
