using ApiSystemContable.Data;
using ApiSystemContable.DTOs;
using ApiSystemContable.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSystemContable.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwt;
    public AuthService(AppDbContext context, IJwtTokenService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    public async Task<Usuario?> RegisterNewUserAsync(RegisterUserDto dto)
    {
        // Verificar si el usuario ya existe
        var exists = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
        if (exists)
        {
            throw new InvalidOperationException("El correo ya está registrado.");
        }

        // Generar el hash usando pgcrypto en PostgreSQL
        var hashList = await _context.Database
            .SqlQueryRaw<string>("SELECT crypt({0}, gen_salt('bf'))", dto.Password)
            .ToListAsync();

        var passwordHash = hashList.FirstOrDefault();
        if (string.IsNullOrEmpty(passwordHash))
        {
            throw new InvalidOperationException("No se pudo generar el hash de la contraseña.");
        }

        var nuevoUsuario = new Usuario
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            PasswordHash = passwordHash,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        return nuevoUsuario;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Usuarios
            .FromSqlRaw(@"
            SELECT *
            FROM Usuarios
            WHERE Email = {0}
            AND password_hash = crypt({1}, password_hash)
        ", dto.Email, dto.Password)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return null;
        }

        var token = _jwt.GetToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Usuario = new UsuarioDto
            {
                Id = user.IdUsuario.ToString(),
                Nombre = user.Nombre,
                email = user.Email
            }
        };
    }
}
