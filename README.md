# ⚡ TaskPro

**Autor:** Jonathan Blanco  
**Repositorio:** `https://github.com/JonathanRblanco/TaskPro`

---

## Tabla de Contenidos

- [Stack Tecnológico](#-stack-tecnológico)
- [Arquitectura](#-arquitectura)
- [Requisitos Previos](#-requisitos-previos)
- [Levantar con Docker](#-levantar-con-docker-recomendado)
- [Levantar en Local (sin Docker)](#-levantar-en-local-sin-docker)
- [Usuarios de Prueba](#-usuarios-de-prueba)
- [Variables de Entorno](#-variables-de-entorno)
- [Migraciones](#-migraciones)



## Stack Tecnológico

| Capa | Tecnología |
|---|---|
| Frontend | Next.js 15, React, Tailwind CSS v4 |
| Backend | .NET 8, C#, Entity Framework 8 |
| Base de datos relacional | SQL Server 2022 |
| Base de datos no relacional | MongoDB 7 |
| Autenticación | JWT Bearer |
| Contenedores | Docker + Docker Compose |
| Logging | NLog |

---

## Arquitectura

```
TaskPro/
├── BackEnd/
│   ├── TaskPro.Domain/          → Entidades, Value Objects, Enums, Interfaces
│   ├── TaskPro.Application/     → Casos de uso, DTOs, Servicios de aplicación
│   ├── TaskPro.Infrastructure/  → EF Core, Repositorios, MongoDB, Seeders
│   └── TaskPro.API/             → Controllers, Middlewares, Program.cs
│
└── FrontEnd/
    ├── app/                     → Páginas (App Router de Next.js)
    ├── components/              → Componentes reutilizables
    ├── hooks/                   → Custom hooks
    ├── lib/                     → Utilidades, validaciones, cliente Axios
    ├── styles/                  → Estilos globales de CSS
    └── types/                   → Tipos TypeScript del dominio
```

Los datos **relacionales** (usuarios, proyectos, tareas) se almacenan en **SQL Server**.  
Los **comentarios** de tareas se almacenan en **MongoDB**, aprovechando su esquema flexible.

---

##Requisitos Previos

### Con Docker _(recomendado)_
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y corriendo

### Sin Docker
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 20+](https://nodejs.org/)
- SQL Server (local o remoto)
- MongoDB (local o remoto)

---

## Levantar con Docker (Recomendado)

Este es el método más simple. Un solo comando levanta **frontend, backend, SQL Server y MongoDB** de forma aislada.

### 1. Clonar el repositorio

```bash
git clone https://github.com/JonathanRblanco/TaskPro
cd taskpro
```

### 2. Levantar todos los servicios

```bash
docker-compose up --build
```

### 3. Acceder a la aplicación

| Servicio | URL |
|---|---|
| 🌐 Frontend | http://localhost:3000 |
| 🔌 Backend API | http://localhost:5000 |
| 📄 Swagger | http://localhost:5000/swagger |

---

## 💻 Levantar en Local (sin Docker)

### Backend

```bash
cd BackEnd

# Restaurar dependencias
dotnet restore

# Configurar cadena de conexión en appsettings.Development.json
# (ver sección Variables de Entorno)

# Ejecutar la API
dotnet run --project TaskPro.API
```

> Las migraciones y el seed de datos se aplican automáticamente al iniciar la aplicación.

### Frontend

```bash
cd FrontEnd

# Instalar dependencias
npm install

# Iniciar en modo desarrollo
npm run dev
```

El frontend estará disponible en **http://localhost:3000**.

---

## 👤 Usuarios de Prueba

Al levantar la aplicación por primera vez, el sistema **crea automáticamente dos usuarios de prueba** para que puedas explorar todas las funcionalidades sin necesidad de registrarte.

| Rol | Email | Contraseña |
|---|---|---|
| 🔴 Administrador | `admin@taskpro.com` | `Admin123!` |
| 🔵 Miembro | `juan@taskpro.com` | `Member123!` |

---

## Migraciones

**No necesitas hacer nada manualmente.** Al iniciar la aplicación (tanto con Docker como en local), el sistema:

1. Detecta si hay migraciones de Entity Framework pendientes
2. Las aplica automáticamente sobre SQL Server
3. Verifica si la base de datos está vacía
4. Si está vacía, inserta los datos de prueba (usuarios, proyecto demo y tareas)

Si en algún momento necesitas crear una nueva migración (en desarrollo):

```bash
cd BackEnd

dotnet ef migrations add NombreDeLaMigracion \
  --project TaskPro.Infraestructure \
  --startup-project TaskPro.API
```

---

## Variables de Entorno

### Backend — `appsettings.json`

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost,1433;Database=TaskPro;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TaskProDb"
  },
  "Jwt": {
    "Secret": "tu-clave-secreta-de-al-menos-32-caracteres",
    "Issuer": "TaskPro.API",
    "Audience": "TaskPro.Client",
    "ExpirationMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000"
    ]
  }
}
```


> En Docker, estas variables se configuran automáticamente a través del `docker-compose.yml`.

