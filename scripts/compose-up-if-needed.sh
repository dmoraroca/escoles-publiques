#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/.."

# Check running services for this compose project
running_services=$(docker compose ps --status running --services || true)

need_up=true
if [[ -n "$running_services" ]]; then
  has_api=$(printf '%s\n' "$running_services" | grep -x "api" || true)
  has_web=$(printf '%s\n' "$running_services" | grep -x "web" || true)
  if [[ -n "$has_api" && -n "$has_web" ]]; then
    need_up=false
  fi
fi

if [[ "$need_up" == "true" ]]; then
  echo "Starting docker compose..."
  docker compose up -d
else
  echo "Docker compose already running (api, web)."
fi
