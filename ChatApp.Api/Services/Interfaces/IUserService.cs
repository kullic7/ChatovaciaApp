using ChatApp.Shared.DTOs;

namespace ChatApp.Api.Services.Interfaces;

public interface IUserService
{
	Task<UserResponse> CreateOrGetUserAsync(CreateUserRequest request);
}
