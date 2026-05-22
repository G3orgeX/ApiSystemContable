using ApiSystemContable.DTOs;
using ApiSystemContable.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiSystemContable.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TarjetasController : ControllerBase
{
    private readonly ITarjetaService _tarjetaService;

    public TarjetasController(ITarjetaService tarjetaService)
    {
        _tarjetaService = tarjetaService;
    }

    private int GetUsuarioIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            throw new UnauthorizedAccessException("Usuario no identificado");
        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetTarjetas()
    {
        try
        {
            var userId = GetUsuarioIdFromToken();
            var tarjetas = await _tarjetaService.GetTarjetasByUsuarioAsync(userId);
            return Ok(new { data = tarjetas });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener tarjetas", detalle = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTarjetaById(int id)
    {
        try
        {
            var tarjeta = await _tarjetaService.GetTarjetaByIdAsync(id);
            if (tarjeta == null)
                return NotFound(new { mensaje = "Tarjeta no encontrada" });

            return Ok(new { data = tarjeta });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener tarjeta", detalle = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTarjeta([FromBody] CreateTarjetaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUsuarioIdFromToken();
            var tarjeta = await _tarjetaService.CreateTarjetaAsync(userId, dto);
            return CreatedAtAction(nameof(GetTarjetaById), new { id = tarjeta.IdTarjeta }, tarjeta);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al crear tarjeta", detalle = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTarjeta(int id, [FromBody] UpdateTarjetaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tarjeta = await _tarjetaService.UpdateTarjetaAsync(id, dto);
            if (tarjeta == null)
                return NotFound(new { mensaje = "Tarjeta no encontrada" });

            return Ok(new { data = tarjeta });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al actualizar tarjeta", detalle = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTarjeta(int id)
    {
        try
        {
            var resultado = await _tarjetaService.DeleteTarjetaAsync(id);
            if (!resultado)
                return NotFound(new { mensaje = "Tarjeta no encontrada" });

            return Ok(new { mensaje = "Tarjeta eliminada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al eliminar tarjeta", detalle = ex.Message });
        }
    }
}
