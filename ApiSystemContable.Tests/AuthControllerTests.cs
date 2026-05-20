using ApiSystemContable.Controllers;
using ApiSystemContable.DTOs;
using ApiSystemContable.Models;
using ApiSystemContable.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ApiSystemContable.Tests;

public class AuthControllerTests
{
    private class FakeAuthService : IAuthService
    {
        public bool ShouldFailRegister { get; set; }
        public bool ShouldExistRegister { get; set; }
        public bool ShouldFailLogin { get; set; }

        public Task<Usuario?> RegisterNewUserAsync(RegisterUserDto dto)
        {
            if (ShouldExistRegister)
            {
                throw new InvalidOperationException("El correo ya está registrado.");
            }
            if (ShouldFailRegister)
            {
                return Task.FromResult<Usuario?>(null);
            }
            return Task.FromResult<Usuario?>(new Usuario
            {
                IdUsuario = 1,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = "hashed_pwd",
                FechaCreacion = DateTime.UtcNow
            });
        }

        public Task<Usuario?> LoginAsync(LoginDto dto)
        {
            if (ShouldFailLogin)
            {
                return Task.FromResult<Usuario?>(null);
            }
            return Task.FromResult<Usuario?>(new Usuario
            {
                IdUsuario = 1,
                Nombre = "Test",
                Apellido = "User",
                Email = dto.Email,
                PasswordHash = "hashed_pwd",
                FechaCreacion = DateTime.UtcNow
            });
        }
    }

    [Fact]
    public async Task RegisterNewUser_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var fakeService = new FakeAuthService();
        var controller = new AuthController(fakeService);
        var dto = new RegisterUserDto
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@example.com",
            Password = "password123"
        };

        // Act
        var result = await controller.RegisterNewUser(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task RegisterNewUser_ReturnsBadRequest_WhenUserAlreadyExists()
    {
        // Arrange
        var fakeService = new FakeAuthService { ShouldExistRegister = true };
        var controller = new AuthController(fakeService);
        var dto = new RegisterUserDto
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@example.com",
            Password = "password123"
        };

        // Act
        var result = await controller.RegisterNewUser(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        // Arrange
        var fakeService = new FakeAuthService();
        var controller = new AuthController(fakeService);
        var dto = new LoginDto
        {
            Email = "juan@example.com",
            Password = "password123"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var fakeService = new FakeAuthService { ShouldFailLogin = true };
        var controller = new AuthController(fakeService);
        var dto = new LoginDto
        {
            Email = "invalid@example.com",
            Password = "wrongpassword"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }
}
