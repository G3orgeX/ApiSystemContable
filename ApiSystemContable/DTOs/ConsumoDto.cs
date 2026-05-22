namespace ApiSystemContable.DTOs;

public class ConsumoDto
{
    public int IdConsumo { get; set; }
    public int IdTarjeta { get; set; }
    public DateTime FechaCompra { get; set; }
    public string Concepto { get; set; }
    public decimal MontoTotal { get; set; }
    public int Cuotas { get; set; }
    public decimal ValorCuota { get; set; }
    public bool EsDebitoAutomatico { get; set; }
}

public class CreateConsumoDto
{
    public int IdTarjeta { get; set; }
    public DateTime FechaCompra { get; set; }
    public required string Concepto { get; set; }
    public decimal MontoTotal { get; set; }
    public int Cuotas { get; set; } = 1;
    public bool EsDebitoAutomatico { get; set; } = false;
}

public class UpdateConsumoDto
{
    public DateTime? FechaCompra { get; set; }
    public string? Concepto { get; set; }
    public decimal? MontoTotal { get; set; }
    public int? Cuotas { get; set; }
    public bool? EsDebitoAutomatico { get; set; }
}
