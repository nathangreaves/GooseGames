using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace GooseGames
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
            services.AddControllers();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddSignalR(); 
           
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(RequestLogger<>));

            MSSQLRepository.RepositoryConfiguration.ConfigureServices(services, Configuration);

            services.AddScoped<Services.JustOne.SessionService>();
            services.AddScoped<Services.JustOne.PlayerDetailsService>();
            services.AddScoped<Services.JustOne.PlayerStatusService>();
            services.AddScoped<Services.JustOne.RoundService>();
            services.AddScoped<Services.JustOne.PlayerResponseService>();
            services.AddScoped<Services.JustOne.PlayerActionInformationService>();
            services.AddScoped<Services.JustOne.PrepareNextRoundService>();
            services.AddScoped<Services.JustOne.RoundStatus.NewRoundStatusService>();
            services.AddScoped<Services.JustOne.PlayerStatusQueryService>();

            services.AddScoped<Services.Fuji.SessionService>();
            services.AddScoped<Services.Fuji.PlayerDetailsService>();
            services.AddScoped<Services.Fuji.CardService>();
            services.AddScoped<Services.Fuji.HandService>();

            services.AddScoped<PlayerHubContext>();
            services.AddScoped<FujiHubContext>();

            services.AddTransient<Services.JustOne.RoundStatus.RoundServiceProvider>();
            AddKeyedServices<Services.JustOne.RoundStatus.IRoundStatusKeyedService>(services);
        }

        private void AddKeyedServices<T>(IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var assembly = typeof(Startup).Assembly;
            var typesFromAssemblies = assembly.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))).Where(x => !x.IsAbstract);
            foreach (var type in typesFromAssemblies)
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<PlayerHub>("/lobbyhub");
                endpoints.MapHub<FujiHub>("/fujihub");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
