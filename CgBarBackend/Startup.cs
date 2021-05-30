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
using CgBarBackend.Services;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            Plugins.Add<AspNetPlugin>();

            var twitterClient = new TwitterClient(app.ApplicationServices.GetService<ITwitterCredentialsSupplier>()
                .GetTwitterCredentials());

            app.UseTweetinviWebhooks(new WebhookMiddlewareConfiguration(twitterClient.AccountActivity.CreateRequestHandler()));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
