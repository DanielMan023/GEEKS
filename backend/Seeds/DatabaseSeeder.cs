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
                
                // Crear categorías y productos demo
                await SeedCategoriesAsync();
                await SeedProductsAsync();
                
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

        private async Task SeedCategoriesAsync()
        {
            // Verificar si ya existen categorías
            var existingCategories = await _context.Categories.ToListAsync();
            
            if (!existingCategories.Any())
            {
                Console.WriteLine("🏷️ Creando categorías demo...");
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
                Console.WriteLine($"✅ {categories.Count} categorías demo creadas");
            }
            else
            {
                Console.WriteLine($"ℹ️ Categorías ya existen ({existingCategories.Count} categorías)");
            }
        }

        private async Task SeedProductsAsync()
        {
            // Verificar si ya existen productos
            var existingProducts = await _context.Products.ToListAsync();
            
            if (!existingProducts.Any())
            {
                Console.WriteLine("📦 Creando productos demo...");
                
                // Obtener categorías para asignar
                var categories = await _context.Categories.ToListAsync();
                if (!categories.Any())
                {
                    Console.WriteLine("⚠️ No hay categorías disponibles para crear productos");
                    return;
                }

                var products = new List<Product>
                {
                    // Electrónicos
                    new Product
                    {
                        Name = "Smartphone Galaxy S23",
                        Description = "El último smartphone de Samsung con cámara de 108MP y procesador Snapdragon 8 Gen 2",
                        ShortDescription = "Smartphone premium con cámara profesional",
                        Price = 899.99m,
                        DiscountPrice = 799.99m,
                        Stock = 50,
                        MinStock = 10,
                        SKU = "SAMS-S23-128",
                        MainImage = "https://picsum.photos/400/400?random=1",
                        Images = new List<string> { "https://picsum.photos/400/400?random=2", "https://picsum.photos/400/400?random=3" },
                        CategoryId = categories.First(c => c.Name == "Electrónicos").Id,
                        Brand = "Samsung",
                        IsFeatured = true,
                        Weight = 0.168m,
                        Length = 15.4m,
                        Width = 7.6m,
                        Height = 0.8m,
                        State = "Active"
                    },
                    new Product
                    {
                        Name = "Laptop Dell XPS 13",
                        Description = "Laptop ultrabook con pantalla InfinityEdge, procesador Intel i7 y 16GB RAM",
                        ShortDescription = "Laptop ultrabook premium para profesionales",
                        Price = 1299.99m,
                        DiscountPrice = 1199.99m,
                        Stock = 25,
                        MinStock = 5,
                        SKU = "DELL-XPS13-16GB",
                        MainImage = "https://picsum.photos/400/400?random=4",
                        Images = new List<string> { "https://picsum.photos/400/400?random=5", "https://picsum.photos/400/400?random=6" },
                        CategoryId = categories.First(c => c.Name == "Electrónicos").Id,
                        Brand = "Dell",
                        IsFeatured = true,
                        Weight = 1.2m,
                        Length = 30.2m,
                        Width = 20.0m,
                        Height = 1.4m,
                        State = "Active"
                    },
                    
                    // Gaming
                    new Product
                    {
                        Name = "PlayStation 5",
                        Description = "Consola de nueva generación con SSD ultra-rápido y ray tracing",
                        ShortDescription = "La consola más potente de Sony",
                        Price = 499.99m,
                        DiscountPrice = null,
                        Stock = 15,
                        MinStock = 3,
                        SKU = "SONY-PS5-DISC",
                        MainImage = "https://picsum.photos/400/400?random=7",
                        Images = new List<string> { "https://picsum.photos/400/400?random=8", "https://picsum.photos/400/400?random=9" },
                        CategoryId = categories.First(c => c.Name == "Gaming").Id,
                        Brand = "Sony",
                        IsFeatured = true,
                        Weight = 4.5m,
                        Length = 39.0m,
                        Width = 26.0m,
                        Height = 10.4m,
                        State = "Active"
                    },
                    new Product
                    {
                        Name = "Nintendo Switch OLED",
                        Description = "Nintendo Switch con pantalla OLED de 7 pulgadas y almacenamiento de 64GB",
                        ShortDescription = "Switch con pantalla OLED mejorada",
                        Price = 349.99m,
                        DiscountPrice = 329.99m,
                        Stock = 30,
                        MinStock = 8,
                        SKU = "NINT-SW-OLED",
                        MainImage = "https://picsum.photos/400/400?random=10",
                        Images = new List<string> { "https://picsum.photos/400/400?random=11" },
                        CategoryId = categories.First(c => c.Name == "Gaming").Id,
                        Brand = "Nintendo",
                        IsFeatured = false,
                        Weight = 0.42m,
                        Length = 24.2m,
                        Width = 13.9m,
                        Height = 1.4m,
                        State = "Active"
                    },
                    
                    // Ropa
                    new Product
                    {
                        Name = "Camiseta Nike Dri-FIT",
                        Description = "Camiseta deportiva con tecnología Dri-FIT para máxima transpirabilidad",
                        ShortDescription = "Camiseta deportiva de alto rendimiento",
                        Price = 34.99m,
                        DiscountPrice = 29.99m,
                        Stock = 100,
                        MinStock = 20,
                        SKU = "NIKE-DRIFIT-M",
                        MainImage = "https://picsum.photos/400/400?random=12",
                        Images = new List<string> { "https://picsum.photos/400/400?random=13" },
                        CategoryId = categories.First(c => c.Name == "Ropa").Id,
                        Brand = "Nike",
                        IsFeatured = false,
                        Weight = 0.15m,
                        Length = 0.0m,
                        Width = 0.0m,
                        Height = 0.0m,
                        State = "Active"
                    },
                    new Product
                    {
                        Name = "Jeans Levi's 501",
                        Description = "Jeans clásicos de corte recto con el sello de calidad de Levi's",
                        ShortDescription = "Jeans clásicos de Levi's",
                        Price = 79.99m,
                        DiscountPrice = 69.99m,
                        Stock = 75,
                        MinStock = 15,
                        SKU = "LEVI-501-32",
                        MainImage = "https://picsum.photos/400/400?random=14",
                        Images = new List<string> { "https://picsum.photos/400/400?random=15", "https://picsum.photos/400/400?random=16" },
                        CategoryId = categories.First(c => c.Name == "Ropa").Id,
                        Brand = "Levi's",
                        IsFeatured = false,
                        Weight = 0.5m,
                        Length = 0.0m,
                        Width = 0.0m,
                        Height = 0.0m,
                        State = "Active"
                    },
                    
                    // Hogar
                    new Product
                    {
                        Name = "Cafetera Nespresso Vertuo",
                        Description = "Cafetera automática con sistema de cápsulas Vertuo para café perfecto",
                        ShortDescription = "Cafetera automática Nespresso",
                        Price = 199.99m,
                        DiscountPrice = 179.99m,
                        Stock = 40,
                        MinStock = 8,
                        SKU = "NESP-VERTUO-BLK",
                        MainImage = "https://picsum.photos/400/400?random=17",
                        Images = new List<string> { "https://picsum.photos/400/400?random=18" },
                        CategoryId = categories.First(c => c.Name == "Hogar").Id,
                        Brand = "Nespresso",
                        IsFeatured = true,
                        Weight = 3.2m,
                        Length = 30.0m,
                        Width = 20.0m,
                        Height = 35.0m,
                        State = "Active"
                    },
                    new Product
                    {
                        Name = "Lámpara de Mesa LED",
                        Description = "Lámpara de mesa moderna con luz LED ajustable y diseño minimalista",
                        ShortDescription = "Lámpara LED moderna y funcional",
                        Price = 49.99m,
                        DiscountPrice = 39.99m,
                        Stock = 60,
                        MinStock = 12,
                        SKU = "LAMP-LED-DESK",
                        MainImage = "https://picsum.photos/400/400?random=19",
                        Images = new List<string> { "https://picsum.photos/400/400?random=20" },
                        CategoryId = categories.First(c => c.Name == "Hogar").Id,
                        Brand = "IKEA",
                        IsFeatured = false,
                        Weight = 0.8m,
                        Length = 25.0m,
                        Width = 15.0m,
                        Height = 45.0m,
                        State = "Active"
                    },
                    
                    // Deportes
                    new Product
                    {
                        Name = "Pelota de Fútbol Adidas",
                        Description = "Pelota oficial de fútbol con tecnología de última generación para máximo control",
                        ShortDescription = "Pelota oficial de fútbol Adidas",
                        Price = 89.99m,
                        DiscountPrice = 79.99m,
                        Stock = 45,
                        MinStock = 10,
                        SKU = "ADID-FBALL-5",
                        MainImage = "https://picsum.photos/400/400?random=21",
                        Images = new List<string> { "https://picsum.photos/400/400?random=22" },
                        CategoryId = categories.First(c => c.Name == "Deportes").Id,
                        Brand = "Adidas",
                        IsFeatured = false,
                        Weight = 0.43m,
                        Length = 22.0m,
                        Width = 22.0m,
                        Height = 22.0m,
                        State = "Active"
                    },
                    new Product
                    {
                        Name = "Raqueta de Tenis Wilson",
                        Description = "Raqueta profesional con marco de grafito y encordado premium",
                        ShortDescription = "Raqueta de tenis profesional Wilson",
                        Price = 159.99m,
                        DiscountPrice = 139.99m,
                        Stock = 25,
                        MinStock = 5,
                        SKU = "WILS-TENNIS-PRO",
                        MainImage = "https://picsum.photos/400/400?random=23",
                        Images = new List<string> { "https://picsum.photos/400/400?random=24" },
                        CategoryId = categories.First(c => c.Name == "Deportes").Id,
                        Brand = "Wilson",
                        IsFeatured = false,
                        Weight = 0.31m,
                        Length = 68.6m,
                        Width = 25.4m,
                        Height = 2.5m,
                        State = "Active"
                    },
                    
                    // Libros
                    new Product
                    {
                        Name = "El Señor de los Anillos",
                        Description = "Trilogía completa de J.R.R. Tolkien en edición de lujo con mapas y ilustraciones",
                        ShortDescription = "Trilogía épica de fantasía",
                        Price = 45.99m,
                        DiscountPrice = 39.99m,
                        Stock = 80,
                        MinStock = 15,
                        SKU = "TOLK-LOTR-TRI",
                        MainImage = "https://picsum.photos/400/400?random=25",
                        Images = new List<string> { "https://picsum.photos/400/400?random=26" },
                        CategoryId = categories.First(c => c.Name == "Libros").Id,
                        Brand = "Minotauro",
                        IsFeatured = true,
                        Weight = 1.2m,
                        Length = 23.0m,
                        Width = 15.0m,
                        Height = 4.0m,
                        State = "Active"
                    },
                    new Product
                    {
                        Name = "Clean Code",
                        Description = "Guía esencial para escribir código limpio y mantenible por Robert C. Martin",
                        ShortDescription = "Guía para código limpio y mantenible",
                        Price = 29.99m,
                        DiscountPrice = 24.99m,
                        Stock = 120,
                        MinStock = 25,
                        SKU = "MART-CLEAN-CODE",
                        MainImage = "https://picsum.photos/400/400?random=27",
                        Images = new List<string> { "https://picsum.photos/400/400?random=28" },
                        CategoryId = categories.First(c => c.Name == "Libros").Id,
                        Brand = "Prentice Hall",
                        IsFeatured = false,
                        Weight = 0.8m,
                        Length = 23.5m,
                        Width = 18.8m,
                        Height = 2.5m,
                        State = "Active"
                    }
                };

                _context.Products.AddRange(products);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ {products.Count} productos demo creados");
            }
            else
            {
                Console.WriteLine($"ℹ️ Productos ya existen ({existingProducts.Count} productos)");
            }
        }
    }
}
