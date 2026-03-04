*** Per aixecar postgress? ***

1 => docker compose up -d
2 => docker ps

o per vsCode amb el menu contextual Start

*** Aturar PostgreSQL ***

docker compose down

*** Reiniciar PostgreSQL ***

docker compose restart

*** Aixecar PostgreSQL ***


docker start escoles-db

*** Veure logs PostgreSQL ***

docker logs escoles-db

*** Comprova estat ***
docker ps

*** Recomanació pràctica ***

Per ús diari → VS Code (més còmode).
Per scripts / CI → docker compose up -d.

*** scaffold ***

dotnet ef dbcontext scaffold \
"Host=localhost;Port=5432;Database=escoles;Username=app;Password=app" \
Npgsql.EntityFrameworkCore.PostgreSQL \
--project src/Infrastructure \
--output-dir Persistence/Scaffold \
--schema public \
--no-context \
--force

*** Git create a new repository on the command line ***

echo "# escoles-publiques" >> README.md
git init
git add README.md
git commit -m "first commit"
git branch -M main
git remote add origin https://github.com/dmoraroca/escoles-publiques.git
git push -u origin main

*** Git push an existing repository from the command line ***

git remote add origin https://github.com/dmoraroca/escoles-publiques.git
git branch -M main
git push -u origin main

*** Coverage gates (DDD) ***

Script local/CI:

`./scripts/check-coverage-gates.sh`

Mode check-only (sense executar tests):

`COVERAGE_FILE=path/to/coverage.cobertura.xml SKIP_TEST_RUN=1 ./scripts/check-coverage-gates.sh`

Llindars per defecte:

- Domain: 80%
- Application: 80%
- Infrastructure: 15%
- Web: 50%
- Api: 10%

Override de llindars (exemple):

`DOMAIN_MIN=85 APPLICATION_MIN=85 INFRASTRUCTURE_MIN=20 WEB_MIN=55 API_MIN=20 ./scripts/check-coverage-gates.sh`

CI GitHub Actions:

`.github/workflows/coverage-gates.yml`

*** Error contract API (ProblemDetails) ***

Tots els errors controlats de l'API retornen `application/problem+json` amb extensions:

- `errorCode`: codi estable de l'error (`validation_error`, `not_found`, etc.)
- `traceId`: correlació de petició (`X-Correlation-ID`)
- `timestamp`: marca temporal UTC
- `fieldErrors`: només en validació

*** Critical flows (risk-based tests) ***

Execució local de tests crítics:

`./scripts/run-critical-tests.sh`

CI dedicat:

`.github/workflows/critical-flows.yml`
