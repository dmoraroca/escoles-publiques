-- 013_update_birthdates.sql
-- Script template to update users.birth_date for students.
-- WARNING: This file contains placeholder dates; replace 'YYYY-MM-DD' with real dates
-- before executing. The UPDATE lines are commented out by default for safety.

-- Preview current values for the affected users:
SELECT s.id AS student_id, s.user_id, u.birth_date
FROM students s
LEFT JOIN users u ON s.user_id = u.id
WHERE s.user_id IN (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,22)
ORDER BY s.id;

-- Begin a transaction when ready to apply changes
-- BEGIN;
-- Replace the 'YYYY-MM-DD' below with the correct date for each user
-- then UNCOMMENT the UPDATE lines (remove the leading '--') and run the script.
-- Example:
-- UPDATE users SET birth_date = '2008-05-12' WHERE id = 3;
-- Template updates for students' users (fill dates):
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 1;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 2;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 3;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 4;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 5;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 6;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 7;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 8;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 9;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 10;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 11;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 12;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 13;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 14;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 15;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 16;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 17;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 18;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 19;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 20;
-- UPDATE users SET birth_date = 'YYYY-MM-DD' WHERE id = 22;
-- When done uncomment the BEGIN/COMMIT and UPDATE lines, then run.
-- COMMIT;

-- OPTION: assignar dates aleatòries (proves).
-- Aquesta opció assigna per cada usuari una data aleatòria dins un rang (2008-01-01 .. 2012-12-31).
-- També és segura dins d'una transacció i mostra els canvis al final.

BEGIN;

-- Single UPDATE: set a random birth_date for users linked to students
-- only where birth_date IS NULL. Random date between 2008-01-01 and 2012-12-31.
UPDATE users
SET birth_date = (date '2008-01-01' + (floor(random() * 1825)::int) * INTERVAL '1 day')::date
WHERE id IN (SELECT user_id FROM students WHERE user_id IS NOT NULL)
	AND birth_date IS NULL;

-- Verify results for students' users
SELECT s.id AS student_id, s.user_id, u.birth_date
FROM students s
LEFT JOIN users u ON s.user_id = u.id
ORDER BY s.id;

COMMIT;

-- Optional: a safe fallback to preview changes before commit
-- SELECT id, birth_date FROM users WHERE id IN (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,22);
