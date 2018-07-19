using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplePatch.Examples.Core2.WebAPI.Domain;
using SimplePatch.Mapping;
using System;
using System.Globalization;

namespace SimplePatch.Examples.Core2.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            DeltaConfig.Init(cfg =>
            {
                cfg.AddEntity<Person>();

                cfg.AddMapping((propertyType, newValue) =>
                {
                    var result = new MapResult<object>();

                    if (propertyType != typeof(DateTime?) || newValue.GetType() != typeof(string))
                    {
                        return result.SkipMap();
                    }

                    if (DateTime.TryParseExact((string)newValue, "dd/MM/yyyy", new CultureInfo("it-IT"), DateTimeStyles.None, out var date))
                    {
                        result.Value = date;
                    }
                    else
                    {
                        result.Value = null;
                    }

                    return result;
                });
            });
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<AppDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("AppDbContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
