﻿namespace Webhooks.API.Services
{
	public class WebhookPayload<T>
	{
		public Guid Id { get; set; }
		public string EventType { get; set; }
		public Guid SubscriptionId { get; set; }
		public DateTime TimeStamp { get; set; }
		public T Data { get; set; }
	}
}
