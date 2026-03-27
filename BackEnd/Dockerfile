# Etapa 1 — BUILD: compila el código
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los .csproj primero para aprovechar el cache de capas.
# Si el código cambia pero no las dependencias, Docker no reinstala los paquetes.
COPY ["TaskPro.API/TaskPro.API.csproj","TaskPro.API/"]
COPY ["TaskPro.Application/TaskPro.Application.csproj", "TaskPro.Application/"]
COPY ["TaskPro.Domain/TaskPro.Domain.csproj", "TaskPro.Domain/"]
COPY ["TaskPro.Infraestructure/TaskPro.Infraestructure.csproj", "TaskPro.Infraestructure/"]

RUN dotnet restore "TaskPro.API/TaskPro.API.csproj"

# Ahora copia el resto del código y compila
COPY . .
RUN dotnet publish "TaskPro.API/TaskPro.API.csproj" \
    -c Release -o /app/publish

# Etapa 2 — RUNTIME: solo lo necesario para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskPro.API.dll"]