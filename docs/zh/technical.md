# 技术文档 (ZH)

## 1. 架构
高层模块：
- **Web**（`src/Web`）：ASP.NET Core MVC + Razor Views，负责渲染 UI 并调用 API。
- **API**（`src/Api`）：ASP.NET Core Web API，提供受 JWT 保护的 endpoints。
- **DB**：PostgreSQL，使用 EF Core（Npgsql）。

主流程：
1. Web 通过 cookie 对用户进行认证。
2. Web 向 API 请求 JWT（`POST /api/auth/token`）并存入 session。
3. Web 调用 API 时附带 `Authorization: Bearer <token>`（handler 为 `ApiAuthTokenHandler`）。

分层结构：
- `src/Domain`（实体）
- `src/Application`（服务/用例、搜索查询）
- `src/Infrastructure`（EF Core、仓储、迁移）
- `src/Api`（controllers、JWT、swagger、CORS、seed）
- `src/Web`（MVC、视图、静态资源、i18n、API 客户端）

## 2. 技术栈
- .NET 8 / ASP.NET Core
- EF Core 8 + Npgsql
- API 使用 Swagger/OpenAPI（Swashbuckle）
- Serilog（Web 日志）
- Bootstrap 5 + 自定义 CSS
- Render（Docker 部署）

## 3. 仓库结构（关键路径）
- `docker/render/api.Dockerfile`
- `docker/render/web.Dockerfile`
- `docker-compose.yml`（本地）
- `src/Infrastructure/Migrations/*`
- `sql/001_seed_render.sql`（可选 SQL seed）

## 4. 认证与安全
### 4.1 API（JWT）
- `POST /api/auth/token`
- claims：`NameIdentifier`、`Role`
- 配置：
  - `Jwt__Key`（密钥）
  - `Jwt__Issuer`
  - `Jwt__Audience`

### 4.2 Web（cookies + session）
- cookie 方案：`CookieAuth`
- API token 保存在 session 键 `ApiToken`
- `ApiAuthTokenHandler` 负责附加 token；若 401/403，则清空 session 并登出

## 5. API endpoints（摘要）
除 Auth 外，均由 `[Authorize]` 保护：
- `/api/schools`
- `/api/students`
- `/api/enrollments`
- `/api/annualfees`
- `/api/scopes`

维护接口：
- `POST /api/maintenance/seed`（要求 `ADM` 角色 + `X-Seed-Key` header）

## 6. Swagger
启用方式：
- `Swagger__Enabled=true`

启用后：
- UI：`/api`
- JSON：`/swagger/v1/swagger.json`

## 7. CORS（API）
生产环境需要配置允许来源：
- `Cors__AllowedOrigins__0=https://<web-domain>`

## 8. 数据库与迁移
- API 启动时执行 `db.Database.Migrate()`
- migrations assembly 固定为 `Infrastructure`

Seed 方式：
- 启动 seed：`Seed__Enabled=true`（当 `users` 为空时一次性执行）
- 维护接口：`Seed__Key` + `X-Seed-Key`
- SQL 脚本：`sql/001_seed_render.sql`

## 9. Render 部署
服务：
- `escoles-db`（PostgreSQL）
- `escoles-api`（API）
- `escoles`（Web）

Dockerfiles：
- API：`docker/render/api.Dockerfile`
- Web：`docker/render/web.Dockerfile`

推荐环境变量：
- API：`ConnectionStrings__Default`、`ASPNETCORE_URLS`、`Jwt__*`、`Cors__AllowedOrigins__*`、`Swagger__Enabled`
- Web：`Api__BaseUrl`、`ASPNETCORE_URLS`

## 10. 故障排查
- 401/403：token 过期或无效，重新登录
- CORS：检查并配置允许来源
- 迁移问题：查看启动日志
- 小数处理：Web 通过 flexible binder 同时支持 `,` 与 `.`

## 附录 A：数据库
### A.1 ER 图
占位图片：
- `docs/assets/er-en.png`

关键关系：
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools`（若使用 `schools.scope_id`）

### A.2 数据表（摘要）
#### A.2.1 `schools`
- `id`（PK）、`name`、`code`、`city`、`is_favorite`、`created_at`
- `scope`（legacy 文本）
- 可选 `scope_id` 外键到 `scope_mnt.id`

#### A.2.2 `scope_mnt`
- `id`（PK）、`name`、`description`、`is_active`、`created_at`、`updated_at`

#### A.2.3 `users`
- `id`（PK）、`email`（unique）、`password_hash`、`role`、`is_active`、timestamps

#### A.2.4 `students`
- `id`（PK）、`school_id`（FK）、`user_id`（FK，存在时 unique）、`created_at`

#### A.2.5 `enrollments`
- `id`（PK）、`student_id`（FK）、`academic_year`、`course_name`、`status`、`enrolled_at`
- `school_id`（FK），用于查询一致性

#### A.2.6 `annual_fees`
- `id`（PK）、`enrollment_id`（FK）、`amount`、`currency`、`due_date`、`paid_at`、`payment_ref`

#### A.2.7 `__EFMigrationsHistory`
EF Core 内部迁移历史表。

### A.3 Seed / 演示数据
可选方式：
- 启动 seed（API）：数据库为空时创建 scopes 和管理员用户
- SQL seed：`sql/001_seed_render.sql`（完整演示数据集）
