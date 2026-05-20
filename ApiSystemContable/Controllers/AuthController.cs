using ApiSystemContable.DTOs;
using ApiSystemContable.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiSystemContable.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registerNewUser")]
    public async Task<IActionResult> RegisterNewUser([FromBody] RegisterUserDto dto)
    {
        try
        {
            var user = await _authService.RegisterNewUserAsync(dto);
            if (user == null)
            {
                return StatusCode(500, new { mensaje = "Error al crear el usuario." });
            }

            return Ok(new
            {
                mensaje = "Usuario creado exitosamente",
                usuario = new
                {
                    user.IdUsuario,
                    user.Nombre,
                    user.Apellido,
                    user.Email,
                    user.FechaCreacion
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error en el servidor al registrar el usuario", detalle = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _authService.LoginAsync(dto);
            if (user == null)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });
            }

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso",
                usuario = new
                {
                    user.IdUsuario,
                    user.Nombre,
                    user.Apellido,
                    user.Email,
                    user.FechaCreacion
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error en el servidor al iniciar sesión", detalle = ex.Message });
        }
    }
}
