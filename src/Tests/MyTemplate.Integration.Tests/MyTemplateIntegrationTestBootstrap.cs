using System;
using Aertssen.Framework.Api.ServiceAgents;
using Aertssen.Framework.Core.Extensions;
using Aertssen.Framework.Infra;
using Aertssen.Framework.Services.Cqrs;
using MyTemplate.Core;
using MyTemplate.Data;
using MyTemplate.Services.Cqrs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;

namespace MyTemplate.Integration.Tests
{
    public class MyTemplateIntegrationTestBootstrap
    {
        public static void BootstrapContainer(IServiceCollection services,
            IConfiguration configuration)
        {
            // Framework
            services.RegisterModule<AertssenFrameworkInfraModule>(configuration);
            services.RegisterModule<AertssenFrameworkCqrsServicesModule>(configuration);
            //services.RegisterModule<AertssenFrameworkAuditModule>(configuration);

            // Automapper
            services.AddAutoMapper(typeof(RequestToCqrsMappings),
                typeof(CqrsServiceRequestToCqrsMappings),
                //typeof(AuditRequestToCqrsMappings),
                typeof(DomainDtoMappings));

            // Application specific
            services.RegisterModule<MyTemplateDataModule>(configuration);
            services.RegisterModule<MyTemplateCoreModule>(configuration);
            services.RegisterModule<MyTemplateServicesCqrsModule>(configuration);

            // Mock Test items
            //services.AddScoped<IRequestData, MockRequestData>();
        }

        public static void BootstrapContainerForApiTests(IServiceCollection services,
            IConfiguration configuration)
        {
            // Http 
            services.AddHttpClient();
            //services.AddScoped<IInterfacingService, WebApiInterfacingService>();
            //services.AddScoped<IWebApiClientProxy, WebApiClientProxy>();
            services.AddSingleton(provider => new Func<IWebApiClientProxy>(provider.GetRequiredService<IWebApiClientProxy>));

            // Framework
            services.RegisterModule<AertssenFrameworkInfraModule>(configuration);
            services.RegisterModule<AertssenFrameworkCqrsServicesModule>(configuration);
            //services.RegisterModule<AertssenFrameworkAuditModule>(configuration);

            // Automapper
            services.AddAutoMapper(typeof(RequestToCqrsMappings),
                typeof(CqrsServiceRequestToCqrsMappings),
                //typeof(AuditRequestToCqrsMappings),
                typeof(DomainDtoMappings));

            // Application specific
            services.RegisterModule<MyTemplateCoreModule>(configuration);
            services.RegisterModule<MyTemplateDataModule>(configuration);
            //services.RegisterModule<MyTemplateServicesCqrsModule>(configuration);

            // Mock Test items
            //services.AddScoped<IRequestData, MockRequestData>();
        }
    }
}