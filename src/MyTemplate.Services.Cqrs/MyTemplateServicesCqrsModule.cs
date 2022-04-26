using Aertssen.Framework.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyTemplate.Services.Cqrs
{
    public class MyTemplateServicesCqrsModule : IModule
    {
        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Scan(s => s.FromAssemblyOf<MyTemplateServicesCqrsModule>()
                .AddClasses(true)
                .AsMatchingInterface()
                .WithScopedLifetime());
        }
    }
}