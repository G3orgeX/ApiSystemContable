using ApiSystemContable.DTOs;
using ApiSystemContable.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiSystemContable.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsumosController : ControllerBase
{
    private readonly IConsumoService _consumoService;
    private readonly ITarjetaService _tarjetaService;

    public ConsumosController(IConsumoService consumoService, ITarjetaService tarjetaService)
    {
        _consumoService = consumoService;
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
    public async Task<IActionResult> GetConsumosByUsuario()
    {
        try
        {
            var userId = GetUsuarioIdFromToken();
            var consumos = await _consumoService.GetConsumosByUsuarioAsync(userId);
            return Ok(new { data = consumos });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener consumos", detalle = ex.Message });
        }
    }

    [HttpGet("tarjeta/{idTarjeta}")]
    public async Task<IActionResult> GetConsumosByTarjeta(int idTarjeta)
    {
        try
        {
            var consumos = await _consumoService.GetConsumosByTarjetaAsync(idTarjeta);
            return Ok(new { data = consumos });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener consumos", detalle = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsumoById(int id)
    {
        try
        {
            var consumo = await _consumoService.GetConsumoByIdAsync(id);
            if (consumo == null)
                return NotFound(new { mensaje = "Consumo no encontrado" });

            return Ok(new { data = consumo });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener consumo", detalle = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateConsumo([FromBody] CreateConsumoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { mensaje = "Validación fallida", errores = errors });
            }

            var consumo = await _consumoService.CreateConsumoAsync(dto);
            return CreatedAtAction(nameof(GetConsumoById), new { id = consumo.IdConsumo }, new { data = consumo, mensaje = "Consumo creado" });
        }
        catch (InvalidOperationException ex)
        {
            // Known validation/operation error
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear consumo: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, new { mensaje = "Error al crear consumo", detalle = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConsumo(int id, [FromBody] UpdateConsumoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var consumo = await _consumoService.UpdateConsumoAsync(id, dto);
            if (consumo == null)
                return NotFound(new { mensaje = "Consumo no encontrado" });

            return Ok(new { data = consumo });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al actualizar consumo", detalle = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsumo(int id)
    {
        try
        {
            var resultado = await _consumoService.DeleteConsumoAsync(id);
            if (!resultado)
                return NotFound(new { mensaje = "Consumo no encontrado" });

            return Ok(new { mensaje = "Consumo eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al eliminar consumo", detalle = ex.Message });
        }
    }
}
