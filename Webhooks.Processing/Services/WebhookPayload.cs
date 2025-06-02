﻿namespace Webhooks.Processing.Services
{
	public class WebhookPayload
	{
		public Guid Id { get; set; }
		public string EventType { get; set; }
		public Guid SubscriptionId { get; set; }
		public DateTime TimeStamp { get; set; }
		public object Data { get; set; }
	}
}
