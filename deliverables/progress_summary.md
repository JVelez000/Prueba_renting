# Eventia - Plataforma de Gestión de Tickets (Resumen Final)

Este documento resume el trabajo realizado para la prueba técnica de Eventia S.A.S., destacando la implementación de la gestión de roles y administración de usuarios.

## 1. Estado del Proyecto
- **Backend**: 100% Completado (.NET 8, Clean Architecture, DDD, CQRS).
- **Frontend**: 100% Completado (Angular 17+, Material UI, RBAC).
- **Doc/Diagramas**: 100% Completado (C4, ER, Despliegue, Justificación).
- **Despliegue**: 100% Dockerizado y orquestado.

---

## 2. Mejoras de Seguridad y Roles (V2)

Se ha optimizado el sistema de roles para que la gestión sea completamente dinámica desde la interfaz:

- **Registro Público**: Cualquier usuario puede registrarse. Por defecto, el sistema le asigna el rol de `Agent` (menor privilegio).
- **Gestión Administrativa**: Desde la pantalla de "Usuarios", un administrador puede:
    - Habilitar o Deshabilitar cuentas.
    - Cambiar el rol de cualquier usuario (`Agent`, `Supervisor`, `Admin`) mediante un selector intuitivo.
- **Acceso por Defecto**: Se incluye un seed automático para un administrador maestro:
    - **Email**: `admin@eventia.com`
    - **Password**: `Admin123!`

---

## 3. Componentes Implementados

### Backend
- **Dominio**: Entidades `Ticket`, `Event`, `User`, `TicketHistory`.
- **Aplicación**: Comandos y Consultas con MediatR. Validaciones con FluentValidation.
- **Infraestructura**: Persistencia con PostgreSQL (EF Core). Seguridad con JWT.
- **Roles**: Implementación de RBAC con Claims de JWT.

### Frontend
- **Autenticación**: Login y Registro funcional.
- **Dashboard**: Estadísticas y métricas en tiempo real.
- **Gestión de Usuarios**: UI avanzada para el administrador con control de estados y roles.
- **Gestión de Tickets**: CRUD completo con trazabilidad de historial.

---

## 4. Guía de Ejecución
Para levantar el entorno completo:
```bash
docker-compose up -d --build
```
- **App**: `http://localhost:4200`
- **Swagger**: `http://localhost:5000/swagger`
- **DB Admin**: `localhost:5433` (Postgres / postgres)

---

## 5. Justificación Técnica
Se utilizó **Clean Architecture** para garantizar que las reglas de negocio sean independientes de los frameworks y la base de datos. El uso de **CQRS** con MediatR permite una separación clara entre lecturas y escrituras, facilitando el mantenimiento y la escalabilidad futura del sistema Eventia.
