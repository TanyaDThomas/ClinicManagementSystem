

namespace Clinic.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(Guid userId, string email, IEnumerable<string> roles);
    
    }
}
