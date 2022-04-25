using Microsoft.Extensions.DependencyInjection;

namespace MyTemplate.Integration.Tests
{
    public class MyTemplateIntegrationInternalBaseSpecification : MyTemplateIntegrationBaseSpecification
    {
        protected override void BootstrapContainer(ServiceCollection services)
        {

            MyTemplateIntegrationTestBootstrap.BootstrapContainer(services, Configuration);
        }
    }
}