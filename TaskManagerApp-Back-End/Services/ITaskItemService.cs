using TaskManagerApp.Models;

namespace TaskManagerApp.Services
{
	public interface ITaskItemService
	{
		Task<bool> CreateTaskItemAsync(TaskItem taskItem);
		Task<TaskItem?> GetTaskItemByIdAsync(int id);
		Task<IEnumerable<TaskItem>> GetAllTaskItemsAsync();
		Task<bool> UpdateTaskItemAsync(TaskItem taskItem);
		Task<bool> DeleteTaskItemAsync(int id);
	}
}
