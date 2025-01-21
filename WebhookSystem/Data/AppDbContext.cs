
using Microsoft.EntityFrameworkCore;
using Webhooks.API.Models;


namespace Webhooks.API.Data
{
	internal sealed class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Order> Orders { get; set; }
		public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
		public DbSet<WebhookDeliveryAttempt> WebhookDeliveryAttemps { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Order>(builder =>
			{
				builder.ToTable("orders");
				builder.HasKey(e => e.Id);
			});

			modelBuilder.Entity<WebhookSubscription>(builder =>
			{
				builder.ToTable("subscriptions", "webhooks");
				builder.HasKey(e => e.Id);
			});

			modelBuilder.Entity<WebhookDeliveryAttempt>(builder =>
			{
				builder.ToTable("delivery_attempts", "webhooks");
				builder.HasKey(e => e.Id);
				builder.HasOne<WebhookSubscription>()
				.WithMany()
					.HasForeignKey(e => e.WebhookSubscriptionId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
