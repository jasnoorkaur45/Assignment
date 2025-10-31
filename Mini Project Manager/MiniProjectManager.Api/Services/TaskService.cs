using Microsoft.EntityFrameworkCore;
using MiniProjectManager.Api.Data;
using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Models;

namespace MiniProjectManager.Api.Services
{
	public class TaskService : ITaskService
	{
		private readonly AppDbContext _dbContext;
		public TaskService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<TaskItem> AddTaskAsync(int userId, int projectId, TaskCreateDto dto)
		{
			var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
			if (project == null) throw new KeyNotFoundException("Project not found");

			var task = new TaskItem
			{
				ProjectId = projectId,
				Title = dto.Title,
				DueDate = dto.DueDate,
				IsCompleted = false
			};
			_dbContext.Tasks.Add(task);
			await _dbContext.SaveChangesAsync();
			return task;
		}

		public async Task<TaskItem?> UpdateTaskAsync(int userId, int taskId, TaskUpdateDto dto)
		{
			var task = await _dbContext.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId && t.Project!.UserId == userId);
			if (task == null) return null;
			task.Title = dto.Title;
			task.DueDate = dto.DueDate;
			task.IsCompleted = dto.IsCompleted;
			await _dbContext.SaveChangesAsync();
			return task;
		}

		public async Task<bool> DeleteTaskAsync(int userId, int taskId)
		{
			var task = await _dbContext.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId && t.Project!.UserId == userId);
			if (task == null) return false;
			_dbContext.Tasks.Remove(task);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}

