using CriticalEvents.Domain.Interfaces;
using CriticalEvents.Domain.Services;
using CriticalEvents.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriticalEvents.Application
{
    public static class Configuration
    {
        public static void AddCriticalEventsHandler(this IServiceCollection services)
        {
            services.AddScoped<CriticalEventHandler>();
            services.AddScoped<IAccidentStorage, AccidentStorage>();
            services.AddSingleton<CrititicalEventsProcessor>();
        }
    }
}
