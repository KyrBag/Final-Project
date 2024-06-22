namespace TaskManagerApp.DTOs
{
	public class CreateTaskItemDTO
	{

        
        public string? Username { get; set; }
        public string? Title { get; set; }
		public string? Description { get; set; }
		public DateTime DueDate { get; set; }
	}
}
