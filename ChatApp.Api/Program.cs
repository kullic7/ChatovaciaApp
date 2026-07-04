using ChatApp.Api.Data;
using ChatApp.Api.Services;
using ChatApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<ChatDbContext>(options =>
{
	var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

	options.UseMySql(
		connectionString,
		new MySqlServerVersion(new Version(8, 4, 0))
	);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddOpenApi();

var app = builder.Build();
await ApplyMigrationsWithRetryAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static async Task ApplyMigrationsWithRetryAsync(WebApplication app)
{
	const int maxAttempts = 10;

	for (var attempt = 1; attempt <= maxAttempts; attempt++)
	{
		try
		{
			using var scope = app.Services.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

			await dbContext.Database.MigrateAsync();

			Console.WriteLine("Database migration completed.");
			return;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Database migration failed. Attempt {attempt}/{maxAttempts}. Error: {ex.Message}");

			if (attempt == maxAttempts)
			{
				throw;
			}

			await Task.Delay(TimeSpan.FromSeconds(5));
		}
	}
}