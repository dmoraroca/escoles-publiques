-- 008_add_student_to_annual_fees.sql
ALTER TABLE annual_fees ADD COLUMN IF NOT EXISTS student_id BIGINT;
ALTER TABLE annual_fees ADD CONSTRAINT fk_annual_fees_student FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE CASCADE;
CREATE INDEX IF NOT EXISTS ix_annual_fees_student_id ON annual_fees(student_id);