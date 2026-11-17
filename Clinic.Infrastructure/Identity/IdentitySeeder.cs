using Clinic.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Clinic.Infrastructure.Identity
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public IdentitySeeder(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            string[] roles =
            {
            "Administrator",
            "Front Desk",
            "Nurse",
            "Doctor",
            "Patient"
        };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(
                        new IdentityRole<Guid>(role));

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Could not create role {role}: " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            var email = _configuration["SeedAdmin:Email"];
            var fullName = _configuration["SeedAdmin:FullName"];
            var password = _configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Administrator seed settings are missing.");
            }

            var admin = await _userManager.FindByEmailAsync(email);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(admin, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Could not create administrator: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await _userManager.IsInRoleAsync(admin, "Administrator"))
            {
                var result = await _userManager.AddToRoleAsync(
                    admin,
                    "Administrator");

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Could not assign Administrator role: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
 }
