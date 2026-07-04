using ChatApp.Shared.DTOs;
using ChatApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;

	public UsersController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpPost]
	public async Task<ActionResult<UserResponse>> CreateOrGetUser(CreateUserRequest request)
	{
		try
		{
			var user = await _userService.CreateOrGetUserAsync(request);
			return Ok(user);
		}
		catch (ArgumentException exception)
		{
			return BadRequest(exception.Message);
		}
	}
}
