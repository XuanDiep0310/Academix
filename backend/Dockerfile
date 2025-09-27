# backend/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Academix.API/Academix.API.csproj Academix.API/
COPY Academix.Application/Academix.Application.csproj Academix.Application/
COPY Academix.Domain/Academix.Domain.csproj Academix.Domain/
COPY Academix.Infrastructure/Academix.Infrastructure.csproj Academix.Infrastructure/

RUN dotnet restore Academix.API/Academix.API.csproj

COPY . .
RUN dotnet publish Academix.API/Academix.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Academix.API.dll"]
