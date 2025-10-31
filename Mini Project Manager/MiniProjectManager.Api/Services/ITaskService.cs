using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Models;

namespace MiniProjectManager.Api.Services
{
	public interface ITaskService
	{
		Task<TaskItem> AddTaskAsync(int userId, int projectId, TaskCreateDto dto);
		Task<TaskItem?> UpdateTaskAsync(int userId, int taskId, TaskUpdateDto dto);
		Task<bool> DeleteTaskAsync(int userId, int taskId);
	}
}

