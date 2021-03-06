using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CgBarBackend.Factories;
using CgBarBackend.Hubs;
using CgBarBackend.Repositories;
using CgBarBackend.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.AspNet;

namespace CgBarBackend
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ITwitterCredentialsSupplier, TwitterCredentialsSupplier>();
            services.AddSingleton<ITwitterClientFactory, TwitterClientFactory>();
            services.AddSingleton<ITwitterWebhookManager, TwitterWebhookManager>();
            services.AddSingleton<IBarTender, BarTender>();
            services.AddSingleton<IBarTenderRepository, BarTenderRepository>();

            // remove when adding mvc
            services.AddMemoryCache();

            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddFile("app.log", append: true);
            });

            services.AddCors(
            );

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            Plugins.Add<AspNetPlugin>();

            var twitterClient = app.ApplicationServices.GetService<ITwitterClientFactory>().ApplicationBearerTokenClient;

            var accountActivityRequestHandler = twitterClient.AccountActivity.CreateRequestHandler();
            app.UseTweetinviWebhooks(new WebhookMiddlewareConfiguration(accountActivityRequestHandler));

            var twitterWebhookHandler = app.ApplicationServices.GetService<ITwitterWebhookManager>();
            twitterWebhookHandler.Initialize(accountActivityRequestHandler);

            var existingUserSubscriptions = twitterClient.AccountActivity.GetAccountActivitySubscriptionsAsync(
                    app.ApplicationServices.GetService<IConfiguration>()["TwitterApi:Environment"]).GetAwaiter()
                .GetResult();
            twitterWebhookHandler.AddMissingSubscriptions(existingUserSubscriptions);

            ConfigureBartender(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            ConfigureCors(app,env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TwitterBarHub>("signalR/TwitterBar");
            });

        }

        private void ConfigureCors(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder =>
            {
                builder
                    //.WithOrigins("http://127.0.0.1:8080/")
                    .SetIsOriginAllowed((host) => true) //todo figure out why this doesn't work with simply .WithOrigins
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        }

        private void ConfigureBartender(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var bartender = app.ApplicationServices.GetService<IBarTender>();
            var hubContext = app.ApplicationServices.GetService<IHubContext<TwitterBarHub, ITwitterBarHub>>();

            bartender.PatronAdded += async (sender, patron) =>
                await hubContext.NotifyAllPatronAdded(patron).ConfigureAwait(false);
            bartender.DrinkDelivered += async (sender, patron) =>
                await hubContext.NotifyAllDrinkOrdered(patron).ConfigureAwait(false);
            bartender.DrinkExpired += async (sender, screenName) =>
                await hubContext.NotifyAllDrinkExpired(screenName).ConfigureAwait(false);
            bartender.PatronExpired += async (sender, screenName) =>
                await hubContext.NotifyAllPatronExpired(screenName).ConfigureAwait(false);
            bartender.PatronPolitenessChanged += async (sender, patron) =>
                await hubContext.NotifyAllPatronPolitenessChanged(patron).ConfigureAwait(false);
            bartender.BarTenderSpeaks += async (sender, message) =>
                await hubContext.NotifyAllBarTenderMessage(message).ConfigureAwait(false);

            bartender.Load();

        }
    }
}
