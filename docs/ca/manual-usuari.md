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

## 1.1 Idioma
La web permet canviar l'idioma des del selector superior. La selecció es manté entre pantalles.

## 2. Inici de sessio
1. A la pantalla de login, introdueix el correu i la contrasenya.
2. Prem "Entrar".
3. Si l'usuari es vàlid, s'accedeix a la pagina d'inici.

Captures:
- (pendent) Login (CA/ES/EN/DE) amb dades anonimitzades.

## 3. Navegacio
Menu principal (rol `ADM`):
- Inici
- Escoles
- Alumnes
- Inscripcions
- Quotes

Canvi d'idioma:
- desplega el selector d'idioma (a la barra superior fosca)
- tria l'idioma (la pagina es recarrega)

## 4. Gestio d'escoles
### 4.1 Llistar, cercar i ordenar
1. Ves a `Escoles`.
2. Usa el camp de cerca de la taula per filtrar.
3. Fes clic als encapçalaments (si son ordenables) per ordenar.

### 4.2 Crear escola
1. Prem el botó per crear nova escola.
2. Omple els camps obligatoris (codi i nom).
3. Desa.

Captures:
- (pendent) Modal/formulari "Nova Escola" (CA/ES/EN/DE) amb dades anonimitzades.

### 4.3 Editar o eliminar
- A la columna d'accions, usa `Detalls/Editar` o `Esborrar`.
- En eliminar, confirma quan el navegador ho demani.

## 5. Gestio d'alumnes
### 5.1 Crear alumne
1. Ves a `Alumnes`.
2. Crea alumne amb nom, cognoms, email, data naixement (opcional) i escola.

Nota:
- si l'email ja existeix com a usuari, el sistema reutilitza l'usuari existent.

### 5.2 Editar i eliminar
Igual que a escoles: accions des de la taula.

## 6. Inscripcions
1. Ves a `Inscripcions`.
2. Crea una nova inscripcio seleccionant alumne, any academic, curs (opcional), estat.
3. Desa.

Estats tipics:
- Activa
- Pendent
- Cancel·lada

## 7. Quotes anuals
### 7.1 Crear quota anual
1. Ves a `Quotes`.
2. Prem `Nova Quota` (obre un modal).
3. Selecciona una inscripcio.
4. Introdueix import, data de venciment i (opcionalment) referencia.
5. Marca "pagada" si cal.
6. Accepta condicions de privacitat i desa.

Captures:
- (pendent) Modal "Nova quota" i pantalla d'edicio/detall (CA/ES/EN/DE) amb dades anonimitzades.

### 7.2 Editar quota anual
1. Ves a `Quotes`.
2. Obre `Detalls` de la quota.
3. Prem `Editar`.

### 7.3 Decimals
L'import accepta decimals:
- `1000,25` o `1000.25`

Si el navegador no permet el separador, prova l'altre format.

### 7.4 Privacitat
Alguns formularis requereixen marcar el checkbox d'acceptacio de privacitat abans de desar.

### 7.5 Missatges d'error habituals
- "Acces no autoritzat": la sessio ha expirat, torna a fer login.
- "Omple els camps obligatoris": falta algun camp requerit.

## 8. FAQ / Problemes frequents
### No puc iniciar sessio
- comprova email i contrasenya
- si persisteix, demana reset a un administrador

### No veig dades
- revisa que estiguis al rol correcte (ADM/USER)
- comprova que hi hagi dades carregades (entorn demo)

### Error de permisos
- el rol `USER` no pot fer operacions d'administracio
