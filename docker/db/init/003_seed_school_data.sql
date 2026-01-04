-- Update some existing schools with test data for IsFavorite and Scope
-- Date: 2026-01-04

-- Mark first 3 schools as favorites
UPDATE schools 
SET is_favorite = true 
WHERE id IN (SELECT id FROM schools ORDER BY id LIMIT 3);

-- Set scopes for schools (examples based on typical Spanish education system)
UPDATE schools 
SET scope = 'Infantil i Primària'
WHERE id IN (SELECT id FROM schools ORDER BY id LIMIT 10 OFFSET 0);

UPDATE schools 
SET scope = 'Secundària i Batxillerat'
WHERE id IN (SELECT id FROM schools ORDER BY id LIMIT 10 OFFSET 10);

UPDATE schools 
SET scope = 'Formació Professional'
WHERE id IN (SELECT id FROM schools ORDER BY id LIMIT 5 OFFSET 20);

-- Show results
SELECT id, code, name, is_favorite, scope 
FROM schools 
ORDER BY id 
LIMIT 10;
