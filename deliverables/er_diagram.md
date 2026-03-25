# Entity-Relationship Diagram — Eventia S.A.S.

## Database: PostgreSQL

```mermaid
erDiagram
    USERS {
        uuid id PK
        varchar name
        varchar email UK
        varchar password_hash
        varchar role "agent | supervisor | admin"
        boolean is_active
        timestamp created_at
    }

    EVENTS {
        uuid id PK
        varchar name
        text description
        timestamp event_date
        varchar location
        uuid created_by FK
        timestamp created_at
    }

    TICKETS {
        uuid id PK
        varchar title
        text description
        uuid status_id FK
        uuid assigned_to FK
        uuid created_by FK
        uuid event_id FK
        timestamp created_at
        timestamp updated_at
    }

    TICKET_STATUSES {
        uuid id PK
        varchar name "open | in_progress | closed"
    }

    TICKET_HISTORY {
        uuid id PK
        uuid ticket_id FK
        uuid changed_by FK
        uuid old_status_id FK
        uuid new_status_id FK
        text notes
        timestamp changed_at
    }

    USERS ||--o{ TICKETS : "creates"
    USERS ||--o{ TICKETS : "assigned_to"
    USERS ||--o{ EVENTS : "creates"
    USERS ||--o{ TICKET_HISTORY : "registers"
    EVENTS ||--o{ TICKETS : "contains"
    TICKET_STATUSES ||--o{ TICKETS : "current_status"
    TICKET_STATUSES ||--o{ TICKET_HISTORY : "old_status"
    TICKET_STATUSES ||--o{ TICKET_HISTORY : "new_status"
    TICKETS ||--o{ TICKET_HISTORY : "has"
```

## Entity Descriptions

| Entity | Description |
|---|---|
| **USERS** | System users with differentiated roles: agent, supervisor, administrator. Can be disabled without deletion. |
| **EVENTS** | Corporate or academic events that group related tickets. |
| **TICKETS** | Core unit of work. Linked to an event, a creator, and an assignee. Has a current status. |
| **TICKET_STATUSES** | Catalog of possible states: `open`, `in_progress`, `closed`. |
| **TICKET_HISTORY** | Full audit trail of every status change on a ticket, including who made it and when. |

## Key Design Decisions

- **UUIDs as primary keys**: avoids sequential ID exposure and simplifies distributed scenarios.
- **Soft delete for users**: `is_active` flag instead of physical deletion, preserving referential integrity.
- **Audit via TICKET_HISTORY**: every state transition is recorded, fulfilling the traceability requirement.
- **role as varchar on USERS**: simple and sufficient for the current three-role model; can evolve to a roles table if needed.
