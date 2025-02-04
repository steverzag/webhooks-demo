using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http;
using System.Text.Json;
using Webhooks.API.Models;
using Webhooks.API.Data;

namespace Webhooks.API.Services
{
	internal sealed class WebhookTriggeredConsumer(IHttpClientFactory httpClientFactory, AppDbContext dbContext) : IConsumer<WebhookTriggered>
	{
		public async Task Consume(ConsumeContext<WebhookTriggered> context)
		{
			using var httpClient = httpClientFactory.CreateClient();
			var payload = new WebhookPayload
			{
				Id = new Guid(),
				EventType = context.Message.EventType,
				SubscriptionId = context.Message.SubscriptionId,
				TimeStamp = DateTime.UtcNow,
				Data = context.Message.Data
			};

			var jsonPayload = JsonSerializer.Serialize(payload);


			try
			{
				var response = await httpClient.PostAsJsonAsync(context.Message.WebhookUrl, payload);
				response.EnsureSuccessStatusCode();

				var attemp = new WebhookDeliveryAttempt
				{
					Id = Guid.NewGuid(),
					WebhookSubscriptionId = context.Message.SubscriptionId,
					Timestamp = DateTime.UtcNow,
					Payload = jsonPayload,
					ResponseStatusCode = (int)response.StatusCode,
					Success = response.IsSuccessStatusCode
				};

				dbContext.WebhookDeliveryAttemps.Add(attemp);


			}
			catch (Exception ex)
			{
				var attemp = new WebhookDeliveryAttempt
				{
					Id = Guid.NewGuid(),
					WebhookSubscriptionId = context.Message.SubscriptionId,
					Timestamp = DateTime.UtcNow,
					Payload = jsonPayload,
					ResponseStatusCode = null,
					Success = false
				};

				dbContext.WebhookDeliveryAttemps.Add(attemp);
			}

			await dbContext.SaveChangesAsync();
		}
	}
}
