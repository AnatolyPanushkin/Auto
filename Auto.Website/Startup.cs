using Auto.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using Auto.Website.Controllers.Api;
using Auto.Website.GraphQL.Schemas;
using Auto.Website.Hubs;
using EasyNetQ;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using MassTransit;
using IBus = EasyNetQ.IBus;


namespace Auto.Website {
    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddSingleton<IAutoDatabase, AutoCsvFileDatabase>();
            services.AddSignalR();
            services.AddScoped<ISchema,AutoSchema>();
            services.AddScoped<ISchema,AutoSchema>();
            services.AddGraphQL(options => { options.EnableMetrics = true; }).AddSystemTextJson();

            services.AddSwaggerGen(
                config => {
                    config.SwaggerDoc("v1", new OpenApiInfo() {
                        Title = "Auto API"
                    });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    config.IncludeXmlComments(xmlPath);
                });
            
            var bus = RabbitHutch.CreateBus(Configuration.GetConnectionString("AutoRabbitMQ"));
            services.AddSingleton<IBus>(bus);
            
            
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq();
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseGraphQLAltair();
                app.UseExceptionHandler("/Home/Error");
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            //app.MapHub<AutoHub>("/hub");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseGraphQL<ISchema>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AutoHub>("/hub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
