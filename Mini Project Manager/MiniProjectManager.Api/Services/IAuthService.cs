using MiniProjectManager.Api.DTOs;
using MiniProjectManager.Api.Models;

namespace MiniProjectManager.Api.Services
{
	public interface IAuthService
	{
		Task<User> RegisterAsync(string username, string password);
		Task<string?> LoginAsync(string username, string password);
	}
}

