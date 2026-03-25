# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first so restore layer is cached independently of source
COPY src/CRM.Domain/CRM.Domain.csproj           src/CRM.Domain/
COPY src/CRM.Application/CRM.Application.csproj src/CRM.Application/
COPY src/CRM.Infrastructure/CRM.Infrastructure.csproj src/CRM.Infrastructure/
COPY src/CRM.API/CRM.API.csproj                 src/CRM.API/

RUN dotnet restore src/CRM.API/CRM.API.csproj

# Copy source and publish
COPY src/ src/
RUN dotnet publish src/CRM.API/CRM.API.csproj \
        -c Release \
        -o /app/publish \
        --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CRM.API.dll"]
