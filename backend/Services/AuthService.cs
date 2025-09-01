using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using GEEKS.Data;
using GEEKS.Dto;
using GEEKS.Interfaces;
using GEEKS.Models;
using GEEKS.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace GEEKS.Services
{
    public class AuthService : IAuthService
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(DBContext context, IConfiguration configuration, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<dynamic>> Register(RegisterDTO registerRequest)
        {
            var response = new ServiceResponse<dynamic>();

            try
            {
                // Verificar si el email ya existe
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == registerRequest.Email);

                if (existingUser != null)
                {
                    response.Success = false;
                    response.Message = "El email ya está registrado";
                    return response;
                }

                // Obtener rol por defecto (puedes cambiar esto según tus necesidades)
                var defaultRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name.ToLower() == "user");

                if (defaultRole == null)
                {
                    response.Success = false;
                    response.Message = "Error: No se encontró rol por defecto";
                    return response;
                }

                // Crear nuevo usuario
                var newUser = new User
                {
                    Email = registerRequest.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    RoleId = defaultRole.Id,
                    State = "Active"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Generar token JWT para el nuevo usuario
                var token = GenerateJwtToken(newUser);
                
                // Establecer cookie
                SetAuthCookie(token);

                response.Success = true;
                response.Message = "Usuario registrado exitosamente";
                response.Data = new { user = newUser, token = token };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error en el registro: {Error}", ex.Message);
                response.Success = false;
                response.Message = "Error interno del servidor";
                return response;
            }
        }

        public async Task<ServiceResponse<dynamic>> Login(LoginDTO loginRequest)
        {
            var response = new ServiceResponse<dynamic>();

            try
            {
                _logger.LogInformation("🔐 Intentando login para email: {Email}", loginRequest.Email);

                // Buscar usuario por email
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.State.ToLower() == "active");

                if (user == null)
                {
                    _logger.LogWarning("❌ Usuario no encontrado para email: {Email}", loginRequest.Email);
                    response.Success = false;
                    response.Message = "Credenciales inválidas";
                    return response;
                }

                _logger.LogInformation("✅ Usuario encontrado: ID={UserId}, Email={Email}, RoleId={RoleId}", 
                    user.Id, user.Email, user.RoleId);

                // Verificar que el rol se cargó correctamente
                if (user.Role == null)
                {
                    _logger.LogError("❌ Usuario {UserId} no tiene rol cargado. RoleId: {RoleId}", user.Id, user.RoleId);
                    
                    // Intentar cargar el rol manualmente
                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                    if (role != null)
                    {
                        user.Role = role;
                        _logger.LogInformation("✅ Rol cargado manualmente: {RoleName} (Scope: {Scope})", role.Name, role.Scope);
                    }
                    else
                    {
                        _logger.LogError("❌ No se pudo encontrar el rol con ID: {RoleId}", user.RoleId);
                        response.Success = false;
                        response.Message = "Error: Rol no encontrado";
                        return response;
                    }
                }
                else
                {
                    _logger.LogInformation("✅ Rol cargado: {RoleName} (Scope: {Scope})", user.Role.Name, user.Role.Scope);
                }

                // Verificar contraseña
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                {
                    _logger.LogWarning("❌ Contraseña incorrecta para usuario: {UserId}", user.Id);
                    response.Success = false;
                    response.Message = "Credenciales inválidas";
                    return response;
                }

                _logger.LogInformation("✅ Contraseña verificada correctamente");

                // Generar token JWT
                _logger.LogInformation("🔑 Generando token JWT...");
                var token = GenerateJwtToken(user);
                _logger.LogInformation("✅ Token JWT generado");

                // Establecer cookie
                _logger.LogInformation("🍪 Estableciendo cookie de autenticación...");
                SetAuthCookie(token);
                _logger.LogInformation("✅ Cookie establecida");

                _logger.LogInformation("🎉 Login exitoso para usuario: {UserId}", user.Id);
                response.Success = true;
                response.Message = "Login exitoso";
                response.Data = new { user = user, token = token };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error crítico en el login para email {Email}: {Error}", loginRequest.Email, ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                response.Success = false;
                response.Message = "Error interno del servidor";
                return response;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Scope ?? "USER")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(8);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void SetAuthCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Cambiado a false para desarrollo local
                SameSite = SameSiteMode.Lax, // Cambiado a Lax para desarrollo
                Expires = DateTime.Now.AddHours(8)
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append("auth-token", token, cookieOptions);
        }
    }
}
