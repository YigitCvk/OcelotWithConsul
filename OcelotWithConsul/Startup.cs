using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using System;

namespace OcelotWithConsul
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //private void RegisterServiceToConsul()
        //{
        //    using (var client = new ConsulClient())
        //    {
        //        var registration = new AgentServiceRegistration()
        //        {
        //            ID = "consul-sample-api-9090",
        //            Name = "consul-sample-api",
        //            Address = "localhost",
        //            Port = 8500,
        //            Check = new AgentCheckRegistration()
        //            {
        //                HTTP = "http://localhost:9090/health",
        //                Interval = TimeSpan.FromSeconds(10)
        //            }
        //        };

        //        client.Agent.ServiceRegister(registration).Wait();
        //    }
        //}
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddConsul();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OcelotWithConsul", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OcelotWithConsul v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            await app.UseOcelot();

            //RegisterServiceToConsul();
        }
    }
}
