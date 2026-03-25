# UML Deployment Diagram — Eventia S.A.S.

---

## Deployment Overview

```mermaid
graph TB
    subgraph "Client Device (Browser)"
        browser["🌐 Web Browser\nAngular SPA\n(HTML/CSS/JS)"]
    end

    subgraph "Azure Cloud"
        subgraph "Azure Container Apps / App Service"
            nginx["📦 Nginx Container\nServes Angular SPA\n:443 (HTTPS)"]
            api["📦 .NET 8 API Container\nASP.NET Core Web API\n:8080 (internal)"]
        end

        subgraph "Azure Database"
            postgres[("🗄️ Azure Database\nfor PostgreSQL\nFlexible Server")]
        end

        subgraph "Azure Container Registry"
            registry["🐳 Docker Registry\nAPI Image\nFrontend Image"]
        end
    end

    subgraph "CI/CD Pipeline"
        repo["📁 GitHub / Azure Repos\n(source code)"]
        pipeline["⚙️ GitHub Actions /\nAzure DevOps Pipeline\nBuild · Test · Deploy"]
    end

    email["📧 Email Service\n(SendGrid / SMTP)"]

    browser -->|"HTTPS :443"| nginx
    nginx -->|"Proxy /api/*\nHTTP internal"| api
    api -->|"TCP :5432\nSSL"| postgres
    api -->|"SMTP / API"| email
    repo -->|"push / PR"| pipeline
    pipeline -->|"docker push"| registry
    registry -->|"pull & deploy"| nginx
    registry -->|"pull & deploy"| api
```

---

## Infrastructure Notes

| Component | Technology | Notes |
|---|---|---|
| Frontend hosting | Nginx container | Serves built Angular app, proxies `/api/*` to backend |
| Backend hosting | Azure Container Apps | Auto-scaling, managed HTTPS |
| Database | Azure Database for PostgreSQL Flexible Server | Managed, backups included |
| Container registry | Azure Container Registry | Stores versioned Docker images |
| CI/CD | GitHub Actions | Triggers on push to `main`; deploys to production |
| Secrets management | Azure Key Vault / GitHub Secrets | JWT secrets, DB connection strings |

## Deployment Flow

```
Developer pushes code to GitHub
        ↓
GitHub Actions triggers pipeline
        ↓
1. dotnet build + dotnet test  (.NET)
2. ng build --configuration=production  (Angular)
3. docker build → docker push (API + Frontend images)
        ↓
Azure Container Apps pulls new images
        ↓
Rolling update (zero downtime)
        ↓
Rollback available via previous image tag
```
