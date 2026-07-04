using ChatApp.Api.Data;
using ChatApp.Shared.DTOs;
using ChatApp.Api.Entities;
using ChatApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Services;

internal sealed class UserService : IUserService
{
	private readonly ChatDbContext _dbContext;

	public UserService(ChatDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<UserResponse> CreateOrGetUserAsync(CreateUserRequest request)
	{
		var username = request.Username.Trim();

		if (string.IsNullOrWhiteSpace(username))
		{
			throw new ArgumentException("Username cannot be empty.");
		}

		var existingUser = await _dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Username == username);

		if (existingUser is not null)
		{
			return new UserResponse
			{
				Id = existingUser.Id,
				Username = existingUser.Username
			};
		}

		var user = new User
		{
			Username = username
		};

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync();

		return new UserResponse
		{
			Id = user.Id,
			Username = user.Username
		};
	}
}
