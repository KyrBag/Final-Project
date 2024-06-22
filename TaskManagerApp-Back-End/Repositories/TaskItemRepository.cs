using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Repositories
{
	public class TaskItemRepository : IEntityRepository<TaskItem>
	{
		private readonly TaskManagerDbContext _context;

		public TaskItemRepository(TaskManagerDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<TaskItem>> GetAllAsync()
		{
			return await _context.TaskItems.ToListAsync();
		}

		public async Task<TaskItem> GetByIdAsync(int id)
		{
			return await _context.TaskItems.FindAsync(id);
		}

		public async Task<TaskItem> AddAsync(TaskItem entity)
		{
			_context.TaskItems.Add(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<TaskItem> UpdateAsync(TaskItem entity)
		{
			_context.TaskItems.Update(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task DeleteAsync(int id)
		{
			var taskItem = await _context.TaskItems.FindAsync(id);
			if (taskItem != null)
			{
				_context.TaskItems.Remove(taskItem);
				await _context.SaveChangesAsync();
			}
		}
	}
}

