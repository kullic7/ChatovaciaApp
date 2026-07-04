namespace ChatApp.Api.Entities;
public class Message
{
	public int Id { get; set; }

	public int UserId { get; set; }

	public User User { get; set; } = null!;

	public string Text { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
