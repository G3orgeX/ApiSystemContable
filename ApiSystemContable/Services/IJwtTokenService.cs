using ApiSystemContable.Models;

namespace ApiSystemContable.Services
{
    public interface IJwtTokenService
    {
        string GetToken(Usuario usuario);
    }
}
