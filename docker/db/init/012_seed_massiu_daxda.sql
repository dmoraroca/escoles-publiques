-- 012_seed_massiu_daxda.sql
-- Neteja de totes les taules (ordre correcte per FK)
TRUNCATE TABLE annual_fees RESTART IDENTITY CASCADE;
TRUNCATE TABLE enrollments RESTART IDENTITY CASCADE;
TRUNCATE TABLE students RESTART IDENTITY CASCADE;
TRUNCATE TABLE users RESTART IDENTITY CASCADE;
TRUNCATE TABLE schools RESTART IDENTITY CASCADE;
TRUNCATE TABLE scope_mnt RESTART IDENTITY CASCADE;

-- 1. Omplir scope_mnt
INSERT INTO scope_mnt (name, description, is_active, created_at, updated_at) VALUES
  ('Infantil', 'Educació Infantil (0-6 anys)', true, NOW(), NOW()),
  ('Primària', 'Educació Primària (6-12 anys)', true, NOW(), NOW()),
  ('Infantil i Primària', 'Educació Infantil i Primària', true, NOW(), NOW()),
  ('Secundària', 'Educació Secundària Obligatòria (ESO)', true, NOW(), NOW()),
  ('Batxillerat', 'Batxillerat', true, NOW(), NOW()),
  ('FP', 'Formació Professional', true, NOW(), NOW()),
  ('Adults', 'Educació d''Adults', true, NOW(), NOW());

-- 2. Omplir schools (20 escoles)
-- Primer, obtenir els IDs de scope_mnt per nom
-- (Aquesta part s'ha de fer manualment si no es coneixen els IDs, o es pot fer amb subqueries si la BBDD ho permet)
-- Per simplicitat, assignem els IDs segons l'ordre d'inserció anterior:
-- 1: Infantil, 2: Primària, 3: Infantil i Primària, 4: Secundària, 5: Batxillerat, 6: FP, 7: Adults
INSERT INTO schools (name, code, city, is_favorite, scope_id, created_at) VALUES
  ('Institut Joan XXIII', '08001234', 'Barcelona', true, 4, NOW() - INTERVAL '2 years'),
  ('Escola Montserrat', '08002345', 'Girona', true, 3, NOW() - INTERVAL '18 months'),
  ('Col·legi Sant Jordi', '08003456', 'Lleida', false, 3, NOW() - INTERVAL '3 years'),
  ('Institut Ramon Llull', '08004567', 'Tarragona', true, 4, NOW() - INTERVAL '1 year'),
  ('Escola Pompeu Fabra', '08005678', 'Sabadell', false, 2, NOW() - INTERVAL '2 years'),
  ('Institut Jaume I', '08006789', 'Terrassa', false, 4, NOW() - INTERVAL '4 years'),
  ('Escola Pau Casals', '08007890', 'Badalona', true, 1, NOW() - INTERVAL '1 year'),
  ('Col·legi Verdaguer', '08008901', 'Mataró', false, 3, NOW() - INTERVAL '3 years'),
  ('Institut Prat de la Riba', '08009012', 'Reus', false, 5, NOW() - INTERVAL '2 years'),
  ('Escola Joan Miró', '08010123', 'Vic', true, 2, NOW() - INTERVAL '18 months'),
  ('Institut Salvador Dalí', '08011234', 'Figueres', false, 4, NOW() - INTERVAL '2 years'),
  ('Escola Antoni Gaudí', '08012345', 'Manresa', false, 2, NOW() - INTERVAL '1 year'),
  ('Col·legi Josep Pla', '08013456', 'Palafrugell', false, 6, NOW() - INTERVAL '3 years'),
  ('Institut Carles Riba', '08014567', 'Vilanova i la Geltrú', false, 4, NOW() - INTERVAL '2 years'),
  ('Escola Maria Mercè Marçal', '08015678', 'Igualada', true, 1, NOW() - INTERVAL '1 year'),
  ('Institut Pere Calders', '08016789', 'Sant Cugat', false, 5, NOW() - INTERVAL '4 years'),
  ('Escola Joan Maragall', '08017890', 'Granollers', false, 2, NOW() - INTERVAL '2 years'),
  ('Col·legi Mercè Rodoreda', '08018901', 'Sant Feliu de Llobregat', false, 3, NOW() - INTERVAL '18 months'),
  ('Institut Eugeni d''Ors', '08019012', 'Mollet del Vallès', false, 4, NOW() - INTERVAL '3 years'),
  ('Escola Jacint Verdaguer', '08020123', 'Rubí', false, 6, NOW() - INTERVAL '1 year');

-- 3. Omplir users (20 usuaris, 1 per escola)
INSERT INTO users (first_name, last_name, email, password_hash, role, is_active, created_at, updated_at) VALUES
  ('Admin', 'Sistema', 'admin@admin.adm', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'ADM', true, NOW(), NOW()),
  ('David', 'Mora', 'dmoraroca@gmail.com', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Marc', 'García López', 'marc.garcia@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Laura', 'Martínez Ferrer', 'laura.martinez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('David', 'Rodríguez Pons', 'david.rodriguez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Anna', 'Sánchez Vila', 'anna.sanchez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Jordi', 'Fernández Tort', 'jordi.fernandez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Maria', 'López Cunill', 'maria.lopez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Pau', 'Pérez Masó', 'pau.perez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Clara', 'Gómez Serra', 'clara.gomez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Albert', 'Ruiz Casas', 'albert.ruiz@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Marta', 'Díaz Pont', 'marta.diaz@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Roger', 'Moreno Bosch', 'roger.moreno@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Elena', 'Jiménez Font', 'elena.jimenez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Nil', 'Álvarez Mir', 'nil.alvarez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Núria', 'Romero Puig', 'nuria.romero@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Arnau', 'Torres Valls', 'arnau.torres@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Carla', 'Ramírez Pla', 'carla.ramirez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Martí', 'Gil Mas', 'marti.gil@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Sofia', 'Navarro Soler', 'sofia.navarro@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Oriol', 'Domínguez Roca', 'oriol.dominguez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW()),
  ('Emma', 'Vázquez Camps', 'emma.vazquez@email.cat', '6ca13d52ca70c883e0f0bb101e425a89e8624de51db2d2392593af6a84118090', 'USER', true, NOW(), NOW());

-- 4. Omplir students (id, school_id, user_id, created_at)
INSERT INTO students (school_id, user_id, created_at) VALUES
  (1, 1, NOW() - INTERVAL '1 year'),
  (1, 2, NOW() - INTERVAL '1 year'),
  (2, 3, NOW() - INTERVAL '8 months'),
  (2, 4, NOW() - INTERVAL '9 months'),
  (3, 5, NOW() - INTERVAL '2 years'),
  (3, 6, NOW() - INTERVAL '18 months'),
  (4, 7, NOW() - INTERVAL '1 year'),
  (4, 8, NOW() - INTERVAL '1 year'),
  (5, 9, NOW() - INTERVAL '6 months'),
  (5, 10, NOW() - INTERVAL '7 months'),
  (6, 11, NOW() - INTERVAL '2 years'),
  (6, 12, NOW() - INTERVAL '18 months'),
  (7, 13, NOW() - INTERVAL '1 year'),
  (8, 14, NOW() - INTERVAL '8 months'),
  (9, 15, NOW() - INTERVAL '2 years'),
  (10, 16, NOW() - INTERVAL '1 year'),
  (11, 17, NOW() - INTERVAL '9 months'),
  (12, 18, NOW() - INTERVAL '6 months'),
  (13, 19, NOW() - INTERVAL '2 years'),
  (14, 20, NOW() - INTERVAL '1 year');

  -- 5. Omplir enrollments (cada student té una matrícula per 2025-2026)
  INSERT INTO enrollments (student_id, academic_year, course_name, status, enrolled_at, school_id) VALUES
    (1, '2025-2026', '1r ESO', 'Active', NOW() - INTERVAL '4 months', 1),
    (2, '2025-2026', '1r ESO', 'Active', NOW() - INTERVAL '4 months', 1),
    (3, '2025-2026', '2n Primària', 'Active', NOW() - INTERVAL '3 months', 2),
    (4, '2025-2026', '3r Primària', 'Active', NOW() - INTERVAL '3 months', 2),
    (5, '2025-2026', '2n ESO', 'Active', NOW() - INTERVAL '5 months', 3),
    (6, '2025-2026', '2n ESO', 'Active', NOW() - INTERVAL '5 months', 3),
    (7, '2025-2026', '3r ESO', 'Active', NOW() - INTERVAL '4 months', 4),
    (8, '2025-2026', '3r ESO', 'Active', NOW() - INTERVAL '4 months', 4),
    (9, '2025-2026', '4t Primària', 'Active', NOW() - INTERVAL '2 months', 5),
    (10, '2025-2026', '5è Primària', 'Active', NOW() - INTERVAL '2 months', 5),
    (11, '2025-2026', '4t ESO', 'Active', NOW() - INTERVAL '6 months', 6),
    (12, '2025-2026', '4t ESO', 'Active', NOW() - INTERVAL '6 months', 6),
    (13, '2025-2026', '1r Batxillerat', 'Active', NOW() - INTERVAL '3 months', 7),
    (14, '2025-2026', '1r Batxillerat', 'Active', NOW() - INTERVAL '3 months', 8),
    (15, '2025-2026', '2n Batxillerat', 'Completed', NOW() - INTERVAL '5 months', 9),
    (16, '2025-2026', '6è Primària', 'Active', NOW() - INTERVAL '4 months', 10),
    (17, '2025-2026', '1r ESO', 'Active', NOW() - INTERVAL '3 months', 11),
    (18, '2025-2026', '2n ESO', 'Active', NOW() - INTERVAL '2 months', 12),
    (19, '2025-2026', '3r ESO', 'Active', NOW() - INTERVAL '5 months', 13),
    (20, '2025-2026', '2n Batxillerat', 'Active', NOW() - INTERVAL '4 months', 14);

  -- 6. Omplir annual_fees (una quota per matrícula)
  INSERT INTO annual_fees (enrollment_id, amount, currency, due_date, paid_at, payment_ref) VALUES
    (1, 850.00, 'EUR', '2026-09-30', '2026-09-15', 'PAY-2026-001'),
    (2, 850.00, 'EUR', '2026-09-30', '2026-09-20', 'PAY-2026-002'),
    (3, 750.00, 'EUR', '2026-10-31', NULL, NULL),
    (4, 750.00, 'EUR', '2026-10-31', '2026-10-25', 'PAY-2026-003'),
    (5, 800.00, 'EUR', '2026-09-30', '2026-09-10', 'PAY-2026-004'),
    (6, 800.00, 'EUR', '2026-09-30', NULL, NULL),
    (7, 900.00, 'EUR', '2026-09-30', '2026-09-18', 'PAY-2026-005'),
    (8, 900.00, 'EUR', '2026-09-30', '2026-09-22', 'PAY-2026-006'),
    (9, 700.00, 'EUR', '2026-11-30', NULL, NULL),
    (10, 700.00, 'EUR', '2026-11-30', NULL, NULL),
    (11, 950.00, 'EUR', '2026-09-30', '2026-09-12', 'PAY-2026-007'),
    (12, 950.00, 'EUR', '2026-09-30', '2026-09-16', 'PAY-2026-008'),
    (13, 720.00, 'EUR', '2026-10-31', NULL, NULL),
    (14, 680.00, 'EUR', '2026-10-31', '2026-10-20', 'PAY-2026-009'),
    (15, 1000.00, 'EUR', '2026-09-30', NULL, NULL),
    (16, 780.00, 'EUR', '2026-09-30', '2026-09-14', 'PAY-2026-010'),
    (17, 820.00, 'EUR', '2026-10-31', NULL, NULL),
    (18, 650.00, 'EUR', '2026-11-30', NULL, NULL),
    (19, 880.00, 'EUR', '2026-09-30', '2026-09-19', 'PAY-2026-011'),
    (20, 920.00, 'EUR', '2026-09-30', '2026-09-17', 'PAY-2026-012');

-- (continua: enrollments i annual_fees...)
