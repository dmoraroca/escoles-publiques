#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

RESULTS_DIR="${RESULTS_DIR:-artifacts/test-results-coverage}"
RUNSETTINGS_FILE="${RUNSETTINGS_FILE:-coverlet.runsettings}"
TEST_PROJECT="${TEST_PROJECT:-src/UnitTest/UnitTest.csproj}"
NO_RESTORE="${NO_RESTORE:-1}"
SKIP_TEST_RUN="${SKIP_TEST_RUN:-0}"
COVERAGE_FILE="${COVERAGE_FILE:-}"

DOMAIN_MIN="${DOMAIN_MIN:-80}"
APPLICATION_MIN="${APPLICATION_MIN:-80}"
INFRASTRUCTURE_MIN="${INFRASTRUCTURE_MIN:-15}"
WEB_MIN="${WEB_MIN:-50}"

if [[ "$SKIP_TEST_RUN" != "1" && -z "$COVERAGE_FILE" ]]; then
  rm -rf "$RESULTS_DIR"
  mkdir -p "$RESULTS_DIR"

  echo "Running tests with coverage..."
  test_args=(
    "$TEST_PROJECT"
    --settings "$RUNSETTINGS_FILE"
    --collect:"XPlat Code Coverage"
    --results-directory "$RESULTS_DIR"
    -v minimal
  )

  if [[ "$NO_RESTORE" == "1" ]]; then
    test_args+=(--no-restore)
  fi

  dotnet test "${test_args[@]}"

  COVERAGE_FILE="$(find "$RESULTS_DIR" -type f -name 'coverage.cobertura.xml' | head -n 1 || true)"
fi

if [[ -z "$COVERAGE_FILE" ]]; then
  echo "ERROR: coverage.cobertura.xml not found. Set COVERAGE_FILE or run without SKIP_TEST_RUN=1."
  exit 1
fi

declare -A COVERAGE
while IFS= read -r line; do
  name="$(echo "$line" | sed -E 's/.*name="([^"]+)".*/\1/')"
  rate="$(echo "$line" | sed -E 's/.*line-rate="([^"]+)".*/\1/')"
  percent="$(awk -v r="$rate" 'BEGIN { printf "%.2f", r * 100 }')"
  COVERAGE["$name"]="$percent"
done < <(grep -oE '<package name="[^"]+" line-rate="[^"]+"' "$COVERAGE_FILE")

to_fail=0

check_gate() {
  local layer="$1"
  local min="$2"
  local value="${COVERAGE[$layer]:-}"

  if [[ -z "$value" ]]; then
    echo "FAIL  $layer: not found in coverage report"
    to_fail=1
    return
  fi

  if awk -v v="$value" -v m="$min" 'BEGIN { exit !(v < m) }'; then
    echo "FAIL  $layer: ${value}% < ${min}%"
    to_fail=1
  else
    echo "PASS  $layer: ${value}% >= ${min}%"
  fi
}

echo
echo "Coverage gates"
check_gate "Domain" "$DOMAIN_MIN"
check_gate "Application" "$APPLICATION_MIN"
check_gate "Infrastructure" "$INFRASTRUCTURE_MIN"
check_gate "Web" "$WEB_MIN"

if [[ "$to_fail" -ne 0 ]]; then
  echo
  echo "Coverage gates failed."
  exit 1
fi

echo
echo "All coverage gates passed."
