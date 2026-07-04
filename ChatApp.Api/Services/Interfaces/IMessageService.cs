using ChatApp.Shared.DTOs;

namespace ChatApp.Api.Services.Interfaces;

public interface IMessageService
{
	Task<List<MessageResponse>> GetMessagesAsync(int? afterId);
	Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request);
}
