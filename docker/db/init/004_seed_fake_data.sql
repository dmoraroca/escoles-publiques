-- Script per inserir dades de prova (fake data)
-- 20 registres per cada taula

-- Esborrar dades existents (respectant foreign keys)
DELETE FROM annual_fees;
DELETE FROM enrollments;
DELETE FROM students;
DELETE FROM schools;

-- Reset sequences
ALTER SEQUENCE schools_id_seq RESTART WITH 1;
ALTER SEQUENCE students_id_seq RESTART WITH 1;
ALTER SEQUENCE enrollments_id_seq RESTART WITH 1;
ALTER SEQUENCE annual_fees_id_seq RESTART WITH 1;

-- Inserir 20 escoles
INSERT INTO schools (name, code, city, is_favorite, scope, created_at) VALUES
('Institut Joan XXIII', '08001234', 'Barcelona', true, 'Secundària i Batxillerat', NOW() - INTERVAL '2 years'),
('Escola Montserrat', '08002345', 'Girona', true, 'Infantil i Primària', NOW() - INTERVAL '18 months'),
('Col·legi Sant Jordi', '08003456', 'Lleida', false, 'Infantil i Primària', NOW() - INTERVAL '3 years'),
('Institut Ramon Llull', '08004567', 'Tarragona', true, 'Secundària', NOW() - INTERVAL '1 year'),
('Escola Pompeu Fabra', '08005678', 'Sabadell', false, 'Primària', NOW() - INTERVAL '2 years'),
('Institut Jaume I', '08006789', 'Terrassa', false, 'Secundària i Batxillerat', NOW() - INTERVAL '4 years'),
('Escola Pau Casals', '08007890', 'Badalona', true, 'Infantil', NOW() - INTERVAL '1 year'),
('Col·legi Verdaguer', '08008901', 'Mataró', false, 'Infantil i Primària', NOW() - INTERVAL '3 years'),
('Institut Prat de la Riba', '08009012', 'Reus', false, 'Batxillerat', NOW() - INTERVAL '2 years'),
('Escola Joan Miró', '08010123', 'Vic', true, 'Primària', NOW() - INTERVAL '18 months'),
('Institut Salvador Dalí', '08011234', 'Figueres', false, 'Secundària', NOW() - INTERVAL '2 years'),
('Escola Antoni Gaudí', '08012345', 'Manresa', false, 'Infantil i Primària', NOW() - INTERVAL '1 year'),
('Col·legi Josep Pla', '08013456', 'Palafrugell', false, 'Primària', NOW() - INTERVAL '3 years'),
('Institut Carles Riba', '08014567', 'Vilanova i la Geltrú', false, 'Secundària i Batxillerat', NOW() - INTERVAL '2 years'),
('Escola Maria Mercè Marçal', '08015678', 'Igualada', true, 'Infantil', NOW() - INTERVAL '1 year'),
('Institut Pere Calders', '08016789', 'Sant Cugat', false, 'Batxillerat', NOW() - INTERVAL '4 years'),
('Escola Joan Maragall', '08017890', 'Granollers', false, 'Primària', NOW() - INTERVAL '2 years'),
('Col·legi Mercè Rodoreda', '08018901', 'Sant Feliu de Llobregat', false, 'Infantil i Primària', NOW() - INTERVAL '18 months'),
('Institut Eugeni d''Ors', '08019012', 'Mollet del Vallès', false, 'Secundària', NOW() - INTERVAL '3 years'),
('Escola Jacint Verdaguer', '08020123', 'Rubí', false, 'Formació Professional', NOW() - INTERVAL '1 year');

-- Inserir 20 alumnes
INSERT INTO students (first_name, last_name, email, birth_date, school_id, created_at) VALUES
('Marc', 'García López', 'marc.garcia@email.cat', '2008-03-15', 1, NOW() - INTERVAL '1 year'),
('Laura', 'Martínez Ferrer', 'laura.martinez@email.cat', '2007-09-22', 1, NOW() - INTERVAL '1 year'),
('David', 'Rodríguez Pons', 'david.rodriguez@email.cat', '2009-11-30', 2, NOW() - INTERVAL '8 months'),
('Anna', 'Sánchez Vila', 'anna.sanchez@email.cat', '2010-05-12', 2, NOW() - INTERVAL '9 months'),
('Jordi', 'Fernández Tort', 'jordi.fernandez@email.cat', '2008-07-08', 3, NOW() - INTERVAL '2 years'),
('Maria', 'López Cunill', 'maria.lopez@email.cat', '2009-01-19', 3, NOW() - INTERVAL '18 months'),
('Pau', 'Pérez Masó', 'pau.perez@email.cat', '2007-12-03', 4, NOW() - INTERVAL '1 year'),
('Clara', 'Gómez Serra', 'clara.gomez@email.cat', '2008-04-25', 4, NOW() - INTERVAL '1 year'),
('Albert', 'Ruiz Casas', 'albert.ruiz@email.cat', '2010-08-14', 5, NOW() - INTERVAL '6 months'),
('Marta', 'Díaz Pont', 'marta.diaz@email.cat', '2009-10-07', 5, NOW() - INTERVAL '7 months'),
('Roger', 'Moreno Bosch', 'roger.moreno@email.cat', '2007-06-18', 6, NOW() - INTERVAL '2 years'),
('Elena', 'Jiménez Font', 'elena.jimenez@email.cat', '2008-02-28', 6, NOW() - INTERVAL '18 months'),
('Nil', 'Álvarez Mir', 'nil.alvarez@email.cat', '2009-09-11', 7, NOW() - INTERVAL '1 year'),
('Núria', 'Romero Puig', 'nuria.romero@email.cat', '2010-03-05', 8, NOW() - INTERVAL '8 months'),
('Arnau', 'Torres Valls', 'arnau.torres@email.cat', '2007-11-23', 9, NOW() - INTERVAL '2 years'),
('Carla', 'Ramírez Pla', 'carla.ramirez@email.cat', '2008-08-16', 10, NOW() - INTERVAL '1 year'),
('Martí', 'Gil Mas', 'marti.gil@email.cat', '2009-05-29', 11, NOW() - INTERVAL '9 months'),
('Sofia', 'Navarro Soler', 'sofia.navarro@email.cat', '2010-01-12', 12, NOW() - INTERVAL '6 months'),
('Oriol', 'Domínguez Roca', 'oriol.dominguez@email.cat', '2007-04-20', 13, NOW() - INTERVAL '2 years'),
('Emma', 'Vázquez Camps', 'emma.vazquez@email.cat', '2008-12-09', 14, NOW() - INTERVAL '1 year');

-- Inserir 20 matrícules
INSERT INTO enrollments (student_id, academic_year, course_name, status, enrolled_at) VALUES
(1, '2024-2025', '1r ESO', 'Active', NOW() - INTERVAL '4 months'),
(2, '2024-2025', '1r ESO', 'Active', NOW() - INTERVAL '4 months'),
(3, '2024-2025', '2n Primària', 'Active', NOW() - INTERVAL '3 months'),
(4, '2024-2025', '3r Primària', 'Active', NOW() - INTERVAL '3 months'),
(5, '2024-2025', '2n ESO', 'Active', NOW() - INTERVAL '5 months'),
(6, '2024-2025', '2n ESO', 'Active', NOW() - INTERVAL '5 months'),
(7, '2024-2025', '3r ESO', 'Active', NOW() - INTERVAL '4 months'),
(8, '2024-2025', '3r ESO', 'Active', NOW() - INTERVAL '4 months'),
(9, '2024-2025', '4t Primària', 'Active', NOW() - INTERVAL '2 months'),
(10, '2024-2025', '5è Primària', 'Active', NOW() - INTERVAL '2 months'),
(11, '2024-2025', '4t ESO', 'Active', NOW() - INTERVAL '6 months'),
(12, '2024-2025', '4t ESO', 'Active', NOW() - INTERVAL '6 months'),
(13, '2024-2025', '1r Batxillerat', 'Active', NOW() - INTERVAL '3 months'),
(14, '2024-2025', '1r Batxillerat', 'Active', NOW() - INTERVAL '3 months'),
(15, '2024-2025', '2n Batxillerat', 'Completed', NOW() - INTERVAL '5 months'),
(16, '2024-2025', '6è Primària', 'Active', NOW() - INTERVAL '4 months'),
(17, '2024-2025', '1r ESO', 'Active', NOW() - INTERVAL '3 months'),
(18, '2024-2025', '2n ESO', 'Active', NOW() - INTERVAL '2 months'),
(19, '2024-2025', '3r ESO', 'Active', NOW() - INTERVAL '5 months'),
(20, '2024-2025', '2n Batxillerat', 'Active', NOW() - INTERVAL '4 months');

-- Inserir 20 quotes anuals
INSERT INTO annual_fees (enrollment_id, amount, currency, due_date, paid_at, payment_ref) VALUES
(1, 850.00, 'EUR', '2024-09-30', '2024-09-15', 'PAY-2024-001'),
(2, 850.00, 'EUR', '2024-09-30', '2024-09-20', 'PAY-2024-002'),
(3, 750.00, 'EUR', '2024-10-31', NULL, NULL),
(4, 750.00, 'EUR', '2024-10-31', '2024-10-25', 'PAY-2024-003'),
(5, 800.00, 'EUR', '2024-09-30', '2024-09-10', 'PAY-2024-004'),
(6, 800.00, 'EUR', '2024-09-30', NULL, NULL),
(7, 900.00, 'EUR', '2024-09-30', '2024-09-18', 'PAY-2024-005'),
(8, 900.00, 'EUR', '2024-09-30', '2024-09-22', 'PAY-2024-006'),
(9, 700.00, 'EUR', '2024-11-30', NULL, NULL),
(10, 700.00, 'EUR', '2024-11-30', NULL, NULL),
(11, 950.00, 'EUR', '2024-09-30', '2024-09-12', 'PAY-2024-007'),
(12, 950.00, 'EUR', '2024-09-30', '2024-09-16', 'PAY-2024-008'),
(13, 720.00, 'EUR', '2024-10-31', NULL, NULL),
(14, 680.00, 'EUR', '2024-10-31', '2024-10-20', 'PAY-2024-009'),
(15, 1000.00, 'EUR', '2024-09-30', NULL, NULL),
(16, 780.00, 'EUR', '2024-09-30', '2024-09-14', 'PAY-2024-010'),
(17, 820.00, 'EUR', '2024-10-31', NULL, NULL),
(18, 650.00, 'EUR', '2024-11-30', NULL, NULL),
(19, 880.00, 'EUR', '2024-09-30', '2024-09-19', 'PAY-2024-011'),
(20, 920.00, 'EUR', '2024-09-30', '2024-09-17', 'PAY-2024-012');

-- Verificar recomptes
SELECT 
    'schools' as taula, 
    COUNT(*) as total 
FROM schools
UNION ALL
SELECT 
    'students' as taula, 
    COUNT(*) as total 
FROM students
UNION ALL
SELECT 
    'enrollments' as taula, 
    COUNT(*) as total 
FROM enrollments
UNION ALL
SELECT 
    'annual_fees' as taula, 
    COUNT(*) as total 
FROM annual_fees;
