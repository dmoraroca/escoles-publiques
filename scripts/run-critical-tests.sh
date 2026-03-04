#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

echo "Running critical flows tests (auth + core CRUD + global error handling)..."

dotnet test src/UnitTest/UnitTest.csproj -c Debug --no-build --no-restore --filter "FullyQualifiedName~AuthController|FullyQualifiedName~SchoolService|FullyQualifiedName~StudentService|FullyQualifiedName~EnrollmentService|FullyQualifiedName~AnnualFeeService|FullyQualifiedName~ApiExceptionHandlingMiddlewareTests|FullyQualifiedName~SchoolCqrsIntegrationTests"
