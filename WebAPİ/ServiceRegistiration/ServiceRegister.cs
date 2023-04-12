using Microsoft.AspNetCore.RateLimiting;
using Presentation.ActionFilters;
using Repositories.Abstract;
using Repositories.Concrate;
using Services.Abstract;
using Services.Concrate;
using System.Threading.RateLimiting;

namespace WebAPİ.ServiceRegistiration
{
    public static class ServiceRegister
    {
        public static void ExtensionService(this IServiceCollection collection)
        {
            collection.AddScoped<IRepositoryManager, RepositoryManager>();
            collection.AddScoped<IServiceManager, ServiceManager>();
            collection.AddSingleton<ILoggerService, LoggerService>();
            collection.AddAutoMapper(typeof(Program));
            collection.AddScoped<ValidationFilterAttribute>();
            collection.AddSingleton<LogDetailsAttribute>();
            collection.AddResponseCaching();
            collection.AddRateLimiter(config =>
            {
                config.AddFixedWindowLimiter("Fixed", config =>
                {
                    config.Window = TimeSpan.FromSeconds(10);
                    config.PermitLimit = 5;
                    config.QueueLimit = 10;
                    config.AutoReplenishment = false;
                    config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
            //collection.AddHttpCacheHeaders(config =>
            //{
            //    config.MaxAge = 60;
            //    config.CacheLocation = Marvin.Cache.Headers.CacheLocation.Public;
            //});
            collection.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination");
                });
            });
        }
    }
}
