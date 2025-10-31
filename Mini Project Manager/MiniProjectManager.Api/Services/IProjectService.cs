using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Models;

namespace MiniProjectManager.Api.Services
{
	public interface IProjectService
	{
		Task<List<Project>> GetUserProjectsAsync(int userId);
		Task<Project> CreateProjectAsync(int userId, ProjectCreateDto dto);
		Task<Project?> GetProjectAsync(int userId, int projectId);
		Task<bool> DeleteProjectAsync(int userId, int projectId);
	}
}

