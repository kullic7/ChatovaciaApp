namespace ChatApp.Shared.DTOs;

public class CreateMessageRequest
{
	public int UserId { get; set; }

	public string Text { get; set; } = string.Empty;
}
