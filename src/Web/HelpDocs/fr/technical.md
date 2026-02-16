# Document technique (FR)

## 1. Architecture
Blocs de haut niveau :
- **Web** (`src/Web`) : ASP.NET Core MVC + Razor Views. Rend l'interface et appelle l'API.
- **API** (`src/Api`) : ASP.NET Core Web API. Expose des endpoints protégés par JWT.
- **DB** : PostgreSQL. EF Core (Npgsql).

Flux principal :
1. Le Web authentifie l'utilisateur (cookie).
2. Le Web demande un JWT à l'API (`POST /api/auth/token`) et le stocke en session.
3. Le Web appelle l'API avec `Authorization: Bearer <token>` (handler `ApiAuthTokenHandler`).

Structure en couches :
- `src/Domain` (entités)
- `src/Application` (services/cas d'usage, requête de recherche)
- `src/Infrastructure` (EF Core, repositories, migrations)
- `src/Api` (controllers, JWT, swagger, CORS, seed)
- `src/Web` (MVC, vues, assets, i18n, clients API)

## 2. Stack technique
- .NET 8 / ASP.NET Core
- EF Core 8 + Npgsql
- Swagger/OpenAPI (Swashbuckle) dans l'API
- Serilog (logs Web)
- Bootstrap 5 + CSS personnalisé
- Render (déploiement) via Docker

## 3. Structure du dépôt (chemins clés)
- `docker/render/api.Dockerfile`
- `docker/render/web.Dockerfile`
- `docker-compose.yml` (local)
- `src/Infrastructure/Migrations/*`
- `sql/001_seed_render.sql` (seed SQL optionnel)

## 4. Authentification et sécurité
### 4.1 API (JWT)
- `POST /api/auth/token`
- claims : `NameIdentifier`, `Role`
- configuration :
  - `Jwt__Key` (secret)
  - `Jwt__Issuer`
  - `Jwt__Audience`

### 4.2 Web (cookies + session)
- schéma cookie `CookieAuth`
- token API stocké en session avec clé `ApiToken`
- `ApiAuthTokenHandler` attache le token ; en cas de 401/403, il vide la session et déconnecte

## 5. Endpoints API (résumé)
Protégés avec `[Authorize]` (sauf Auth) :
- `/api/schools`
- `/api/students`
- `/api/enrollments`
- `/api/annualfees`
- `/api/scopes`

Maintenance :
- `POST /api/maintenance/seed` (nécessite rôle `ADM` + header `X-Seed-Key`)

## 6. Swagger
Activation :
- `Swagger__Enabled=true`

Quand activé :
- UI : `/api`
- JSON : `/swagger/v1/swagger.json`

## 7. CORS (API)
En production, configurer les origines autorisées :
- `Cors__AllowedOrigins__0=https://<web-domain>`

## 8. Base de données et migrations
- `db.Database.Migrate()` est exécuté au démarrage de l'API
- l'assembly des migrations est forcé à `Infrastructure`

Options de seed :
- seed au démarrage : `Seed__Enabled=true` (one-shot si table `users` vide)
- endpoint maintenance : `Seed__Key` + `X-Seed-Key`
- script SQL : `sql/001_seed_render.sql`

## 9. Déploiement Render
Services :
- `escoles-db` (PostgreSQL)
- `escoles-api` (API)
- `escoles` (Web)

Dockerfiles :
- API : `docker/render/api.Dockerfile`
- Web : `docker/render/web.Dockerfile`

Variables d'environnement recommandées :
- API : `ConnectionStrings__Default`, `ASPNETCORE_URLS`, `Jwt__*`, `Cors__AllowedOrigins__*`, `Swagger__Enabled`
- Web : `Api__BaseUrl`, `ASPNETCORE_URLS`

## 10. Résolution de problèmes
- 401/403 : token expiré/invalide, se reconnecter
- CORS : configurer les origines autorisées
- migrations : vérifier les logs de démarrage
- décimales : le Web accepte `,` et `.` via binder flexible

## Annexe A : Base de données
### A.1 Diagramme ER
Image de référence :
- `docs/assets/er-en.png`

Relations clés :
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (si `schools.scope_id` est utilisé)

### A.2 Tables (résumé)
#### A.2.1 `schools`
- `id` (PK), `name`, `code`, `city`, `is_favorite`, `created_at`
- `scope` (texte legacy)
- `scope_id` optionnel FK vers `scope_mnt.id`

#### A.2.2 `scope_mnt`
- `id` (PK), `name`, `description`, `is_active`, `created_at`, `updated_at`

#### A.2.3 `users`
- `id` (PK), `email` (unique), `password_hash`, `role`, `is_active`, timestamps

#### A.2.4 `students`
- `id` (PK), `school_id` (FK), `user_id` (FK, unique si présent), `created_at`

#### A.2.5 `enrollments`
- `id` (PK), `student_id` (FK), `academic_year`, `course_name`, `status`, `enrolled_at`
- `school_id` (FK) pour cohérence de requêtes

#### A.2.6 `annual_fees`
- `id` (PK), `enrollment_id` (FK), `amount`, `currency`, `due_date`, `paid_at`, `payment_ref`

#### A.2.7 `__EFMigrationsHistory`
Suivi interne des migrations EF Core.

### A.3 Seed / données de démo
Options :
- seed au démarrage (API) : crée des scopes et un admin si DB vide
- seed SQL : `sql/001_seed_render.sql` (jeu de données complet de démonstration)
