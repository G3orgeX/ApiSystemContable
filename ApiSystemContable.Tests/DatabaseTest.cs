using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ApiSystemContable.Data;
using ApiSystemContable.DTOs;
using ApiSystemContable.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ApiSystemContable.Tests;

public class DatabaseTest
{
    private readonly ITestOutputHelper _output;
    private readonly IJwtTokenService _jwtToken;

    public DatabaseTest(ITestOutputHelper output, IJwtTokenService jwtToken)
    {
        _output = output;
        _jwtToken = jwtToken;   
    }

    [Fact]
    public async Task ListUsuariosTableColumns()
    {
        var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ApiSystemContable", "appsettings.json");
        if (!File.Exists(appSettingsPath))
        {
            appSettingsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "ApiSystemContable", "appsettings.json");
        }

        Assert.True(File.Exists(appSettingsPath), $"No se encontró el archivo appsettings.json en: {appSettingsPath}");

        var jsonText = File.ReadAllText(appSettingsPath);
        using var doc = JsonDocument.Parse(jsonText);
        var connectionString = doc.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString();

        Assert.NotNull(connectionString);

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();

        using var context = new AppDbContext(optionsBuilder.Options);

        try
        {
            var columns = await context.Database
                .SqlQueryRaw<string>(@"
                    SELECT column_name 
                    FROM information_schema.columns 
                    WHERE table_name = 'usuarios'")
                .ToListAsync();

            _output.WriteLine("=== COLUMNS IN 'usuarios' TABLE ===");
            foreach (var col in columns)
            {
                _output.WriteLine($"- {col}");
            }
            
            Assert.NotEmpty(columns);
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Error: {ex.Message}");
            _output.WriteLine(ex.StackTrace ?? "");
            throw;
        }
    }

    [Fact]
    public async Task RealLoginTest()
    {
        var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ApiSystemContable", "appsettings.json");
        if (!File.Exists(appSettingsPath))
        {
            appSettingsPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "ApiSystemContable", "appsettings.json");
        }

        Assert.True(File.Exists(appSettingsPath));

        var jsonText = File.ReadAllText(appSettingsPath);
        using var doc = JsonDocument.Parse(jsonText);
        var connectionString = doc.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString();

        Assert.NotNull(connectionString);

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();

        using var context = new AppDbContext(optionsBuilder.Options);
        var authService = new AuthService(context,_jwtToken);

        var email = "adminprueba@prueba.com";
        var password = "12345678";

        try
        {
            _output.WriteLine($"Buscando usuario {email} en la base de datos...");
            var user = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _output.WriteLine($"El usuario {email} no existe. Registrándolo con la contraseña proporcionada...");
                user = await authService.RegisterNewUserAsync(new RegisterUserDto
                {
                    Nombre = "Admin",
                    Apellido = "Prueba",
                    Email = email,
                    Password = password
                });
                _output.WriteLine($"Usuario registrado exitosamente. ID: {user.IdUsuario}, Email: {user.Email}");
            }
            else
            {
                _output.WriteLine($"El usuario {email} ya existe. ID: {user.IdUsuario}, Hash en BD: {user.PasswordHash}");
            }

            _output.WriteLine($"Probando inicio de sesión con {email} y la contraseña proporcionada...");
            var loggedInUser = await authService.LoginAsync(new LoginDto
            {
                Email = email,
                Password = password
            });

            Assert.NotNull(loggedInUser);
            _output.WriteLine($"¡INICIO DE SESIÓN EXITOSO! Usuario autenticado: {loggedInUser.Usuario.Nombre} {loggedInUser.Usuario.email} (ID: {loggedInUser.Usuario.Id})");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Error durante el test: {ex.Message}");
            _output.WriteLine(ex.StackTrace ?? "");
            throw;
        }
    }
}
