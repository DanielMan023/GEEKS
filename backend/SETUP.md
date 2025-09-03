# Configuración del Backend - GEEKS

## Configuración Inicial

### 1. Configurar appsettings.json

Copia el archivo de ejemplo y configura tus credenciales:

```bash
cp appsettings.Example.json appsettings.json
```

### 2. Configurar Base de Datos PostgreSQL

1. Instala PostgreSQL en tu sistema
2. Crea una base de datos llamada `geeks-auth`
3. Actualiza la cadena de conexión en `appsettings.json`:

```json
"ConnectionStrings": {
  "PostgreSQLConnection": "Server=localhost;Port=5432;Database=geeks-auth;Username=postgres;Password=TU_PASSWORD_AQUI;"
}
```

### 3. Configurar Google Cloud Gemini API

1. Ve a [Google AI Studio](https://aistudio.google.com/)
2. Crea un nuevo proyecto o usa uno existente
3. Genera una API Key para Gemini
4. Actualiza la configuración en `appsettings.json`:

```json
"GoogleCloud": {
  "ProjectId": "tu-project-id-aqui",
  "Location": "us-central1",
  "Model": "gemini-1.5-flash",
  "MaxTokens": 500,
  "Temperature": 0.7,
  "CredentialsPath": "credentials/service-account.json",
  "ApiKey": "TU_GEMINI_API_KEY_AQUI"
}
```

### 4. Ejecutar Migraciones

```bash
dotnet ef database update
```

### 5. Ejecutar el Backend

```bash
dotnet run
```

El backend estará disponible en `http://localhost:5000`

## Características del Chatbot

- **Intent Detection**: Detecta automáticamente la intención del usuario
- **Product Recommendations**: Recomienda productos del catálogo
- **Specifications**: Proporciona especificaciones técnicas detalladas de cualquier producto
- **Comparisons**: Compara productos técnicamente
- **Context Awareness**: Mantiene contexto de la conversación
- **Gemini Pro Integration**: Usa Google Cloud Gemini Pro para respuestas inteligentes

## Endpoints del Chatbot

- `POST /api/Chatbot/chat` - Enviar mensaje al chatbot
- `GET /api/Chatbot/recommendations` - Obtener recomendaciones
- `GET /api/Chatbot/context` - Obtener contexto del chatbot
- `GET /api/Chatbot/health` - Verificar estado del chatbot
- `POST /api/Chatbot/test-chat` - Endpoint de prueba (sin autenticación)