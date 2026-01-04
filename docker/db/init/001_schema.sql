-- 1) Escoles
create table if not exists schools (
  id            bigserial primary key,
  name          text not null,
  code          text not null unique,
  city          text null,
  created_at    timestamptz not null default now()
);

-- 2) Alumnat
create table if not exists students (
  id            bigserial primary key,
  school_id     bigint not null references schools(id) on delete restrict,
  first_name    text not null,
  last_name     text not null,
  birth_date    date null,
  email         text null,
  created_at    timestamptz not null default now()
);

create index if not exists ix_students_school_id on students(school_id);

-- 3) Inscripcions (per curs acadèmic)
create table if not exists enrollments (
  id              bigserial primary key,
  student_id      bigint not null references students(id) on delete cascade,
  academic_year   text not null,                 -- ex: '2025-2026'
  status          text not null default 'Active', -- Active / Cancelled / Finished
  enrolled_at     timestamptz not null default now(),
  unique (student_id, academic_year)
);

create index if not exists ix_enrollments_student_id on enrollments(student_id);
als (mon
-- 4) Quotes anuetari) 
create table if not exists annual_fees (
  id              bigserial primary key,
  enrollment_id   bigint not null references enrollments(id) on delete cascade,
  amount          numeric(12,2) not null,
  currency        char(3) not null default 'EUR',
  due_date        date not null,
  paid_at         timestamptz null,
  payment_ref     text null
);

create index if not exists ix_annual_fees_enrollment_id on annual_fees(enrollment_id);


CREATE EXTENSION IF NOT EXISTS pgcrypto;