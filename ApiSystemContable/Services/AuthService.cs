using ApiSystemContable.Data;
using ApiSystemContable.DTOs;
using ApiSystemContable.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSystemContable.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
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

    public async Task<Usuario?> LoginAsync(LoginDto dto)
    {
        // Buscar el usuario por email
        //var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
        //if (user == null)
        //{
        //    return null;
        //}

        //// Verificar la contraseña usando pgcrypto en PostgreSQL
        //var isValidList = await _context.Database
        //    .SqlQueryRaw<bool>("SELECT crypt({0}, {1}) = {1}", dto.Password, user.PasswordHash)
        //    .ToListAsync();

        //if (!isValidList.FirstOrDefault())
        //{
        //    return null;
        //}

        //return user;
        var user = await _context.Usuarios
       .FromSqlRaw(@"
            SELECT *
            FROM Usuarios
            WHERE Email = {0}
            AND password_hash = crypt({1}, password_hash)
        ", dto.Email, dto.Password)
       .FirstOrDefaultAsync();

        return user;
    }
}
