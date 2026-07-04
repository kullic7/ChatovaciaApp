using ChatApp.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Api.Data;

public class ChatDbContext : DbContext
{
	public DbSet<User> Users => Set<User>();
	public DbSet<Message> Messages => Set<Message>();

	public ChatDbContext(DbContextOptions<ChatDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(u => u.Id);

			entity.Property(u => u.Username)
				.IsRequired()
				.HasMaxLength(50);

			entity.HasIndex(u => u.Username)
				.IsUnique();
		});

		modelBuilder.Entity<Message>(entity =>
		{
			entity.HasKey(m => m.Id);

			entity.Property(m => m.Text)
				.IsRequired()
				.HasMaxLength(1000);

			entity.Property(m => m.CreatedAt)
				.IsRequired();

			entity.HasOne(m => m.User)
				.WithMany(u => u.Messages)
				.HasForeignKey(m => m.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});
	}
}