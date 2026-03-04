# Functional document (EN)

## 1. Executive summary
"Escoles Publiques" supports managing:
- schools
- students
- enrollments (by academic year and course)
- annual fees (linked to an enrollment)
- scopes used to classify schools

The system is split into a Web UI and an API. The Web consumes the API.

## 2. Scope
In scope:
- CRUD for schools, students, enrollments and annual fees
- scope assignment and filtering
- home search (text + scope)
- authentication and role-based access (`ADM`/`USER`)
- language selector and responsive UI
- help center (user manual, functional, technical)

Out of scope (at the time of writing):
- advanced permission model beyond `ADM`/`USER`
- external integrations (email/push notifications)
- massive imports from official datasets

## 3. Actors and roles
Actors:
- `ADM` (administrator)
- `USER` (end user)

Roles:
- `ADM`: full access to management features
- `USER`: limited access (dashboard and related information)

## 4. Domain (main entities)
Entities:
- `School`
- `Student`
- `Enrollment`
- `AnnualFee`
- `Scope`
- `User`

High-level relationships:
- a `School` has 0..N `Student`
- a `Student` has 0..N `Enrollment`
- an `Enrollment` has 0..N `AnnualFee`
- a `Scope` classifies 0..N `School`
- a `User` can be linked to 0..1 `Student` (optional 1:1)

## 5. Diagrams

### 5.1 System context
```mermaid
flowchart LR
  U[User] -->|Browser| W[Web (MVC/Razor)]
  W -->|HTTP + JWT| A[API (REST)]
  A -->|EF Core| DB[(PostgreSQL)]
```

### 5.2 Use cases (UML-style)
```mermaid
flowchart LR
  ADM[[ADM]]
  USER[[USER]]

  subgraph S[Escoles Publiques]
    UC01([UC-01 Sign in])
    UC02([UC-02 Change language])
    UC03([UC-03 Open help])
    UC10([UC-10 Manage schools])
    UC20([UC-20 Manage students])
    UC30([UC-30 Manage enrollments])
    UC40([UC-40 Manage annual fees])
    UC50([UC-50 Search and filter])
    UC60([UC-60 View dashboard])
  end

  ADM --> UC01
  USER --> UC01
  ADM --> UC02
  USER --> UC02
  ADM --> UC03
  USER --> UC03
  ADM --> UC10
  ADM --> UC20
  ADM --> UC30
  ADM --> UC40
  ADM --> UC50
  USER --> UC60
```

### 5.3 Login flow (high level)
```mermaid
sequenceDiagram
  participant U as User
  participant W as Web
  participant A as API

  U->>W: Login (email+password)
  W->>A: POST /api/auth/token
  A-->>W: JWT
  W-->>U: Session started (cookie) and navigation
```

## 6. Use case catalog

### UC-01 Sign in
Actors:
- `ADM`, `USER`

Main flow:
1. Open login page.
2. Enter email and password.
3. System validates credentials.
4. Session starts and user is redirected based on role.

### UC-02 Change language
1. Select a language in the top bar.
2. Page reloads.
3. Selection is persisted via cookie.

Languages:
- documented: CA, ES, EN, DE
- planned: FR, RU, ZH

### UC-03 Open help
1. Click the "Help" button.
2. Select a document: user manual, functional or technical.
3. System shows the document in the active language.

### UC-10 Manage schools (ADM)
Includes: list/search/sort, create/edit/delete, favorites, scope assignment.

### UC-20 Manage students (ADM)
Includes: CRUD; reuse user by email; optional 1:1 user<->student link.

### UC-30 Manage enrollments (ADM)
Includes: CRUD; academic year and status.

### UC-40 Manage annual fees (ADM)
Includes: CRUD; mark as paid (stores payment date).

Rules:
- some forms require accepting privacy checkbox
- amount supports decimals with comma or dot

### UC-50 Search and filter (ADM)
Text search and scope filtering from the home page.

### UC-60 View dashboard (USER)
View user-specific information (related enrollments/fees).

## 7. Business rules (summary)
- School: code and name are required
- User: email must be unique
- Enrollment: student, school, academic year and status
- Annual fee: enrollment, amount and due date

## 8. Non-functional requirements (brief)
- multi-language UI
- responsive (mobile/tablet)
- operational logs for troubleshooting
- persistence: PostgreSQL

## 9. Acceptance criteria (checklist)
- admin and user login works
- CRUD works for all entities
- search and scope filter works
- amounts accept `,` and `.`
- language is persisted and help follows active language

## 10. Functional Complement 2026

Functional improvements added without changing baseline scope:
- Unified API error contract with traceability (`traceId`) and stable codes (`errorCode`) to simplify functional support.
- Stronger domain-level business validation (invariants) to prevent inconsistent school codes, emails, and monetary amounts.
- Improved reliability in critical flows (authentication and core CRUD) using risk-based tests and coverage gates.
- Help center keeps existing structure and now includes these capabilities in functional and technical docs.

## 11. Mermaid Complement 2026

### 11.1 Request lifecycle with traceability

a) Correlation id is propagated end-to-end.
b) API returns stable error contract when a business/validation error occurs.

```mermaid
sequenceDiagram
  participant U as User
  participant W as Web
  participant A as API
  participant D as Domain

  U->>W: Submit action
  W->>A: HTTP request + X-Correlation-ID
  A->>D: Execute use case
  alt Validation or domain error
    D-->>A: ValidationException/NotFound/etc.
    A-->>W: ProblemDetails(errorCode, traceId, fieldErrors)
    W-->>U: Friendly error + trace id
  else Success
    D-->>A: Result
    A-->>W: 2xx response
    W-->>U: Updated view
  end
```

### 11.2 CQRS functional split (Schools)

Commands modify state, queries read state.

```mermaid
flowchart LR
  UI[User action in Web] --> C{Intent}
  C -->|Create/Update/Delete| CMD[Command Handler]
  C -->|Read/Get/List| QRY[Query Handler]
  CMD --> SVC[Application Service]
  QRY --> SVC
  SVC --> REPO[(Repository)]
  REPO --> DB[(PostgreSQL)]
```

### 11.3 Quality and release gates

Risk-based tests plus coverage gates reduce regressions.

```mermaid
flowchart LR
  DEV[Code changes] --> TEST[Unit + integration tests]
  TEST --> CRIT[Critical flows tests]
  CRIT --> COV[Coverage gates]
  COV -->|Pass| MERGE[Ready to merge]
  COV -->|Fail| FIX[Fix + re-run]
  FIX --> TEST
```
