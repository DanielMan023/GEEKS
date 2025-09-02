using Microsoft.EntityFrameworkCore;
using GEEKS.Data;
using GEEKS.Models;
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
                
                // Crear roles
                await SeedRolesAsync();
                
                // Crear usuario admin
                await SeedAdminUserAsync();
                
                // Crear categorías (sin productos demo)
                await SeedCategoriesAsync();
                
                Console.WriteLine("✅ Seeder completado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en seeder: {ex.Message}");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            // Verificar si ya existen roles
            var existingRoles = await _context.Roles.ToListAsync();
            
            if (!existingRoles.Any())
            {
                Console.WriteLine("👑 Creando roles...");
                var roles = new List<Role>
                {
                    new Role { Name = "Admin", Scope = "ALL", Description = "Administrador del sistema", State = "Active" },
                    new Role { Name = "User", Scope = "USER", Description = "Usuario regular", State = "Active" }
                };

                _context.Roles.AddRange(roles);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ {roles.Count} roles creados");
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

        private async Task SeedCategoriesAsync()
        {
            // Verificar si ya existen categorías
            var existingCategories = await _context.Categories.ToListAsync();
            
            if (!existingCategories.Any())
            {
                Console.WriteLine("🏷️ Creando categorías...");
                var categories = new List<Category>
                {
                    new Category
                    {
                        Name = "Electrónicos",
                        Description = "Productos electrónicos y tecnológicos",
                        Image = "https://via.placeholder.com/300x200/3B82F6/FFFFFF?text=Electronicos",
                        State = "Active"
                    },
                    new Category
                    {
                        Name = "Gaming",
                        Description = "Videojuegos, consolas y accesorios gaming",
                        Image = "https://via.placeholder.com/300x200/8B5CF6/FFFFFF?text=Gaming",
                        State = "Active"
                    },
                    new Category
                    {
                        Name = "Ropa",
                        Description = "Vestimenta y accesorios de moda",
                        Image = "https://via.placeholder.com/300x200/EC4899/FFFFFF?text=Ropa",
                        State = "Active"
                    },
                    new Category
                    {
                        Name = "Hogar",
                        Description = "Artículos para el hogar y decoración",
                        Image = "https://via.placeholder.com/300x200/10B981/FFFFFF?text=Hogar",
                        State = "Active"
                    },
                    new Category
                    {
                        Name = "Deportes",
                        Description = "Equipamiento y ropa deportiva",
                        Image = "https://via.placeholder.com/300x200/F59E0B/FFFFFF?text=Deportes",
                        State = "Active"
                    },
                    new Category
                    {
                        Name = "Libros",
                        Description = "Libros, revistas y material educativo",
                        Image = "https://via.placeholder.com/300x200/EF4444/FFFFFF?text=Libros",
                        State = "Active"
                    }
                };

                _context.Categories.AddRange(categories);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ {categories.Count} categorías creadas");
            }
            else
            {
                Console.WriteLine($"ℹ️ Categorías ya existen ({existingCategories.Count} categorías)");
            }
        }
    }
}
