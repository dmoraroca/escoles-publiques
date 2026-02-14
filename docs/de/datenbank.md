# Datenbank (DE)

## 1. ER-Diagramm
Platzhalter:
- `docs/assets/er-de.png`

Beziehungen:
- `schools (1) -> (N) students`
- `users (1) -> (0..1) students`
- `students (1) -> (N) enrollments`
- `enrollments (1) -> (N) annual_fees`
- `scope_mnt (1) -> (N) schools` (optional via `scope_id`)

## 2. Tabellen (Kurz)
- `schools`: Stammdaten der Schule (optional `scope_id`, legacy `scope`)
- `scope_mnt`: Bereiche/Scopes
- `users`: Login-Benutzer (email unique)
- `students`: Student + Schule + optional User
- `enrollments`: Einschreibungen (inkl. `school_id`)
- `annual_fees`: Jahresgebuhren pro Einschreibung
- `__EFMigrationsHistory`: EF Core intern

## 3. Seed / Demo-Daten
- Startup Seed (API): Scopes + Admin Benutzer wenn DB leer ist
- SQL Seed: `sql/001_seed_render.sql`
