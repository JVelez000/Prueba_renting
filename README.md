# Eventia - Plataforma de Gestión de Tickets

Eventia es una solución integral diseñada para centralizar la gestión de requerimientos y eventos corporativos. Construida con una arquitectura robusta y moderna, la plataforma permite el seguimiento detallado de tickets, administración de usuarios con control de acceso basado en roles (RBAC) y trazabilidad completa de operaciones.

## 🚀 Inicio Rápido (Docker)

La forma más sencilla de ejecutar el proyecto es utilizando Docker Compose para levantar todos los servicios (API, Base de Datos y Aplicación Web).

1.  Asegúrate de tener instalado **Docker** y **Docker Compose**.
2.  Desde la raíz del proyecto, ejecuta:
    ```bash
    docker-compose up -d --build
    ```
3.  Accede a las aplicaciones:
    -   **Frontend**: `http://localhost:4200`
    -   **API (Swagger)**: `http://localhost:5000/swagger`
    -   **Admin de DB (opcional)**: `http://localhost:5433`

---

## 🔐 Credenciales de Acceso (Admin por Defecto)

Para facilitar la evaluación inicial, el sistema crea automáticamente un usuario administrador al arrancar.

| Usuario | Contraseña | Rol |
| :--- | :--- | :--- |
| **admin@eventia.com** | **Admin123!** | **Admin** |

> [!TIP]
> Una vez logueado como Admin, puedes dirigirte a la sección de **Gestión de Usuarios** para registrar nuevos miembros o elevar el rol de usuarios registrados ("Agent" por defecto).

---

## 🛠️ Stack Tecnológico

### Backend (Eventia.API)
-   **Core**: .NET 8 con C#.
-   **Arquitectura**: Clean Architecture + DDD (Domain-Driven Design).
-   **Patrones**: CQRS (MediatR), Repository y Unit of Work.
-   **Persistencia**: Entity Framework Core con PostgreSQL.
-   **Seguridad**: Autenticación JWT y encriptación de claves con BCrypt.

### Frontend (Eventia-Web)
-   **Framework**: Angular 17 (Standalone Components).
-   **UI**: Angular Material para un diseño premium y responsivo.
-   **Estado**: Signal-based state management.
-   **Autorización**: Guards y directivas para RBAC (Admin, Supervisor, Agent).

---

## ✨ Características Principales

1.  **Gestión de Ciclo de Vida**: Creación, asignación y cambio de estado de tickets vinculados a eventos.
2.  **Control de Roles Dinámico**: Los administradores pueden cambiar el rol de cualquier usuario en tiempo real desde la UI.
3.  **Trazabilidad**: Historial detallado de cada acción realizada sobre un ticket.
4.  **Validación Robusta**: Implementación de FluentValidation en el backend y Reactive Forms en el frontend.

## 📂 Estructura de Entregables (`/deliverables`)

-   `architecture_justification.md`: Justificación técnica de las decisiones de diseño.
-   `c4_diagram.md`: Visualización arquitectónica (Contenedores y Componentes).
-   `er_diagram.md`: Esquema relacional de la base de datos.
-   `deployment_diagram.md`: Recomendaciones de infraestructura en Azure.
-   `progress_summary.md`: Resumen detallado de la implementación final.
