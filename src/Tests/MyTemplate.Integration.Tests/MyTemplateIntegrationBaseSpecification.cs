using System;
using System.Data.Common;
using System.IO;
using System.Security.Principal;
using Aertssen.Framework.Tests;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Data;
using NUnit.Framework;

namespace MyTemplate.Integration.Tests
{
    public abstract class MyTemplateIntegrationBaseSpecification : BaseSpecification
    {
        protected MyTemplateDbContext Context;

        protected IConfiguration Configuration { get; private set; }

        protected virtual bool ShouldLoadTestObjects { get; set; } = true;

        protected ServiceProvider ServiceProvider;

        protected TestCaseService TestCaseService;

        protected MyTemplateTestDataObjects MyTemplateTestDataObjects;

        protected DateTime TestStartedDatTime;

        protected override void Establish_context()
        {
            LoadConfiguration();

            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("INTEGRATION_TESTS"), Array.Empty<string>());
            var dir = TestContext.CurrentContext.TestDirectory;
            Directory.SetCurrentDirectory(dir);

            SetupServiceProvider();

            Context = ServiceProvider.GetRequiredService<MyTemplateDbContext>();

            TestCaseService = new TestCaseService(Context);
            if (ShouldLoadTestObjects)
            {
                MyTemplateTestDataObjects = new MyTemplateTestDataObjects(Context);
                MyTemplateTestDataObjects.LoadObjects();
                MyTemplateTestDataObjects.SetupMissingObjects();
                Context.SaveChanges();
            }

            TestStartedDatTime = DateTime.Now;

            base.Establish_context();
        }

        protected virtual void SetupServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddLogging();

            BootstrapContainer(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected abstract void BootstrapContainer(ServiceCollection services);

        protected virtual void LoadConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                ;

            Configuration = configurationBuilder.Build();

        }

        protected override void Dispose_context()
        {
            RunWithoutException(CleanupActionsAfterTest);
            RunWithoutException(() => Context.SaveChanges());
            RunWithoutException(() => Context?.Dispose());
        }

        private void RunWithoutException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception) { }
        }

        protected override void Because()
        {
            //Context.SaveChanges();
            base.Because();
            //Context.SaveChanges();
        }

        protected virtual void CleanupActionsAfterTest() { }
    }
}