# Database (EN)

## 1. ER diagram
Placeholder image:
- `docs/assets/er-en.png`

Key relationships:
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (if `schools.scope_id` is used)

## 2. Tables (summary)
### 2.1 `schools`
- `id` (PK), `name`, `code`, `city`, `is_favorite`, `created_at`
- `scope` (legacy text)
- optional `scope_id` FK to `scope_mnt.id`

### 2.2 `scope_mnt`
- `id` (PK), `name`, `description`, `is_active`, `created_at`, `updated_at`

### 2.3 `users`
- `id` (PK), `email` (unique), `password_hash`, `role`, `is_active`, timestamps

### 2.4 `students`
- `id` (PK), `school_id` (FK), `user_id` (FK, unique when present), `created_at`

### 2.5 `enrollments`
- `id` (PK), `student_id` (FK), `academic_year`, `course_name`, `status`, `enrolled_at`
- `school_id` (FK) for consistent queries

### 2.6 `annual_fees`
- `id` (PK), `enrollment_id` (FK), `amount`, `currency`, `due_date`, `paid_at`, `payment_ref`

### 2.7 `__EFMigrationsHistory`
EF Core internal migrations tracking.

## 3. Seed / demo data
Options:
- Startup seed (API): creates scopes and an admin user when DB is empty
- SQL seed: `sql/001_seed_render.sql` (full demo dataset)
