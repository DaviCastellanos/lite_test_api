using lite_test.Infrastructure.AppSettings;
using lite_test.Core.Interfaces;
using lite_test.Infrastructure.CosmosDbData.Repository;
using lite_test.Infrastructure.Extensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Serilog;
using System.Collections.Generic;

// add the FunctionsStartup assembly attribute that specifies the type name used during startup
[assembly: FunctionsStartup(typeof(lite_test_api.Startup))]

namespace lite_test_api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureServices(builder.Services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configurations
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Use a singleton Configuration throughout the application
            services.AddSingleton<IConfiguration>(configuration);

            // Singleton instance. See example usage in SendGridEmailService: inject IOptions<SendGridEmailSettings> in SendGridEmailService constructor
            //services.Configure<SendGridEmailSettings>(configuration.GetSection("SendGridEmailSettings"));

            // if default ILogger is desired instead of Serilog
            //services.AddLogging();

            // configure serilog
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            services.AddLogging(lb => lb.AddSerilog(logger));

            //Register SendGrid Email
            //services.AddScoped<IEmailService, SendGridEmailService>();

            // Bind database-related bindings
            var cosmosDbConfig = configuration.GetSection("ConnectionStrings:LiteTestCosmosDB").Get<CosmosDbSettings>();
            // register CosmosDB client and data repositories
            ContainerInfo container = new ContainerInfo() { Name = "business", PartitionKey = "id" };

            List<ContainerInfo> containers = new List<ContainerInfo>() { container };

            services.AddCosmosDb("https://lite-test-db.documents.azure.com:443/",
                                 "E9Vm0QMTFexAgoFvQbRSJtDnfuyBjp4YRr0f5ykmFWSNhuQVr8ke3rCcC7BxXAapUNpiKWtZgaU4ACDb4sgTgQ ==",
                                 "liteDb",
                                containers);

            services.AddScoped<IBusinessRepository, BusinessRepository>();
            

        }
    }
}
