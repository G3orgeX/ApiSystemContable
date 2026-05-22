namespace ApiSystemContable.DTOs;

public class TarjetaDto
{
    public int IdTarjeta { get; set; }
    public string Nombre { get; set; }
    public string Titular { get; set; }
    public string Tipo { get; set; }
    public int DiaCierre { get; set; }
    public bool Activo { get; set; }
}

public class CreateTarjetaDto
{
    public required string Nombre { get; set; }
    public required string Titular { get; set; }
    public required string Tipo { get; set; }
    public int DiaCierre { get; set; }
    public bool Activo { get; set; } = true;
}

public class UpdateTarjetaDto
{
    public string? Nombre { get; set; }
    public string? Titular { get; set; }
    public string? Tipo { get; set; }
    public int? DiaCierre { get; set; }
    public bool? Activo { get; set; }
}
