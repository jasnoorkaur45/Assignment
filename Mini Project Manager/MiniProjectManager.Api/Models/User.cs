using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Api.Models
{
	public class User
	{
		public int Id { get; set; }

		[Required]
		[MinLength(3)]
		public string Username { get; set; } = string.Empty;

		[Required]
		public string PasswordHash { get; set; } = string.Empty;

		public ICollection<Project> Projects { get; set; } = new List<Project>();
	}
}

