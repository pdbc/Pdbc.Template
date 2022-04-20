using System;
using System.Collections.Generic;
using Aertssen.Framework.Core;
using Aertssen.Framework.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyTemplate.Data
{
    public class MyTemplateDataModule : IModule
    {
        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var settings = new DataSettings(configuration);
            serviceCollection.AddSingleton(settings);

            serviceCollection.AddDbContext<MyTemplateDbContext>(options => options
                .UseLazyLoadingProxies(true)
                .UseSqlServer(
                    settings.DbConnectionString, builder =>
                    {
                        if (settings.UseRetries)
                        {
                            builder.EnableRetryOnFailure(
                                settings.SqlServerMaxRetryCount,
                                TimeSpan.FromMilliseconds(settings.SqlServerMaxDelay),
                                new List<int>()
                            );
                        }

                        builder.MigrationsHistoryTable(DemoDbConstants.Migrations.TableName, DemoDbConstants.SchemaName.Default);
                    })
                );

            serviceCollection.AddOptions(); //Is necessary in order for IOptions<T> to work

            // Scans all classes to register them as its matching interface.
            serviceCollection.Scan(scan => scan.FromAssemblyOf<MyTemplateDataModule>()
                .AddClasses(true)
                .AsMatchingInterface()
                .WithScopedLifetime()
            );

            // Scans to register all repositories as IEntityRepository
            serviceCollection.Scan(scan => scan.FromAssemblyOf<MyTemplateDataModule>()
                .AddClasses(classes => classes.AssignableTo(typeof(IEntityRepository<>)).Where(_ => !_.IsGenericType))  // Get all classes inheriting the entity repository
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );
            
            serviceCollection.AddScoped<DbContext>(x => x.GetRequiredService<MyTemplateDbContext>());
        }
    }
}
