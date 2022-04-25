using System;
using Aertssen.Framework.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Data;

namespace MyTemplate.DatabaseMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddLogging();

            services.RegisterModule<MyTemplateDataModule>(configuration);
            services.AddScoped<ClearDatabaseService>();
            var serviceProvider = services.BuildServiceProvider();

            var scharnierDbContext = serviceProvider.GetRequiredService<MyTemplateDbContext>();
            var connectionString = scharnierDbContext.Database.GetDbConnection().ConnectionString;
            var isLocalRun = connectionString.Contains("localhost");

            var clearDatabase = configuration.GetValue<bool>("DatabaseMigrator:ClearDatabase");
            if (clearDatabase && isLocalRun)
            {
                Console.WriteLine("Start clearing database...");

                // Drop all tables
                var service = serviceProvider.GetRequiredService<ClearDatabaseService>();
                service.ClearDatabaseTables();
                Console.WriteLine("Finished clearing database...");
            }

            // Migrate database
            
            Console.WriteLine("Start migrating database...");
            //var migrations = dbContext.Database.GetPendingMigrations();
            scharnierDbContext.Database.Migrate();
            Console.WriteLine("Finished migrating database...");
        }
    }
}
