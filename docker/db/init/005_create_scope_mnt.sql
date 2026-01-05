-- Migration: Create SCOPE_MNT maintenance table
-- Date: 2026-01-04
-- Description: Creates maintenance table for school scopes (education levels)

-- Create SCOPE_MNT table
CREATE TABLE IF NOT EXISTS scope_mnt (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(255),
    is_active BOOLEAN DEFAULT true NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL
);

-- Add indexes
CREATE INDEX IF NOT EXISTS idx_scope_mnt_name ON scope_mnt(name);
CREATE INDEX IF NOT EXISTS idx_scope_mnt_active ON scope_mnt(is_active);

-- Add comments
COMMENT ON TABLE scope_mnt IS 'Taula de manteniment dels àmbits/nivells educatius de les escoles';
COMMENT ON COLUMN scope_mnt.id IS 'Identificador únic de l''àmbit';
COMMENT ON COLUMN scope_mnt.name IS 'Nom de l''àmbit educatiu';
COMMENT ON COLUMN scope_mnt.description IS 'Descripció de l''àmbit';
COMMENT ON COLUMN scope_mnt.is_active IS 'Indica si l''àmbit està actiu';
COMMENT ON COLUMN scope_mnt.created_at IS 'Data de creació del registre';
COMMENT ON COLUMN scope_mnt.updated_at IS 'Data d''última actualització';

-- Insert distinct scopes from schools table
INSERT INTO scope_mnt (name, description, is_active)
SELECT DISTINCT 
    scope as name,
    'Nivell educatiu: ' || scope as description,
    true as is_active
FROM schools 
WHERE scope IS NOT NULL AND scope != ''
ON CONFLICT (name) DO NOTHING;

-- Add additional common scopes if they don't exist
INSERT INTO scope_mnt (name, description, is_active)
VALUES 
    ('Infantil', 'Educació Infantil (0-6 anys)', true),
    ('Primària', 'Educació Primària (6-12 anys)', true),
    ('Infantil i Primària', 'Educació Infantil i Primària', true),
    ('Secundària', 'Educació Secundària Obligatòria (ESO)', true),
    ('Batxillerat', 'Batxillerat', true),
    ('FP', 'Formació Professional', true),
    ('Adults', 'Educació d''Adults', true)
ON CONFLICT (name) DO NOTHING;

-- Add trigger to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_scope_mnt_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_scope_mnt_updated_at
    BEFORE UPDATE ON scope_mnt
    FOR EACH ROW
    EXECUTE FUNCTION update_scope_mnt_updated_at();

-- Display inserted records
SELECT id, name, description, is_active FROM scope_mnt ORDER BY name;
