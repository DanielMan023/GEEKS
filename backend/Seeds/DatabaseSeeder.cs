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
                
                // Crear roles primero y guardarlos
                await SeedRolesAsync();
                
                // Crear usuario admin después
                await SeedAdminUserAsync();
                
                Console.WriteLine("✅ Seeder completado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en seeder: {ex.Message}");
                throw new Exception($"Error ejecutando seeders: {ex.Message}");
            }
        }

        private async Task SeedRolesAsync()
        {
            // Verificar si ya existen roles
            var existingRoles = await _context.Roles.ToListAsync();
            
            if (!existingRoles.Any())
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
                
                // Guardar roles inmediatamente para obtener los IDs
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Roles creados y guardados: Admin, User");
            }
            else
            {
                Console.WriteLine($"ℹ️ Roles ya existen ({existingRoles.Count} roles)");
            }
        }

        private async Task SeedAdminUserAsync()
        {
            // Verificar si ya existe el usuario admin
            var existingAdmin = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "admin@example.com");
            
            if (existingAdmin == null)
            {
                Console.WriteLine("👤 Creando usuario administrador...");
                
                // Buscar el rol Admin en la base de datos
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
                    
                    // Guardar usuario admin
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Usuario admin creado: admin@example.com / admin123");
                }
                else
                {
                    Console.WriteLine("⚠️ No se encontró rol Admin para crear usuario");
                    throw new Exception("Rol Admin no encontrado. El seeder de roles falló.");
                }
            }
            else
            {
                Console.WriteLine("ℹ️ Usuario admin ya existe, saltando creación...");
            }
        }
    }
}
