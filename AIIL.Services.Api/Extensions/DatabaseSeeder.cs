using Entity.Data;
using Entity;
// DatabaseSeeder.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AIIL.Services.Api.Extensions
{
    public static class DatabaseSeeder
    {
        public static async Task SeedSpecializationsAndUserAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Seed Specializations if they don't exist
                if (!context.Specializations.Any())
                {
                    context.Specializations.AddRange(
                        new Specialization { Id = Guid.NewGuid(), Name = "Speaking" },
                        new Specialization { Id = Guid.NewGuid(), Name = "Writing" },
                        new Specialization { Id = Guid.NewGuid(), Name = "Reading" },
                        new Specialization { Id = Guid.NewGuid(), Name = "Listening" }
                    );

                    await context.SaveChangesAsync();
                }

                // Seed Roles if they don't exist
                var roles = new[] { "USER", "TEACHER", "ADMIN" };
                foreach (var roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var role = new IdentityRole(roleName);
                        var result = await roleManager.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            Console.WriteLine($"Role '{roleName}' created successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to create role '{roleName}': " +
                                              string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                }

                // Seed ApplicationUser if it doesn't exist and assign roles
                var email = "aienhancedieltsprep@gmail.com";
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "aienhancedieltsprep",
                        Email = email,
                        Name = "AI-Enhanced IELTS Prep"
                    };

                    var result = await userManager.CreateAsync(user, "Abc@1234");

                    if (result.Succeeded)
                    {
                        // Assign all roles to the user
                        foreach (var roleName in roles)
                        {
                            var roleResult = await userManager.AddToRoleAsync(user, roleName);
                            if (!roleResult.Succeeded)
                            {
                                Console.WriteLine($"Failed to assign role '{roleName}' to user: " +
                                                  string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                            }
                        }

                        Console.WriteLine("User created and roles assigned successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
    }

}
