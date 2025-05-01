using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ProductWarranty.Services;
using Nop.Services.Events;
using Nop.Web.Framework.Events;
using static Nop.Plugin.Misc.ProductWarranty.ProductWarrantyPlugin;

namespace Nop.Plugin.Misc.ProductWarranty.Infrastructure
{
    /// <summary>
    /// Represents a plugin startup class for registering services
    /// </summary>
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register warranty service
            services.AddScoped<IWarrantyService, WarrantyService>();

            // Register event consumer
            services.AddScoped<IConsumer<AdminMenuCreatedEvent>, AdminMenuEventConsumer>();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            // No additional middleware configuration needed
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 3000;
    }
}