using Microsoft.AspNetCore.Identity;

namespace TaskManagerApp.Models
{
	public class User : IdentityUser
	{
        //public string? Username { get; set; }
        public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public ICollection<TaskItem>? Tasks { get; set; }
		public string? UserRole { get; set; }

	}

}
