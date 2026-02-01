#!/usr/bin/env bash
set -euo pipefail

URL=${1:-http://localhost:5042}
TIMEOUT=${2:-60}

echo "Waiting for $URL (timeout ${TIMEOUT}s)..."
SECS=0
while [ $SECS -lt $TIMEOUT ]; do
  if curl -sSf "$URL" >/dev/null 2>&1; then
    echo "OK: $URL is available"
    exit 0
  fi
  sleep 1
  SECS=$((SECS+1))
done

echo "Timeout waiting for $URL"
exit 1
