

using Clinic.Application.DTOs.Authentication;

namespace Clinic.Application.Services.Authentication
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginRequestDto request);
    }
}
