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
- (pending) Login (CA/ES/EN/DE) with anonymized data.

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

## 8. FAQ
- Unauthorized: session expired, re-login
- Missing required fields: complete mandatory inputs
