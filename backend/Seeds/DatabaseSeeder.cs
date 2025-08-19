using GEEKS.Data;
using GEEKS.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GEEKS.Seeds
{
    public class DatabaseSeeder
    {
        private readonly DBContext _context;

        public DatabaseSeeder(DBContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                Console.WriteLine("🌱 Iniciando seeder de base de datos...");
                
                // Crear roles por defecto
                await SeedRolesAsync();
                
                // Crear usuario admin por defecto
                await SeedAdminUserAsync();
                
                // Guardar todos los cambios al final
                var changes = await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Seeder completado. {changes} cambios guardados.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en seeder: {ex.Message}");
                throw new Exception($"Error ejecutando seeders: {ex.Message}");
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!_context.Roles.Any())
            {
                Console.WriteLine("📝 Creando roles por defecto...");
                var roles = new List<Role>
                {
                    new Role
                    {
                        Name = "Admin",
                        Scope = "ALL",
                        Description = "Administrador del sistema",
                        State = "Active"
                    },
                    new Role
                    {
                        Name = "User",
                        Scope = "USER",
                        Description = "Usuario estándar",
                        State = "Active"
                    }
                };

                _context.Roles.AddRange(roles);
                Console.WriteLine("✅ Roles creados: Admin, User");
            }
            else
            {
                Console.WriteLine("ℹ️ Roles ya existen, saltando creación...");
            }
        }

        private async Task SeedAdminUserAsync()
        {
            if (!_context.Users.Any())
            {
                Console.WriteLine("👤 Creando usuario administrador...");
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    var adminUser = new User
                    {
                        Email = "admin@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        FirstName = "Admin",
                        LastName = "User",
                        RoleId = adminRole.Id,
                        State = "Active"
                    };

                    _context.Users.Add(adminUser);
                    Console.WriteLine("✅ Usuario admin creado: admin@example.com / admin123");
                }
                else
                {
                    Console.WriteLine("⚠️ No se encontró rol Admin para crear usuario");
                }
            }
            else
            {
                Console.WriteLine("ℹ️ Usuarios ya existen, saltando creación...");
            }
        }
    }
}
