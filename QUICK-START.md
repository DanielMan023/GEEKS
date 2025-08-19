# 🚀 Inicio Rápido - Template de Autenticación

## ⚡ Configuración en 5 minutos

### 1. Copiar el Template
```bash
# Copia la carpeta auth-template a tu proyecto
cp -r auth-template/ /ruta/a/tu/proyecto/
cd /ruta/a/tu/proyecto/auth-template
```

### 2. Configurar Automáticamente (Recomendado)
```powershell
# En PowerShell como administrador
.\setup.ps1 -ProjectName "MiProyecto" -Namespace "MiProyecto" -DatabaseType "PostgreSQL"
```

### 3. Configurar Base de Datos
Edita `backend/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiProyectoDB;User Id=tuusuario;Password=tucontraseña;"
  }
}
```

### 4. Instalar Dependencias
```bash
# Backend
cd backend
dotnet restore

# Frontend
cd ../frontend
npm install
```

### 5. Crear Base de Datos
```bash
cd ../backend
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 6. Ejecutar
```bash
# Terminal 1 - Backend
cd backend
dotnet run

# Terminal 2 - Frontend  
cd frontend
npm run dev
```

### 7. ¡Listo! 🎉
- **Backend**: http://localhost:5000
- **Frontend**: http://localhost:5173
- **Swagger**: http://localhost:5000/swagger
- **Login**: admin@example.com / admin123

---

## 🔧 Configuración Manual

Si prefieres configurar manualmente:

1. **Cambiar namespace**: Reemplaza `YourNamespace` por tu namespace
2. **Cambiar nombre del proyecto**: Reemplaza `auth-template` por tu nombre
3. **Configurar base de datos**: Modifica `appsettings.json`
4. **Instalar paquetes**: Ejecuta `dotnet restore` y `npm install`

---

## 📚 Documentación Completa

Ver `README.md` para instrucciones detalladas y personalización avanzada.
