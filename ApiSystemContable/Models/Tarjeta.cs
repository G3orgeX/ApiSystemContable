using System.ComponentModel.DataAnnotations;

namespace ApiSystemContable.Models;

public class Tarjeta
{
    [Key]
    public int IdTarjeta { get; set; }
    public int IdUsuario { get; set; }
    public required string Nombre { get; set; }
    public required string Titular { get; set; }
    public required string Tipo { get; set; }
    public int DiaCierre { get; set; }
    public bool Activo { get; set; }

    // Propiedades de navegación para relaciones
    public ICollection<Consumo> Consumos { get; set; } = new List<Consumo>();
    public ICollection<ResumenTarjeta> ResumenTarjetas { get; set; } = new List<ResumenTarjeta>();
}