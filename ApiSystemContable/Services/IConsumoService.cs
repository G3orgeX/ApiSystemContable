using ApiSystemContable.DTOs;
using ApiSystemContable.Models;

namespace ApiSystemContable.Services;

public interface IConsumoService
{
    Task<List<ConsumoDto>> GetConsumosByTarjetaAsync(int idTarjeta);
    Task<List<ConsumoDto>> GetConsumosByUsuarioAsync(int idUsuario);
    Task<ConsumoDto?> GetConsumoByIdAsync(int idConsumo);
    Task<ConsumoDto> CreateConsumoAsync(CreateConsumoDto dto);
    Task<ConsumoDto?> UpdateConsumoAsync(int idConsumo, UpdateConsumoDto dto);
    Task<bool> DeleteConsumoAsync(int idConsumo);
}
