namespace Webhooks.API.Services
{
	internal sealed record WebhookDispatch(string eventType, object Data, string activityId);
}
