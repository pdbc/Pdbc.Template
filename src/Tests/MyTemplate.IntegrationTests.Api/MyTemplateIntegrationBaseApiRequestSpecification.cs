using System;
using Aertssen.Framework.Api.ServiceAgents;
using Aertssen.Framework.Core.Configurations;
using Aertssen.Framework.Tests.Infra;
using Aertssen.Framework.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Integration.Tests;

namespace MyTemplate.IntegrationTests.Api
{
    public abstract class MyTemplateIntegrationBaseApiRequestSpecification : MyTemplateExternalIntegrationBaseSpecification
    {
        protected IIntegrationTest IntegrationTest;

        protected Func<IWebApiClientProxy> CreateWebApiProxy => () => BuildWebApiProxy();

        private IWebApiClientProxy BuildWebApiProxy()
        {
            var azureAdSettings = new AzureAdSettings(Configuration);

            // Scharnier info
            var scharnierApiUrl = Configuration.GetValue<string>("MyTemplate:ApiUrl");
            var scharnierTestsClientId = Configuration.GetValue<string>("MyTemplateTests:ClientId");
            var scharnierTestsClientSecret = Configuration.GetValue<string>("MyTemplateTests:ClientSecret");

            // Build up scope
            var scope = $"api://{azureAdSettings.ClientId}/.default";

            // Adjust AzureAdSettings (for integration tests)
            azureAdSettings.ClientId = scharnierTestsClientId;
            azureAdSettings.ClientSecret = scharnierTestsClientSecret;

            var clientAccessToken = new ClientTokenAccess(azureAdSettings, new MockLogger<ClientTokenAccess>());
            var originalAccessToken = clientAccessToken.GetClientAccessToken(scope);

            if (originalAccessToken.IsError)
            {
                throw new UnauthorizedAccessException("AccessToken cannot be retrieved");
            }

            //return new ScharnierTokenAccessWebApiClientProxy(clientAccessToken, null, originalAccessToken, scharnierApiUrl, 30000);
            return new TokenAccessWebApiClientProxy(clientAccessToken, null, originalAccessToken, scharnierApiUrl, 30000);
        }

        protected override void SetupServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddLogging();

            MyTemplateIntegrationTestBootstrap.BootstrapContainerForApiTests(services, Configuration);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected virtual void EditApiTest()
        {

        }

        protected override void CleanupActionsAfterTest()
        {
            if (IntegrationTest != null)
            {
                try
                {
                    IntegrationTest.Cleanup();
                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    //Ignore fails because if the setup fails, this might as well fail.
                }
            }
        }
    }
}


