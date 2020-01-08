using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GlobalDomination.Utilities;
using Polly.Registry;
using Database.Entity;

namespace GlobalDomination
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var readOptions = new DbContextOptionsBuilder<GDomContext>();
            readOptions.UseSqlServer(Configuration.GetConnectionString("GDomDatabaseReadonly"));
            GDomContext readContext = new GDomContext(readOptions.Options);

            var writeOptions = new DbContextOptionsBuilder<GDomContext>();
            writeOptions.UseSqlServer(Configuration.GetConnectionString("GDomDatabaseReadWrite"));
            GDomContext writeContext = new GDomContext(writeOptions.Options);

            services.AddSingleton(typeof((GDomContext readGDomContext, GDomContext writeGDomContext)), (readContext, writeContext));

            //services.AddDbContext<GDomContext>(opt =>
            //    opt.UseSqlServer(Configuration.GetConnectionString("GDomDatabase")));
            //services.AddDbContext<GDomContext>(opt =>
            //    opt.UseInMemoryDatabase("GDomDatabase"));

            if (Environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();

            }
            else
            {
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = Configuration.GetValue<string>("GDomReddisConnection");
                    options.InstanceName = Configuration.GetValue<string>("GDomReddisInstance");
                });
            }
            ConfigureCacheHelper();
            CachePolicyHelper.AddPollyCacheProviders(services);
            services.AddSingleton<Polly.Registry.IReadOnlyPolicyRegistry<string>, Polly.Registry.PolicyRegistry>((serviceProvider) =>
            {
                PolicyRegistry registry = new PolicyRegistry();
                CachePolicyHelper.RegisterCachePolicies(serviceProvider, registry);
                return registry;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation($"Application Environment: {env.EnvironmentName}");
            CachePolicyHelper.RegisterCacheLoggingProvider(logger);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void ConfigureCacheHelper()
        {
            CachePolicyHelper.EntityCacheDurations.Add(typeof(Facilities), Configuration.GetValue<int>(CachePolicyHelper.FacilitiesCacheDurationKey));
            CachePolicyHelper.EntityCacheDurations.Add(typeof(List<Facilities>), Configuration.GetValue<int>(CachePolicyHelper.FacilitiesCacheDurationKey));
            CachePolicyHelper.EntityCacheDurations.Add(typeof(FacilityTypes), Configuration.GetValue<int>(CachePolicyHelper.FacilityTypesCacheDurationKey));
            CachePolicyHelper.EntityCacheDurations.Add(typeof(List<FacilityTypes>), Configuration.GetValue<int>(CachePolicyHelper.FacilityTypesCacheDurationKey));
        }
    }
}
