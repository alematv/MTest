﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MTest.Services.Search;
using MTest.Services.Search.Abstraction;

namespace MTest
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
            services.AddDbContext<Contexts.MAppContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("Default")));

            if (Configuration.GetValue<bool>("SearchServices:Google:Use"))
                services.AddScoped<ISearchService, GoogleSearchService>();
            if (Configuration.GetValue<bool>("SearchServices:Bing:Use"))
                services.AddScoped<ISearchService, BingSearchService>();
            if (Configuration.GetValue<bool>("SearchServices:Yandex:Use"))
                services.AddScoped<ISearchService, YandexSearchService>(prov => new YandexSearchService(new System.Net.Http.HttpClient()));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(
                    options => {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.DateFormatString = "dd.MM.yyyy HH:mm:ss";
                    }
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "search",
                    template: "search",
                    defaults: new { controller = "search", action = "search" });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Search}/{action=Index}/{id?}");
            });
        }
    }
}
