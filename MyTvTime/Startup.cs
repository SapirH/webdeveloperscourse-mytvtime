using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyTvTime.Data;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace MyTvTime
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
			services.AddControllersWithViews();

			services.AddDbContext<TVContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TVContext")));

			services.AddSession(options => options.IdleTimeout = TimeSpan.FromHours(3));

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => 
			{
				options.LoginPath = "/Auth/Login";
				options.LogoutPath = "/Auth/Logout";
				options.AccessDeniedPath = "/Auth/AccessDenied";
			});

            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Home");
                options.Conventions.AuthorizeFolder("/Movies");
                options.Conventions.AuthorizeFolder("/Statistics");
                options.Conventions.AuthorizeFolder("/Users");
				options.Conventions.AllowAnonymousToPage("/Login");
				options.Conventions.AllowAnonymousToPage("/Create");
				options.Conventions.AllowAnonymousToPage("/ResetPassword");
			});

            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });


            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
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

			app.UseSession();

			app.UseAuthentication();
			app.UseAuthorization();

            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                    response.StatusCode == (int)HttpStatusCode.Forbidden)
                    response.Redirect("/Home/Error");
            });


            app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}");
			});
		}
	}
}
