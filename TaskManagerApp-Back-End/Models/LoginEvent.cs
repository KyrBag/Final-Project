namespace TaskManagerApp.Models
{
	public class LoginEvent
	{
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime LoginTime { get; set; }
    }
}
