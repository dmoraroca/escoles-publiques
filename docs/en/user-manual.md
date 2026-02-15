# User manual (EN)

## 1. Introduction
This manual explains how to use the "Escoles Publiques" web application.

Audience:
- administrators (`ADM`)
- end users (`USER`) (limited access)

Requirements:
- modern browser
- valid credentials

## 1.1 Data protection (important)
- Do not include real personal data (names/emails/IDs) in documentation.
- For screenshots, use test data and blur/pixelate anything sensitive before sharing.
- This repository should not contain screenshots with real data.

## 2. Login
1. Open the login page.
2. Enter email and password.
3. Click "Login".

Screenshots:
![Login (EN)](../assets/screenshots/admin/01-login-en.png)

## 3. Navigation
Admin main menu:
- Home, Schools, Students, Enrollments, Annual fees

Language selector:
- pick a language in the top bar (persisted via cookie)

## 4. Schools (ADM)
- list/search/sort
- create/edit/delete
- favorites and scope assignment

## 5. Students (ADM)
- create/edit/delete
- users can be reused by email

## 6. Enrollments (ADM)
- create/edit/delete enrollment (academic year, course, status)

## 7. Annual fees (ADM)
Create:
1. Go to Annual fees.
2. Click "New" (opens modal).
3. Select enrollment, enter amount and due date.
4. Accept privacy checkbox and save.

Decimals:
- amount supports `1000,25` and `1000.25`

Screenshots:
![Annual fees: list (EN)](../assets/screenshots/admin/40-quotes-llistat-en.png)
![Annual fees: new (modal) (EN)](../assets/screenshots/admin/41-quotes-nova-modal-en.png)
![Annual fees: edit (EN)](../assets/screenshots/admin/42-quotes-editar-en.png)

## Appendix: Screenshot index (order)
1. `docs/assets/screenshots/admin/01-login-en.png`
2. `docs/assets/screenshots/admin/02-inici-en.png`
3. `docs/assets/screenshots/admin/10-escoles-llistat-en.png`
4. `docs/assets/screenshots/admin/11-escoles-nova-modal-en.png`
5. `docs/assets/screenshots/admin/12-escoles-detall-en.png`
6. `docs/assets/screenshots/admin/13-escoles-editar-en.png`
7. `docs/assets/screenshots/admin/20-alumnes-llistat-en.png`
8. `docs/assets/screenshots/admin/30-inscripcions-llistat-en.png`
9. `docs/assets/screenshots/admin/40-quotes-llistat-en.png`
10. `docs/assets/screenshots/admin/41-quotes-nova-modal-en.png`
11. `docs/assets/screenshots/admin/42-quotes-editar-en.png`

## 8. FAQ
- Unauthorized: session expired, re-login
- Missing required fields: complete mandatory inputs
