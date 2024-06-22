namespace TaskManagerApp.DTOs
{
	public class UserTasksDTO
	{
        public string? Username { get; set; }
        public string? UserId { get; set; }
		
		public string? Email { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public List<TaskItemDTO>? Tasks { get; set; }

	}
}
