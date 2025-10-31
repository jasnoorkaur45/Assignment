using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniProjectManager.Api.Data;
using MiniProjectManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=app.db";
	options.UseSqlite(connectionString);
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowVite",
		policy => policy
			.AllowAnyHeader()
			.AllowAnyMethod()
			.WithOrigins("http://localhost:5173")
			.AllowCredentials());
});

// JWT Authentication
var jwtKey = configuration["Jwt:Key"] ?? "dev_super_secret_key_change_me";
var jwtIssuer = configuration["Jwt:Issuer"] ?? "MiniProjectManager";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtIssuer,
			IssuerSigningKey = signingKey
		};
	});

builder.Services.AddAuthorization();

// DI for app services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// Apply migrations / create database
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}

app.UseCors("AllowVite");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

