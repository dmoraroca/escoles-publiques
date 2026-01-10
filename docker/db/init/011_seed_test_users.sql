-- Inserció d'usuaris de prova per a la taula users
INSERT INTO users (first_name, last_name, email, password_hash, role, birth_date, is_active, created_at, updated_at, last_login_at)
VALUES
  ('David', 'Moraro', 'dmoraroca@gmail.com', '$2a$12$EXEMPLEHASHUSER', 'USER', '1990-01-01', true, NOW(), NOW(), NULL),
  ('Admin', 'Escoles', 'admin@escoles.cat', '$2a$12$EXEMPLEHASHADMIN', 'ADM', '1980-01-01', true, NOW(), NOW(), NULL);

-- Pots substituir els valors de password_hash per un hash real de bcrypt si vols seguretat real.