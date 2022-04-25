using Microsoft.Extensions.DependencyInjection;

namespace MyTemplate.Integration.Tests
{
    public class MyTemplateExternalIntegrationBaseSpecification : MyTemplateIntegrationBaseSpecification
    {
        protected override void BootstrapContainer(ServiceCollection services)
        {

            MyTemplateIntegrationTestBootstrap.BootstrapContainerForApiTests(services, Configuration);
        }
    }
}
