-- Crear taula d'usuaris per autenticació
CREATE TABLE IF NOT EXISTS users (
    id BIGSERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(64) NOT NULL, -- SHA256 genera 64 caràcters hexadecimals
    role VARCHAR(10) NOT NULL CHECK (role IN ('ADM', 'USER')),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);

-- Inserir usuari administrador
-- Password: "admin123" en SHA256 = 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9
INSERT INTO users (first_name, last_name, email, password_hash, role) VALUES
('Administrador', 'Sistema', 'admin@admin.adm', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'ADM');

-- Afegir camp user_id a la taula students per relacionar alumnes amb usuaris
ALTER TABLE students ADD COLUMN IF NOT EXISTS user_id BIGINT;
ALTER TABLE students ADD CONSTRAINT fk_students_users FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL;
CREATE INDEX IF NOT EXISTS idx_students_user_id ON students(user_id);

-- Crear alguns usuaris de prova i relacionar-los amb alumnes existents
-- Password per tots els usuaris: "user123" en SHA256 = 6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090
INSERT INTO users (first_name, last_name, email, password_hash, role) VALUES
('Marc', 'García López', 'marc.garcia@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER'),
('Laura', 'Martínez Ferrer', 'laura.martinez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER'),
('David', 'Rodríguez Pons', 'david.rodriguez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER'),
('Anna', 'Sánchez Vila', 'anna.sanchez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER'),
('Jordi', 'Fernández Tort', 'jordi.fernandez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER');

-- Relacionar usuaris amb alumnes (assumint que els IDs dels students són 1-5)
UPDATE students SET user_id = 2 WHERE id = 1; -- Marc
UPDATE students SET user_id = 3 WHERE id = 2; -- Laura
UPDATE students SET user_id = 4 WHERE id = 3; -- David
UPDATE students SET user_id = 5 WHERE id = 4; -- Anna
UPDATE students SET user_id = 6 WHERE id = 5; -- Jordi
