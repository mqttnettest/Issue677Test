using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.AspNetCore;
using MQTTnet.Server;

namespace Issue677Test2
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var mqttOptions = new MqttServerOptionsBuilder()
                        .WithSubscriptionInterceptor(c => {
                            // This callback area is not fired regardless tested using breakpoint and Console.WriteLine
                            Console.WriteLine($"WithSubscriptionInterceptor: client id = {c.ClientId}");
                            c.AcceptSubscription = false;
                            c.CloseConnection = true;
                        })
                // Other validation + interceptor
                .Build();

            services.AddHostedMqttServer(mqttOptions);
            services.AddMqttTcpServerAdapter();
            services.AddMqttWebSocketServerAdapter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
