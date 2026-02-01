#!/usr/bin/env bash
set -euo pipefail

echo "== Docker version =="
docker --version || true

echo "\n== All containers =="
docker ps -a --format "{{.ID}}\t{{.Names}}\t{{.Image}}\t{{.Status}}" || true

ids_to_remove=""
while read -r id name rest; do
  if [ "$name" = "escoles-db" ]; then
    continue
  fi
  case "$name" in
    *web*|*api*|*escoles-publiques*) ids_to_remove="$ids_to_remove $id" ;;
  esac
done < <(docker ps -a --format "{{.ID}} {{.Names}} {{.Image}} {{.Status}}" 2>/dev/null || true)

if [ -z "$ids_to_remove" ]; then
  echo "\nNo api/web/escoles-publiques containers found to stop/remove."
else
  echo "\nStopping containers:$ids_to_remove"
  docker stop $ids_to_remove || true
  echo "Removing containers:$ids_to_remove"
  docker rm $ids_to_remove || true
fi

echo "\n== Remaining containers =="
docker ps -a --format "{{.ID}}\t{{.Names}}\t{{.Image}}\t{{.Status}}" || true
