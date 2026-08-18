# ============================================================
# Stage 1: Restore dependencies
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copy solution and project files first for layer caching
COPY Arak.sln ./
COPY Arak.PLL/Arak.PLL.csproj Arak.PLL/
COPY Arak.BLL/Arak.BLL.csproj Arak.BLL/
COPY Arak.DAL/Arak.DAL.csproj Arak.DAL/

# Restore packages (cached unless .csproj files change)
RUN dotnet restore Arak.PLL/Arak.PLL.csproj

# ============================================================
# Stage 2: Build
# ============================================================
FROM restore AS build
COPY . .

# Build in Release mode
RUN dotnet publish Arak.PLL/Arak.PLL.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================================
# Stage 3: Runtime (final image)
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Install curl for health checks (before switching user)
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Security: run as non-root user
RUN groupadd -r arak && useradd -r -g arak arak

WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Create directory for uploaded files (wwwroot)
RUN mkdir -p /app/wwwroot && chown -R arak:arak /app

# Switch to non-root user
USER arak

# Expose API port
EXPOSE 5000

# Environment variables
ENV ASPNETCORE_URLS=http://+:5000 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:5000/api/metrics || exit 1

ENTRYPOINT ["dotnet", "Arak.PLL.dll"]
