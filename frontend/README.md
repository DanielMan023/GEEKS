# GEEKS Frontend - Sistema de Autenticación

Este es el frontend del sistema de autenticación GEEKS, construido con React, TypeScript y Tailwind CSS.

## 🚀 Características

- **Login**: Formulario de inicio de sesión con validación
- **Registro**: Formulario de registro completo con validaciones
- **Dashboard**: Panel principal para usuarios autenticados
- **Autenticación**: Manejo de estado global con Context API
- **Responsive**: Diseño adaptativo para todos los dispositivos
- **TypeScript**: Tipado completo para mejor desarrollo

## 🛠️ Tecnologías

- React 18
- TypeScript
- Tailwind CSS
- Vite
- Axios
- React Router DOM

## 📦 Instalación

1. **Instalar dependencias:**
   ```bash
   npm install
   ```

2. **Configurar variables de entorno:**
   - El frontend está configurado para conectarse al backend en `http://localhost:5000`
   - Si tu backend corre en otro puerto, modifica `vite.config.ts`

## 🚀 Desarrollo

1. **Iniciar servidor de desarrollo:**
   ```bash
   npm run dev
   ```

2. **Abrir en el navegador:**
   - El frontend estará disponible en `http://localhost:3000`

## 🏗️ Build

1. **Construir para producción:**
   ```bash
   npm run build
   ```

2. **Previsualizar build:**
   ```bash
   npm run preview
   ```

## 📁 Estructura del Proyecto

```
src/
├── components/          # Componentes React
│   ├── Login.tsx       # Formulario de login
│   ├── Register.tsx    # Formulario de registro
│   └── Dashboard.tsx   # Panel principal
├── contexts/           # Contextos de React
│   └── AuthContext.tsx # Contexto de autenticación
├── services/           # Servicios y APIs
│   └── authService.ts  # Servicio de autenticación
├── types/              # Tipos TypeScript
│   └── auth.ts         # Tipos de autenticación
├── App.tsx             # Componente principal
├── main.tsx            # Punto de entrada
└── index.css           # Estilos globales
```

## 🔐 Endpoints del Backend

El frontend se conecta a estos endpoints:

- `POST /api/auth/login` - Inicio de sesión
- `POST /api/auth/register` - Registro de usuario
- `POST /api/auth/logout` - Cierre de sesión

## 🎨 Personalización

### Colores
Los colores principales se pueden modificar en `tailwind.config.js`:

```javascript
colors: {
  primary: {
    50: '#eff6ff',
    500: '#3b82f6',
    600: '#2563eb',
    700: '#1d4ed8',
  }
}
```

### Estilos
Los estilos personalizados están en `src/index.css` con clases utilitarias:

- `.btn-primary` - Botón principal
- `.btn-secondary` - Botón secundario
- `.input-field` - Campo de entrada
- `.form-card` - Tarjeta de formulario

## 📱 Responsive Design

El frontend está completamente optimizado para:
- Móviles (320px+)
- Tablets (768px+)
- Desktop (1024px+)

## 🔒 Seguridad

- Tokens JWT almacenados en localStorage
- Validación de formularios en frontend y backend
- Interceptores de Axios para headers de autorización
- Manejo seguro de errores

## 🚨 Solución de Problemas

### Error de conexión al backend
1. Verifica que el backend esté corriendo en `http://localhost:5000`
2. Revisa la configuración del proxy en `vite.config.ts`

### Problemas de build
1. Limpia la caché: `npm run clean`
2. Reinstala dependencias: `rm -rf node_modules && npm install`

### Errores de TypeScript
1. Verifica que todos los tipos estén correctamente definidos
2. Ejecuta: `npx tsc --noEmit`

## 📄 Licencia

Este proyecto es parte del sistema GEEKS de autenticación.

