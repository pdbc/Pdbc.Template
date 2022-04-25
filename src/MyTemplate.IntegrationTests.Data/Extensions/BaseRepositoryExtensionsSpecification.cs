using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Integration.Tests;

namespace MyTemplate.IntegrationTests.Data.Extensions
{
    public abstract class BaseRepositoryExtensionsSpecification<TRepository> : MyTemplateIntegrationInternalBaseSpecification 
    {
        public TRepository Repository { get; set; }
        
        protected override void Establish_context()
        {
            base.Establish_context();

            Repository = base.ServiceProvider.GetRequiredService<TRepository>();
        }
    }
}
