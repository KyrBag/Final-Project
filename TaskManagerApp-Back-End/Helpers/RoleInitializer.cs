using Microsoft.AspNetCore.Identity;

namespace TaskManagerApp.Helpers
{
	public static class RoleInitializer
	{
		public static async Task CreateRoles(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			// Check if the role exists
			var roleExists = await roleManager.RoleExistsAsync("User");
			if (!roleExists)
			{
				// If the role doesn't exist, create it
				await roleManager.CreateAsync(new IdentityRole("User"));
			}
		}
	}
}
