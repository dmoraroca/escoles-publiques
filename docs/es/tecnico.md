# Documento técnico (ES)

## 1. Introducción
Este documento describe el diseño técnico de **Escoles Publiques**.

Objetivos:
- explicar la arquitectura y los límites DDD
- documentar la configuración de Web y API
- describir modelo de datos y autenticación
- documentar aspectos transversales (errores, observabilidad, pruebas)

Credenciales de prueba:
- usuario: `admin@admin.adm`
- contraseña: `admin123`

## 2. Arquitectura general (Web + API + DDD)

```mermaid
flowchart LR
  U[Usuario] -->|Navegador| W[Web MVC/Razor]
  W -->|HTTP + JWT| A[API ASP.NET Core]
  A -->|EF Core| DB[(PostgreSQL)]

  subgraph DDD[Interno DDD]
    D[Domain]
    AP[Application]
    I[Infrastructure]
  end

  W --> AP
  A --> AP
  AP --> D
  AP --> I
  I --> D
```

Flujo principal:
1. El usuario inicia sesión en Web (cookie auth)
2. Web solicita JWT a la API (`POST /api/auth/token`)
3. El token se guarda en sesión
4. Web llama a API con `Authorization: Bearer <token>`

## 3. Estructura de proyectos DDD
- `src/Domain`: entidades, value objects, excepciones de dominio, contratos de repositorio
- `src/Application`: casos de uso, servicios, handlers CQRS
- `src/Infrastructure`: EF Core, repositorios, migraciones
- `src/Api`: controladores REST, JWT, CORS, swagger, middleware
- `src/Web`: interfaz MVC, localización, clientes API

## 4. Modelo de dominio
Entidades principales:
- `School`
- `Student`
- `Enrollment`
- `AnnualFee`
- `Scope`
- `User`

Relaciones clave:
- School 1..N Students
- Student 1..N Enrollments
- Enrollment 1..N AnnualFees
- Scope 1..N Schools
- User 0..1 Student

## 5. Autenticación y autorización
- Web usa autenticación por cookies.
- API usa autenticación JWT bearer.
- Modelo de roles: `ADM` y `USER`.
- Flujos no autorizados fuerzan logout y reautenticación.

## 6. Contrato de errores
La API devuelve `application/problem+json` con:
- `errorCode`
- `traceId`
- `timestamp`
- `fieldErrors` (en validación)

Mapeos estándar:
- validación -> 400
- entidad duplicada -> 409
- no encontrado -> 404
- no autorizado -> 401
- error no controlado -> 500

## 7. Value Objects e invariantes
Las invariantes se refuerzan con value objects:
- `SchoolCode`
- `EmailAddress`
- `MoneyAmount`

Beneficios:
- validación centralizada
- calidad de datos consistente
- menos lógica defensiva en controladores

## 8. CQRS (ligero)
En Schools se separan lectura y escritura:
- Commands: crear/actualizar/eliminar
- Queries: obtener por id/listado/por código

Esto mantiene responsabilidades claras y testeables.

## 9. Observabilidad
Middleware transversal implementado:
- `CorrelationIdMiddleware` (`X-Correlation-ID`)
- `RequestMetricsMiddleware` (conteo + latencia)
- middleware global de excepciones

Logging estructurado y trazable.

## 10. Persistencia
- PostgreSQL + EF Core
- migraciones en `Infrastructure`
- patrón repositorio
- convención snake_case en mapeos

## 11. Capa Web
- vistas Razor y controladores MVC
- localización con `.resx`
- SignalR para tiempo real
- componentes JS/CSS reutilizables

## 12. Estrategia de pruebas
- unit tests para dominio, aplicación, controladores y helpers
- integración para flujos clave
- suite de flujos críticos por riesgo
- coverage gates en CI

## 13. Gates de calidad CI/CD
Se aplican umbrales de cobertura por capa:
- Domain
- Application
- Infrastructure
- Web
- Api

Build y tests deben pasar antes de merge.

## 14. Notas operativas
- flujo local orientado a Docker
- perfiles de debug simplificados a Docker attach
- centro de ayuda con markdown multidioma y exportación DOCX
- recomendación: actualizar docs y código en la misma PR
