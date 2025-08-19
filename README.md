# 🔐 Sistema GEEKS



## 📋 Funcionalidades Implementadas

### ✅ **Backend (.NET Core)**
- **Autenticación JWT completa**
  - Login con email/password
  - Registro de nuevos usuarios
  - Logout con limpieza de cookies
  - Tokens seguros con BCrypt

- **Gestión de Roles**
  - Roles predefinidos (Admin, User)
  - Asignación automática de roles
  - Control de acceso por scope

- **Base de Datos PostgreSQL**
  - Migraciones automáticas
  - Seeding inicial de datos
  - Usuario admin por defecto

### 📁 **Frontend (Estructura Preparada)**
- Estructura organizada para React/TypeScript
- Carpetas para componentes, hooks, servicios
- Preparado para desarrollo futuro

## 🚀 Cómo Ejecutar

### **Requisitos Previos**
- .NET 9.0 SDK
- PostgreSQL
- Git

### **Configuración**
1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/tu-usuario/geeks-auth.git
   cd geeks-auth
   ```

2. **Configurar base de datos:**
   - Crear base de datos PostgreSQL: `geeks-auth`
   - Actualizar conexión en `backend/appsettings.json`

3. **Ejecutar el backend:**
   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```

4. **Abrir Swagger:**
   - URL: http://localhost:5000/swagger
   - Documentación completa de la API

## 👤 Usuario Demo

- **Email:** admin@example.com
- **Password:** admin123

## 🧪 Probar la API

### **1. Login**
```bash
POST /api/Auth/login
{
  "email": "admin@example.com",
  "password": "admin123"
}
```

### **2. Registro**
```bash
POST /api/Auth/register
{
  "email": "nuevo@usuario.com",
  "password": "password123",
  "firstName": "Nuevo",
  "lastName": "Usuario"
}
```

### **3. Logout**
```bash
POST /api/Auth/logout
```

## 🏗️ Arquitectura del Proyecto

```
GEEKS/
├── backend/                 # Backend .NET Core
│   ├── Controllers/        # Controladores API
│   ├── Models/            # Entidades de datos
│   ├── Services/          # Lógica de negocio
│   ├── Data/              # Contexto de base de datos
│   ├── Seeds/             # Datos iniciales
│   ├── Utils/             # Utilidades
│   └── Program.cs         # Configuración principal
├── frontend/              # Estructura frontend (preparada)
│   ├── components/        # Componentes React
│   ├── hooks/            # Custom hooks
│   ├── pages/            # Páginas
│   ├── services/         # Servicios API
│   └── types/            # Tipos TypeScript
└── README.md             # Este archivo
```

## 🛠️ Tecnologías Utilizadas

### **Backend**
- **.NET 9.0** - Framework principal
- **Entity Framework Core** - ORM para base de datos
- **PostgreSQL** - Base de datos relacional
- **JWT** - Autenticación por tokens
- **BCrypt.Net-Next** - Hashing de contraseñas
- **Swagger/OpenAPI** - Documentación de API

### **Frontend (Preparado)**
- **React** - Framework de interfaz
- **TypeScript** - Tipado estático
- **Axios** - Cliente HTTP

## 📊 Base de Datos

### **Tablas Principales**
- **Users** - Información de usuarios
- **Roles** - Roles del sistema
- **Base** - Clase base con auditoría

### **Datos Iniciales**
- **Roles:** Admin (ALL), User (USER)
- **Usuario Admin:** admin@example.com / admin123

## 🔧 Configuración

### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "PostgreSQLConnection": "Server=localhost;Port=5432;Database=geeks-auth;Username=postgres;Password=tu-password;"
  },
  "JWT": {
    "Key": "TuClaveSecretaSuperLarga123456789",
    "Issuer": "GEEKS",
    "Audience": "GEEKS-Users"
  }
}
```

## 🚀 Próximos Pasos

1. **Desarrollo Frontend** - Implementar interfaz de usuario
2. **Autenticación Frontend** - Integrar con backend
3. **Gestión de Usuarios** - CRUD completo
4. **Autorización Avanzada** - Middleware de roles
5. **Testing** - Unit tests y integration tests


