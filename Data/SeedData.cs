using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task CriarRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "Editor", "Leitor" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task CriarAdminAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var email = "admin@portal.local";

        var admin = await userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@site.com",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "Senha@123");
        }

        // 🔥 ISSO TEM QUE FICAR FORA DO IF
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }

    }
}

