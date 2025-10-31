using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Services;

namespace MiniProjectManager.Api.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class TasksController : ControllerBase
	{
		private readonly ITaskService _taskService;
		public TasksController(ITaskService taskService)
		{
			_taskService = taskService;
		}

		private int GetUserId() => int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.Security.Claims.ClaimTypes.Name) ?? "0");

		[HttpPut("{taskId}")]
		public async Task<ActionResult<object>> Update([FromRoute] int taskId, [FromBody] TaskUpdateDto dto)
		{
			var userId = GetUserId();
			var updated = await _taskService.UpdateTaskAsync(userId, taskId, dto);
			if (updated == null) return NotFound();
			return Ok(new { updated.Id, updated.Title, updated.DueDate, updated.IsCompleted });
		}

		[HttpDelete("{taskId}")]
		public async Task<ActionResult> Delete([FromRoute] int taskId)
		{
			var userId = GetUserId();
			var ok = await _taskService.DeleteTaskAsync(userId, taskId);
			if (!ok) return NotFound();
			return NoContent();
		}
	}
}

