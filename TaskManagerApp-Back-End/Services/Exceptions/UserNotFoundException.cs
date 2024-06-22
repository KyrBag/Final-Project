namespace TaskManagerApp.Services.Exceptions
{
	public class UserNotFoundException : Exception
	{
		public UserNotFoundException(string? s)
			: base(s)
		{ }
	}
}
