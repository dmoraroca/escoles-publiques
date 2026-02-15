# Captures de Pantalla (Manual d'Usuari)

Aquest directori conté captures de pantalla per al manual d'usuari.

## Regles (proteccio de dades)
- **PROHIBIT** pujar captures amb dades personals reals (noms, emails reals, identificadors, etc.).
- Utilitza **dades de prova** i, si cal, **difumina/pixel·la** qualsevol dada sensible.
- No incloguis tokens, secrets, cookies, URL amb credencials, etc.
- Evita captures amb el **gestor de contrasenyes del navegador** desplegat (sovint mostra emails reals).
- Si apareix qualsevol dada sensible, repeteix la captura o difumina-la abans de guardar-la.

## Estructura
- `admin/` captures del rol administrador (`ADM`)
- `user/` captures del rol usuari final (`USER`)

## Convencio de noms
Fes servir un prefix numeric per mantenir l'ordre (img1..imgN) i sufix d'idioma:

`<rol>/<NN>-<slug>-<lang>.png`

Exemples:
- `admin/01-login-ca.png`
- `admin/01-login-es.png`
- `admin/01-login-en.png`
- `admin/01-login-de.png`
- `admin/10-escoles-llistat-ca.png`
- `admin/20-quotes-nova-modal-en.png`

Idiomes:
- `ca`, `es`, `en`, `de`

## Llista recomanada (ADM)
Login / inici / cerca:
1. `admin/01-login-<lang>.png`
2. `admin/02-inici-<lang>.png`
3. `admin/03-cerca-resultats-<lang>.png` (resultats de cerca per ambit o text)

Escoles:
1. `admin/10-escoles-llistat-<lang>.png`
2. `admin/11-escoles-nova-modal-<lang>.png`
3. `admin/12-escoles-detall-<lang>.png`
4. `admin/13-escoles-editar-<lang>.png`
5. `admin/14-escoles-esborrar-confirm-<lang>.png` (dialog del navegador)

Alumnes:
1. `admin/20-alumnes-llistat-<lang>.png`
2. `admin/21-alumnes-nou-modal-<lang>.png`
3. `admin/22-alumnes-detall-<lang>.png`
4. `admin/23-alumnes-editar-<lang>.png`
5. `admin/24-alumnes-esborrar-confirm-<lang>.png` (dialog del navegador)

Inscripcions:
1. `admin/30-inscripcions-llistat-<lang>.png`
2. `admin/31-inscripcions-nova-modal-<lang>.png`
3. `admin/32-inscripcions-detall-<lang>.png`
4. `admin/33-inscripcions-editar-<lang>.png`
5. `admin/34-inscripcions-esborrar-confirm-<lang>.png` (dialog del navegador)

Quotes anuals:
1. `admin/40-quotes-llistat-<lang>.png`
2. `admin/41-quotes-nova-modal-<lang>.png`
3. `admin/42-quotes-detall-<lang>.png`
4. `admin/43-quotes-editar-<lang>.png`
5. `admin/44-quotes-esborrar-confirm-<lang>.png` (dialog del navegador)

## Llista recomanada (USER)
1. `user/01-login-<lang>.png` (opcional, si el login es diferent/mostra un flux diferent)
2. `user/02-dashboard-<lang>.png`

## Nota
Si canvies o afegeixes captures, mantingues el mateix `NN-` per no trencar referencies dels manuals.
