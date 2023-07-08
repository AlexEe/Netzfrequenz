using API.Data;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddScoped<iFrequencyRepository, FrequencyRepository>();
            return services;
        }
    }
}