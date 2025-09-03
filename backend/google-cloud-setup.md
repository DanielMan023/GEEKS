# 🚀 Configuración de Google Cloud + Gemini Pro

## 📋 **PASOS PARA CONFIGURAR GOOGLE CLOUD:**

### **1. CREAR PROYECTO EN GOOGLE CLOUD:**
1. Ve a [console.cloud.google.com](https://console.cloud.google.com)
2. **Crea un nuevo proyecto** o selecciona uno existente
3. **Anota el Project ID** (lo necesitarás para la configuración)

### **2. HABILITAR VERTEX AI API:**
1. Ve a **"APIs & Services"** → **"Library"**
2. Busca **"Vertex AI API"**
3. **Habilita la API**

### **3. CREAR CUENTA DE SERVICIO:**
1. Ve a **"IAM & Admin"** → **"Service Accounts"**
2. **Crea una nueva cuenta de servicio**
3. **Asigna el rol "Vertex AI User"**
4. **Descarga el archivo JSON** de credenciales

### **4. CONFIGURAR CREDENCIALES:**
1. **Coloca el archivo JSON** en la carpeta del proyecto
2. **Actualiza la ruta** en `appsettings.json`
3. **Configura las variables de entorno**

### **5. ACTUALIZAR APPSETTINGS.JSON:**
```json
"GoogleCloud": {
  "ProjectId": "TU-PROJECT-ID-AQUI",
  "Location": "us-central1",
  "Model": "gemini-1.5-flash",
  "MaxTokens": 500,
  "Temperature": 0.7,
  "CredentialsPath": "path/to/service-account.json"
}
```

## 🔑 **VARIABLES DE ENTORNO (ALTERNATIVA):**
```bash
GOOGLE_APPLICATION_CREDENTIALS=path/to/service-account.json
GOOGLE_CLOUD_PROJECT=tu-project-id
```

## 📍 **UBICACIONES DISPONIBLES:**
- `us-central1` (Iowa) - **RECOMENDADO**
- `us-east1` (South Carolina)
- `europe-west1` (Belgium)
- `asia-northeast1` (Tokyo)

## 🤖 **MODELOS DISPONIBLES:**
- `gemini-1.5-flash` - **RECOMENDADO** (rápido y económico)
- `gemini-1.5-pro` - Más potente pero más lento
- `gemini-1.0-pro` - Versión anterior

## 💰 **COSTOS APROXIMADOS:**
- **Gemini 1.5 Flash**: ~$0.000075 por 1K tokens
- **Gemini 1.5 Pro**: ~$0.0035 por 1K tokens
- **Cuota gratuita**: $300 por 90 días

## ✅ **VERIFICAR CONFIGURACIÓN:**
1. **Compila el proyecto**: `dotnet build`
2. **Ejecuta el backend**: `dotnet run`
3. **Prueba el endpoint**: `/api/Chatbot/test-openai-direct`

## 🆘 **SOLUCIÓN DE PROBLEMAS:**
- **Error 403**: Verifica permisos de la cuenta de servicio
- **Error 404**: Verifica que la API esté habilitada
- **Error de autenticación**: Verifica la ruta del archivo JSON
