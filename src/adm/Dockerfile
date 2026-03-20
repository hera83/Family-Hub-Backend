# ─────────────────────────────────────────────────────────────────────────────
# Multi-stage build for FamilyHub Admin (ADM)
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

# Build ADM project
RUN dotnet build src/adm/adm.csproj -c Release --no-restore

# Publish ADM project
RUN dotnet publish src/adm/adm.csproj -c Release -o /build/publish --no-build

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Copy published application from builder
COPY --from=builder /build/publish .

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Expose port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "adm.dll"]
