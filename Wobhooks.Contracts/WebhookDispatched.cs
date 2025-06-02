namespace Webhooks.Contracts
{
	public sealed record WebhookDispatched(string eventType, object Data);
}
