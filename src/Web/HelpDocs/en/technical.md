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

## Appendix A: Database
### A.1 ER diagram
Placeholder image:
- `docs/assets/er-en.png`

Key relationships:
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (if `schools.scope_id` is used)

### A.2 Tables (summary)
#### A.2.1 `schools`
- `id` (PK), `name`, `code`, `city`, `is_favorite`, `created_at`
- `scope` (legacy text)
- optional `scope_id` FK to `scope_mnt.id`

#### A.2.2 `scope_mnt`
- `id` (PK), `name`, `description`, `is_active`, `created_at`, `updated_at`

#### A.2.3 `users`
- `id` (PK), `email` (unique), `password_hash`, `role`, `is_active`, timestamps

#### A.2.4 `students`
- `id` (PK), `school_id` (FK), `user_id` (FK, unique when present), `created_at`

#### A.2.5 `enrollments`
- `id` (PK), `student_id` (FK), `academic_year`, `course_name`, `status`, `enrolled_at`
- `school_id` (FK) for consistent queries

#### A.2.6 `annual_fees`
- `id` (PK), `enrollment_id` (FK), `amount`, `currency`, `due_date`, `paid_at`, `payment_ref`

#### A.2.7 `__EFMigrationsHistory`
EF Core internal migrations tracking.

### A.3 Seed / demo data
Options:
- Startup seed (API): creates scopes and an admin user when DB is empty
- SQL seed: `sql/001_seed_render.sql` (full demo dataset)
