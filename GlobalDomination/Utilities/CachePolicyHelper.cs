using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Entity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Caching;
using Polly.Caching.Distributed;
using Polly.Registry;

namespace GlobalDomination.Utilities
{
    public class CachePolicyHelper
    {
        private static ILogger _logger;
        public static Dictionary<Type, int> EntityCacheDurations = new Dictionary<Type, int>();
        public const string FacilitiesEntityCachePolicyName = "FacilitiesEntityCachePolicy";
        public const string FacilitiesEntityCollectionCachePolicyName = "FacilitiesEntityCollectionCachePolicy";
        public const string FacilitiesCacheDurationKey = "FacilitiesCacheDuration";

        public const string FacilityTypesEntityCachePolicyName = "FacilityTypesEntityCachePolicy";
        public const string FacilityTypesEntityCollectionCachePolicyName = "FacilityTypesEntityCollectionCachePolicy";
        public const string FacilityTypesCacheDurationKey = "FacilityTypesCacheDuration";

        private static Func<Context, object, Ttl> SkipNullsFilter =
               (context, result) =>
               {
                   int duration = 0;
                   return new Ttl(
                       timeSpan: (result != null && CachePolicyHelper.EntityCacheDurations.TryGetValue(result.GetType(), out duration)) ? TimeSpan.FromMinutes(duration) : TimeSpan.Zero,
                       slidingExpiration: false
                   );
               };
        private static Action<Context, string> OnCacheGet =
            (context, cacheKey) =>
            {
                _logger.LogInformation($"Get {cacheKey}, context: {context.ToString()}");
            };
        private static Action<Context, string> OnCacheMiss =
            (context, cacheKey) =>
            {
                _logger.LogInformation($"Miss {cacheKey}, context: {context.ToString()}");
            };
        private static Action<Context, string> OnCachePut =
            (context, cacheKey) =>
            {
                _logger.LogInformation($"Put {cacheKey}, context: {context.ToString()}");
            };
        private static Action<Context, string, Exception> OnCacheGetError =
            (context, cacheKey, exception) =>
            {
                _logger.LogError(exception, $"Get Error {cacheKey}, context: {context.ToString()}, {exception.ToString()}");
            };
        private static Action<Context, string, Exception> OnCachePutError =
            (context, cacheKey, exception) =>
            {
                _logger.LogError(exception, $"Put Error {cacheKey}, context: {context.ToString()}, {exception.ToString()}");
            };
        public static void RegisterCachePolicies(IServiceProvider serviceProvider, PolicyRegistry registry)
        {
            registry.Add(CachePolicyHelper.FacilitiesEntityCachePolicyName, Policy.CacheAsync<Facilities>(serviceProvider.GetRequiredService<IAsyncCacheProvider<Facilities>>(),
                       ttlStrategy: new ResultTtl<Facilities>(SkipNullsFilter), OnCacheGet, OnCacheMiss, OnCachePut, OnCacheGetError, OnCachePutError));
            registry.Add(CachePolicyHelper.FacilitiesEntityCollectionCachePolicyName, Policy.CacheAsync<List<Facilities>>(serviceProvider.GetRequiredService<IAsyncCacheProvider<List<Facilities>>>(),
                       ttlStrategy: new ResultTtl<List<Facilities>>(SkipNullsFilter), OnCacheGet, OnCacheMiss, OnCachePut, OnCacheGetError, OnCachePutError));

            registry.Add(CachePolicyHelper.FacilityTypesEntityCachePolicyName, Policy.CacheAsync<FacilityTypes>(serviceProvider.GetRequiredService<IAsyncCacheProvider<FacilityTypes>>(),
                       ttlStrategy: new ResultTtl<FacilityTypes>(SkipNullsFilter), OnCacheGet, OnCacheMiss, OnCachePut, OnCacheGetError, OnCachePutError));
            registry.Add(CachePolicyHelper.FacilityTypesEntityCollectionCachePolicyName, Policy.CacheAsync<List<FacilityTypes>>(serviceProvider.GetRequiredService<IAsyncCacheProvider<List<FacilityTypes>>>(),
                       ttlStrategy: new ResultTtl<List<FacilityTypes>>(SkipNullsFilter), OnCacheGet, OnCacheMiss, OnCachePut, OnCacheGetError, OnCachePutError));
        }

        public static void RegisterCacheLoggingProvider(ILogger logger)
        {
            logger.LogInformation($"Registered Logger for Caching.");
            _logger = logger;
        }

        public static void AddPollyCacheProviders(IServiceCollection services)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                // Any configuration options
            };

            services.AddSingleton<IAsyncCacheProvider<Facilities>>(serviceProvider =>
                serviceProvider
                    .GetRequiredService<IDistributedCache>()
                    .AsAsyncCacheProvider<string>()
                    .WithSerializer<Facilities, string>(
                        new Polly.Caching.Serialization.Json.JsonSerializer<Facilities>(serializerSettings)
                    ));

            services.AddSingleton<IAsyncCacheProvider<List<Facilities>>>(serviceProvider =>
               serviceProvider
                   .GetRequiredService<IDistributedCache>()
                   .AsAsyncCacheProvider<string>()
                   .WithSerializer<List<Facilities>, string>(
                       new Polly.Caching.Serialization.Json.JsonSerializer<List<Facilities>>(serializerSettings)
                   ));

            services.AddSingleton<IAsyncCacheProvider<FacilityTypes>>(serviceProvider =>
                serviceProvider
                    .GetRequiredService<IDistributedCache>()
                    .AsAsyncCacheProvider<string>()
                    .WithSerializer<FacilityTypes, string>(
                        new Polly.Caching.Serialization.Json.JsonSerializer<FacilityTypes>(serializerSettings)
                    ));

            services.AddSingleton<IAsyncCacheProvider<List<FacilityTypes>>>(serviceProvider =>
               serviceProvider
                   .GetRequiredService<IDistributedCache>()
                   .AsAsyncCacheProvider<string>()
                   .WithSerializer<List<FacilityTypes>, string>(
                       new Polly.Caching.Serialization.Json.JsonSerializer<List<FacilityTypes>>(serializerSettings)
                   ));
        }

    }
}
