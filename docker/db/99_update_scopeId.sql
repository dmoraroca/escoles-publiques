-- Elimina la columna antiga 'scope' i afegeix la nova columna 'scopeId'
ALTER TABLE schools DROP COLUMN IF EXISTS scope;
ALTER TABLE schools ADD COLUMN scopeId bigint;

-- Omple la columna scopeId amb valors vàlids i aleatoris de la taula scope_mnt
UPDATE schools
SET scopeId = (
  SELECT id
  FROM scope_mnt
  ORDER BY random()
  LIMIT 1
);

-- Omple la columna city amb ciutats catalanes de manera cíclica
WITH cities AS (
  SELECT city, ROW_NUMBER() OVER () AS rn FROM (
    VALUES ('Barcelona'), ('Girona'), ('Lleida'), ('Tarragona'), ('Manresa'), ('Sabadell'), ('Terrassa'), ('Reus'), ('Mataró'), ('Granollers')
  ) AS c(city)
),
schools_to_update AS (
  SELECT id, ROW_NUMBER() OVER (ORDER BY id) AS rn FROM schools
),
assignments AS (
  SELECT s.id AS school_id, c.city
  FROM schools_to_update s
  JOIN cities c ON ((s.rn - 1) % (SELECT COUNT(*) FROM cities)) + 1 = c.rn
)
UPDATE schools
SET city = a.city
FROM assignments a
WHERE schools.id = a.school_id;

-- Marca 8 escoles com a favorites
UPDATE schools
SET is_favorite = true
WHERE id IN (
  SELECT id FROM schools ORDER BY id LIMIT 8
);
