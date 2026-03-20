# FamilyHub API & Admin Portal

> Backend API og administration system for FamilyHub. Hele løsningen kan køres lokalt eller deployed via Docker på production-servere.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet) ![License](https://img.shields.io/badge/license-MIT-green) ![Status](https://img.shields.io/badge/status-stable-brightgreen)

---

## 📋 Indholdsfortegnelse

1. [Overblik](#overblik)
2. [Hurtig start](#hurtig-start)
3. [Lokal udvikling](#lokal-udvikling)
4. [Docker deployment](#docker-deployment)
5. [API dokumentation](#api-dokumentation)
6. [Admin portal (ADM)](#admin-portal-adm)
7. [Konfiguration](#konfiguration)
8. [Troubleshooting](#troubleshooting)
9. [Arkitektur](#arkitektur)

---

## 🎯 Overblik

FamilyHub er et system til at styre familiedata i et touch-screen interface. Løsningen består af:

| Komponent | Beskrivelse | Teknologi |
|-----------|-------------|-----------|
| **API** | REST backend med SQLite database | .NET 10, EF Core, Swagger |
| **ADM** | Admin portal til data-management | .NET 10 Razor Pages, Excel import/export |
| **Docker** | Production deployment | Docker Compose, multi-stage builds |

### Stack
- **.NET 10** / ASP.NET Core
- **Entity Framework Core** + SQLite
- **Swagger/OpenAPI** for API dokumentation
- **Docker/Docker Compose** for containerization
- **Health Checks** for monitoring

---

## 🚀 Hurtig start

### Lokal udvikling (Windows/Mac/Linux)

```bash
# 1. Klon projektet
git clone <repository-url>
cd Family-Hub\ API

# 2. Start API
cd src/api
dotnet run

# API er nu tilgængelig: http://localhost:5000

# 3. Start ADM (i anden terminal)
cd src/adm
dotnet run

# ADM er nu tilgængelig: http://localhost:5000 eller https://localhost:5001
```

### Docker deployment (Ubuntu 24.04)

```bash
# 1. Klon projektet
git clone <repository-url>
cd Family-Hub\ API

# 2. Opsæt miljø
cp .env.example .env
# Rediger .env og skift API_KEY til sikker værdi

# 3. Start services
docker-compose up -d

# API: http://localhost:5000
# ADM: http://localhost:5001
```

---

## 💻 Lokal udvikling

### Forudsætninger

- .NET 10 SDK (https://dotnet.microsoft.com/download)
- SQLite (automatisk via NuGet)
- Terminal/PowerShell

### Startup API

```bash
cd src/api
dotnet run
```

**Automatisk ved startup:**
- ✅ EF Core migrations køres (Database:AutoMigrateOnStartup)
- ✅ SQLite database oprettes
- ✅ Demo seed-data indsættes (Database:SeedOnStartup)

**Tilgængelig på:**
- Swagger UI: http://localhost:5000/swagger (eller https://localhost:5001/)
- Health check: http://localhost:5000/health
- API root: http://localhost:5000/api/v1/

### Startup ADM

```bash
cd src/adm
dotnet run
```

**ADM forbinder til API på:**
- Development: http://localhost:5000/ (fra appsettings.Development.json)

**Tilgængelig på:**
- https://localhost:5001/ (eller http://localhost:5000/)

### Database & Migrations

**Opret ny migration:**
```bash
cd src/api
dotnet ef migrations add <MigrationName> --output-dir Infrastructure/Persistence/Migrations
```

**Kør migrationer manuelt:**
```bash
dotnet ef database update
```

**Database-konfiguration** (src/api/appsettings.json):
```json
{
  "Database": {
    "Path": "familyhub.db",
    "AutoMigrateOnStartup": true,
    "RecreateIfNoMigrations": false,
    "SeedOnStartup": true
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=familyhub.db"
  }
}
```

### Seed-data

Seed-systemet (Development) opretter demo-data:
- 3 family members
- 5 calendar events
- 5 item categories
- 10 products
- 4 recipe categories
- 5 recipes + ingredients
- 3 orders + order lines

Se: [src/api/Infrastructure/Persistence/Seed/DevelopmentDatabaseSeeder.cs](src/api/Infrastructure/Persistence/Seed/DevelopmentDatabaseSeeder.cs)

---

## 🐳 Docker Deployment

FamilyHub er fuldt dockeriseret og deployable på Linux/Ubuntu-servere.

### Installation på serveren

```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo bash get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/download/v2.24.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify
docker --version
docker-compose --version
```

### Setup & Start

```bash
# 1. Klon/upload løsningen
git clone <repo-url> /opt/family-hub-api
cd /opt/family-hub-api

# 2. Opret .env
cp .env.example .env
nano .env

# Vigtige indstillinger i .env:
# API_KEY=YOUR_SUPER_SECURE_KEY_HERE
# API_HOST_PORT=5000
# ADM_HOST_PORT=8080

# 3. Build og start
docker-compose up -d

# 4. Tjek status
docker-compose ps
docker-compose logs -f api
```

### Services & Porte

| Service | Host Port | Container Port | URL |
|---------|-----------|-----------------|-----|
| API | 5000 | 8080 | http://localhost:5000 |
| ADM | 5001 | 8080 | http://localhost:5001 |

### Daglig drift

```bash
# Se logs
docker-compose logs -f api
docker-compose logs --tail 50 adm

# Stop services
docker-compose stop

# Restart services
docker-compose start

# Fuld shutdown
docker-compose down

# Rebuild efter kodeændringer
docker-compose up -d --build
```

### Container health check

```bash
# Test API health
curl http://localhost:5000/health

# Respons skal være: {"status":"Healthy"}
```

### Konfiguration via miljøvariabler

Alle indstillinger styres fra `.env`:

```ini
# Fælles
API_KEY=dev-api-key-12345

# API
Database__Path=/data/familyhub.db
ApiKey__HeaderName=x-api-key
Swagger__EnableInProduction=true

# ADM
FamilyHubApi__BaseUrl=http://api:8080/
FamilyHubApi__ApiKeyHeaderName=x-api-key
```

Se [.env.example](.env.example) for alle tilgængelige variabler.

### Database persistent storage

- SQLite ligger i container på: `/data/familyhub.db`
- Docker volume: `familyhub-data` (persistent)
- Data bevares selvom container slettes

**Backup database:**
```bash
docker exec familyhub-api sqlite3 /data/familyhub.db ".dump" > backup-$(date +%Y%m%d).sql
```

**Restore database:**
```bash
docker exec -i familyhub-api sqlite3 /data/familyhub.db < backup-20260320.sql
```

### HTTPS & Reverse Proxy (produktion)

Brug Nginx eller anden reverse proxy:

```nginx
server {
    listen 443 ssl http2;
    server_name api.example.com;

    ssl_certificate /etc/letsencrypt/live/api.example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.example.com/privkey.pem;

    location / {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Firewall (produktion)

```bash
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw deny 5000/tcp   # Block direct Docker port
sudo ufw deny 8080/tcp   # Block direct Docker port
sudo ufw enable
```

---

## 📡 API Dokumentation

### REST Endpoints

Alle endpoints er dokumenteret i **Swagger UI**:
- Local: http://localhost:5000/swagger
- Docker: http://localhost:5000/swagger

### API Key authentifikation

API'et kræver API key i header:

```bash
curl -H "x-api-key: dev-api-key-12345" http://localhost:5000/api/v1/recipes
```

**Swagger test:**
1. Klik låse-ikonet øverst til højre
2. Skriv API key i "ApiKey"-feltet
3. Klik "Authorize"

### Endpoint-grupper

```
GET    /api/v1/calendar/*         # Familie medlemmer og begivenheder
GET    /api/v1/catalog/*          # Produkter og kategorier
GET    /api/v1/recipes/*          # Opskrifter og ingredienser
GET    /api/v1/orders/*           # Ordrer og ordrelinjer
POST   /api/v1/sync/*             # Data sync for frontend
GET    /api/v1/health             # Health check
```

### Health endpoints

```bash
# Standard health endpoint (auth ikke påkrævet)
GET /health

# Detailed health check (auth ikke påkrævet)
GET /api/v1/health
```

### Response format

**Enkelt objekt:**
```json
{
  "id": 1,
  "name": "Family Member Name",
  ...
}
```

**Paged liste:**
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5
}
```

**Fejl:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7331#section-11.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Invalid input"
}
```

### Orders (Snapshot model)

Orders er en historisk snapshot med tilhørende order lines.

```bash
# Alle ordrer (pageret)
GET /api/v1/orders?status=pending&page=1&pageSize=10

# Ordre detaljer
GET /api/v1/orders/{id}

# Ordre PDF
GET /api/v1/orders/{id}/pdf

# Slet ordre
DELETE /api/v1/orders/{id}
```

### Sync endpoints

Optimeret til frontend-synkronisering:

```bash
# Fuld sync (alle data)
GET /api/v1/sync/full

# Manifest (statuser og counts)
GET /api/v1/sync/manifest

# Changes siden timestamp
GET /api/v1/sync/changes?since=2026-03-20T10:00:00Z
```

### Frontend integration

**JavaScript/React eksempel:**

```javascript
const API_URL = 'http://localhost:5000';
const API_KEY = 'dev-api-key-12345';

const response = await fetch(`${API_URL}/api/v1/recipes`, {
  headers: {
    'x-api-key': API_KEY,
    'Content-Type': 'application/json'
  }
});

const data = await response.json();
```

---

## 🎛️ Admin Portal (ADM)

Razor Pages admin portal til data management.

### Features

- 📊 **Dashboard**: API health status + data oversigt
- 📅 **Calendar**: Familie medlemmer og begivenheder CRUD
- 📦 **Catalog**: Produkter og kategorier CRUD
- 📋 **Recipes**: Opskrifter og ingredienser CRUD
- 📬 **Orders**: Ordre-liste, detaljer, PDF, slet
- 📊 **Import/Export**: Excel-baseret data import og export

### Import/Export

**Export data til Excel:**
1. Gå til "Import & Eksport"
2. Klik "Eksportér" for datasættet
3. Excel-fil med aktuelle live-data gemmes

**Download import-skabelon:**
1. Klik "Skabelon" knap
2. Modtager blank Excel-fil med:
   - Farvede header-rækker
   - Eksempel-data
   - Help-sheet med kolone-beskrivelser
   - Live lookup-sheets (for FK-referencer)

**Importér data:**
1. Udfyld eksporteret eller skabelon-fil
2. Upload på "Import" side
3. Gennemse "Preview" for fejl
4. Bekræft og importér gyldig data
5. Se resultat-oversigt (created/updated/failed)

**Upsert-regel:**
- Hvis `Id` findes: opdatér record
- Hvis `Id` er tom: opret ny record

### Konfiguration

ADM forbinder til API via miljøvariabler:

```json
{
  "FamilyHubApi": {
    "BaseUrl": "http://localhost:5000/",
    "ApiKeyHeaderName": "x-api-key",
    "ApiKey": "dev-api-key-12345"
  }
}
```

**Docker:** ADM bruger `http://api:8080/` internt via Docker network.

### API Client-bibliotek

Alle HTTP-clients er automatisk konfigureret med API key via `ApiKeyHeaderHandler`.

Se: [src/adm/Infrastructure/Clients/ServiceCollectionExtensions.cs](src/adm/Infrastructure/Clients/ServiceCollectionExtensions.cs)

---

## ⚙️ Konfiguration

### Environment variable references

ASP.NET Core konverterer automatisk `:` til `__` for environment variables.

**appsettings.json:**
```json
{
  "Database": {
    "Path": "familyhub.db"
  }
}
```

**Environment variable:**
```bash
Database__Path=/data/familyhub.db
```

### API-specifik konfiguration

**src/api/appsettings.json:**

```json
{
  "Database": {
    "Path": "familyhub.db",
    "AutoMigrateOnStartup": true,
    "SeedOnStartup": true
  },
  "ApiKey": {
    "Enabled": true,
    "HeaderName": "x-api-key",
    "Key": "dev-api-key-12345"
  },
  "Swagger": {
    "EnableInProduction": true
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:5001"
    ]
  }
}
```

### ADM-specifik konfiguration

**src/adm/appsettings.json:**

```json
{
  "FamilyHubApi": {
    "BaseUrl": "http://localhost:5000/",
    "ApiKeyHeaderName": "x-api-key",
    "ApiKey": "dev-api-key-12345"
  }
}
```

---

## 🔧 Troubleshooting

### API starter ikke

**Problem:** `dotnet run` fejler i src/api/

**Løsning:**
```bash
# Rens og genbyg
dotnet clean
dotnet restore
dotnet build
dotnet run
```

### ADM kan ikke nå API

**Problem:** ADM viser "Connection refused"

**Tjek:**
1. API er startet: `curl http://localhost:5000/health`
2. API URL i appsettings er korrekt
3. API Key matcher

**Docker:**
```bash
# Test fra ADM container
docker exec familyhub-adm curl http://api:8080/health
```

### Docker container restarter

**Problem:** `docker-compose ps` viser `Exited`

**Løsning:**
```bash
# Se fejlen
docker-compose logs api

# Nulstil database
docker volume rm familyhub-data
docker-compose up -d
```

### Port allerede i brug

**Problem:** Port 5000 eller 8080 konflikt

**Løsning 1 - Docker:**
```yaml
# docker-compose.yml
api:
  ports:
    - "5001:8080"  # Skift host-port
```

**Løsning 2 - Find hvad der bruger porten:**
```bash
lsof -i :5000
# Kill processen
kill -9 <PID>
```

### Database ødelagt

**Problem:** "Database is locked" eller migration fejl

**Løsning:**
```bash
# Lokal dev
rm familyhub.db
dotnet run  # Genopret database

# Docker
docker volume rm familyhub-data
docker-compose up -d  # Genopret volume
```

---

## 🏗️ Arkitektur

### Løsning struktur

```
Family-Hub API/
├── FamilyHub.sln                    # Solution file
├── docker-compose.yml               # Docker orchestration
├── Dockerfile                       # (i src/api og src/adm)
├── .dockerignore                    # Build cleanup
├── .env.example                     # Environment template
│
├── src/
│   ├── api/                         # REST API project
│   │   ├── Controllers/             # Route handlers
│   │   ├── Features/                # Domain logic (Calendar, Catalog, etc.)
│   │   ├── Entities/                # EF Core data models
│   │   ├── Contracts/               # Request/Response DTOs
│   │   ├── Infrastructure/
│   │   │   ├── Persistence/         # EF Core DbContext & migrations
│   │   │   ├── Middleware/          # HTTP middleware
│   │   │   └── Common/
│   │   ├── appsettings.json
│   │   ├── Program.cs               # DI & pipeline setup
│   │   └── api.csproj
│   │
│   └── adm/                         # Admin portal project
│       ├── Pages/                   # Razor Pages (.cshtml)
│       ├── Services/                # Business logic
│       ├── Infrastructure/Clients/   # HTTP clients to API
│       ├── Models/                  # View models
│       ├── wwwroot/                 # StaticFiles (CSS, JS)
│       ├── appsettings.json
│       ├── Program.cs               # DI & pipeline setup
│       └── adm.csproj
│
└── README.md                        # This file
```

### Technology decisions

| Valg | Årsag |
|------|-------|
| SQLite | Simpel lokal opstart, ingen server |
| EF Core | Type-safe queries, migrations |
| Razor Pages | Hurtig admin UI uden JS framework |
| Docker | Easy deployment på Linux/Ubuntu servers |
| Swagger | API dokumentation + testing |

### V1 limitations

- ⚠️ Ingen authentication/authorization (intern brug)
- ⚠️ Ingen soft delete
- ⚠️ Ingen deletion tracking i sync changes
- ⚠️ SQLite (ikke scalable til mange concurrent users)

---

## 📝 License

MIT License - se [LICENSE](LICENSE) fil.

---

## 🤝 Bidrag

Contributions er velkomne! Fork projektet og send pull requests.

---

**Sidst opdateret:** Marts 2026  
**Version:** 1.0  
**Status:** Production-ready ✅
