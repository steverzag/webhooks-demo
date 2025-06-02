namespace Webhooks.Processing.Models
{
	public sealed record class WebhookSubscription(Guid Id, string EventType, string WebhookUrl, DateTime CreatedAtUtc);
	public sealed record class CreateWebhookRequest(string EventType, string WebhookUrl);
}
