-- Neteja de totes les taules

-- Neteja de totes les taules
TRUNCATE annual_fees, enrollments, students, users, schools RESTART IDENTITY CASCADE;

-- Inserció de 20 escoles
DO $$
DECLARE
	school_id integer;
	user_id integer;
	student_id integer;
	enrollment_id integer;
	admin_id integer;
BEGIN
	-- 20 escoles amb code
	FOR i IN 1..20 LOOP
		INSERT INTO schools (name, code) VALUES ('Escola Pública ' || i, 'ESC' || i) RETURNING id INTO school_id;
		-- 1 usuari per escola
		IF i = 1 THEN
			   INSERT INTO users (email, password_hash, first_name, last_name, role, is_active, created_at, updated_at)
			   VALUES ('admin@escola.cat', encode(digest('admin123', 'sha256'), 'hex'), 'Admin', 'User', 'ADM', true, NOW(), NOW()) RETURNING id INTO admin_id;
			INSERT INTO students (user_id, school_id) VALUES (admin_id, school_id) RETURNING id INTO student_id;
		ELSE
			INSERT INTO users (email, password_hash, first_name, last_name, role, is_active, created_at, updated_at)
			VALUES ('user' || i || '@escola.cat', encode(digest('user123', 'sha256'), 'hex'), 'Nom' || i, 'Cognom' || i, 'USER', true, NOW(), NOW()) RETURNING id INTO user_id;
			INSERT INTO students (user_id, school_id) VALUES (user_id, school_id) RETURNING id INTO student_id;
		END IF;
		INSERT INTO enrollments (student_id, academic_year) VALUES (student_id, '2025-2026') RETURNING id INTO enrollment_id;
		IF i = 2 THEN
			-- Quotes per user2@escola.cat
			INSERT INTO annual_fees (enrollment_id, student_id, amount, currency, due_date, paid_at, payment_ref)
			VALUES (enrollment_id, student_id, 120, 'EUR', TO_DATE('2026-01-10', 'YYYY-MM-DD'), NOW(), 'REF201');
			INSERT INTO annual_fees (enrollment_id, student_id, amount, currency, due_date, paid_at, payment_ref)
			VALUES (enrollment_id, student_id, 130, 'EUR', TO_DATE('2026-02-10', 'YYYY-MM-DD'), NOW(), 'REF202');
			INSERT INTO annual_fees (enrollment_id, student_id, amount, currency, due_date, paid_at, payment_ref)
			VALUES (enrollment_id, student_id, 140, 'EUR', TO_DATE('2026-03-10', 'YYYY-MM-DD'), NULL, 'REF203');
		ELSE
			INSERT INTO annual_fees (enrollment_id, student_id, amount, currency, due_date, paid_at, payment_ref)
			VALUES (enrollment_id, student_id, 100 + i * 10, 'EUR', TO_DATE('2026-01-' || LPAD(i::text,2,'0'), 'YYYY-MM-DD'), NULL, 'REF' || LPAD(i::text,3,'0'));
		END IF;
	END LOOP;
END$$;
