using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSystemContable.Models;

public class ResumenTarjeta
{
    [Key]
    public int IdResumen { get; set; }
    public int IdTarjeta { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    
    [Column(TypeName = "decimal(14,2)")]
    public decimal TotalResumen { get; set; }
    public required string Estado { get; set; }

    [ForeignKey("IdTarjeta")]
    public Tarjeta? Tarjeta { get; set; }

    public ICollection<ResumenConsumo> ResumenConsumos { get; set; } = new List<ResumenConsumo>();
    public ICollection<DetalleCierreMes> DetalleCierreMeses { get; set; } = new List<DetalleCierreMes>();
}
