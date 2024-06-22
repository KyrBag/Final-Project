using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using TaskManagerApp.Repositories;

namespace TaskManagerApp.Services
{
	public class TaskItemService : ITaskItemService
	{

		private readonly TaskManagerDbContext _context;
		private readonly IMapper? _mapper;
		private readonly ILogger<UserService>? _logger;


		public TaskItemService(TaskManagerDbContext context, ILogger<UserService>? logger, IMapper? mapper)
		{
			_context = context;
			_logger = logger;
			_mapper = mapper;
		}
		
		public async Task<bool> CreateTaskItemAsync(TaskItem taskItem)
		{
			_logger!.LogInformation("1");
			await _context.TaskItems.AddAsync(taskItem);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<TaskItem?> GetTaskItemByIdAsync(int id)
		{
			return await _context.TaskItems.FindAsync(id);
		}
		public async Task<IEnumerable<TaskItem>> GetAllTaskItemsAsync()
		{
			return await _context.TaskItems.ToListAsync();
		}

		

		public async Task<bool> UpdateTaskItemAsync(TaskItem taskItem)
		{
			_logger!.LogInformation("UpdateTaskItemAsync called with TaskItem ID: {Id}", taskItem.Id);

			
			var existingTask = await _context.TaskItems.FindAsync(taskItem.Id);
			if (existingTask == null)
			{
				_logger!.LogWarning("TaskItem with ID: {Id} not found in database.", taskItem.Id);
				throw new KeyNotFoundException("Task item not found.");
			}

			// Update the properties of the existing task
			existingTask.Title = taskItem.Title;
			existingTask.Description = taskItem.Description;
			// Retain the UserId or any other properties as necessary
			existingTask.UserId = taskItem.UserId; // Update UserId if needed

			// Mark the entity as modified
			_context.Entry(existingTask).State = EntityState.Modified;

			await _context.SaveChangesAsync();
			_logger!.LogInformation("TaskItem with ID: {Id} updated in the database.", taskItem.Id);

			// Return the task completion
			return true;
		}



		public async Task<bool> DeleteTaskItemAsync(int id)
		{
			var taskItem = await _context.TaskItems.FindAsync(id);
			if (taskItem == null)
			{
				return false;
			}

			_context.TaskItems.Remove(taskItem);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
