using API.Contracts;
using API.Controllers;

namespace API.Services
{
    public class DefaultServiceRegistrar
    {
        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            services.AddSingleton<ServiceController.IConfiguration, ServiceController.Configuration>();
            services.AddSingleton<DefaultReceiptManager.IConfiguration, DefaultReceiptManager.Configuration>();
            services.AddSingleton<DefaultReceiptValidator.IConfiguration, DefaultReceiptValidator.Configuration>();
            services.AddSingleton<DefaultReceiptPointValueCalculator.IConfiguration, DefaultReceiptPointValueCalculator.Configuration>();

            services.AddSingleton<ServiceController.ICache, ServiceController.Cache>();

            services.AddTransient<IReceiptValidator, DefaultReceiptValidator>();
            services.AddTransient<IReceiptPointValueCalculator, DefaultReceiptPointValueCalculator>();

            services.AddTransient<IReceiptManager, DefaultReceiptManager>();
        }
    }
}
