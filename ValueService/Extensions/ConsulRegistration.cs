using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace ValueService.Extensions
{
    public static class ConsulRegistration
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));
            return services;
        }
        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration)
        {
            #region old registry consul

            //var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            //var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            //var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            ////Get Server Ip Adress
            //var features = app.Properties["server.Features"] as FeatureCollection;

            //var addresses = features.Get<IServerAddressesFeature>();

            //if (addresses != null && addresses.Addresses.Any())
            //{
            //    var address = addresses.Addresses.First();
            //    var uri = new Uri(address);
            //    var registration = new AgentServiceRegistration()
            //    {
            //        ID = $"ValueService",
            //        Name = "ValueService",
            //        Address = $"{uri.Host}",
            //        Port = uri.Port,
            //        Tags = new[] { "Values Service", "Value" }
            //    };

            //    logger.LogInformation("Registering with Consul");
            //    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            //    consulClient.Agent.ServiceRegister(registration).Wait();

            //    lifetime.ApplicationStopping.Register(() =>
            //    {
            //        logger.LogInformation("Deregitereing from Consul");
            //        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            //    }); 

            //}

            //Register service with consul
            //using (var client = new ConsulClient())
            //{
            //    var registration = new AgentServiceRegistration()
            //    {
            //        ID = "ValueService",
            //        Name = "ValueService",
            //        Address = "http://127.0.0.1",
            //        Port = 8500,
            //        Check = new AgentCheckRegistration()
            //        {
            //            HTTP = "http://localhost:5000/value",
            //            Interval = TimeSpan.FromSeconds(10)
            //        }
            //    };

            //    client.Agent.ServiceRegister(registration).Wait();
            //}
            #endregion

            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var uri = configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
            var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");

            var registration = new AgentServiceRegistration()
            {
                ID = serviceId ?? "ValuService",
                Name = serviceName ?? "ValueService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] {serviceId, serviceName} 
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering with Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });


            return app;
        }
    }
}
