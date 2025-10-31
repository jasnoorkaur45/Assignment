using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Api.DTOs
{
	public class ProjectCreateDto
	{
		[Required]
		[StringLength(100, MinimumLength = 3)]
		public string Title { get; set; } = string.Empty;
		[StringLength(500)]
		public string? Description { get; set; }
	}

	public class ProjectDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public DateTime CreationDate { get; set; }
	}

	public class TaskCreateDto
	{
		[Required]
		public string Title { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
	}

	public class TaskUpdateDto
	{
		[Required]
		public string Title { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
		public bool IsCompleted { get; set; }
	}
}

