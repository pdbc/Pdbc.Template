using System;
using Aertssen.Framework.Tests.Infra;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MyTemplate.IntegrationTests.Api
{
    public abstract class MyTemplateIntegrationApiRequestSpecification : MyTemplateIntegrationBaseApiRequestSpecification
    {
        protected abstract IIntegrationTest CreateIntegrationTest();

        [Test]
        public void Execute_Test()
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Running {TestExecutionContext.CurrentContext.CurrentTest.FullName}");

            IntegrationTest = CreateIntegrationTest();
            EditApiTest();

            using (var transaction = Context.Database.BeginTransaction())
            {
                IntegrationTest.Setup();
                Context.SaveChanges();

                transaction.Commit();
            }


            IntegrationTest.Run();
            IntegrationTest.Cleanup();

            // Save changes after cleanup
            Context.SaveChanges();

            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Finished {TestExecutionContext.CurrentContext.CurrentTest.FullName}");
        }
    }
}


