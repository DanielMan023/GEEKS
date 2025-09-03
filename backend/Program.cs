using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GEEKS.Services;
using GEEKS.Interfaces;
using GEEKS.Data;
using GEEKS.Seeds;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configuración de JWT
var jwtKey = builder.Configuration["JWT:Key"] ?? "YourSuperSecretKeyHere12345678901234567890";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"] ?? "YourApp",
        ValidAudience = builder.Configuration["JWT:Audience"] ?? "YourAppUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("auth-token"))
            {
                context.Token = context.Request.Cookies["auth-token"];
            }
            return Task.CompletedTask;
        }
    };
});

// SERVICIOS DE AUTENTICACIÓN
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<DatabaseSeeder>();

// SERVICIOS DE IA Y CHATBOT
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();

// Contexto de base de datos
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseNpgsql(connectionString); // Cambiar a SQL Server si usas: options.UseSqlServer(connectionString);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Habilitar Swagger siempre para facilitar pruebas manuales
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Configurar archivos estáticos para las imágenes subidas
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Endpoint raíz para redirigir a Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Crear base de datos y ejecutar seeders
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DBContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("🚀 Iniciando configuración de base de datos...");
        
        // Aplicar migraciones para asegurar que la base de datos esté actualizada
        logger.LogInformation("📊 Aplicando migraciones...");
        await context.Database.MigrateAsync();
        logger.LogInformation("✅ Migraciones aplicadas correctamente");
        
        // Ejecutar seeder
        logger.LogInformation("🌱 Ejecutando seeder...");
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
        logger.LogInformation("✅ Seeder ejecutado correctamente");
        
        logger.LogInformation("🎉 Base de datos configurada y lista para usar");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error crítico ejecutando seeders: {Error}", ex.Message);
        
        // En desarrollo, mostrar el error completo
        if (app.Environment.IsDevelopment())
        {
            logger.LogError(ex, "Stack trace completo: {StackTrace}", ex.StackTrace);
        }
        
        // No lanzar la excepción para que la app pueda continuar
        // pero loguear el error para debugging
    }
}

app.Run();
