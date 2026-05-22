using ApiSystemContable.Models;

namespace ApiSystemContable.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; internal set; }
        public UsuarioDto Usuario { get; internal set; }
    }
}
