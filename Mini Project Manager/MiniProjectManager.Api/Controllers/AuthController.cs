using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Services;

namespace MiniProjectManager.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
		{
			try
			{
				var user = await _authService.RegisterAsync(dto.Username, dto.Password);
				var token = await _authService.LoginAsync(dto.Username, dto.Password);
				return Ok(new AuthResponseDto { Token = token ?? string.Empty, Username = user.Username });
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
		}

		[HttpPost("login")]
		public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
		{
			var token = await _authService.LoginAsync(dto.Username, dto.Password);
			if (token == null) return Unauthorized(new { message = "Invalid credentials" });
			return Ok(new AuthResponseDto { Token = token, Username = dto.Username });
		}
	}
}

