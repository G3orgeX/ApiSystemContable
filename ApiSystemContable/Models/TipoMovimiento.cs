using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSystemContable.Models;

public class TipoMovimiento
{
    [Key]
    public int IdTipoMovimiento { get; set; }
    public required string Nombre { get; set; }
    
    [Column(TypeName = "char(1)")]
    public required string Naturaleza { get; set; }

    public ICollection<DetalleCierreMes> DetalleCierreMeses { get; set; } = new List<DetalleCierreMes>();
}
