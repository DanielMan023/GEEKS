# 🚀 Configuración del Backend GEEKS

## 📋 **PASOS PARA CONFIGURAR EL PROYECTO**

### 1. **Configurar Base de Datos**
```bash
# Instalar PostgreSQL
# Crear base de datos: geeks-auth
# Actualizar ConnectionString en appsettings.json
```

### 2. **Configurar API Keys**

#### **Para Gemini Pro (Google Cloud):**
1. Ve a [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Crea una nueva API Key
3. Copia `appsettings.Example.json` a `appsettings.json`
4. Reemplaza `YOUR_GEMINI_API_KEY_HERE` con tu API Key real

#### **Para Google Cloud Service Account (Opcional):**
1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea un Service Account
3. Descarga el archivo JSON de credenciales
4. Colócalo en `credentials/service-account.json`

### 3. **Ejecutar el Proyecto**
```bash
cd backend
dotnet restore
dotnet run
```

## 🔒 **SEGURIDAD**

- **NUNCA** subas archivos con API Keys reales a GitHub
- Usa `appsettings.Development.json` para desarrollo local
- Usa variables de entorno en producción

## 📁 **ESTRUCTURA DE ARCHIVOS**

```
backend/
├── appsettings.json          # Configuración base (sin API Keys)
├── appsettings.Example.json  # Ejemplo con placeholders
├── credentials/              # Credenciales (ignorado por git)
│   └── service-account.json  # Google Cloud credentials
└── .gitignore               # Archivos a ignorar
```

## 🎯 **ENDPOINTS PRINCIPALES**

- `GET /api/Chatbot/health` - Estado del chatbot
- `POST /api/Chatbot/chat` - Enviar mensaje al chatbot
- `GET /api/Chatbot/test-ai` - Probar IA directamente

## 🤖 **CHATBOT CON GEMINI PRO**

El chatbot está integrado con **Google Cloud Gemini Pro** para:
- Respuestas inteligentes y contextuales
- Recomendaciones de productos
- Soporte en español
- Fallback inteligente si la IA falla
