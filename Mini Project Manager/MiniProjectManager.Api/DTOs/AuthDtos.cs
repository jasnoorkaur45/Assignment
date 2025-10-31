using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Api.DTOs
{
	public class RegisterRequestDto
	{
		[Required]
		[MinLength(3)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[MinLength(6)]
		public string Password { get; set; } = string.Empty;
	}

	public class LoginRequestDto
	{
		[Required]
		public string Username { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;
	}

	public class AuthResponseDto
	{
		public string Token { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
	}
}

