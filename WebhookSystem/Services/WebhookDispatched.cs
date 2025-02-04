namespace Webhooks.API.Services
{
	internal sealed record WebhookDispatched(string eventType, object Data);
}
