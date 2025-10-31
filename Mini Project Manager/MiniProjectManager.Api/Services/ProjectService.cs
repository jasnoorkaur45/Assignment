using Microsoft.EntityFrameworkCore;
using MiniProjectManager.Api.Data;
using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Models;

namespace MiniProjectManager.Api.Services
{
	public class ProjectService : IProjectService
	{
		private readonly AppDbContext _dbContext;
		public ProjectService(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Project>> GetUserProjectsAsync(int userId)
		{
			return await _dbContext.Projects
				.Where(p => p.UserId == userId)
				.OrderByDescending(p => p.CreationDate)
				.ToListAsync();
		}

		public async Task<Project> CreateProjectAsync(int userId, ProjectCreateDto dto)
		{
			var project = new Project
			{
				Title = dto.Title,
				Description = dto.Description,
				UserId = userId,
				CreationDate = DateTime.UtcNow
			};
			_dbContext.Projects.Add(project);
			await _dbContext.SaveChangesAsync();
			return project;
		}

		public async Task<Project?> GetProjectAsync(int userId, int projectId)
		{
			return await _dbContext.Projects
				.Include(p => p.Tasks)
				.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
		}

		public async Task<bool> DeleteProjectAsync(int userId, int projectId)
		{
			var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);
			if (project == null) return false;
			_dbContext.Projects.Remove(project);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}

