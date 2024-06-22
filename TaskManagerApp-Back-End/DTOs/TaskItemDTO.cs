using System.Diagnostics.CodeAnalysis;

namespace TaskManagerApp.DTOs
{
	public class TaskItemDTO
	{
		[NotNull]
		public int Id { get; set; }  
		public string? Title { get; set; } 
		public string? Description { get; set; }
		public DateTime? DueDate { get; set; }
		public bool IsCompleted { get; set; }
		public string? Username { get; set; }	
	}
}
