using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Models;


namespace TaskManagerApp.Data
{
	public class TaskManagerDbContext : IdentityDbContext<User>     
	{
		public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
		: base(options)
		{
		}

		
		public DbSet<TaskItem> TaskItems { get; set; }

		public DbSet<LoginEvent> LoginEvents { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>()
				.HasMany(u => u.Tasks)
				.WithOne(t => t.User)
				.HasForeignKey(t => t.UserId)
				.OnDelete(DeleteBehavior.Cascade); // Cascade delete
		}
	}
}

