# Document tecnic (CA)

## 1. Arquitectura
### 1.1 Visio general
Arquitectura en 3 blocs:
- **Web** (`src/Web`): ASP.NET Core MVC + Razor Views. Mostra pantalles i consumeix l'API.
- **API** (`src/Api`): ASP.NET Core Web API. Exposa endpoints protegits amb JWT.
- **BBDD**: PostgreSQL. Persistencia via EF Core (Npgsql).

Flux principal:
1. Web autentica usuari (cookie).
2. Web demana token a l'API (`POST /api/auth/token`) i el desa a sessio.
3. Web crida l'API amb `Authorization: Bearer <token>` (handler `ApiAuthTokenHandler`).

### 1.2 Capes del codi (estructura)
El repositori esta organitzat en capes:
- `src/Domain`: entitats del domini (School, Student, Enrollment, AnnualFee, Scope, User)
- `src/Application`: serveis/casos d'us (interfaces i implementacions) i queries de cerca
- `src/Infrastructure`: EF Core `DbContext`, repositoris, migrations
- `src/Api`: controllers API, auth JWT, swagger, seed i CORS
- `src/Web`: controllers MVC, views, assets, i18n, http clients cap a l'API

## 2. Stack i dependències
- .NET 8 / ASP.NET Core
- EF Core 8 + `Npgsql.EntityFrameworkCore.PostgreSQL`
- Swagger/OpenAPI (Swashbuckle) a l'API
- Serilog (web) per logs
- Bootstrap 5 (web) + CSS propi (`wwwroot/css/*`)
- Render (deploy) amb Docker

## 3. Estructura del repositori
Fitxers/claus:
- `docker/render/api.Dockerfile`
- `docker/render/web.Dockerfile`
- `docker-compose.yml` (entorn local)
- `src/Infrastructure/Migrations/*` (migrations EF)
- `sql/001_seed_render.sql` (seed SQL per entorns)

## 4. Decisions i patrons
- **MVC (Web)**: controllers + views + viewmodels
- **API controllers (Api)**: DTOs d'entrada/sortida, errors com `ValidationProblem`
- **Repositori + servei**: `I*Repository` a Infrastructure i `I*Service` a Application
- **DTOs**: l'API no exposa directament entitats (retorna `*DtoOut`)
- **I18n**: `resx` per vista i per components
- **Responsive**: CSS amb media queries i taules adaptades a mobil (veure `RESPONSIVE.md`)

## 5. Autenticacio i seguretat
### 5.1 API (JWT)
- Endpoint: `POST /api/auth/token`
- Valida credencials via `IAuthService`
- Retorna `token` (JWT) amb claims:
  - `NameIdentifier` (id usuari)
  - `Role` (`ADM`/`USER`)
- Validacio a `src/Api/Program.cs` (`JwtBearer`)

Config JWT (produccio):
- `Jwt__Key` (secret llarg; minim recomanat 32 chars)
- `Jwt__Issuer`
- `Jwt__Audience`

### 5.2 Web (Cookies + sessio)
- Autenticacio per cookie `CookieAuth`
- Sessio server-side amb `DistributedMemoryCache`
- Token de l'API desat a sessio (`SessionKeys.ApiToken`)
- Handler `ApiAuthTokenHandler`:
  - adjunta token a cada request cap a l'API
  - si rep 401/403, fa logout i neteja sessio

## 6. Endpoints API (resum)
Tots protegits amb `[Authorize]` excepte Auth:
- `POST /api/auth/token`
- `GET/POST/PUT/DELETE /api/schools`
- `GET/POST/PUT/DELETE /api/students`
- `GET/POST/PUT/DELETE /api/enrollments`
- `GET/POST/PUT/DELETE /api/annualfees`
- `GET /api/scopes`

Manteniment:
- `POST /api/maintenance/seed` (requereix rol `ADM` + header `X-Seed-Key`)

## 7. Swagger
Swagger es pot habilitar amb:
- `Swagger__Enabled=true`

Quan esta habilitat:
- Swagger UI es serveix a `https://<host>/api`
- Swagger JSON: `/swagger/v1/swagger.json`

## 8. CORS (API)
En produccio l'API requereix llista d'orígens:
- `Cors__AllowedOrigins__0=https://<domini-web>`
- `Cors__AllowedOrigins__1=...` (si cal)

Si no esta configurat i no es dev, l'API falla a l'arrencada (per evitar desplegar insegur).

## 9. Base de dades i migrations
### 9.1 EF Core
- `SchoolDbContext` a `src/Infrastructure/SchoolDbContext.cs` (namespace `Infrastructure.Persistence`)
- L'API aplica `db.Database.Migrate()` a l'arrencada (a `src/Api/Program.cs`)
- L'API forca l'assembly de migrations: `MigrationsAssembly(\"Infrastructure\")`

### 9.2 Seed
Opcio A (arrencada API):
- `Seed__Enabled=true` activa `DbSeeder.SeedIfEmpty`
- El seed nomes s'executa si `users` esta buida (one-shot)

Opcio B (endpoint):
- `POST /api/maintenance/seed`
- Requereix `Seed__Key` i enviar header `X-Seed-Key`

Opcio C (SQL manual):
- `sql/001_seed_render.sql` (idempotent) per entorns Render/proves

Nota: no posar passwords reals als scripts. Rotar credencials i secrets a produccio.

## 10. Desplegament a Render
Serveis tipics:
- `escoles-db` (PostgreSQL)
- `escoles-api` (Web Service)
- `escoles` (Web Service)

Dockerfiles:
- API: `docker/render/api.Dockerfile` (publica `src/Api`)
- Web: `docker/render/web.Dockerfile` (publica `src/Web`)

Variables d'entorn recomanades (Render):
- API:
  - `ConnectionStrings__Default` (Internal DB URL o External amb SSL)
  - `ASPNETCORE_URLS=http://0.0.0.0:$PORT`
  - `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`
  - `Cors__AllowedOrigins__0=https://<domini-web>`
  - `Swagger__Enabled=true` (si es vol)
  - `Seed__Enabled` i `Seed__Key` (nomes per entorns controlats)
- Web:
  - `ConnectionStrings__Default` (si la web accedeix a DB directament; si no cal, eliminar)
  - `Api__BaseUrl=https://<domini-api>`
  - `ASPNETCORE_URLS=http://0.0.0.0:$PORT`

## 11. Troubleshooting (resum)
- 401/403 des de web: token expirat o invalid -> tornar a login
- CORS: configurar `Cors__AllowedOrigins__*`
- Migrations: revisar logs d'arrencada (EF Core)
- Decimals (web): accepta `,` i `.` via binder flexible
- Swagger: habilitar `Swagger__Enabled` i usar ruta `/api`

## Annex A: BBDD
Vegeu `docs/ca/bbdd.md`.
