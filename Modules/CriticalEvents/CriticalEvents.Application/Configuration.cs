using CriticalEvents.Domain.Interfaces;
using CriticalEvents.Domain.Services;
using CriticalEvents.Persistence;
using Microsoft.Extensions.DependencyInjection;


namespace CriticalEvents.Application
{
    public static class Configuration
    {
        public static void AddCriticalEventsHandler(this IServiceCollection services)
        {
            services.AddScoped<CriticalEventHandler>();
            services.AddScoped<IAccidentStorage, AccidentStorage>();
            services.AddSingleton<CrititicalEventsProcessor>();
            services.AddSingleton<IAccidentStorageFactory, AccidentStorageFactory>();
        }
    }
}
