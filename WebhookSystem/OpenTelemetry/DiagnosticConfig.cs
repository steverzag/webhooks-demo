using System.Diagnostics;

namespace Webhooks.API.OpenTelemetry
{
	internal static class DiagnosticConfig
	{
		internal static readonly ActivitySource Source = new ActivitySource("webhooks-api");
	}
}
