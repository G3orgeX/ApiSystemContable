using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSystemContable.Models;

public class ResumenConsumo
{
    [Key]
    public int IdResumenConsumo { get; set; }
    public int IdResumen { get; set; }
    public int IdConsumo { get; set; }
    public int NumeroCuota { get; set; }
    
    [Column(TypeName = "decimal(14,2)")]
    public decimal MontoCuota { get; set; }

    [ForeignKey("IdResumen")]
    public ResumenTarjeta? ResumenTarjeta { get; set; }

    [ForeignKey("IdConsumo")]
    public Consumo? Consumo { get; set; }
}
