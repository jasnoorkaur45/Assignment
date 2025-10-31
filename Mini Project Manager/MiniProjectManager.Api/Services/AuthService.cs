using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniProjectManager.Api.Data;
using MiniProjectManager.Api.Models;

namespace MiniProjectManager.Api.Services
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext _dbContext;
		private readonly IConfiguration _configuration;

		public AuthService(AppDbContext dbContext, IConfiguration configuration)
		{
			_dbContext = dbContext;
			_configuration = configuration;
		}

		public async Task<User> RegisterAsync(string username, string password)
		{
			var existing = await _dbContext.Users.AnyAsync(u => u.Username == username);
			if (existing)
			{
				throw new InvalidOperationException("Username already exists.");
			}

			var user = new User
			{
				Username = username,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
			};

			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();
			return user;
		}

		public async Task<string?> LoginAsync(string username, string password)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
			if (user == null) return null;

			var valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
			if (!valid) return null;

			return GenerateJwt(user);
		}

		private string GenerateJwt(User user)
		{
			var jwtKey = _configuration["Jwt:Key"] ?? "dev_super_secret_key_change_me";
			var issuer = _configuration["Jwt:Issuer"] ?? "MiniProjectManager";
			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
			};

			var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: null,
				claims: claims,
				expires: DateTime.UtcNow.AddDays(7),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}

