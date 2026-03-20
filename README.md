# FamilyHub Backend

FamilyHub Backend contains two .NET 10 applications:

- api: REST API with SQLite
- adm: Razor Pages administration UI

The solution is prepared for local development and Docker deployment on Ubuntu.

## Contents

1. Overview
2. Prerequisites
3. Local development
4. Docker deployment
5. Configuration
6. API usage
7. Troubleshooting

## Overview

Projects in this repository:

- src/api: ASP.NET Core Web API
- src/adm: ASP.NET Core Razor Pages admin portal
- docker-compose.yml: local and server orchestration

Tech stack:

- .NET 10
- ASP.NET Core
- Entity Framework Core
- SQLite
- Docker and Docker Compose

## Prerequisites

Install:

- .NET 10 SDK
- Docker (for container flow)
- Docker Compose v2

## Local development

Start API:

```bash
cd src/api
dotnet run
```

Start ADM in a second terminal:

```bash
cd src/adm
dotnet run
```

Default local endpoints:

- API: <http://localhost:5000>
- API health: <http://localhost:5000/health>
- Swagger: <http://localhost:5000/swagger>
- ADM: <http://localhost:5001>

## Docker deployment

Create environment file:

```bash
cp .env.example .env
```

Build and start:

```bash
docker compose up --build -d
```

Stop:

```bash
docker compose down
```

Service ports:

- API host port 5000 mapped to container port 8080
- ADM host port 5001 mapped to container port 8080

Persistent data:

- SQLite is stored in Docker volume familyhub-data
- Container path is /data/familyhub.db

## Configuration

Main environment variables:

- API_KEY
- ASPNETCORE_ENVIRONMENT
- ASPNETCORE_URLS
- ConnectionStrings__DefaultConnection
- Database__Path
- ApiKey__HeaderName
- ApiKey__Key
- FamilyHubApi__BaseUrl
- FamilyHubApi__ApiKeyHeaderName
- FamilyHubApi__ApiKey

ADM connects to API through Docker network using:

- FamilyHubApi__BaseUrl=<http://api:8080/>

## API usage

Health endpoints do not require API key:

- /health
- /api/v1/health

Other API endpoints require x-api-key header.

Example request:

```bash
curl -H "x-api-key: dev-api-key-12345" http://localhost:5000/api/v1/recipes
```

Main endpoint groups:

- /api/v1/calendar/*
- /api/v1/catalog/*
- /api/v1/orders/*
- /api/v1/recipes/*
- /api/v1/sync/*

## Troubleshooting

ADM has no styling:

- Rebuild containers: docker compose up --build -d
- Hard refresh browser
- Verify static files:
  - <http://localhost:5001/lib/bootstrap/dist/css/bootstrap.min.css>
  - <http://localhost:5001/css/site.css>

API is up but ADM cannot connect:

- Check logs: docker compose logs -f adm
- Check API health: <http://localhost:5000/health>
- Ensure API key in ADM matches API key in API

## License

MIT
