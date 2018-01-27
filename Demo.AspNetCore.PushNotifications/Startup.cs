using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Demo.AspNetCore.PushNotifications.Services;
using Demo.AspNetCore.PushNotifications.Formatters;
using Demo.AspNetCore.PushNotifications.Models;
using Microsoft.AspNetCore.Identity;
using Demo.AspNetCore.PushNotifications.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.AspNetCore.PushNotifications
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            options.UseSqlite("Data Source=DemoAuth.db"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddPushSubscriptionStore(Configuration)
                .AddPushNotificationService(Configuration)
                .AddMvc(options =>
                {
                    options.InputFormatters.Add(new TextPlainInputFormatter());
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            context.Database.Migrate();

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("push-notifications.html");

            app.UseAuthentication();

            app//.UseDefaultFiles(defaultFilesOptions)
                .UseStaticFiles()
                .UsePushSubscriptionStore()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                })
            //.Run(async (context2) =>
            //{
            //  await context2.Response.WriteAsync("-- Demo.AspNetCore.PushNotifications --");
            //})
            ;
        }
    }
}
