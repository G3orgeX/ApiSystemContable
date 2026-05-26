using ApiSystemContable.Data;
using ApiSystemContable.DTOs;
using ApiSystemContable.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiSystemContable.Services;

public class ConsumoService : IConsumoService
{
    private readonly AppDbContext _context;

    public ConsumoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsumoDto>> GetConsumosByTarjetaAsync(int idTarjeta)
    {
        return await _context.Consumos
            .Where(c => c.IdTarjeta == idTarjeta)
            .Select(c => new ConsumoDto
            {
                IdConsumo = c.IdConsumo,
                IdTarjeta = c.IdTarjeta,
                FechaCompra = c.FechaCompra,
                Concepto = c.Concepto,
                MontoTotal = c.MontoTotal,
                Cuotas = c.Cuotas,
                ValorCuota = c.ValorCuota,
                EsDebitoAutomatico = c.EsDebitoAutomatico
            })
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }

    public async Task<List<ConsumoDto>> GetConsumosByUsuarioAsync(int idUsuario)
    {
        return await _context.Consumos
            .Include(c => c.Tarjeta)
            .Where(c => c.Tarjeta != null && c.Tarjeta.IdUsuario == idUsuario)
            .Select(c => new ConsumoDto
            {
                IdConsumo = c.IdConsumo,
                IdTarjeta = c.IdTarjeta,
                FechaCompra = c.FechaCompra,
                Concepto = c.Concepto,
                MontoTotal = c.MontoTotal,
                Cuotas = c.Cuotas,
                ValorCuota = c.ValorCuota,
                EsDebitoAutomatico = c.EsDebitoAutomatico
            })
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }

    public async Task<ConsumoDto?> GetConsumoByIdAsync(int idConsumo)
    {
        var consumo = await _context.Consumos.FindAsync(idConsumo);
        if (consumo == null) return null;

        return new ConsumoDto
        {
            IdConsumo = consumo.IdConsumo,
            IdTarjeta = consumo.IdTarjeta,
            FechaCompra = consumo.FechaCompra,
            Concepto = consumo.Concepto,
            MontoTotal = consumo.MontoTotal,
            Cuotas = consumo.Cuotas,
            ValorCuota = consumo.ValorCuota,
            EsDebitoAutomatico = consumo.EsDebitoAutomatico
        };
    }

    public async Task<ConsumoDto> CreateConsumoAsync(CreateConsumoDto dto)
    {
        // Verify tarjeta exists to avoid FK violations
        var tarjeta = await _context.Tarjetas.FindAsync(dto.IdTarjeta);
        if (tarjeta == null)
        {
            throw new InvalidOperationException($"Tarjeta con id {dto.IdTarjeta} no encontrada.");
        }

        var valorCuota = dto.Cuotas > 0 ? dto.MontoTotal / dto.Cuotas : dto.MontoTotal;

        var consumo = new Consumo
        {
            IdTarjeta = dto.IdTarjeta,
            FechaCompra = dto.FechaCompra,
            Concepto = dto.Concepto,
            MontoTotal = dto.MontoTotal,
            Cuotas = dto.Cuotas,
            ValorCuota = valorCuota,
            EsDebitoAutomatico = dto.EsDebitoAutomatico
        };

        _context.Consumos.Add(consumo);
        await _context.SaveChangesAsync();

        return new ConsumoDto
        {
            IdConsumo = consumo.IdConsumo,
            IdTarjeta = consumo.IdTarjeta,
            FechaCompra = consumo.FechaCompra,
            Concepto = consumo.Concepto,
            MontoTotal = consumo.MontoTotal,
            Cuotas = consumo.Cuotas,
            ValorCuota = consumo.ValorCuota,
            EsDebitoAutomatico = consumo.EsDebitoAutomatico
        };
    }

    public async Task<ConsumoDto?> UpdateConsumoAsync(int idConsumo, UpdateConsumoDto dto)
    {
        var consumo = await _context.Consumos.FindAsync(idConsumo);
        if (consumo == null) return null;

        if (dto.FechaCompra.HasValue) consumo.FechaCompra = dto.FechaCompra.Value;
        if (dto.Concepto != null) consumo.Concepto = dto.Concepto;
        if (dto.MontoTotal.HasValue)
        {
            consumo.MontoTotal = dto.MontoTotal.Value;
            consumo.ValorCuota = consumo.Cuotas > 0 ? consumo.MontoTotal / consumo.Cuotas : consumo.MontoTotal;
        }
        if (dto.Cuotas.HasValue)
        {
            consumo.Cuotas = dto.Cuotas.Value;
            consumo.ValorCuota = consumo.Cuotas > 0 ? consumo.MontoTotal / consumo.Cuotas : consumo.MontoTotal;
        }
        if (dto.EsDebitoAutomatico.HasValue) consumo.EsDebitoAutomatico = dto.EsDebitoAutomatico.Value;

        _context.Consumos.Update(consumo);
        await _context.SaveChangesAsync();

        return new ConsumoDto
        {
            IdConsumo = consumo.IdConsumo,
            IdTarjeta = consumo.IdTarjeta,
            FechaCompra = consumo.FechaCompra,
            Concepto = consumo.Concepto,
            MontoTotal = consumo.MontoTotal,
            Cuotas = consumo.Cuotas,
            ValorCuota = consumo.ValorCuota,
            EsDebitoAutomatico = consumo.EsDebitoAutomatico
        };
    }

    public async Task<bool> DeleteConsumoAsync(int idConsumo)
    {
        var consumo = await _context.Consumos.FindAsync(idConsumo);
        if (consumo == null) return false;

        _context.Consumos.Remove(consumo);
        await _context.SaveChangesAsync();
        return true;
    }
}
