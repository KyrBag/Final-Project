namespace TaskManagerApp.Services
{
	public class IApplicationService
	{
		UserService? userService { get; }
		TaskItemService? taskItemService { get; }
	}
}
