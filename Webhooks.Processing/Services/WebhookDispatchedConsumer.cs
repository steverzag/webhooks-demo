﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using Webhooks.Processing.Data;
using Webhooks.Contracts;

namespace Webhooks.Processing.Services
{
	internal sealed class WebhookDispatchedConsumer(AppDbContext dbContext) : IConsumer<WebhookDispatched>
	{
		public async Task Consume(ConsumeContext<WebhookDispatched> context)
		{
			var message = context.Message;

			var subscriptions = await dbContext
				.WebhookSubscriptions
				.AsNoTracking()
				.Where(e => e.EventType == message.eventType)
				.ToListAsync();

			foreach (var subscription in subscriptions)
			{
				await context.Publish(new WebhookTriggered(
					subscription.Id,
					subscription.EventType,
					subscription.WebhookUrl,
					message.Data));
			}
			//await context.PublishBatch(subscriptions.Select(e => new WebhookTrigger(
			//		e.Id,
			//		e.EventType,
			//		e.WebhookUrl,
			//		message.Data)));
		}
	}
}
