-- Migration: Add IsFavorite and Scope to schools table
-- Date: 2026-01-04
-- Description: Adds boolean field for favorite schools and scope field for school classification

-- Add IsFavorite column (default false)
ALTER TABLE schools 
ADD COLUMN IF NOT EXISTS is_favorite BOOLEAN DEFAULT false NOT NULL;

-- Add Scope column (nullable for existing records)
ALTER TABLE schools 
ADD COLUMN IF NOT EXISTS scope VARCHAR(100);

-- Add index for filtering favorite schools
CREATE INDEX IF NOT EXISTS idx_schools_is_favorite ON schools(is_favorite);

-- Add index for scope filtering
CREATE INDEX IF NOT EXISTS idx_schools_scope ON schools(scope);

-- Optional: Add comment to document the fields
COMMENT ON COLUMN schools.is_favorite IS 'Indica si l''escola és marcada com a favorita';
COMMENT ON COLUMN schools.scope IS 'Àmbit de l''escola (ex: Infantil, Primària, Secundària, Batxillerat)';
