# C4 Architecture Diagrams — Eventia S.A.S.

---

## Level 1 — System Context

> Who uses the system and what external systems does it interact with?

```mermaid
graph TB
    agent["👤 Agent\n(Internal user)"]
    supervisor["👤 Supervisor\n(Internal user)"]
    admin["👤 Administrator\n(Internal user)"]
    client["👤 External Client\n(Event attendee)"]

    eventia["🖥️ Eventia Platform\n(Ticket Management System)"]

    email["📧 Email Service\n(SMTP / SendGrid)"]
    azure_ad["🔐 Azure AD\n(Optional SSO)"]

    agent -->|"Manages tickets\nvia web browser"| eventia
    supervisor -->|"Supervises and\nassigns tickets"| eventia
    admin -->|"Full administration\nof the system"| eventia
    client -->|"Reports incidents\n(future)"| eventia

    eventia -->|"Sends\nnotifications"| email
    eventia -->|"Authenticates\nusers (optional)"| azure_ad
```

---

## Level 2 — Containers

> What are the main deployable pieces of the system?

```mermaid
graph TB
    user["👤 User\n(any role, via browser)"]

    subgraph Eventia Platform
        frontend["📦 Angular SPA\n(Angular 17+)\nSingle Page Application\nPort: 4200 / Nginx"]
        api["📦 REST API\n(.NET 8 Web API)\nBusiness logic + CQRS\nPort: 8080"]
        db[("🗄️ PostgreSQL\nRelational database\nTickets, Users, Events, Audit")]
    end

    email["📧 Email Service\n(SMTP / SendGrid)"]

    user -->|"HTTPS - Browser"| frontend
    frontend -->|"HTTP/REST + JWT"| api
    api -->|"EF Core\nSQL queries"| db
    api -->|"SMTP"| email
```

---

## Level 3 — Components (REST API)

> What are the main components inside the API container?

```mermaid
graph TB
    client["Angular SPA"]

    subgraph Eventia.API
        tc["TicketsController\n/api/v1/tickets"]
        uc["UsersController\n/api/v1/users"]
        ac["AuthController\n/api/v1/auth"]
        mw["JWT Middleware\n+ Error Handler"]
    end

    subgraph Eventia.Application
        mediator["MediatR\nDispatcher"]
        cmds["Commands\nCreateTicket / AssignTicket\nChangeStatus / DisableUser"]
        qrs["Queries\nGetTickets / GetTicketById\nGetUsers"]
    end

    subgraph Eventia.Domain
        entities["Entities\nTicket · User · Event"]
        events["Domain Events\nTicketCreated · StatusChanged"]
        repos["Repository Interfaces\nITicketRepository · IUnitOfWork"]
    end

    subgraph Eventia.Infrastructure
        ef["AppDbContext\n(EF Core)"]
        jwt["JwtTokenService"]
        notif["NotificationService"]
    end

    db[("PostgreSQL")]

    client -->|"HTTP + Bearer Token"| mw
    mw --> tc
    mw --> uc
    mw --> ac
    tc --> mediator
    uc --> mediator
    ac --> jwt
    mediator --> cmds
    mediator --> qrs
    cmds --> entities
    cmds --> events
    qrs --> repos
    entities --> repos
    repos --> ef
    ef --> db
    events --> notif
```
