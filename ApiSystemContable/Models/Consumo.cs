using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSystemContable.Models;

public class Consumo
{
    [Key]
    public int IdConsumo { get; set; }
    public int IdTarjeta { get; set; }
    public DateTime FechaCompra { get; set; }
    public required string Concepto { get; set; }
    
    [Column(TypeName = "decimal(14,2)")]
    public decimal MontoTotal { get; set; }
    public int Cuotas { get; set; }
    
    [Column(TypeName = "decimal(14,2)")]
    public decimal ValorCuota { get; set; }
    public bool EsDebitoAutomatico { get; set; }

    [ForeignKey("IdTarjeta")]
    public Tarjeta? Tarjeta { get; set; }

    public ICollection<ResumenConsumo> ResumenConsumos { get; set; } = new List<ResumenConsumo>();
}
