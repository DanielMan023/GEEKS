# Script de configuración automática para el template de autenticación
# Ejecutar en PowerShell como administrador

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,
    
    [Parameter(Mandatory=$true)]
    [string]$Namespace,
    
    [Parameter(Mandatory=$false)]
    [string]$DatabaseType = "PostgreSQL"
)

Write-Host "🚀 Configurando template de autenticación para: $ProjectName" -ForegroundColor Green

# Función para reemplazar texto en archivos
function Replace-TextInFiles {
    param(
        [string]$SearchText,
        [string]$ReplaceText,
        [string]$FileExtension
    )
    
    Get-ChildItem -Recurse -Include "*.$FileExtension" | ForEach-Object {
        Write-Host "Procesando: $($_.Name)" -ForegroundColor Yellow
        (Get-Content $_.FullName) | 
        ForEach-Object { $_ -replace $SearchText, $ReplaceText } | 
        Set-Content $_.FullName
    }
}

# 1. Reemplazar YourNamespace por el namespace del proyecto
Write-Host "📝 Configurando namespace..." -ForegroundColor Blue
Replace-TextInFiles -SearchText "YourNamespace" -ReplaceText $Namespace -FileExtension "cs"

# 2. Reemplazar auth-template por el nombre del proyecto
Write-Host "📝 Configurando nombre del proyecto..." -ForegroundColor Blue
Replace-TextInFiles -SearchText "auth-template" -ReplaceText $ProjectName -FileExtension "csproj"

# 3. Configurar base de datos según el tipo
if ($DatabaseType -eq "SQLServer") {
    Write-Host "🗄️ Configurando para SQL Server..." -ForegroundColor Blue
    
    # Reemplazar en Program.cs
    $programContent = Get-Content "backend/Program.cs" -Raw
    $programContent = $programContent -replace "options\.UseNpgsql\(connectionString\);", "options.UseSqlServer(connectionString);"
    $programContent = $programContent -replace "Npgsql\.EntityFrameworkCore\.PostgreSQL", "Microsoft.EntityFrameworkCore.SqlServer"
    Set-Content "backend/Program.cs" $programContent
    
    # Actualizar .csproj
    $csprojContent = Get-Content "backend/$ProjectName.csproj" -Raw
    $csprojContent = $csprojContent -replace "Npgsql\.EntityFrameworkCore\.PostgreSQL", "Microsoft.EntityFrameworkCore.SqlServer"
    $csprojContent = $csprojContent -replace "8\.0\.0", "8.0.0"
    Set-Content "backend/$ProjectName.csproj" $csprojContent
    
    Write-Host "✅ Configurado para SQL Server" -ForegroundColor Green
} else {
    Write-Host "✅ Configurado para PostgreSQL" -ForegroundColor Green
}

# 4. Crear archivo de configuración personalizado
Write-Host "⚙️ Creando archivo de configuración..." -ForegroundColor Blue
$appsettingsContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=$($ProjectName)DB;User Id=youruser;Password=yourpassword;"
  },
  "JWT": {
    "Key": "YourSuperSecretKeyHere12345678901234567890",
    "Issuer": "$ProjectName",
    "Audience": "$($ProjectName)Users"
  }
}
"@

Set-Content "backend/appsettings.json" $appsettingsContent

# 5. Crear archivo .gitignore
Write-Host "📁 Creando .gitignore..." -ForegroundColor Blue
$gitignoreContent = @"
# .NET
bin/
obj/
*.user
*.suo
*.cache
*.dll
*.exe
*.pdb

# Logs
logs/
*.log

# Environment variables
.env
.env.local

# IDE
.vs/
.vscode/
*.swp
*.swo

# OS
.DS_Store
Thumbs.db
"@

Set-Content ".gitignore" $gitignoreContent

# 6. Crear script de migración
Write-Host "🗃️ Creando script de migración..." -ForegroundColor Blue
$migrationScript = @"
# Script de migración para $ProjectName
# Ejecutar en la carpeta backend

Write-Host "Creando migración inicial..." -ForegroundColor Green
dotnet ef migrations add InitialCreate

Write-Host "Aplicando migración a la base de datos..." -ForegroundColor Green
dotnet ef database update

Write-Host "✅ Migración completada!" -ForegroundColor Green
"@

Set-Content "migrate.ps1" $migrationScript

Write-Host "`n🎉 ¡Template configurado exitosamente!" -ForegroundColor Green
Write-Host "`n📋 Próximos pasos:" -ForegroundColor Yellow
Write-Host "1. Configura tu cadena de conexión en backend/appsettings.json" -ForegroundColor White
Write-Host "2. Ejecuta 'dotnet restore' en la carpeta backend" -ForegroundColor White
Write-Host "3. Ejecuta 'npm install' en la carpeta frontend" -ForegroundColor White
Write-Host "4. Ejecuta 'migrate.ps1' para crear la base de datos" -ForegroundColor White
Write-Host "5. ¡Listo para usar!" -ForegroundColor White

Write-Host "`n🔐 Usuario por defecto: admin@example.com / admin123" -ForegroundColor Cyan
