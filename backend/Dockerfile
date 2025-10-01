# backend/Dockerfile

# ---------------------
# Development stage
# ---------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /src

# copy csproj files first to leverage docker cache for restore
COPY Academix.API/Academix.API.csproj Academix.API/
COPY Academix.Application/Academix.Application.csproj Academix.Application/
COPY Academix.Domain/Academix.Domain.csproj Academix.Domain/
COPY Academix.Infrastructure/Academix.Infrastructure.csproj Academix.Infrastructure/

RUN dotnet restore Academix.API/Academix.API.csproj

# copy everything for dev
COPY . .
EXPOSE 5000
# run with watch for hot-reload in dev compose override
CMD ["dotnet", "watch", "run", "--project", "Academix.API/Academix.API.csproj", "--urls", "http://+:5000"]

# ---------------------
# Build stage
# ---------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Academix.API/Academix.API.csproj Academix.API/
COPY Academix.Application/Academix.Application.csproj Academix.Application/
COPY Academix.Domain/Academix.Domain.csproj Academix.Domain/
COPY Academix.Infrastructure/Academix.Infrastructure.csproj Academix.Infrastructure/

RUN dotnet restore Academix.API/Academix.API.csproj

COPY . .
RUN dotnet publish Academix.API/Academix.API.csproj -c Release -o /app

# ---------------------
# Production runtime stage
# ---------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS production
WORKDIR /app

# Copy published app
COPY --from=build /app .

# install curl (for container healthcheck)
RUN apt-get update \
  && apt-get install -y --no-install-recommends curl ca-certificates \
  && rm -rf /var/lib/apt/lists/*

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Academix.API.dll"]
