using System;
using System.Transactions;
using Aertssen.Framework.Tests.Infra;
using MyTemplate.Integration.Tests;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MyTemplate.IntegrationTests.Core
{
    public abstract class MyTemplateIntegrationCqrsRequestSpecification : MyTemplateIntegrationInternalBaseSpecification
    {
        protected IIntegrationTest IntegrationTest;

        protected override void Establish_context()
        {
            base.Establish_context();

            IntegrationTest = CreateIntegrationTest();

            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                IntegrationTest.Setup();


                transaction.Complete();
            }

            // Set the name in the setup
            TestExecutionContext.CurrentContext.CurrentTest.Name = $"{IntegrationTest.GetType().Name}.Execute_Test";
        }

        protected abstract IIntegrationTest CreateIntegrationTest();

        [Test]
        public void Execute_Test()
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Running {TestExecutionContext.CurrentContext.CurrentTest.FullName}");
            IntegrationTest.Run();
            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Finished {TestExecutionContext.CurrentContext.CurrentTest.FullName}");
        }

        protected override void CleanupActionsAfterTest()
        {
            IntegrationTest?.Cleanup();
        }
    }
}
