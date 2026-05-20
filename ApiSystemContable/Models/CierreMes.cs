using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSystemContable.Models;

public class CierreMes
{
    [Key]
    public int IdCierreMes { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    [Column(TypeName = "decimal(14,2)")]
    public decimal TotalDebe { get; set; }
    [Column(TypeName = "decimal(14,2)")]
    public decimal TotalHaber { get; set; }
    [Column(TypeName = "decimal(14,2)")]
    public decimal Saldo { get; set; } // Esta es la columna calculada en la DB
    public required string Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public ICollection<DetalleCierreMes> DetalleCierreMeses { get; set; } = new List<DetalleCierreMes>();
}