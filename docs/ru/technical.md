# Технический документ (RU)

## 1. Архитектура
Компоненты верхнего уровня:
- **Web** (`src/Web`): ASP.NET Core MVC + Razor Views. Рендерит UI и вызывает API.
- **API** (`src/Api`): ASP.NET Core Web API. Предоставляет endpoints, защищенные JWT.
- **DB**: PostgreSQL. EF Core (Npgsql).

Основной поток:
1. Web аутентифицирует пользователя (cookie).
2. Web запрашивает JWT у API (`POST /api/auth/token`) и сохраняет его в сессии.
3. Web вызывает API с `Authorization: Bearer <token>` (handler `ApiAuthTokenHandler`).

Слоистая структура:
- `src/Domain` (сущности)
- `src/Application` (сервисы/сценарии, поисковый запрос)
- `src/Infrastructure` (EF Core, репозитории, миграции)
- `src/Api` (controllers, JWT, swagger, CORS, seed)
- `src/Web` (MVC, views, assets, i18n, API-клиенты)

## 2. Технологический стек
- .NET 8 / ASP.NET Core
- EF Core 8 + Npgsql
- Swagger/OpenAPI (Swashbuckle) в API
- Serilog (логи Web)
- Bootstrap 5 + кастомный CSS
- Render (деплой) через Docker

## 3. Структура репозитория (ключевые пути)
- `docker/render/api.Dockerfile`
- `docker/render/web.Dockerfile`
- `docker-compose.yml` (локально)
- `src/Infrastructure/Migrations/*`
- `sql/001_seed_render.sql` (опциональный SQL seed)

## 4. Аутентификация и безопасность
### 4.1 API (JWT)
- `POST /api/auth/token`
- claims: `NameIdentifier`, `Role`
- конфигурация:
  - `Jwt__Key` (secret)
  - `Jwt__Issuer`
  - `Jwt__Audience`

### 4.2 Web (cookies + session)
- cookie-схема `CookieAuth`
- API token хранится в сессии под ключом `ApiToken`
- `ApiAuthTokenHandler` прикрепляет токен; при 401/403 очищает сессию и выполняет logout

## 5. API endpoints (кратко)
Защищены `[Authorize]` (кроме Auth):
- `/api/schools`
- `/api/students`
- `/api/enrollments`
- `/api/annualfees`
- `/api/scopes`

Обслуживание:
- `POST /api/maintenance/seed` (требует роль `ADM` + header `X-Seed-Key`)

## 6. Swagger
Включение:
- `Swagger__Enabled=true`

Если включено:
- UI: `/api`
- JSON: `/swagger/v1/swagger.json`

## 7. CORS (API)
В production нужно настроить разрешенные origin:
- `Cors__AllowedOrigins__0=https://<web-domain>`

## 8. База данных и миграции
- `db.Database.Migrate()` выполняется при старте API
- assembly миграций принудительно указывает на `Infrastructure`

Варианты seed:
- seed при старте: `Seed__Enabled=true` (one-shot, если таблица `users` пуста)
- maintenance endpoint: `Seed__Key` + `X-Seed-Key`
- SQL script: `sql/001_seed_render.sql`

## 9. Деплой в Render
Сервисы:
- `escoles-db` (PostgreSQL)
- `escoles-api` (API)
- `escoles` (Web)

Dockerfiles:
- API: `docker/render/api.Dockerfile`
- Web: `docker/render/web.Dockerfile`

Рекомендуемые переменные окружения:
- API: `ConnectionStrings__Default`, `ASPNETCORE_URLS`, `Jwt__*`, `Cors__AllowedOrigins__*`, `Swagger__Enabled`
- Web: `Api__BaseUrl`, `ASPNETCORE_URLS`

## 10. Диагностика проблем
- 401/403: токен истек/некорректный, войти снова
- CORS: настроить разрешенные origin
- миграции: проверить стартовые логи
- десятичные: Web принимает `,` и `.` через гибкий binder

## Приложение A: База данных
### A.1 ER-диаграмма
Файл-заменитель:
- `docs/assets/er-en.png`

Ключевые связи:
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (если используется `schools.scope_id`)

### A.2 Таблицы (кратко)
#### A.2.1 `schools`
- `id` (PK), `name`, `code`, `city`, `is_favorite`, `created_at`
- `scope` (legacy text)
- опциональный `scope_id` FK на `scope_mnt.id`

#### A.2.2 `scope_mnt`
- `id` (PK), `name`, `description`, `is_active`, `created_at`, `updated_at`

#### A.2.3 `users`
- `id` (PK), `email` (unique), `password_hash`, `role`, `is_active`, timestamps

#### A.2.4 `students`
- `id` (PK), `school_id` (FK), `user_id` (FK, unique при наличии), `created_at`

#### A.2.5 `enrollments`
- `id` (PK), `student_id` (FK), `academic_year`, `course_name`, `status`, `enrolled_at`
- `school_id` (FK) для согласованных запросов

#### A.2.6 `annual_fees`
- `id` (PK), `enrollment_id` (FK), `amount`, `currency`, `due_date`, `paid_at`, `payment_ref`

#### A.2.7 `__EFMigrationsHistory`
Внутреннее хранение истории миграций EF Core.

### A.3 Seed / демо-данные
Варианты:
- startup seed (API): создает scopes и admin пользователя при пустой DB
- SQL seed: `sql/001_seed_render.sql` (полный демонстрационный набор)
