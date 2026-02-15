# Technisches Dokument (DE)

## 1. Architektur
Blöcke:
- **Web** (`src/Web`): ASP.NET Core MVC + Razor Views.
- **API** (`src/Api`): ASP.NET Core Web API (JWT geschützt).
- **DB**: PostgreSQL (EF Core / Npgsql).

Flow:
1. Web authentifiziert per Cookie.
2. Web holt JWT von API (`POST /api/auth/token`) und speichert es in der Session.
3. Web ruft API mit `Authorization: Bearer <token>` auf (`ApiAuthTokenHandler`).

Layer:
- `src/Domain`, `src/Application`, `src/Infrastructure`, `src/Api`, `src/Web`

## 2. Tech-Stack
- .NET 8 / ASP.NET Core
- EF Core 8 + Npgsql
- Swagger/OpenAPI (API)
- Serilog (Web)
- Bootstrap 5 + Custom CSS
- Render Deployment via Docker

## 3. Repo-Struktur
- `docker/render/api.Dockerfile`
- `docker/render/web.Dockerfile`
- `src/Infrastructure/Migrations/*`
- `sql/001_seed_render.sql`

## 4. Auth/Sicherheit
API JWT:
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`

Web:
- CookieAuth + Session
- API Token in Session (`ApiToken`)

## 5. API Endpoints (Kurz)
- `/api/schools`, `/api/students`, `/api/enrollments`, `/api/annualfees`, `/api/scopes`
- Maintenance: `POST /api/maintenance/seed` (Role `ADM` + `X-Seed-Key`)

## 6. Swagger
- `Swagger__Enabled=true`
- UI unter `/api`

## 7. CORS
In Produktion Origins setzen:
- `Cors__AllowedOrigins__0=https://<web-domain>`

## 8. Datenbank & Migrations
- `db.Database.Migrate()` beim API-Start
- Optional Seed: `Seed__Enabled=true` (nur wenn `users` leer ist)

## 9. Render Deployment
Services:
- `escoles-db`, `escoles-api`, `escoles`

Env vars:
- API: `ConnectionStrings__Default`, `ASPNETCORE_URLS`, `Jwt__*`, `Cors__AllowedOrigins__*`
- Web: `Api__BaseUrl`, `ASPNETCORE_URLS`

## 10. Troubleshooting
- 401/403: neu anmelden
- CORS: Origins konfigurieren
- Dezimal: Web akzeptiert `,` und `.`

## Anhang A: Datenbank
### A.1 ER-Diagramm
Platzhalter:
- `docs/assets/er-de.png`

Beziehungen:
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (optional via `scope_id`)

### A.2 Tabellen (Kurz)
- `schools`: Stammdaten der Schule (optional `scope_id`, legacy `scope`)
- `scope_mnt`: Bereiche/Scopes
- `users`: Login-Benutzer (email unique)
- `students`: Student + Schule + optional User
- `enrollments`: Einschreibungen (inkl. `school_id`)
- `annual_fees`: Jahresgebuhren pro Einschreibung
- `__EFMigrationsHistory`: EF Core intern

### A.3 Seed / Demo-Daten
- Startup Seed (API): Scopes + Admin Benutzer wenn DB leer ist
- SQL Seed: `sql/001_seed_render.sql`
