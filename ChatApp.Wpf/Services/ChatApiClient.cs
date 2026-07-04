using System.Net.Http;
using System.Net.Http.Json;
using ChatApp.Shared.DTOs;

namespace ChatApp.Wpf.Services;

public class ChatApiClient
{
	private readonly HttpClient _httpClient;

	public ChatApiClient()
	{
		_httpClient = new HttpClient
		{
			BaseAddress = new Uri("http://localhost:8080")
		};
	}

	public async Task<UserResponse> CreateOrGetUserAsync(string username)
	{
		var request = new CreateUserRequest
		{
			Username = username
		};

		var response = await _httpClient.PostAsJsonAsync("/api/users", request);

		response.EnsureSuccessStatusCode();

		var user = await response.Content.ReadFromJsonAsync<UserResponse>();

		if (user is null)
		{
			throw new InvalidOperationException("API returned empty user response.");
		}

		return user;
	}

	public async Task<List<MessageResponse>> GetMessagesAsync(int? afterId = null)
	{
		var url = afterId.HasValue
			? $"/api/messages?afterId={afterId.Value}"
			: "/api/messages";

		var messages = await _httpClient.GetFromJsonAsync<List<MessageResponse>>(url);

		return messages ?? new List<MessageResponse>();
	}

	public async Task<MessageResponse> SendMessageAsync(int userId, string text)
	{
		var request = new CreateMessageRequest
		{
			UserId = userId,
			Text = text
		};

		var response = await _httpClient.PostAsJsonAsync("/api/messages", request);

		response.EnsureSuccessStatusCode();

		var message = await response.Content.ReadFromJsonAsync<MessageResponse>();

		if (message is null)
		{
			throw new InvalidOperationException("API returned empty message response.");
		}

		return message;
	}
}
