# BBDD (CA)

## 1. Model relacional (ER)
- Diagrama (inserir imatge)
  - Placeholder: `docs/assets/er-ca.png`

Relacions principals:
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students` (un usuari pot estar associat a un unic alumne)
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (si s'utilitza `schools.scope_id`)

## 2. Taules
Per cada taula s'indica: proposit, camps clau, PK/FK i indexos.

Taules esperades:
- `schools`
- `scope_mnt`
- `users`
- `students`
- `enrollments`
- `annual_fees`

### 2.1 `schools`
Proposit: catalog d'escoles.

Camps (principals):
- `id` (bigint, PK)
- `name` (text, NOT NULL)
- `code` (text, NOT NULL)
- `city` (text, NULL)
- `is_favorite` (boolean, NOT NULL)
- `created_at` (timestamp, NOT NULL)
- `scope` (text, NULL) (camp historic/legacy)
- `scope_id` (bigint, NULL) (si s'ha activat model relacional amb `scope_mnt`)

FK:
- (opcional) `scope_id -> scope_mnt.id`

Indexos:
- recomanat: index per `code` (si es vol unicitat per codi)
- recomanat: index per `scope_id`

### 2.2 `scope_mnt`
Proposit: ambits (Infantil/Primaria/Secundaria/FP).

Camps:
- `id` (bigint, PK)
- `name` (text, NOT NULL)
- `description` (text, NULL)
- `is_active` (boolean, NOT NULL)
- `created_at` (timestamp, NOT NULL)
- `updated_at` (timestamp, NOT NULL)

### 2.3 `users`
Proposit: usuaris del sistema (login).

Camps:
- `id` (bigint, PK)
- `first_name` (text, NOT NULL)
- `last_name` (text, NOT NULL)
- `email` (text, NOT NULL, unique)
- `password_hash` (text, NOT NULL) (hash SHA-256 hex)
- `role` (text, NOT NULL) (`ADM` o `USER`)
- `birth_date` (date, NULL)
- `is_active` (boolean, NOT NULL)
- `created_at` (timestamp, NOT NULL)
- `updated_at` (timestamp, NOT NULL)
- `last_login_at` (timestamp, NULL)

Indexos:
- `IX_users_email` (unique)

### 2.4 `students`
Proposit: relacio alumne <-> escola i (opcionalment) alumne <-> usuari.

Camps:
- `id` (bigint, PK)
- `school_id` (bigint, NOT NULL)
- `user_id` (bigint, NULL) (unique quan existeix)
- `created_at` (timestamp, NOT NULL)

FK:
- `school_id -> schools.id` (ON DELETE CASCADE)
- `user_id -> users.id` (sense cascade)

Indexos:
- `IX_students_school_id`
- `IX_students_user_id` (unique)

### 2.5 `enrollments`
Proposit: inscripcions d'un alumne per any academic/curs.

Camps:
- `id` (bigint, PK)
- `student_id` (bigint, NOT NULL)
- `academic_year` (text, NOT NULL) (p. ex. `2025-2026`)
- `course_name` (text, NULL) (p. ex. `4t ESO`)
- `status` (text, NOT NULL) (p. ex. `Activa`, `Pendent`, `Cancel·lada`)
- `enrolled_at` (timestamp, NOT NULL)
- `school_id` (bigint, NOT NULL) (afegit posteriorment per consistencia i consultes)

FK:
- `student_id -> students.id` (ON DELETE CASCADE)
- `school_id -> schools.id` (recomanat; implementat via migracio idempotent en alguns entorns)

Indexos:
- `IX_enrollments_student_id`
- (recomanat) `ix_enrollments_school_id`

### 2.6 `annual_fees`
Proposit: quotes anuals associades a una inscripcio.

Camps:
- `id` (bigint, PK)
- `enrollment_id` (bigint, NOT NULL)
- `amount` (numeric/decimal, NOT NULL)
- `currency` (text, NOT NULL) (p. ex. `EUR`)
- `due_date` (date, NOT NULL)
- `paid_at` (timestamp, NULL)
- `payment_ref` (text, NULL)

FK:
- `enrollment_id -> enrollments.id` (ON DELETE CASCADE)

Indexos:
- `IX_annual_fees_enrollment_id`

### 2.7 `__EFMigrationsHistory`
Proposit: control intern d'EF Core per saber quines migrations s'han aplicat.

## 3. Relacions clau
- Escola -> Alumne
- Alumne -> Inscripcio
- Inscripcio -> Quotes anuals

## 4. Seed / dades demo
- Quines dades s'inserten en entorns de demo/proves

Opcions de seed:
- Seed automàtic (API) si `users` esta buida: crea scopes basics + usuari admin (configurable)
- Script SQL `sql/001_seed_render.sql` (dades demo completes)

Recomanacio:
- en produccio, evitar seeds automàtics o protegir-los amb claus (header/secret) i rols.
