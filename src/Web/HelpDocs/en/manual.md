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
![Login (EN)](/ajuda/codex_images_real/en/login-admin.png)

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
![Annual fees: list (EN)](/ajuda/codex_images_real/en/quotes-anuals.png)
![Annual fees: new (modal) (EN)](/ajuda/codex_images_real/en/quotes-anuals-crear.png)
![Annual fees: edit (EN)](/ajuda/codex_images_real/en/quotes-anuals-edit.png)

## Appendix: Screenshot index (order)
1. `docs/codex_images_real/en/login-admin.png`
2. `docs/codex_images_real/en/inici-top.png`
3. `docs/codex_images_real/en/inici-down.png`
4. `docs/codex_images_real/en/escoles.png`
5. `docs/codex_images_real/en/escoles-crear.png`
6. `docs/codex_images_real/en/escoles-detall.png`
7. `docs/codex_images_real/en/escoles-edit.png`
8. `docs/codex_images_real/en/estudiants.png`
9. `docs/codex_images_real/en/inscripcions.png`
10. `docs/codex_images_real/en/quotes-anuals.png`
11. `docs/codex_images_real/en/quotes-anuals-crear.png`
12. `docs/codex_images_real/en/quotes-anuals-edit.png`

## 8. FAQ
- Unauthorized: session expired, re-login
- Missing required fields: complete mandatory inputs
