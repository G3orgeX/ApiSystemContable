using ApiSystemContable.DTOs;
using ApiSystemContable.Models;

namespace ApiSystemContable.Services;

public interface ITarjetaService
{
    Task<List<TarjetaDto>> GetTarjetasByUsuarioAsync(int idUsuario);
    Task<TarjetaDto?> GetTarjetaByIdAsync(int idTarjeta);
    Task<TarjetaDto> CreateTarjetaAsync(int idUsuario, CreateTarjetaDto dto);
    Task<TarjetaDto?> UpdateTarjetaAsync(int idTarjeta, UpdateTarjetaDto dto);
    Task<bool> DeleteTarjetaAsync(int idTarjeta);
}
