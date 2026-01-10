-- Dades de prova per escoles
INSERT INTO schools (name) VALUES ('Escola Pública 1'), ('Escola Pública 2');

-- Dades de prova per usuaris
INSERT INTO users (username, password, first_name, last_name, role) VALUES
('admin', 'adminpass', 'Admin', 'User', 'ADMIN'),
('user1', 'userpass', 'Joan', 'Garcia', 'USER'),
('user2', 'userpass', 'Maria', 'López', 'USER');

-- Dades de prova per alumnes
INSERT INTO students (user_id, school_id) VALUES (2, 1), (3, 2);

-- Dades de prova per inscripcions
INSERT INTO enrollments (student_id, academic_year) VALUES (1, '2025-2026'), (2, '2025-2026');

-- Dades de prova per quotes anuals
INSERT INTO annual_fees (enrollment_id, student_id, amount, currency, due_date, paid_at, payment_ref) VALUES
(1, 1, 150.00, 'EUR', '2026-01-15', NULL, 'REF001'),
(2, 2, 200.00, 'EUR', '2026-02-10', '2026-02-15', 'REF002');
