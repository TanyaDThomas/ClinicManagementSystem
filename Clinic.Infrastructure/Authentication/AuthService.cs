using Clinic.Application.DTOs.Authentication;
using Clinic.Application.Interfaces;
using Clinic.Application.Services.Authentication;
using Clinic.Domain.Identity;
using Microsoft.AspNetCore.Identity;



namespace Clinic.Infrastructure.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<string> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(
                user,
                request.Password);

            if (!passwordValid)
            {
                await _userManager.AccessFailedAsync(user);

                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return await _jwtTokenService.GenerateTokenAsync(
                user.Id,
                user.Email!,
                roles);
        }
    }
}
