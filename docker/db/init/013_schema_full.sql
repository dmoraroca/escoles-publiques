TRUNCATE TABLE annual_fees RESTART IDENTITY CASCADE;
TRUNCATE TABLE enrollments RESTART IDENTITY CASCADE;
TRUNCATE TABLE students RESTART IDENTITY CASCADE;
TRUNCATE TABLE users RESTART IDENTITY CASCADE;
TRUNCATE TABLE schools RESTART IDENTITY CASCADE;
TRUNCATE TABLE scope_mnt RESTART IDENTITY CASCADE;

-- 013_schema_full.sql
-- Script únic per crear tota l'estructura de la base de dades escoles

-- 1. Taula scope_mnt
CREATE TABLE IF NOT EXISTS scope_mnt (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(255),
    is_active BOOLEAN DEFAULT true NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_scope_mnt_name ON scope_mnt(name);
CREATE INDEX IF NOT EXISTS idx_scope_mnt_active ON scope_mnt(is_active);

-- 2. Taula schools
CREATE TABLE IF NOT EXISTS schools (
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    code TEXT NOT NULL UNIQUE,
    city TEXT,
    is_favorite BOOLEAN NOT NULL DEFAULT false,
    scope_id BIGINT REFERENCES scope_mnt(id) ON DELETE RESTRICT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS ix_schools_scope_id ON schools(scope_id);

-- 3. Taula users
CREATE TABLE IF NOT EXISTS users (
    id BIGSERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(64) NOT NULL,
    role VARCHAR(10) NOT NULL CHECK (role IN ('ADM', 'USER')),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);

-- 4. Taula students
CREATE TABLE IF NOT EXISTS students (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT REFERENCES users(id) ON DELETE SET NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS ix_students_school_id ON students(school_id);
CREATE INDEX IF NOT EXISTS idx_students_user_id ON students(user_id);

-- 5. Taula enrollments
CREATE TABLE IF NOT EXISTS enrollments (
    id BIGSERIAL PRIMARY KEY,
    student_id BIGINT NOT NULL REFERENCES students(id) ON DELETE CASCADE,
    academic_year TEXT NOT NULL,
    course_name VARCHAR(50),
    status TEXT NOT NULL DEFAULT 'Active',
    enrolled_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    school_id BIGINT,
    UNIQUE (student_id, academic_year)
);
CREATE INDEX IF NOT EXISTS ix_enrollments_student_id ON enrollments(student_id);
CREATE INDEX IF NOT EXISTS idx_enrollments_course_name ON enrollments(course_name);

-- 6. Taula annual_fees
CREATE TABLE IF NOT EXISTS annual_fees (
    id BIGSERIAL PRIMARY KEY,
    enrollment_id BIGINT NOT NULL REFERENCES enrollments(id) ON DELETE CASCADE,
    amount NUMERIC(12,2) NOT NULL,
    currency CHAR(3) NOT NULL DEFAULT 'EUR',
    due_date DATE NOT NULL,
    paid_at TIMESTAMPTZ,
    payment_ref TEXT
);
CREATE INDEX IF NOT EXISTS ix_annual_fees_enrollment_id ON annual_fees(enrollment_id);

-- Extensions
CREATE EXTENSION IF NOT EXISTS pgcrypto;
