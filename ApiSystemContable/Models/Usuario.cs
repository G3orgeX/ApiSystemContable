using System.ComponentModel.DataAnnotations;

namespace ApiSystemContable.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime FechaCreacion { get; set; }
}