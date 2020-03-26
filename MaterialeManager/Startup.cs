using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MaterialeManager.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using MaterialeManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MaterialeManager.Authorization;

namespace MaterialeManager
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Configure Cookie Policy options...
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			// Add Database connection for the data context..
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));


			// Add identity and roles...
			services.AddIdentity<IdentityUser, IdentityRole>(config =>
			{
				config.SignIn.RequireConfirmedEmail = true;
			})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			// Configure Identity options, password, lockout, user etc...
			services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// User settings.
				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789-._@+ ";
				options.User.RequireUniqueEmail = true;
			});

			// Configure Application cookie options...
			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = $"/Identity/Account/Login";
				options.LogoutPath = $"/Identity/Account/Logout";
				options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
			});

			// Configure Email Support for confirmation and password recovery...
			services.AddTransient<IEmailSender, EmailSender>();
			services.Configure<AuthMessageSenderOptions>(Configuration);

			// Configure MVC options...
			services.AddMvc(config =>
			{
				var policy = new AuthorizationPolicyBuilder()
								.RequireAuthenticatedUser()
								.Build();
				config.Filters.Add(new AuthorizeFilter(policy));
			})
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
					options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
				});

			services.AddScoped<IAuthorizationHandler, IsOwnerAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, AdministratorsAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, OperatorsAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, PhotographersAuthorizationHandler>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
				endpoints.MapRazorPages();
			});
		}
	}
}
