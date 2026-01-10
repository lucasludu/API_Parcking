using Application.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence.Seed
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync (IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in RolesConstants.ValidRoles)
            {
                if(!await roleManager.RoleExistsAsync (role))
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }

            //var config = serviceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var adminSettings = configuration.GetSection("AdminSettings");

            var adminEmail = adminSettings["Email"];
            var adminPassword = adminSettings["Password"];
            var name = adminSettings["Name"];
            var lastName = adminSettings["LastName"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                return; // Skip seeding if config is missing
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser { 
                    UserName = adminEmail, 
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RolesConstants.Admin);
                }
            }
        }
    }
}
