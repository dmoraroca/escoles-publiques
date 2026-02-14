# Technical document (EN)

## 1. Architecture
High-level blocks:
- **Web** (`src/Web`): ASP.NET Core MVC + Razor Views. Renders UI and calls the API.
- **API** (`src/Api`): ASP.NET Core Web API. Exposes endpoints protected by JWT.
- **DB**: PostgreSQL. EF Core (Npgsql).

Main flow:
1. Web authenticates the user (cookie).
2. Web requests a JWT from the API (`POST /api/auth/token`) and stores it in session.
3. Web calls the API with `Authorization: Bearer <token>` (handler `ApiAuthTokenHandler`).

Layered structure:
- `src/Domain` (entities)
- `src/Application` (services/use cases, search query)
- `src/Infrastructure` (EF Core, repositories, migrations)
- `src/Api` (controllers, JWT, swagger, CORS, seed)
- `src/Web` (MVC, views, assets, i18n, API clients)

## 2. Tech stack
- .NET 8 / ASP.NET Core
- EF Core 8 + Npgsql
- Swagger/OpenAPI (Swashbuckle) on the API
- Serilog (Web logs)
- Bootstrap 5 + custom CSS
- Render (deployment) via Docker

## 3. Repository structure (key paths)
- `docker/render/api.Dockerfile`
- `docker/render/web.Dockerfile`
- `docker-compose.yml` (local)
- `src/Infrastructure/Migrations/*`
- `sql/001_seed_render.sql` (optional SQL seed)

## 4. Auth and security
### 4.1 API (JWT)
- `POST /api/auth/token`
- claims: `NameIdentifier`, `Role`
- config:
  - `Jwt__Key` (secret)
  - `Jwt__Issuer`
  - `Jwt__Audience`

### 4.2 Web (cookies + session)
- cookie scheme `CookieAuth`
- API token stored in session key `ApiToken`
- `ApiAuthTokenHandler` attaches the token; if 401/403, it clears session and logs out

## 5. API endpoints (summary)
Protected with `[Authorize]` (except Auth):
- `/api/schools`
- `/api/students`
- `/api/enrollments`
- `/api/annualfees`
- `/api/scopes`

Maintenance:
- `POST /api/maintenance/seed` (requires role `ADM` + header `X-Seed-Key`)

## 6. Swagger
Enable with:
- `Swagger__Enabled=true`

When enabled:
- UI at `/api`
- JSON at `/swagger/v1/swagger.json`

## 7. CORS (API)
Production requires configured origins:
- `Cors__AllowedOrigins__0=https://<web-domain>`

## 8. Database & migrations
- `db.Database.Migrate()` is executed on API startup
- migrations assembly is forced to `Infrastructure`

Seed options:
- startup seed: `Seed__Enabled=true` (one-shot if `users` table is empty)
- maintenance endpoint: `Seed__Key` + `X-Seed-Key`
- SQL script: `sql/001_seed_render.sql`

## 9. Render deployment
Services:
- `escoles-db` (PostgreSQL)
- `escoles-api` (API)
- `escoles` (Web)

Dockerfiles:
- API: `docker/render/api.Dockerfile`
- Web: `docker/render/web.Dockerfile`

Recommended env vars:
- API: `ConnectionStrings__Default`, `ASPNETCORE_URLS`, `Jwt__*`, `Cors__AllowedOrigins__*`, `Swagger__Enabled`
- Web: `Api__BaseUrl`, `ASPNETCORE_URLS`

## 10. Troubleshooting
- 401/403: token expired/invalid, re-login
- CORS: configure allowed origins
- migrations: check startup logs
- decimals: web accepts `,` and `.` via flexible binder
