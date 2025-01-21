using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using Webhooks.API.Data;
using Webhooks.API.Models;
using Webhooks.API.OpenTelemetry;

namespace Webhooks.API.Services
{

	internal sealed class WebhookDispatcher (
		Channel<WebhookDispatch> webhookChannel,
		IHttpClientFactory httpClientFactory,
		AppDbContext dbContext)
	{
		public async Task DispatchAsync<T>(string eventType, T data)
			where T : notnull
		{
			using Activity? activity = DiagnosticConfig.Source.StartActivity($"{eventType} dispatch webhook");
			activity?.AddTag("event.type", eventType);

			await webhookChannel.Writer.WriteAsync(new WebhookDispatch(eventType, data, activity?.Id));
		}

		public async Task ProcessAsync<T>(string eventType, T data)
		{
			var subscriptions = await dbContext.WebhookSubscriptions
				.AsNoTracking()
				.Where(e => e.EventType == eventType)
				.ToListAsync();

			foreach (var subscription in subscriptions)
			{
				using var httpClient = httpClientFactory.CreateClient();
				var payload = new WebhookPayload<T>
				{
					Id = new Guid(),
					EventType = subscription.EventType,
					SubscriptionId = subscription.Id,
					TimeStamp = DateTime.UtcNow,
					Data = data
				};

				var jsonPayload = JsonSerializer.Serialize(payload);


				try
				{
					var response = await httpClient.PostAsJsonAsync(subscription.WebhookUrl, payload);

					var attemp = new WebhookDeliveryAttempt
					{
						Id = Guid.NewGuid(),
						WebhookSubscriptionId = subscription.Id,
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
						WebhookSubscriptionId = subscription.Id,
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
}
