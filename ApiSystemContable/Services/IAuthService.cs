using ApiSystemContable.DTOs;
using ApiSystemContable.Models;

namespace ApiSystemContable.Services;

public interface IAuthService
{
    Task<Usuario?> RegisterNewUserAsync(RegisterUserDto dto);
    Task<Usuario?> LoginAsync(LoginDto dto);
}
