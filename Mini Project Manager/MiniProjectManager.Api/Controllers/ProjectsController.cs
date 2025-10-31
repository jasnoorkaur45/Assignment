using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Models;
using MiniProjectManager.Api.Services;

namespace MiniProjectManager.Api.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class ProjectsController : ControllerBase
	{
		private readonly IProjectService _projectService;
		private readonly ITaskService _taskService;

		public ProjectsController(IProjectService projectService, ITaskService taskService)
		{
			_projectService = projectService;
			_taskService = taskService;
		}

		private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? "0");

		[HttpGet]
		public async Task<ActionResult<List<ProjectDto>>> GetProjects()
		{
			var userId = GetUserId();
			var projects = await _projectService.GetUserProjectsAsync(userId);
			var result = projects.Select(p => new ProjectDto
			{
				Id = p.Id,
				Title = p.Title,
				Description = p.Description,
				CreationDate = p.CreationDate
			}).ToList();
			return Ok(result);
		}

		[HttpPost]
		public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] ProjectCreateDto dto)
		{
			var userId = GetUserId();
			var project = await _projectService.CreateProjectAsync(userId, dto);
			return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, new ProjectDto
			{
				Id = project.Id,
				Title = project.Title,
				Description = project.Description,
				CreationDate = project.CreationDate
			});
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<object>> GetProjectById([FromRoute] int id)
		{
			var userId = GetUserId();
			var project = await _projectService.GetProjectAsync(userId, id);
			if (project == null) return NotFound();
			return Ok(new
			{
				project.Id,
				project.Title,
				project.Description,
				project.CreationDate,
				Tasks = project.Tasks.Select(t => new
				{
					t.Id,
					t.Title,
					t.DueDate,
					t.IsCompleted
				})
			});
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteProject([FromRoute] int id)
		{
			var userId = GetUserId();
			var success = await _projectService.DeleteProjectAsync(userId, id);
			if (!success) return NotFound();
			return NoContent();
		}

		[HttpPost("{projectId}/tasks")]
		public async Task<ActionResult> AddTask([FromRoute] int projectId, [FromBody] TaskCreateDto dto)
		{
			var userId = GetUserId();
			var task = await _taskService.AddTaskAsync(userId, projectId, dto);
			return Ok(new { task.Id, task.Title, task.DueDate, task.IsCompleted });
		}
	}
}

