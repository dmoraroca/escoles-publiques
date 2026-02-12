-- Render seed script (idempotent)
-- 1) Ensures schema matches current API model (schools.scope_id FK -> scope_mnt.id)
-- 2) Seeds admin user for login
-- 3) Seeds basic demo data for scopes, schools, students, enrollments, annual fees

BEGIN;

-- Ensure scope_id exists on schools (the current API model expects it)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'public'
          AND table_name = 'schools'
          AND column_name = 'scope_id'
    ) THEN
        ALTER TABLE public.schools ADD COLUMN scope_id bigint NULL;
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'fk_schools_scope_id'
    ) THEN
        ALTER TABLE public.schools
            ADD CONSTRAINT fk_schools_scope_id
            FOREIGN KEY (scope_id) REFERENCES public.scope_mnt(id)
            ON DELETE SET NULL;
    END IF;
END $$;

-- Optional index for scope lookups
CREATE INDEX IF NOT EXISTS ix_schools_scope_id ON public.schools(scope_id);

-- ADMIN user:
-- email: admin@admin.adm
-- password: admin123
-- sha256(admin123) = 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9
INSERT INTO public.users
    (first_name, last_name, email, password_hash, role, birth_date, is_active, created_at, updated_at, last_login_at)
SELECT
    'Admin', 'Root', 'admin@admin.adm',
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'ADM', NULL, true, NOW(), NOW(), NULL
WHERE NOT EXISTS (
    SELECT 1 FROM public.users WHERE email = 'admin@admin.adm'
);

-- Scopes
INSERT INTO public.scope_mnt (name, description, is_active, created_at, updated_at)
SELECT 'Infantil', 'Educacio infantil', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM public.scope_mnt WHERE name = 'Infantil');

INSERT INTO public.scope_mnt (name, description, is_active, created_at, updated_at)
SELECT 'Primaria', 'Educacio primaria', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM public.scope_mnt WHERE name = 'Primaria');

INSERT INTO public.scope_mnt (name, description, is_active, created_at, updated_at)
SELECT 'Secundaria', 'Educacio secundaria', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM public.scope_mnt WHERE name = 'Secundaria');

INSERT INTO public.scope_mnt (name, description, is_active, created_at, updated_at)
SELECT 'FP', 'Formacio professional', true, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM public.scope_mnt WHERE name = 'FP');

-- Schools
INSERT INTO public.schools (name, code, city, is_favorite, scope, created_at, scope_id)
SELECT 'Escola Joan Miro', '08010123', 'Vic', true, NULL, NOW(),
       (SELECT id FROM public.scope_mnt WHERE name = 'Primaria' LIMIT 1)
WHERE NOT EXISTS (SELECT 1 FROM public.schools WHERE code = '08010123');

INSERT INTO public.schools (name, code, city, is_favorite, scope, created_at, scope_id)
SELECT 'Escola Pau Casals', '08007890', 'Badalona', false, NULL, NOW(),
       (SELECT id FROM public.scope_mnt WHERE name = 'Infantil' LIMIT 1)
WHERE NOT EXISTS (SELECT 1 FROM public.schools WHERE code = '08007890');

INSERT INTO public.schools (name, code, city, is_favorite, scope, created_at, scope_id)
SELECT 'Institut Carles Riba', '08014567', 'Vilanova i la Geltru', false, NULL, NOW(),
       (SELECT id FROM public.scope_mnt WHERE name = 'Secundaria' LIMIT 1)
WHERE NOT EXISTS (SELECT 1 FROM public.schools WHERE code = '08014567');

INSERT INTO public.schools (name, code, city, is_favorite, scope, created_at, scope_id)
SELECT 'Centre FP Catalunya', '08020123', 'Rubi', true, NULL, NOW(),
       (SELECT id FROM public.scope_mnt WHERE name = 'FP' LIMIT 1)
WHERE NOT EXISTS (SELECT 1 FROM public.schools WHERE code = '08020123');

-- Student users
INSERT INTO public.users
    (first_name, last_name, email, password_hash, role, birth_date, is_active, created_at, updated_at, last_login_at)
SELECT
    'Anna', 'Serra', 'anna.serra@example.com',
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'USER', DATE '2009-03-12', true, NOW(), NOW(), NULL
WHERE NOT EXISTS (SELECT 1 FROM public.users WHERE email = 'anna.serra@example.com');

INSERT INTO public.users
    (first_name, last_name, email, password_hash, role, birth_date, is_active, created_at, updated_at, last_login_at)
SELECT
    'Marc', 'Vila', 'marc.vila@example.com',
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'USER', DATE '2010-11-02', true, NOW(), NOW(), NULL
WHERE NOT EXISTS (SELECT 1 FROM public.users WHERE email = 'marc.vila@example.com');

-- Students
INSERT INTO public.students (school_id, user_id, created_at)
SELECT
    (SELECT id FROM public.schools WHERE code = '08010123' LIMIT 1),
    (SELECT id FROM public.users WHERE email = 'anna.serra@example.com' LIMIT 1),
    NOW()
WHERE NOT EXISTS (
    SELECT 1
    FROM public.students s
    JOIN public.users u ON u.id = s.user_id
    WHERE u.email = 'anna.serra@example.com'
);

INSERT INTO public.students (school_id, user_id, created_at)
SELECT
    (SELECT id FROM public.schools WHERE code = '08014567' LIMIT 1),
    (SELECT id FROM public.users WHERE email = 'marc.vila@example.com' LIMIT 1),
    NOW()
WHERE NOT EXISTS (
    SELECT 1
    FROM public.students s
    JOIN public.users u ON u.id = s.user_id
    WHERE u.email = 'marc.vila@example.com'
);

-- Enrollments
INSERT INTO public.enrollments (student_id, academic_year, course_name, status, enrolled_at)
SELECT
    (SELECT s.id
     FROM public.students s
     JOIN public.users u ON u.id = s.user_id
     WHERE u.email = 'anna.serra@example.com'
     LIMIT 1),
    '2025-2026',
    '6e Primaria',
    'Activa',
    NOW()
WHERE NOT EXISTS (
    SELECT 1
    FROM public.enrollments e
    JOIN public.students s ON s.id = e.student_id
    JOIN public.users u ON u.id = s.user_id
    WHERE u.email = 'anna.serra@example.com'
      AND e.academic_year = '2025-2026'
);

INSERT INTO public.enrollments (student_id, academic_year, course_name, status, enrolled_at)
SELECT
    (SELECT s.id
     FROM public.students s
     JOIN public.users u ON u.id = s.user_id
     WHERE u.email = 'marc.vila@example.com'
     LIMIT 1),
    '2025-2026',
    '4t ESO',
    'Activa',
    NOW()
WHERE NOT EXISTS (
    SELECT 1
    FROM public.enrollments e
    JOIN public.students s ON s.id = e.student_id
    JOIN public.users u ON u.id = s.user_id
    WHERE u.email = 'marc.vila@example.com'
      AND e.academic_year = '2025-2026'
);

-- Annual fees
INSERT INTO public.annual_fees (enrollment_id, amount, currency, due_date, paid_at, payment_ref)
SELECT
    (SELECT e.id
     FROM public.enrollments e
     JOIN public.students s ON s.id = e.student_id
     JOIN public.users u ON u.id = s.user_id
     WHERE u.email = 'anna.serra@example.com'
       AND e.academic_year = '2025-2026'
     LIMIT 1),
    150.00,
    'EUR',
    DATE '2026-06-30',
    NULL,
    'FEE-ANNA-2025'
WHERE NOT EXISTS (
    SELECT 1
    FROM public.annual_fees af
    WHERE af.payment_ref = 'FEE-ANNA-2025'
);

INSERT INTO public.annual_fees (enrollment_id, amount, currency, due_date, paid_at, payment_ref)
SELECT
    (SELECT e.id
     FROM public.enrollments e
     JOIN public.students s ON s.id = e.student_id
     JOIN public.users u ON u.id = s.user_id
     WHERE u.email = 'marc.vila@example.com'
       AND e.academic_year = '2025-2026'
     LIMIT 1),
    175.00,
    'EUR',
    DATE '2026-06-30',
    NOW(),
    'FEE-MARC-2025'
WHERE NOT EXISTS (
    SELECT 1
    FROM public.annual_fees af
    WHERE af.payment_ref = 'FEE-MARC-2025'
);

COMMIT;
