using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Threading.Channels;
using Webhooks.API.Data;
using Webhooks.API.Extensions;
using Webhooks.API.Models;
using Webhooks.API.OpenTelemetry;
using Webhooks.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddScoped<WebhookDispatcher>();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("webhooks")));

builder.Services.AddMassTransit(busConfig =>
{
	busConfig.SetKebabCaseEndpointNameFormatter();

	busConfig.UsingRabbitMq((context, config) =>
	{
		config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
		config.ConfigureEndpoints(context);
	});
});

builder.Services.AddOpenTelemetry()
	.WithTracing(tracing =>
	{
		tracing.AddSource(DiagnosticConfig.Source.Name)
		.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
		.AddNpgsql();
	});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.MapPost("/orders",
	async (CreateOrderRequest request,
	AppDbContext dbContext,
	WebhookDispatcher webhookDispatcher) =>
{

	var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);
	dbContext.Orders.Add(order);
	await dbContext.SaveChangesAsync();

	await webhookDispatcher.DispatchAsync("order.created", order);

	return Results.Ok(order);
});

app.MapGet("/orders", async (AppDbContext dbContext) =>
{
	var orders = await dbContext.Orders.ToListAsync();
	return Results.Ok(orders);
});

app.MapPost("/webhooks/subscriptions", async (CreateWebhookRequest request, AppDbContext dbContext) =>
{
	var subscription = new WebhookSubscription(Guid.NewGuid(), request.EventType, request.WebhookUrl, DateTime.UtcNow);
	await dbContext.WebhookSubscriptions.AddAsync(subscription);
	await dbContext.SaveChangesAsync();

	return Results.Ok(subscription);
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
