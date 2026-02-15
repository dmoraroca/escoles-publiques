# Documento tecnico (ES)

## 1. Arquitectura
Arquitectura en 3 bloques:
- **Web** (`src/Web`): ASP.NET Core MVC + Razor Views. Renderiza pantallas y consume la API.
- **API** (`src/Api`): ASP.NET Core Web API. Endpoints protegidos con JWT.
- **BBDD**: PostgreSQL (EF Core / Npgsql).

Flujo principal:
1. Web autentica al usuario (cookie).
2. Web solicita un JWT a la API (`POST /api/auth/token`) y lo guarda en sesion.
3. Web llama a la API con `Authorization: Bearer <token>` (`ApiAuthTokenHandler`).

Capas:
- `src/Domain`, `src/Application`, `src/Infrastructure`, `src/Api`, `src/Web`

## 2. Stack
- .NET 8 / ASP.NET Core
- EF Core 8 + Npgsql
- Swagger/OpenAPI (Swashbuckle) en la API
- Bootstrap 5 + CSS propio
- Render (deploy) via Docker

## 3. Rutas clave del repositorio
- `docker/render/api.Dockerfile`
- `docker/render/web.Dockerfile`
- `docker-compose.yml` (local)
- `src/Infrastructure/Migrations/*`
- `sql/001_seed_render.sql` (seed opcional)

## 4. Autenticacion y seguridad
API (JWT):
- `POST /api/auth/token`
- config: `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`

Web (cookie + sesion):
- esquema `CookieAuth`
- token API en sesion (`ApiToken`)
- `ApiAuthTokenHandler` adjunta el token; en 401/403 limpia sesion y hace logout

## 5. Endpoints (resumen)
Protegidos con `[Authorize]` (excepto Auth):
- `/api/schools`
- `/api/students`
- `/api/enrollments`
- `/api/annualfees`
- `/api/scopes`

Mantenimiento:
- `POST /api/maintenance/seed` (rol `ADM` + header `X-Seed-Key`)

## 6. Swagger
Habilitar con:
- `Swagger__Enabled=true`

Cuando esta habilitado:
- UI en `/api`
- JSON en `/swagger/v1/swagger.json`

## 7. CORS (API)
En produccion requiere origenes configurados:
- `Cors__AllowedOrigins__0=https://<dominio-web>`

## 8. BBDD y migrations
- `db.Database.Migrate()` se ejecuta al arrancar la API
- el assembly de migrations se fuerza a `Infrastructure`

Seed:
- por arranque: `Seed__Enabled=true` (solo si `users` esta vacia)
- endpoint: `Seed__Key` + `X-Seed-Key`
- SQL: `sql/001_seed_render.sql`

## 9. Despliegue en Render (Docker)
Servicios tipicos:
- `escoles-db` (PostgreSQL)
- `escoles-api` (API)
- `escoles` (Web)

Variables recomendadas:
- API: `ConnectionStrings__Default`, `ASPNETCORE_URLS`, `Jwt__*`, `Cors__AllowedOrigins__*`, `Swagger__Enabled`
- Web: `Api__BaseUrl`, `ASPNETCORE_URLS`

## 10. Troubleshooting
- 401/403: token invalido/caducado, re-login
- CORS: configurar allowed origins
- migrations: revisar logs de arranque
- decimales: web acepta `,` y `.`

## Anexo A: Base de datos
### A.1 Diagrama ER
Placeholder:
- `docs/assets/er-es.png`

Relaciones principales:
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (si se usa `schools.scope_id`)

### A.2 Tablas (resumen)
- `schools`: datos de escuela (legacy `scope`, opcional `scope_id`)
- `scope_mnt`: ambitos/scopes
- `users`: usuarios (email unico)
- `students`: alumno + escuela + usuario opcional
- `enrollments`: inscripciones (incluye `school_id`)
- `annual_fees`: cuotas anuales
- `__EFMigrationsHistory`: interno EF Core

### A.3 Seed / demo
- Seed por arranque (API) si DB vacia
- SQL seed: `sql/001_seed_render.sql`

