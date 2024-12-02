using API.Contracts;
using API.Controllers;
using API.Services;
using Microsoft.OpenApi.Any;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                            .AddJsonOptions((options) => options.JsonSerializerOptions.RespectNullableAnnotations = true);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.MapType(typeof(DateTime), () => new Microsoft.OpenApi.Models.OpenApiSchema()
                {
                    Type = "string",
                    Format = "yyyy-mm-dd",
                    Example = new OpenApiString(DateTime.UtcNow.ToString("2025-01-31"))
                });
                options.MapType(typeof(ClockTime), () => new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "string",
                    Format = "hh:dd",
                    Example = new OpenApiString("23:11")
                });
            });

            RegisterServiceControllerDependencies(builder.Services, GetConfiguration());

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static IConfiguration GetConfiguration() => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


        private static void RegisterServiceControllerDependencies(IServiceCollection services, IConfiguration configuration) 
        {
            new DefaultServiceRegistrar().RegisterServices(services, configuration);

            services.AddLogging(config => { config.AddConsole(); });
        }
    }
}





