using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialeManager.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MaterialeManager
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<ApplicationDbContext>();
				//context.Database.Migrate();

				// requires using Microsoft.Extensions.Configuration;
				IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
				// Set password with the Secret Manager tool.
				// dotnet user-secrets set SeedUserPW <pw>

				string adminUserPWD = config["SeedUserPW"];

				try
				{
					SeedUserAndRoles.SeedData(services, adminUserPWD).Wait();
					SeedData.SeedDatabaseAsync(context).Wait();
				}
				catch (Exception ex)
				{
					ILogger logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex.Message, "An error occurred seeding the DB.");
				}
			}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				})
				.ConfigureLogging(logging =>
				{
					logging.AddConsole();
				});
	}
}
