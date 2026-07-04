using ChatApp.Shared.DTOs;
using ChatApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
	private readonly IMessageService _messageService;

	public MessagesController(IMessageService messageService)
	{
		_messageService = messageService;
	}

	[HttpGet]
	public async Task<ActionResult<List<MessageResponse>>> GetMessages([FromQuery] int? afterId)
	{
		var messages = await _messageService.GetMessagesAsync(afterId);
		return Ok(messages);
	}

	[HttpPost]
	public async Task<ActionResult<MessageResponse>> CreateMessage(CreateMessageRequest request)
	{
		try
		{
			var message = await _messageService.CreateMessageAsync(request);
			return Ok(message);
		}
		catch (ArgumentException exception)
		{
			return BadRequest(exception.Message);
		}
	}
}
