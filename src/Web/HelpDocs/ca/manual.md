# Manual d'usuari (CA)

## 1. Introduccio
Aquest manual explica l'ús de la web "Escoles Publiques".

A qui va dirigit:
- usuaris administradors (`ADM`)
- usuaris finals (`USER`) (accés limitat)

Requisits:
- navegador modern (Chrome/Firefox/Edge/Safari)
- credencials d'usuari
- connexió a Internet

## 1.1 Proteccio de dades (important)
- **No** incloguis mai dades personals reals (noms, emails reals, identificadors, etc.) en documentacio.
- Per a captures, utilitza **dades de prova** i/o **difumina/pixel·la** qualsevol dada sensible abans de compartir-la.
- Aquest repositori **no ha de contenir** captures de pantalla amb dades reals.
- Evita captures amb el **gestor de contrasenyes** del navegador desplegat (sovint mostra emails reals).

## 1.2 Idioma
La web permet canviar l'idioma des del selector superior. La selecció es manté entre pantalles.

## 2. Inici de sessio
1. A la pantalla de login, introdueix el correu i la contrasenya.
2. Prem "Entrar".
3. Si l'usuari es vàlid, s'accedeix a la pagina d'inici.

Captures:
![Login (CA)](/ajuda/codex_images_real/ca/login-admin.png)

## 3. Navegacio (rol ADM)
Menu principal:
- Inici
- Escoles
- Alumnes
- Inscripcions
- Quotes

Canvi d'idioma:
- desplega el selector d'idioma (a la barra superior fosca)
- tria l'idioma (la pagina es recarrega)

## 4. Inici i cerca
La pagina d'inici mostra un cercador i accessos rapids per ambit.

1. Escriu un text de cerca (opcional).
2. Selecciona un ambit (o fes clic sobre una targeta d'ambit).
3. Revisa els resultats i entra als detalls.

Captures:
![Inici (part superior) (CA)](/ajuda/codex_images_real/ca/inici-top.png)
![Inici (part inferior) (CA)](/ajuda/codex_images_real/ca/inici-down.png)

## 5. Gestio d'escoles
### 5.1 Llistar, cercar i ordenar
1. Ves a `Escoles`.
2. Usa el camp de cerca de la taula per filtrar.
3. Fes clic als encapçalaments (si son ordenables) per ordenar.

Captura:
![Escoles: llistat (CA)](/ajuda/codex_images_real/ca/escoles.png)

### 5.2 Crear escola
1. Prem el botó per crear nova escola.
2. Omple els camps obligatoris (codi i nom).
3. Desa.

Captures:
![Escoles: nova (modal) (CA)](/ajuda/codex_images_real/ca/escoles-crear.png)

### 5.3 Veure detalls
1. A la columna d'accions, prem `Detalls`.
2. Revisa la informacio i torna enrere.

Captura:
![Escoles: detall (CA)](/ajuda/codex_images_real/ca/escoles-detall.png)

### 5.4 Editar escola
1. Des de `Detalls`, prem `Editar` (o entra directament a l'edicio si existeix l'accio).
2. Modifica els camps necessaris.
3. Accepta les condicions de privacitat si es requerit.
4. Desa.

Captura:
![Escoles: editar (CA)](/ajuda/codex_images_real/ca/escoles-edit.png)

### 5.5 Eliminar escola
1. A la taula, prem `Esborrar`.
2. Confirma l'eliminacio al dialeg del navegador.

Captura:
![Escoles: esborrar (confirmacio) (CA)](/ajuda/codex_images_real/ca/escoles-esborrar.png)

## 6. Gestio d'alumnes
### 6.1 Llistar i cercar
1. Ves a `Alumnes`.
2. Usa el camp de cerca per filtrar.

Captura:
![Alumnes: llistat (CA)](/ajuda/codex_images_real/ca/estudiants.png)

### 6.2 Crear alumne
1. Ves a `Alumnes`.
2. Crea alumne amb nom, cognoms, email, data naixement (opcional) i escola.

Nota:
- si l'email ja existeix com a usuari, el sistema reutilitza l'usuari existent.

Captura:
![Alumnes: nou (modal) (CA)](/ajuda/codex_images_real/ca/estudiants-crear.png)

### 6.3 Veure detalls
1. A la columna d'accions, prem `Detalls`.

Captura:
![Alumnes: detall (CA)](/ajuda/codex_images_real/ca/estudiants-detall.png)

### 6.4 Editar alumne
1. Des de `Detalls`, prem `Editar`.
2. Modifica els camps necessaris.
3. Accepta les condicions de privacitat si es requerit.
4. Desa.

Captura:
![Alumnes: editar (CA)](/ajuda/codex_images_real/ca/estudiants-edit.png)

### 6.5 Eliminar alumne
1. A la taula, prem `Esborrar`.
2. Confirma al dialeg del navegador.

Captura:
![Alumnes: esborrar (confirmacio) (CA)](/ajuda/codex_images_real/ca/estudiants-esborrar.png)

## 7. Inscripcions
### 7.1 Llistar i cercar
1. Ves a `Inscripcions`.
2. Usa el camp de cerca per filtrar.

Captura:
![Inscripcions: llistat (CA)](/ajuda/codex_images_real/ca/inscripcions.png)

### 7.2 Crear inscripcio
1. Prem `Nova Inscripcio`.
2. Selecciona alumne i escola.
3. Indica any academic i estat (i el nom del curs si cal).
4. Accepta les condicions de privacitat si es requerit i desa.

Captura:
![Inscripcions: nova (modal) (CA)](/ajuda/codex_images_real/ca/inscripcions-crear.png)

### 7.3 Veure detalls
1. A la taula, prem `Detalls`.

Captura:
![Inscripcions: detall (CA)](/ajuda/codex_images_real/ca/inscripcions-detall.png)

### 7.4 Editar inscripcio
1. Des de `Detalls`, prem `Editar`.
2. Modifica els camps necessaris i desa.

Captura:
![Inscripcions: editar (CA)](/ajuda/codex_images_real/ca/inscripcions-edit.png)

### 7.5 Eliminar inscripcio
1. A la taula, prem `Esborrar`.
2. Confirma al dialeg del navegador.

Captura:
![Inscripcions: esborrar (confirmacio) (CA)](/ajuda/codex_images_real/ca/inscripcions-esborrar.png)

Estats tipics:
- Activa
- Pendent
- Cancel·lada

## 8. Quotes anuals
### 8.1 Llistar i cercar
1. Ves a `Quotes`.
2. Usa el camp de cerca per filtrar.

Captura:
![Quotes: llistat (CA)](/ajuda/codex_images_real/ca/quotes-anuals.png)

### 8.2 Crear quota anual
1. Ves a `Quotes`.
2. Prem `Nova Quota` (obre un modal).
3. Selecciona una inscripcio.
4. Introdueix import, data de venciment i (opcionalment) referencia.
5. Marca "pagada" si cal.
6. Accepta condicions de privacitat i desa.

Captures:
![Quotes: nova (modal) (CA)](/ajuda/codex_images_real/ca/quotes-anuals-crear.png)

### 8.3 Veure detalls
1. A la taula, prem `Detalls`.

Captura:
![Quotes: detall (CA)](/ajuda/codex_images_real/ca/quotes-anuals-detall.png)

### 8.4 Editar quota anual
1. Des de `Detalls`, prem `Editar`.
2. Modifica els camps necessaris.
3. Accepta les condicions de privacitat si es requerit.
4. Desa.

Captura:
![Quotes: editar (CA)](/ajuda/codex_images_real/ca/quotes-anuals-edit.png)

### 8.5 Eliminar quota anual
1. A la taula, prem `Esborrar`.
2. Confirma al dialeg del navegador.

Captura:
![Quotes: esborrar (confirmacio) (CA)](/ajuda/codex_images_real/ca/quotes-anuals-esborrar.png)

### 8.6 Decimals
L'import accepta decimals:
- `1000.25` o `1000,25` (segons configuracio regional del navegador)

Si tens problemes amb la coma, fes servir el punt.

### 8.7 Privacitat
Alguns formularis requereixen marcar el checkbox d'acceptacio de privacitat abans de desar.

### 8.8 Missatges d'error habituals
- "Acces no autoritzat": la sessio ha expirat, torna a fer login.
- "Omple els camps obligatoris": falta algun camp requerit.

## 9. Vista usuari final (rol USER)
El rol `USER` te un acces limitat i habitualment veu un tauler (dashboard) amb:
- inscripcions pròpies
- quotes pròpies

Captura:
![Dashboard (part superior) (CA)](/ajuda/codex_images_real/ca/dashboard-top.png)
![Dashboard (part inferior) (CA)](/ajuda/codex_images_real/ca/dashboard-down.png)

## 10. FAQ / Problemes frequents
### No puc iniciar sessio
- comprova email i contrasenya
- si persisteix, demana reset a un administrador

### No veig dades
- revisa que estiguis al rol correcte (ADM/USER)
- comprova que hi hagi dades carregades (entorn demo)

### Error de permisos
- el rol `USER` no pot fer operacions d'administracio

## Annex: Index de captures (ordre)
Aquest manual assumeix aquesta numeracio d'imatges:

ADM:
1. `docs/codex_images_real/ca/login-admin.png`
2. `docs/codex_images_real/ca/inici-top.png`
3. `docs/codex_images_real/ca/inici-down.png`
4. `docs/codex_images_real/ca/escoles.png`
5. `docs/codex_images_real/ca/escoles-crear.png`
6. `docs/codex_images_real/ca/escoles-detall.png`
7. `docs/codex_images_real/ca/escoles-edit.png`
8. `docs/codex_images_real/ca/escoles-esborrar.png`
9. `docs/codex_images_real/ca/estudiants.png`
10. `docs/codex_images_real/ca/estudiants-crear.png`
11. `docs/codex_images_real/ca/estudiants-detall.png`
12. `docs/codex_images_real/ca/estudiants-edit.png`
13. `docs/codex_images_real/ca/estudiants-esborrar.png`
14. `docs/codex_images_real/ca/inscripcions.png`
15. `docs/codex_images_real/ca/inscripcions-crear.png`
16. `docs/codex_images_real/ca/inscripcions-detall.png`
17. `docs/codex_images_real/ca/inscripcions-edit.png`
18. `docs/codex_images_real/ca/inscripcions-esborrar.png`
19. `docs/codex_images_real/ca/quotes-anuals.png`
20. `docs/codex_images_real/ca/quotes-anuals-crear.png`
21. `docs/codex_images_real/ca/quotes-anuals-detall.png`
22. `docs/codex_images_real/ca/quotes-anuals-edit.png`
23. `docs/codex_images_real/ca/quotes-anuals-esborrar.png`

USER:
1. `docs/codex_images_real/ca/dashboard-top.png`
2. `docs/codex_images_real/ca/dashboard-down.png`
