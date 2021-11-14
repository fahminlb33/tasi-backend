using System;
using System.IO;
using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using Serilog.Sinks.Elasticsearch;
using TASI.Backend.Infrastructure.Database;

namespace TASI.Backend
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // serilog specific configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            if (Environment.GetEnvironmentVariable("DEVELOP_IN_DOCKER_COMPOSE") == "true")
            {
                config.AddJsonFile("appsettings.DockerCompose.json");
            }

            var appSettings = config.Build();
            var logger = new LoggerConfiguration()
                // override log level
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Debug()

                // enrich with extra information
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithElasticApmCorrelationInfo()

                // log sinks
                .WriteTo.Console()
                //.WriteTo.Seq(appSettings["Seq:ServerUrl"])
                .WriteTo.File("logs/tasi-log-.txt", rollingInterval: RollingInterval.Day)
                //.WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), new TraceTelemetryConverter())
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(appSettings["ElasticSearch:NodeUris"]))
                {
                    ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "password"),
                    CustomFormatter = new EcsTextFormatter(),
                    IndexFormat = "tasi-container-{0:yyyy.MM}",
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    MinimumLogEventLevel = LogEventLevel.Debug,
                })

                // build final logger
                .CreateLogger();
            
            Log.Logger = logger;

            try
            {
                logger.Information("Building web host");
                var host = CreateHostBuilder(args).Build();
              
                logger.Information("Seeding database if not exists");
                CreateDbIfNotExists(host);

                logger.Information("Starting web host");
                host.Run();

                logger.Information("Web host shutdown gracefully");
                return 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Host unexpectedly terminated");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // application specific configuration
                    if (Environment.GetEnvironmentVariable("DEVELOP_IN_DOCKER_COMPOSE") != "true")
                    {
                        return;
                    }

                    config.AddJsonFile("appsettings.DockerCompose.json");
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<TasiContext>();
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when running database initialization.");
            }
        }
    }
}
