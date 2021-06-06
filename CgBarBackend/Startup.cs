using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CgBarBackend.Factories;
using CgBarBackend.Hubs;
using CgBarBackend.Services;
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
            services.AddSingleton<ITwitterWebhookHandler, TwitterWebhookHandler>();

            // remove when adding mvc
            services.AddMemoryCache();

            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddFile("app.log", append: true);
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            Plugins.Add<AspNetPlugin>();

            var twitterClient = app.ApplicationServices.GetService<ITwitterClientFactory>().ApplicationBearerTokenClient;

            var accountActivityRequestHandler = twitterClient.AccountActivity.CreateRequestHandler();
            app.UseTweetinviWebhooks(new WebhookMiddlewareConfiguration(accountActivityRequestHandler));

            var twitterWebhookHandler = app.ApplicationServices.GetService<ITwitterWebhookHandler>();
            twitterWebhookHandler.Initialize(accountActivityRequestHandler);

            var existingUserSubscriptions = twitterClient.AccountActivity.GetAccountActivitySubscriptionsAsync(
                    app.ApplicationServices.GetService<IConfiguration>()["TwitterApi:Environment"]).GetAwaiter()
                .GetResult();
            twitterWebhookHandler.AddMissingSubscriptions(existingUserSubscriptions);



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TwitterBarHub>("signalR/TwitterBar");
            });
        }
    }
}
