-- Afegir birthdate a la taula users
ALTER TABLE users ADD COLUMN IF NOT EXISTS birth_date DATE;

-- Crear usuaris per tots els alumnes existents
-- Password per tots: "user123" = 6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090

-- Eliminar els usuaris de prova anteriors (excepte l'admin)
DELETE FROM users WHERE role = 'USER';

-- Inserir tots els alumnes com a usuaris
INSERT INTO users (first_name, last_name, email, birth_date, password_hash, role)
SELECT 
    first_name,
    last_name,
    COALESCE(email, LOWER(first_name || '.' || REPLACE(last_name, ' ', '.') || '@email.cat')),
    birth_date,
    '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090',
    'USER'
FROM students
WHERE NOT EXISTS (
    SELECT 1 FROM users WHERE users.email = COALESCE(students.email, LOWER(students.first_name || '.' || REPLACE(students.last_name, ' ', '.') || '@email.cat'))
);

-- Actualitzar la relació user_id a students
UPDATE students s
SET user_id = u.id
FROM users u
WHERE COALESCE(s.email, LOWER(s.first_name || '.' || REPLACE(s.last_name, ' ', '.') || '@email.cat')) = u.email;

-- Ara que tenim les dades copiades, eliminem les columnes de students
ALTER TABLE students DROP COLUMN IF EXISTS first_name;
ALTER TABLE students DROP COLUMN IF EXISTS last_name;
ALTER TABLE students DROP COLUMN IF EXISTS birth_date;
ALTER TABLE students DROP COLUMN IF EXISTS email;
