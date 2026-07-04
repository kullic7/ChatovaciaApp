using ChatApp.Api.Data;
using ChatApp.Api.Entities;
using ChatApp.Shared.DTOs;
using ChatApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Services;

internal sealed class MessageService : IMessageService
{
	private readonly ChatDbContext _dbContext;

	public MessageService(ChatDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<List<MessageResponse>> GetMessagesAsync(int? afterId)
	{
		var query = _dbContext.Messages
			.AsNoTracking()
			.Include(m => m.User)
			.AsQueryable();

		if (afterId.HasValue)
		{
			query = query.Where(m => m.Id > afterId.Value);
		}

		return await query
			.OrderBy(m => m.Id)
			.Take(100)
			.Select(m => new MessageResponse
			{
				Id = m.Id,
				UserId = m.UserId,
				Username = m.User.Username,
				Text = m.Text,
				CreatedAt = m.CreatedAt
			})
			.ToListAsync();
	}

	public async Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request)
	{
		var text = request.Text.Trim();

		if (string.IsNullOrWhiteSpace(text))
		{
			throw new ArgumentException("Message text cannot be empty.");
		}

		var userExists = await _dbContext.Users
			.AnyAsync(u => u.Id == request.UserId);

		if (!userExists)
		{
			throw new ArgumentException("User does not exist.");
		}

		var message = new Message
		{
			UserId = request.UserId,
			Text = text,
			CreatedAt = DateTime.UtcNow
		};

		_dbContext.Messages.Add(message);
		await _dbContext.SaveChangesAsync();

		var response = await _dbContext.Messages
			.AsNoTracking()
			.Include(m => m.User)
			.Where(m => m.Id == message.Id)
			.Select(m => new MessageResponse
			{
				Id = m.Id,
				UserId = m.UserId,
				Username = m.User.Username,
				Text = m.Text,
				CreatedAt = m.CreatedAt
			})
			.FirstAsync();

		return response;
	}
}
