using Application.Constants;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Request._user;
using Models.Response._user;
using Persistence.Contexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Persistence.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
                UserManager<ApplicationUser> userManager,
                RoleManager<ApplicationRole> roleManager,
                IConfiguration configuration,
                ApplicationDbContext dbContext,
                IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _emailService = emailService;
        }

        /// <summary>
        /// Confirmar el email de un usuario.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Response<string>> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Response<string>.Fail("Usuario no encontrado.");

            try
            {
                // 1. Decodificar el token de forma segura
                var decodedBytes = WebEncoders.Base64UrlDecode(token);
                var decodedToken = Encoding.UTF8.GetString(decodedBytes);

                // 2. Confirmar con Identity
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (!result.Succeeded)
                    return Response<string>.Fail(result.Errors.Select(e => e.Description).ToList());

                return Response<string>.Success(user.Id, "Email confirmado exitosamente.");
            }
            catch (FormatException)
            {
                // Capturamos el error específico de Base64
                return Response<string>.Fail("El token de confirmación es inválido o está corrupto.");
            }
            catch (Exception ex)
            {
                return Response<string>.Fail($"Error al confirmar email: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtener el token para restablecer la contraseña y enviarlo por correo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            // Por seguridad, no revelamos si el usuario existe o no.
            // Si no existe, simplemente retornamos éxito falso (o verdadero, según prefieras para evitar enumeración de usuarios).
            // Aquí seremos transparentes para desarrollo:
            if (user == null)
                return Response<string>.Fail("No existe ningún usuario asociado a este correo.");

            // Generar Token de Reset Password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Codificar Token (Importante para URL)
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Construir URL (Apunta a tu Frontend o a un endpoint de prueba)
            // Nota: El frontend debe tener una pantalla que capture el token de la URL y muestre el formulario de nueva contraseña.
            //var url = $"https://localhost:7042/api/v1/Auth/reset-password?email={request.Email}&token={encodedToken}"; // O URL del Frontend
            var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7042";
            var url = $"{baseUrl}/reset-password?email={request.Email}&token={encodedToken}";

            // Enviar Correo
            await _emailService.SendEmailAsync(request.Email, "Restablecer Contraseña",
                $"<h1>Recuperar Contraseña</h1>" +
                $"<p>Para restablecer tu contraseña, copia el siguiente token:</p>" +
                $"<p><strong>{encodedToken}</strong></p>" +
                $"<p>O haz clic <a href='{url}'>aquí</a> (Solo si es GET, para POST mejor copiar token).</p>");

            return Response<string>.Success(user.Id, $"Se ha enviado un correo a {request.Email} con las instrucciones.");
        }

        /// <summary>
        /// Login a user and generate a JWT token.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response<LoginResponse>> LoginUserAsync(AuthLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email!);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password!))
                return Response<LoginResponse>.Fail("Usuario o contraseña incorrectos.");

            if (!user.EmailConfirmed)
                return Response<LoginResponse>.Fail("El correo electrónico no ha sido confirmado.");

            var roles = await _userManager.GetRolesAsync(user!);
            var token = await GenerateJwtTokenAsync(user);

            var refreshToken = GenerateRefreshToken();

            // Guardamos el Refresh Token en la BD
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Duración de 7 días (ajustable)
            await _userManager.UpdateAsync(user);

            return new Response<LoginResponse>(
            new LoginResponse
            {
                JwToken = token,
                UserId = user.Id,
                Roles = roles.ToList(),
                RefreshToken = refreshToken
            },
            $"Usuario {user.UserName} logueado correctamente.");
        }

        /// <summary>
        /// Refresh JWT token using a valid refresh token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value; // O usa NameIdentifier para ID

            if (email == null) return Response<LoginResponse>.Fail("Token inválido.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Response<LoginResponse>.Fail("Token de refresco inválido o expirado. Por favor inicie sesión nuevamente.");

            var roles = await _userManager.GetRolesAsync(user!);

            // Si todo está bien, generamos NUEVOS tokens (Rotación de tokens)
            var newJwtToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Renovamos 7 días más
            await _userManager.UpdateAsync(user);

            return new Response<LoginResponse>(new LoginResponse
            {
                JwToken = newJwtToken,
                UserId = user.Id,
                Roles = roles.ToList(),
                RefreshToken = newRefreshToken
            }, "Token refrescado exitosamente");
        }

        /// <summary>
        /// Register a new user and assign a role.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response<ApplicationUser>> RegisterUserAsync(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            // 1. Validar que no exista el email
            var existingUser = await _userManager.FindByEmailAsync(request.Email!);
            if (existingUser != null)
                return Response<ApplicationUser>.Fail("El correo electrónico ya está en uso.");

            // 2. Iniciar Transacción con Estrategia de Ejecución (Requerido por EnableRetryOnFailure)
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // A. Crear el Usuario
                    var user = new ApplicationUser
                    {
                        UserName = request.UserName,
                        Email = request.Email,
                        // Todavía no asignamos CocheraId
                    };

                    var result = await _userManager.CreateAsync(user, request.Password!);
                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Response<ApplicationUser>.Fail(result.Errors.Select(r => r.Description).ToList());
                    }

                    // B. Determinar Rol y Lógica de Cochera
                    string rolAsignar = RolesConstants.Usuario; // Por defecto

                    // CASO 1: Es Dueño (Envió datos de nueva cochera)
                    if (!string.IsNullOrEmpty(request.NombreCochera))
                    {
                        rolAsignar = RolesConstants.Admin; // O "Owner" si creas ese rol

                        var nuevaCochera = new Cochera
                        {
                            Nombre = request.NombreCochera,
                            Direccion = request.DireccionCochera!,
                            CapacidadTotal = request.CapacidadCochera ?? 0,
                            OwnerId = user.Id, // Vinculamos al dueño
                            ImagenUrl = "default_garage.png", // Valor por defecto
                            Created = DateTime.UtcNow,
                            IsActive = true
                        };

                        await _dbContext.Cocheras.AddAsync(nuevaCochera, cancellationToken);
                        await _dbContext.SaveChangesAsync(cancellationToken); // Guardar para obtener el ID

                        // Actualizar usuario con la nueva cochera
                        user.CocheraId = nuevaCochera.Id;
                        await _userManager.UpdateAsync(user);
                    }
                    // CASO 2: Es Empleado (Envió ID de cochera existente)
                    else if (request.CocheraIdExistente.HasValue)
                    {
                        // Verificar que la cochera exista
                        var cocheraExiste = await _dbContext.Cocheras.AnyAsync(c => c.Id == request.CocheraIdExistente.Value, cancellationToken);
                        if (!cocheraExiste)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            return Response<ApplicationUser>.Fail("La cochera especificada no existe.");
                        }

                        user.CocheraId = request.CocheraIdExistente.Value;
                        await _userManager.UpdateAsync(user);
                    }

                    // C. Asignar Rol
                    if (!await _roleManager.RoleExistsAsync(rolAsignar))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole { Name = rolAsignar });
                    }
                    await _userManager.AddToRoleAsync(user, rolAsignar);

                    // D. Confirmación de Email
                    // Generamos el token único de Identity
                    var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // Lo codificamos para que pueda viajar seguro en una URL
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(verificationToken));

                    // Construimos la URL (ajusta localhost al puerto de tu API)
                    //var url = $"https://localhost:7042/api/v1/Auth/confirm-email?userId={user.Id}&token={encodedToken}";
                    var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7042";
                    var url = $"{baseUrl}/api/v1/Auth/confirm-email?userId={user.Id}&token={encodedToken}";

                    // Enviamos el correo (Mock o Real)
                    // 1. Obtienes la ruta base de donde está corriendo la app
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string templatePath = Path.Combine(baseDir, "Templates", "WelcomeTemplate.html");

                    // 2. Lees el HTML (Validar que exista el archivo para evitar errores)
                    string emailBody = "Bienvenido"; // Fallback por si falla la lectura
                    if (File.Exists(templatePath))
                    {
                        emailBody = await File.ReadAllTextAsync(templatePath, cancellationToken);

                        // 3. Reemplazas los marcadores
                        emailBody = emailBody
                            .Replace("{UserName}", user.UserName)
                            .Replace("{ConfirmUrl}", url);
                    }

                    // 4. Envías el correo bonito
                    await _emailService.SendEmailAsync(
                        user.Email!,
                        "¡Bienvenido a bordo! - Confirma tu cuenta",
                        emailBody
                    );

                    // E. COMMIT FINAL
                    // Si llegamos hasta aquí, todo salió bien. Guardamos los cambios definitivamente.
                    await transaction.CommitAsync(cancellationToken);

                    return Response<ApplicationUser>.Success(user, $"Usuario registrado exitosamente como {rolAsignar}. Revisa tu correo para activar la cuenta.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Response<ApplicationUser>.Fail($"Error en el servidor: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Restablecer la contraseña de un usuario.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Response<string>.Fail("Usuario no encontrado.");

            try
            {
                // Decodificar Token
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

                // Ejecutar cambio de contraseña
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

                if (!result.Succeeded)
                    return Response<string>.Fail(result.Errors.Select(e => e.Description).ToList());

                return Response<string>.Success(user.Id, "Contraseña restablecida exitosamente.");
            }
            catch (FormatException)
            {
                return Response<string>.Fail("Token inválido o corrupto.");
            }
        }


        #region Métodos Privados

        /// <summary>
        /// Generate a JWT token for the user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate a secure refresh token.
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Retrieve principal from expired token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                ValidateLifetime = false // Importante: Ignoramos la expiración aquí
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        #endregion

    }
}