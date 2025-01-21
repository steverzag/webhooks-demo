using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Webhooks.API.Data;

namespace Webhooks.API.Extensions
{
	public static class WebApplicationExtensions
	{
		public static async Task ApplyMigrationsAsync(this WebApplication app)
		{ 
			using var scope = app.Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			await db.Database.MigrateAsync();
		}

	}
}
