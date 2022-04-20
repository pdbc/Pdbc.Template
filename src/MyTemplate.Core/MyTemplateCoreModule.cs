using Aertssen.Framework.Core;
using Aertssen.Framework.Infra.Extensions;
using Aertssen.Framework.Infra.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Core.FW;

namespace MyTemplate.Core
{
    public class MyTemplateCoreModule : IModule
    {
        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Scan(s => s.FromAssemblyOf<MyTemplateCoreModule>()
                .AddClasses(true)
                .AsMatchingInterface()
                .WithScopedLifetime());

            // Register all handlers
            serviceCollection.AddMediatR(typeof(MyTemplateCoreModule));
            serviceCollection.RegisterCqrsForAssemblyOfType(typeof(MyTemplateCoreModule));

            serviceCollection.AddScoped<IErrorMessagesResourceProvider, ErrorMessagesResourceProvider>();
        }
    }
}