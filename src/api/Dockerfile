# ─────────────────────────────────────────────────────────────────────────────
# Multi-stage build for FamilyHub API
# Stage 1: Build
# Stage 2: Runtime
# ─────────────────────────────────────────────────────────────────────────────

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /build

# Copy solution and project files
COPY FamilyHub.sln .
COPY src/api/api.csproj src/api/
COPY src/adm/adm.csproj src/adm/

# Restore dependencies
RUN dotnet restore FamilyHub.sln

# Copy full source code
COPY src/api src/api
COPY src/adm src/adm

# Build API project
RUN dotnet build src/api/api.csproj -c Release --no-restore

# Publish API project
RUN dotnet publish src/api/api.csproj -c Release -o /build/publish --no-build

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Copy published application from builder
COPY --from=builder /build/publish .

# Create data directory for SQLite
RUN mkdir -p /data

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "api.dll"]
