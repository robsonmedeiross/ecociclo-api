# Etapa 1: Compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["EcoCiclo.API/EcoCiclo.API.csproj", "EcoCiclo.API/"]
COPY ["EcoCiclo.Domain/EcoCiclo.Domain.csproj", "EcoCiclo.Domain/"]
COPY ["EcoCiclo.Infrastructure/EcoCiclo.Infrastructure.csproj", "EcoCiclo.Infrastructure/"]

RUN dotnet restore "EcoCiclo.API/EcoCiclo.API.csproj"

COPY . .

RUN dotnet publish "EcoCiclo.API/EcoCiclo.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Etapa 2: Executar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "EcoCiclo.API.dll"]