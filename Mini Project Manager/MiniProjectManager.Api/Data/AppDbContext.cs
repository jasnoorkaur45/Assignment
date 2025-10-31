using Microsoft.EntityFrameworkCore;
using MiniProjectManager.Api.Models;

namespace MiniProjectManager.Api.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users => Set<User>();
		public DbSet<Project> Projects => Set<Project>();
		public DbSet<TaskItem> Tasks => Set<TaskItem>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Username)
				.IsUnique();

			modelBuilder.Entity<Project>()
				.Property(p => p.Title)
				.IsRequired()
				.HasMaxLength(100);
			modelBuilder.Entity<Project>()
				.Property(p => p.Description)
				.HasMaxLength(500);
			modelBuilder.Entity<Project>()
				.HasOne(p => p.User)
				.WithMany(u => u.Projects)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TaskItem>()
				.Property(t => t.Title)
				.IsRequired();
			modelBuilder.Entity<TaskItem>()
				.HasOne(t => t.Project)
				.WithMany(p => p.Tasks)
				.HasForeignKey(t => t.ProjectId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}

