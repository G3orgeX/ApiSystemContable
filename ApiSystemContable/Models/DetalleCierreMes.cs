using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSystemContable.Models;

public class DetalleCierreMes
{
    [Key]
    public int IdDetalle { get; set; }
    public int IdCierreMes { get; set; }
    public int IdTipoMovimiento { get; set; }
    public int? IdResumenTarjeta { get; set; }

    public DateTime Fecha { get; set; }
    public required string Descripcion { get; set; }
    
    [Column(TypeName = "decimal(14,2)")]
    public decimal Monto { get; set; }
    public DateTime FechaCreacion { get; set; }

    [ForeignKey("IdCierreMes")]
    public CierreMes? CierreMes { get; set; }

    [ForeignKey("IdTipoMovimiento")]
    public TipoMovimiento? TipoMovimiento { get; set; }

    [ForeignKey("IdResumenTarjeta")]
    public ResumenTarjeta? ResumenTarjeta { get; set; }
}
