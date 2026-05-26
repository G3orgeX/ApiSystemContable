using ApiSystemContable.Data;
using ApiSystemContable.DTOs;
using ApiSystemContable.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSystemContable.Services;

public class TarjetaService : ITarjetaService
{
    private readonly AppDbContext _context;

    public TarjetaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TarjetaDto>> GetTarjetasByUsuarioAsync(int idUsuario)
    {
        return await _context.Tarjetas
            .Where(t => t.IdUsuario == idUsuario && t.Activo)
            .Select(t => new TarjetaDto
            {
                IdTarjeta = t.IdTarjeta,
                Nombre = t.Nombre,
                Titular = t.Titular,
                Tipo = t.Tipo,
                DiaCierre = t.DiaCierre,
                Activo = t.Activo
            })
            .ToListAsync();
    }

    public async Task<TarjetaDto?> GetTarjetaByIdAsync(int idTarjeta)
    {
        var tarjeta = await _context.Tarjetas.FindAsync(idTarjeta);
        if (tarjeta == null) return null;

        return new TarjetaDto
        {
            IdTarjeta = tarjeta.IdTarjeta,
            Nombre = tarjeta.Nombre,
            Titular = tarjeta.Titular,
            Tipo = tarjeta.Tipo,
            DiaCierre = tarjeta.DiaCierre,
            Activo = tarjeta.Activo
        };
    }

    public async Task<TarjetaDto> CreateTarjetaAsync(int idUsuario, CreateTarjetaDto dto)
    {
        var tarjeta = new Tarjeta
        {
            IdUsuario = idUsuario,
            Nombre = dto.Nombre,
            Titular = dto.Titular,
            Tipo = dto.Tipo.Trim().ToLower() switch
            {
                "credito" => "Credito",
                "debito" => "Debito",
                _ => throw new Exception("Tipo de tarjeta inválido")
            },
            DiaCierre = dto.DiaCierre,
            Activo = dto.Activo
        };


        _context.Tarjetas.Add(tarjeta);
        await _context.SaveChangesAsync();

        return new TarjetaDto
        {
            IdTarjeta = tarjeta.IdTarjeta,
            Nombre = tarjeta.Nombre,
            Titular = tarjeta.Titular,
            Tipo = tarjeta.Tipo,
            DiaCierre = tarjeta.DiaCierre,
            Activo = tarjeta.Activo
        };
    }

    public async Task<TarjetaDto?> UpdateTarjetaAsync(int idTarjeta, UpdateTarjetaDto dto)
    {
        var tarjeta = await _context.Tarjetas.FindAsync(idTarjeta);
        if (tarjeta == null) return null;

        if (dto.Nombre != null) tarjeta.Nombre = dto.Nombre;
        if (dto.Titular != null) tarjeta.Titular = dto.Titular;
        if (dto.Tipo != null) tarjeta.Tipo = dto.Tipo;
        if (dto.DiaCierre.HasValue) tarjeta.DiaCierre = dto.DiaCierre.Value;
        if (dto.Activo.HasValue) tarjeta.Activo = dto.Activo.Value;

        _context.Tarjetas.Update(tarjeta);
        await _context.SaveChangesAsync();

        return new TarjetaDto
        {
            IdTarjeta = tarjeta.IdTarjeta,
            Nombre = tarjeta.Nombre,
            Titular = tarjeta.Titular,
            Tipo = tarjeta.Tipo,
            DiaCierre = tarjeta.DiaCierre,
            Activo = tarjeta.Activo
        };
    }

    public async Task<bool> DeleteTarjetaAsync(int idTarjeta)
    {
        var tarjeta = await _context.Tarjetas.FindAsync(idTarjeta);
        if (tarjeta == null) return false;

        _context.Tarjetas.Remove(tarjeta);
        await _context.SaveChangesAsync();
        return true;
    }
}
