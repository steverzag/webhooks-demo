﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Webhooks.API.OpenTelemetry;
using Webhooks.Contracts;

namespace Webhooks.API.Services
{

	internal sealed class WebhookDispatcher (
		IPublishEndpoint publishEndpoint)
	{
		public async Task DispatchAsync<T>(string eventType, T data)
			where T : notnull
		{
			using Activity? activity = DiagnosticConfig.Source.StartActivity($"{eventType} dispatch webhook");
			activity?.AddTag("event.type", eventType);

			await publishEndpoint.Publish(new WebhookDispatched(eventType, data));
		}
	}
}
