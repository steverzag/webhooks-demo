using Microsoft.EntityFrameworkCore.Query;
using System.Diagnostics;
using System.Threading.Channels;
using Webhooks.API.OpenTelemetry;

namespace Webhooks.API.Services
{
	internal sealed class WebhookProcessor(
		IServiceScopeFactory scopeFactory,
		Channel<WebhookDispatch> webhookChannel
		) : BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await foreach (WebhookDispatch dispatch in webhookChannel.Reader.ReadAllAsync(stoppingToken))
			{
				using Activity? activity = DiagnosticConfig.Source.StartActivity(
					$"{dispatch.eventType} dispatch webhook", 
					ActivityKind.Internal, 
					parentId: dispatch.activityId);

				using IServiceScope scope = scopeFactory.CreateScope();
				var dispatcher = scope.ServiceProvider.GetRequiredService<WebhookDispatcher>();

				await dispatcher.ProcessAsync(dispatch.eventType, dispatch.Data);
			}
		}
	}
}
