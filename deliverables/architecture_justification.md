# Technical Architecture Justification
## Project: Eventia S.A.S. — Ticket Management Platform

---

## 1. Executive Summary

For the development of the Eventia S.A.S. platform, a layered architecture based on **Clean Architecture + DDD (Domain-Driven Design) + CQRS** principles is proposed for the backend, exposed through a **RESTful Web API** built with **.NET 8**, and consumed from a **SPA built in Angular 17+**. This combination maximizes maintainability, scalability, and separation of concerns required by the business.

---

## 2. Defined Technology Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 17+ (Standalone Components) |
| Backend | .NET 8 (ASP.NET Core Web API) |
| Database | PostgreSQL |
| Authentication | JWT + (optional) MSAL / Azure AD |
| API Documentation | Swagger / OpenAPI |
| CI/CD | GitHub Actions / Azure DevOps Pipelines |
| Containers | Docker + Docker Compose |

---

## 3. Backend Architecture: Clean Architecture + DDD + CQRS

### 3.1 Why Clean Architecture?

Clean Architecture (proposed by Robert C. Martin) organizes the system in concentric layers where dependencies always point **inward**, toward the domain. This ensures:

- The **domain** (business logic) does not depend on frameworks, databases, or external infrastructure.
- Each layer is **replaceable** without affecting others (e.g., switching from SQL Server to PostgreSQL without touching the domain).
- The solution is **highly testable** because the business core is pure C#.

> Clean Architecture is preferred over pure Hexagonal Architecture (Ports & Adapters) because it is more explicit in layer hierarchy, more widely adopted in the .NET ecosystem, and its conventions scale better as teams grow.

### 3.2 Why DDD (Domain-Driven Design)?

Eventia handles rich business concepts: **Tickets**, **Events**, **Users with roles**, **Workflow states**, **Audit history**. DDD allows:

- Modeling these concepts as **Entities**, **Value Objects**, and **Aggregates** with their own behavior.
- Using **Ubiquitous Language**: code speaks the same language as the business (e.g., `Ticket.AssignAgent()`, `Ticket.ChangeStatus()`).
- Defining clear **Bounded Contexts** that allow scaling toward microservices in the future without traumatic refactoring.

**Identified Bounded Contexts:**

| Bounded Context | Responsibility |
|---|---|
| **Tickets** | Full ticket lifecycle (CRUD, states, audit) |
| **Users & Roles** | Registration, authentication, profiles, and role management |
| **Notifications** | Internal state-change events |

### 3.3 Why CQRS with MediatR?

CQRS (Command Query Responsibility Segregation) separates **write (Commands)** from **read (Queries)** operations:

- **Commands**: `CreateTicketCommand`, `AssignTicketCommand`, `ChangeTicketStatusCommand`
- **Queries**: `GetTicketsByUserQuery`, `GetTicketByIdQuery`

**Concrete benefits for Eventia:**
- **Supervisors and agents** have frequent, varied read patterns → optimized Queries.
- **Critical operations** (status change, assignment) have complex validations → Commands with encapsulated logic.
- Enables **domain events** (e.g., `TicketCreated`, `TicketClosed`) for notifications and audit.
- Compatible with **MediatR**, the standard library in the .NET ecosystem.

### 3.4 Backend Layer Structure

```
Eventia.Backend/
├── Eventia.Domain/              # Domain core
│   ├── Entities/                # Ticket, User, Event
│   ├── ValueObjects/            # TicketStatus, UserRole
│   ├── Aggregates/              # TicketAggregate
│   ├── Events/                  # TicketCreated, StatusChanged
│   └── Interfaces/              # ITicketRepository, IUnitOfWork
│
├── Eventia.Application/         # Use cases (CQRS)
│   ├── Tickets/
│   │   ├── Commands/            # CreateTicket, UpdateTicket, ChangeStatus, AssignTicket
│   │   └── Queries/             # GetTickets, GetTicketById
│   ├── Users/
│   │   ├── Commands/            # RegisterUser, DisableUser
│   │   └── Queries/             # GetUserById
│   ├── DTOs/                    # Data Transfer Objects (request/response)
│   └── Interfaces/              # Application service contracts
│
├── Eventia.Infrastructure/      # External implementations
│   ├── Persistence/             # AppDbContext (EF Core), Repositories
│   ├── Identity/                # JWT, role management
│   ├── Notifications/           # Domain events / Email
│   └── Migrations/              # Database migrations
│
└── Eventia.API/                 # Presentation layer
    ├── Controllers/             # TicketsController, UsersController
    ├── Middleware/              # Global error handling
    ├── Filters/                 # Role-based authorization
    └── Program.cs               # Dependency injection setup
```

---

## 4. Frontend Architecture: Angular 17+

The frontend is a functional and presentable SPA. The priority is to correctly consume the API and handle role-based authentication.

```
eventia-frontend/
├── src/
│   ├── app/
│   │   ├── core/               # AuthService, AuthGuard, JwtInterceptor
│   │   ├── shared/             # Reusable components
│   │   ├── features/
│   │   │   ├── auth/           # Login
│   │   │   ├── tickets/        # List, detail, create
│   │   │   └── users/          # User management (admin only)
│   │   └── layout/             # Navbar, Shell
│   └── environments/
```

- **Authentication**: Login with JWT. Token stored in `localStorage` and attached to every request via `HttpInterceptor`.
- **Roles**: `AuthGuard` validates the token role to protect routes by profile (agent, supervisor, administrator).
- **UI**: Angular Material for ready-made components (tables, forms, navigation).

---

## 5. Frontend ↔ Backend Communication

```
Angular SPA  ──HTTP/REST──▶  ASP.NET Core Web API  ──EF Core──▶  Database
                              │
                              └──▶  JWT Auth Middleware
                              └──▶  Domain Events → Notifications
```

- **Protocol**: REST (JSON).
- **Authentication**: JWT Bearer Token. Header `Authorization: Bearer <token>`.
- **Security**: HTTPS mandatory. CORS configured explicitly.
- **API versioning**: `/api/v1/tickets`, `/api/v1/users`.

---

## 6. Database

**Recommended engine: PostgreSQL** (open-source, superior performance, Azure-compatible).

**Access**: Entity Framework Core 8 with **Repository + Unit of Work** pattern, decoupling the ORM from the domain.

---

## 7. Observability & Logging

- **Serilog** for structured logging (console + file).
- **Health Checks** endpoint (`/health`) to monitor API health.
- **Traceable domain events**: every ticket status change is recorded in an audit table.

---

## 8. Deployment (CI/CD)

```
GitHub / Azure Repos
       │
       ▼
GitHub Actions / Azure DevOps Pipeline
       │
       ├── Build & Test (.NET + Angular)
       ├── Docker Build (API + Frontend)
       └── Push → Docker Registry
                  │
                  ▼
           Azure App Service / Azure Container Apps
```

- **Rollback**: via Docker image tags or GitHub release versioning.
- **Environments**: `dev`, `staging`, `production` with separate environment variables.

---

## 9. Conclusion

The **Clean Architecture + DDD + CQRS** architecture in .NET 8 together with **Angular 17+** is the best option because:

1. **Aligns code with the business**: DDD makes the model reflect exactly Eventia's concepts.
2. **Allows growth**: Bounded Contexts are extractable to microservices in the future without rewriting.
3. **Is maintainable**: Any .NET/Angular developer quickly identifies where each responsibility lives.
4. **Meets all non-functional requirements**: security (JWT), observability (Serilog + events), scalability (modular), CI/CD (Docker + pipelines).
5. **Is the industry standard choice** for enterprise .NET systems in 2024–2025.

---

---
---

# Justificación Técnica de la Arquitectura Propuesta
## Proyecto: Eventia S.A.S. — Plataforma de Gestión de Tickets

---

## 1. Resumen Ejecutivo

Para el desarrollo de la plataforma de Eventia S.A.S. se propone una arquitectura basada en los principios de **Clean Architecture + DDD (Domain-Driven Design) + CQRS** en el backend, expuesta mediante una **Web API RESTful** en **.NET 8**, y consumida desde una **SPA desarrollada en Angular 17+**. Esta combinación maximiza la mantenibilidad, la escalabilidad futura y la separación de responsabilidades que el negocio exige.

---

## 2. Stack Tecnológico Definido

| Capa | Tecnología |
|---|---|
| Frontend | Angular 17+ (Standalone Components) |
| Backend | .NET 8 (ASP.NET Core Web API) |
| Base de datos | PostgreSQL |
| Autenticación | JWT + (opcional) MSAL / Azure AD |
| Documentación API | Swagger / OpenAPI |
| CI/CD | GitHub Actions / Azure DevOps Pipelines |
| Contenedores | Docker + Docker Compose |

---

## 3. Arquitectura del Backend: Clean Architecture + DDD + CQRS

### 3.1 ¿Por qué Clean Architecture?

Clean Architecture (propuesta por Robert C. Martin) organiza el sistema en capas concéntricas donde las dependencias siempre apuntan **hacia adentro**, hacia el dominio. Esto garantiza que:

- El **dominio** (lógica de negocio) no depende de frameworks, bases de datos ni infraestructura externa.
- Cada capa es **reemplazable** sin afectar las demás (p. ej.: cambiar de SQL Server a PostgreSQL sin tocar el dominio).
- La solución es **altamente testeable** porque el núcleo del negocio es puro C#.

> Se prefiere **Clean Architecture** sobre la arquitectura hexagonal pura porque es más explícita en la jerarquía de capas, más ampliamente adoptada en el ecosistema .NET, y sus convenciones escalan mejor cuando el equipo crece.

### 3.2 ¿Por qué DDD (Domain-Driven Design)?

Eventia maneja conceptos ricos de negocio: **Tickets**, **Eventos**, **Usuarios con roles**, **Estados de flujo**, **Historial de auditoría**. DDD permite:

- Modelar estos conceptos como **Entidades**, **Value Objects** y **Aggregates** con comportamiento propio.
- Usar un **Lenguaje Ubiquo**: el código habla el mismo idioma que el negocio (ej. `Ticket.AssignAgent()`, `Ticket.ChangeStatus()`).
- Definir **Bounded Contexts** claros que permiten escalar hacia microservicios en el futuro sin refactorizaciones traumáticas.

**Bounded Contexts identificados:**

| Bounded Context | Responsabilidad |
|---|---|
| **Tickets** | Ciclo de vida completo de los tickets (CRUD, estados, auditoría) |
| **Usuarios & Roles** | Registro, autenticación, perfiles y gestión de roles |
| **Notificaciones** | Eventos internos de cambio de estado |

### 3.3 ¿Por qué CQRS con MediatR?

CQRS separa las operaciones de **escritura (Commands)** de las de **lectura (Queries)**:

- **Commands**: `CreateTicketCommand`, `AssignTicketCommand`, `ChangeTicketStatusCommand`
- **Queries**: `GetTicketsByUserQuery`, `GetTicketByIdQuery`

**Beneficios concretos para Eventia:**
- Los **supervisores y agentes** tienen patrones de lectura frecuente y variada → Queries optimizadas.
- Las **operaciones críticas** (cambio de estado, asignación) tienen validaciones complejas → Commands con lógica encapsulada.
- Facilita implementar **eventos de dominio** (`TicketCreated`, `TicketClosed`) para notificaciones y auditoría.
- Compatible con **MediatR**, librería estándar en el ecosistema .NET.

### 3.4 Estructura de Capas del Backend

```
Eventia.Backend/
├── Eventia.Domain/              # Núcleo del dominio
│   ├── Entities/                # Ticket, User, Event
│   ├── ValueObjects/            # TicketStatus, UserRole
│   ├── Aggregates/              # TicketAggregate
│   ├── Events/                  # TicketCreated, StatusChanged
│   └── Interfaces/              # ITicketRepository, IUnitOfWork
│
├── Eventia.Application/         # Casos de uso (CQRS)
│   ├── Tickets/
│   │   ├── Commands/            # CreateTicket, UpdateTicket, ChangeStatus, AssignTicket
│   │   └── Queries/             # GetTickets, GetTicketById
│   ├── Users/
│   │   ├── Commands/            # RegisterUser, DisableUser
│   │   └── Queries/             # GetUserById
│   ├── DTOs/                    # Data Transfer Objects
│   └── Interfaces/              # Contratos de servicios
│
├── Eventia.Infrastructure/      # Implementaciones externas
│   ├── Persistence/             # AppDbContext (EF Core), Repositories
│   ├── Identity/                # JWT, role management
│   ├── Notifications/           # Domain events / Email
│   └── Migrations/              # Database migrations
│
└── Eventia.API/                 # Capa de presentación
    ├── Controllers/             # TicketsController, UsersController
    ├── Middleware/              # Global error handling
    ├── Filters/                 # Role-based authorization
    └── Program.cs               # Dependency injection setup
```

---

## 4. Arquitectura del Frontend: Angular 17+

El frontend es una SPA funcional y presentable. La prioridad es consumir correctamente la API y manejar la autenticación por roles.

```
eventia-frontend/
├── src/
│   ├── app/
│   │   ├── core/               # AuthService, AuthGuard, JwtInterceptor
│   │   ├── shared/             # Componentes reutilizables
│   │   ├── features/
│   │   │   ├── auth/           # Login
│   │   │   ├── tickets/        # Lista, detalle, creación
│   │   │   └── users/          # Gestión de usuarios (solo admin)
│   │   └── layout/             # Navbar, Shell
│   └── environments/
```

- **Autenticación**: Login con JWT. Token en `localStorage`, adjuntado en cada petición via `HttpInterceptor`.
- **Roles**: `AuthGuard` valida el rol del token para proteger rutas.
- **UI**: Angular Material para componentes listos (tablas, formularios, navegación).

---

## 5. Comunicación Frontend ↔ Backend

- **Protocolo**: REST (JSON).
- **Autenticación**: JWT Bearer Token.
- **Seguridad**: HTTPS obligatorio. CORS configurado explícitamente.
- **Versionado de API**: `/api/v1/tickets`, `/api/v1/users`.

---

## 6. Base de Datos

**Motor: PostgreSQL** — open-source, alto rendimiento, compatible con Azure.

**Acceso**: Entity Framework Core 8 con patrón **Repository + Unit of Work**.

---

## 7. Observabilidad y Logging

- **Serilog** para logging estructurado.
- **Health Checks** en `/health`.
- **Auditoría**: cada cambio de estado de ticket queda registrado.

---

## 8. Despliegue (CI/CD)

- Pipeline en GitHub Actions / Azure DevOps: Build → Test → Docker Build → Deploy.
- **Rollback** via tags de imagen Docker.
- **Ambientes**: `dev`, `staging`, `production`.

---

## 9. Conclusión

La arquitectura **Clean Architecture + DDD + CQRS** en .NET 8 junto con **Angular 17+** representa la mejor opción porque:

1. **Alinea el código con el negocio**: DDD refleja exactamente los conceptos de Eventia.
2. **Permite crecer**: Bounded Contexts extractables a microservicios sin reescritura.
3. **Es mantenible**: Clara separación de responsabilidades por capa.
4. **Cumple todos los requisitos no funcionales**: seguridad, observabilidad, escalabilidad, CI/CD.
5. **Estándar de la industria** para sistemas empresariales en .NET.

---
*Document prepared for Eventia S.A.S. project — March 2026*
